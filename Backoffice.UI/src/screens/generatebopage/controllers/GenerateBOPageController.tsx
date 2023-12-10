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

        const menuParentViewDescriptions = [
            "Name: " + this.state.boPageDataResponse.entityDisplayName,
            "Action: action" + this.state.boPageDataResponse.entityDisplayName,
            "CSS Class Name: fa-circle-o"
        ];

        const menuListViewDescriptions = [
            "Name: List " + this.state.boPageDataResponse.entityDisplayName,
            "Action: " + this.state.boPageDataResponse.listEntityName,
            "CSS Class Name: fa-circle-o"
        ];

        const menuCreateViewDescriptions = [
            "Name: Add " + this.state.boPageDataResponse.entityItemName,
            "Action: " + this.state.boPageDataResponse.createEntityName,
            "CSS Class Name: fa-circle-o"
        ];

        const listNavigationViewDescriptions = [
            "import " + this.state.boPageDataResponse.listEntityDisplayName + "Controller from \"../../" + this.state.boPageDataResponse.entityDisplayName.toLowerCase() + "/controllers/" + this.state.boPageDataResponse.listEntityDisplayName + "Controller\";",
            "if (this.state.pageHash === \"" + this.state.boPageDataResponse.listEntityName + "\") { return <" + this.state.boPageDataResponse.listEntityDisplayName + "Controller /> }"
        ];

        const updateNavigationViewDescriptions = [
            "import " + this.state.boPageDataResponse.updateEntityDisplayName + "Controller from \"../../" + this.state.boPageDataResponse.entityDisplayName.toLowerCase() + "/controllers/" + this.state.boPageDataResponse.updateEntityDisplayName + "Controller\";",
            "if (this.state.pageHash === \"" + this.state.boPageDataResponse.updateEntityName + "\") { return <" + this.state.boPageDataResponse.updateEntityDisplayName + "Controller /> }"
        ];

        const deleteNavigationViewDescriptions = [
            "import " + this.state.boPageDataResponse.deleteEntityDisplayName + "Controller from \"../../" + this.state.boPageDataResponse.entityDisplayName.toLowerCase() + "/controllers/" + this.state.boPageDataResponse.deleteEntityDisplayName + "Controller\";",
            "if (this.state.pageHash === \"" + this.state.boPageDataResponse.deleteEntityName + "\") { return <" + this.state.boPageDataResponse.deleteEntityDisplayName + "Controller /> }"
        ];

        const createNavigationViewDescriptions = [
            "import " + this.state.boPageDataResponse.createEntityDisplayName + "Controller from \"../../" + this.state.boPageDataResponse.entityDisplayName.toLowerCase() + "/controllers/" + this.state.boPageDataResponse.createEntityDisplayName + "Controller\";",
            "if (this.state.pageHash === \"" + this.state.boPageDataResponse.createEntityName + "\") { return <" + this.state.boPageDataResponse.createEntityDisplayName + "Controller /> }"
        ];
        
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
                        <div className="clearfix"></div>
                        <CodeBlockView title="Menu"
                            sectionTitle="Add Menu Item"
                            descriptions={menuParentViewDescriptions} />

                        <CodeBlockView title="Menu"
                            sectionTitle="Add Menu Item"
                            descriptions={menuListViewDescriptions} />

                        <div className="clearfix"></div>
                        <CodeBlockView title="Menu"
                            sectionTitle="Add Menu Item"
                            descriptions={menuCreateViewDescriptions} />

                        <div className="clearfix"></div>
                        <CodeBlockView title="Navigation"
                            sectionTitle="Backoffice.UI/src/screens/shared/views/NavigationView.tsx"
                            descriptions={listNavigationViewDescriptions} />

                        <CodeBlockView title="Navigation"
                            sectionTitle="Backoffice.UI/src/screens/shared/views/NavigationView.tsx"
                            descriptions={updateNavigationViewDescriptions} />

                        <div className="clearfix"></div>
                        <CodeBlockView title="Navigation"
                            sectionTitle="Backoffice.UI/src/screens/shared/views/NavigationView.tsx"
                            descriptions={deleteNavigationViewDescriptions} />

                        <CodeBlockView title="Navigation"
                            sectionTitle="Backoffice.UI/src/screens/shared/views/NavigationView.tsx"
                            descriptions={createNavigationViewDescriptions} />
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
        request.updateEntityName = this.state.boPageDataResponse?.updateEntityName ?? "";
        request.updateEntityAPIPath = this.state.boPageDataResponse?.updateEntityAPIPath ?? "";
        request.updateEntityDisplayName = this.state.boPageDataResponse?.updateEntityDisplayName ?? "";
        request.deleteEntityName = this.state.boPageDataResponse?.deleteEntityName ?? "";
        request.deleteEntityAPIPath = this.state.boPageDataResponse?.deleteEntityAPIPath ?? "";
        request.deleteEntityDisplayName = this.state.boPageDataResponse?.deleteEntityDisplayName ?? "";

        return request;
    }
}

export default GenerateBOPageController;