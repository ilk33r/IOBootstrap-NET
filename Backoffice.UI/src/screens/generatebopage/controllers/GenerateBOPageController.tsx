import React from "react";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import Controller from "../../../presentation/controllers/Controller";
import FormType from "../../shared/interfaces/FormType";
import FormTypeTextProps from "../../shared/props/FormTypeTextProps";
import FormView from "../../shared/views/FormView";
import GenerateBOPageProps from "../props/GenerateBOPageProps";
import GenerateBOPageState from "../props/GenerateBOPageState";
import ValidationMinLengthRule from "../../../presentation/validations/ValidationMinLengthRule";
import GenerateBOPageRequestModel from "../models/GenerateBOPageRequestModel";
import GenerateBOPageResponseModel from "../models/GenerateBOPageResponseModel";
import CodeBlockView from "../views/CodeBlockView";
import GenerateBOPageFilesRequestModel from "../models/GenerateBOPageFilesRequestModel";

class GenerateBOPageController extends Controller<GenerateBOPageProps, GenerateBOPageState> {

    constructor(props: GenerateBOPageProps) {
        super(props);

        this.state = new GenerateBOPageState(null);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
        this.downloadAPIFiles = this.downloadAPIFiles.bind(this);
        this.downloadUIFiles = this.downloadUIFiles.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {
        this.indicatorPresenter.present();
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_GENERATE_BOPAGE_CONTROLLER_NAME}/CreateModel`;
        const request = new GenerateBOPageRequestModel();
        request.entityName = values[0];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: GenerateBOPageResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new GenerateBOPageState(response);
                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    downloadAPIFiles(event: React.MouseEvent<HTMLButtonElement>) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_GENERATE_BOPAGE_FILES_CONTROLLER_NAME}/CreateAPIFiles`;
        const request = this.createRequest();
        
        const weakSelf = this;
        this.service.postDownloadFile(requestPath, "APIFiles.zip", request, function () {
            weakSelf.indicatorPresenter.dismiss();
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    downloadUIFiles(event: React.MouseEvent<HTMLButtonElement>) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_GENERATE_BOPAGE_FILES_CONTROLLER_NAME}/CreateUIFiles`;
        const request = this.createRequest();
        
        const weakSelf = this;
        this.service.postDownloadFile(requestPath, "UIFiles.zip", request, function () {
            weakSelf.indicatorPresenter.dismiss();
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("actionGenerateBOPage", "Generate BO Page")
        ];

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Entity Name", "", true, [ ValidationMinLengthRule.initialize("Entity name is too short.", "Invalid entity name.", 3) ]),
        ];

        if (this.state.boPageDataResponse == null) {
            return (
                <React.StrictMode>
                    <FormView navigation={navigation} 
                        resourceHome="Home"
                        title="Generate BO Page"
                        submitButtonName="Generate"
                        errorHandler={this.handleFormError}
                        successHandler={this.handleFormSuccess}
                        formElements={formElements} />
                </React.StrictMode>
            );
        }

        return (
            <React.StrictMode>
                <div className="content-wrapper">
                    <div className="row">
                        <CodeBlockView title="Entity"
                            sectionTitle={this.state.boPageDataResponse.entityName}
                            descriptions={[]} />
                        <div className="col-md-6">
                            <div className="box box-solid">
                                <div className="box-header with-border">
                                    <i className="fa fa-file"></i>
                                    <h3 className="box-title">Files</h3>
                                </div>
                                <div className="box-body">
                                    <div className="col-md-3">
                                        <button type="button" onClick={this.downloadAPIFiles} className="btn btn-success btn-flat">Download API Files</button>
                                    </div>
                                    <div className="col-md-3">
                                        <button type="button" onClick={this.downloadUIFiles} className="btn btn-success btn-flat">Download UI Files</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }

    private createRequest(): GenerateBOPageFilesRequestModel {
        const request = new GenerateBOPageFilesRequestModel();
        request.entityName = this.state.boPageDataResponse?.entityName ?? "";
        request.entityDisplayName = this.state.boPageDataResponse?.entityDisplayName ?? "";
        request.entityItemName = this.state.boPageDataResponse?.entityItemName ?? "";
        request.properties = this.state.boPageDataResponse?.properties ?? [];
        request.listEntityName = this.state.boPageDataResponse?.listEntityName ?? "";
        request.listEntityAPIPath = this.state.boPageDataResponse?.listEntityAPIPath ?? "";
        request.listEntityDisplayName = this.state.boPageDataResponse?.listEntityDisplayName ?? "";

        return request;
    }
}

export default GenerateBOPageController;