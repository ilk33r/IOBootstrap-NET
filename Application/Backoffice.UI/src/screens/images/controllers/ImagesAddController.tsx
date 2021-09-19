import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import Controller from "../../../presentation/controllers/Controller";
import FormDataOptionModel from "../../shared/models/FormDataOptionModel";
import FormType from "../../shared/interfaces/FormType";
import FormTypeNumberProps from "../../shared/props/FormTypeNumberProps";
import FormTypeSelectProps from "../../shared/props/FormTypeSelectProps";
import FormView from "../../shared/views/FormView";
import ImageVariationsModel from "../models/ImageVariationsModel";
import React from "react";
import ValidationMinAmountRule from "../../../presentation/validations/ValidationMinAmountRule";

class ImagesAddController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {        
        const imageData = new ImageVariationsModel();
        imageData.width = Number(values[0]);
        imageData.height = Number(values[1]);
        imageData.scale = Number(values[2]);
        imageData.keepRatio = Boolean(values[3]);

        this.appContext.setObjectForKey("imageData", imageData);
        this.navigateToPage("imageUploadFile");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("imagesEdit", "Images"),
            BreadcrumbNavigationModel.initialize("imageAdd", "Add Image")
        ];

        const formElements: FormType[] = [
            FormTypeNumberProps.initializeWithValidations("Width", "", true, [ ValidationMinAmountRule.initialize("Width must be greater than 0.", "Invalid image width.", 0) ]),
            FormTypeNumberProps.initializeWithValidations("Height", "", true, [ ValidationMinAmountRule.initialize("Height must be greater than 0.", "Invalid image height.", 0) ]),
            FormTypeNumberProps.initializeWithValidations("Scale", "", true, [ ValidationMinAmountRule.initialize("Scale must be greater than 0.", "Invalid image scale.", 0) ]),
            FormTypeSelectProps.initialize("Keep Ratio", "", true, [
                FormDataOptionModel.initialize("No", "0"),
                FormDataOptionModel.initialize("YES", "1")
            ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add an new image"
                    submitButtonName="Next"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ImagesAddController;
