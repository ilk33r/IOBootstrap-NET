import MenuAddRequestModel from "../models/MenuAddRequestModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, DIHooks, ValidationMinAmountRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeNumberProps, FormTypePopupSelectionProps, FormTypeSelectProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class MenuEditorAddController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME}/AddMenuItem`;
        const request = new MenuAddRequestModel();
        request.name = values[0];
        request.action = values[1];
        request.cssClass = values[2];
        request.requiredRole = Number(values[3]);
        request.menuOrder = Number(values[4]);
        request.parentEntityID = Number(values[5]);

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Menu has been added successfully.", "menuEditorList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("menuEditorList", "Menu Editor"),
            BreadcrumbNavigationModel.initialize("menuEditorAdd", "Add Menu Item")
        ];

        let userRoleFormDataOptions: FormDataOptionModel[] = []
        const userRoleFormDataOptionsHook = DIHooks.Instance.hookForKey("userRoleFormDataOptions")
        if (userRoleFormDataOptionsHook != null) {
            const userRoleFormDataOptionsAny = userRoleFormDataOptionsHook(null);
            if (userRoleFormDataOptionsAny != null) {
                userRoleFormDataOptions = userRoleFormDataOptionsAny;
            }
        }
        
        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Name", "", true, [ ValidationRequiredRule.initialize("Name is too short.", "Invalid menu name.") ]),
            FormTypeTextProps.initializeWithValidations("Action", "", true, [ ValidationRequiredRule.initialize("Action is too short.", "Invalid menu action.") ]),
            FormTypeTextProps.initialize("CSS Class Name", "fa-circle-o", true),
            FormTypeSelectProps.initialize("Required Role", "", true, userRoleFormDataOptions),
            FormTypeNumberProps.initializeWithValidations("Menu Order", "", true, [ 
                ValidationRequiredRule.initialize("Menu order must be required.", "Invalid menu order."),
                ValidationMinAmountRule.initialize("Menu order must be greater than zero.", "Invalid menu order.", 0)
            ]),
            FormTypePopupSelectionProps.initialize("Parent Menu", "", 0, "menuEditorSelect", true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add a menu item"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorAddController;
