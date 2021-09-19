import BreadcrumbView from "./BreadcrumbView";
import FormElement from "../interfaces/FormElement";
import FormTypeDateProps from "../props/FormTypeDateProps";
import FormTypeImageProps from "../props/FormTypeImageProps";
import FormTypeImageView from "../views/FormTypeImageView";
import FormTypeNumberProps from "../props/FormTypeNumberProps";
import FormTypePasswordProps from "../props/FormTypePasswordProps";
import FormTypePopupSelectionProps from "../props/FormTypePopupSelectionProps";
import FormTypePopupSelectionView from "../views/FormTypePopupSelectionView";
import FormTypeSelectProps from "../props/FormTypeSelectProps";
import FormTypeSelectView from "../views/FormTypeSelectView";
import FormTypeTextAreaProps from "../props/FormTypeTextAreaProps";
import FormTypeTextAreaView from "../views/FormTypeTextAreaView";
import FormTypeTextProps from "../props/FormTypeTextProps";
import FormTypeTextView from "../views/FormTypeTextView";
import FormViewProps from "../props/FormViewProps";
import Validatable from "../../../presentation/inerfaces/Validatable";
import View from "../../../presentation/views/View";
import React from "react";

class FormView extends View<FormViewProps, {}> {

    private _formElementsRef: React.RefObject<any>[]; 

    constructor(props: FormViewProps) {
        super(props);

        this._formElementsRef = [];
        this.handleForm = this.handleForm.bind(this);
        this.handleFormError = this.handleFormError.bind(this);

        for (let i = 0; i < this.props.formElements.length; i++) {
            this._formElementsRef[i] = React.createRef<FormElement>();
        }
    }

    handleForm(e: { preventDefault: () => void; }) {
        e.preventDefault();

        let isValidated = true;
        let values: string[] = [];

        this._formElementsRef.forEach(formElement => {
            const validatableElement = formElement.current as Validatable;

            if (!validatableElement.validate()) {
                isValidated = false;
            }

            const formElementType = formElement.current as FormElement;
            values.push(formElementType.getValue());
        });

        if (isValidated) {
            this.props.successHandler(values);
        }
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.props.errorHandler(errorTitle, errorMessage);
    }

    render() {
        const formElements = this.props.formElements.map((formElement, index) => {
            if (formElement instanceof FormTypeTextProps) {
                return (<FormTypeTextView index={index}
                            inputType="text"
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypeNumberProps) {
                return (<FormTypeTextView index={index}
                            inputType="number"
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypeDateProps) {
                return (<FormTypeTextView index={index}
                            inputType="date"
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypePasswordProps) {
                return (<FormTypeTextView index={index}
                            inputType="password"
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypeSelectProps) {
                return (<FormTypeSelectView index={index}
                            inputType={formElement.inputType}
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            options={formElement.options}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypeTextAreaProps) {
                return (<FormTypeTextAreaView index={index}
                            inputType={formElement.inputType}
                            name={formElement.name}
                            value={formElement.value}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypePopupSelectionProps) {
                return (<FormTypePopupSelectionView index={index}
                            inputType={formElement.inputType}
                            name={formElement.name}
                            value={formElement.value}
                            selectedItemId={formElement.selectedItemId}
                            selectionURL={formElement.selectionURL}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            if (formElement instanceof FormTypeImageProps) {
                return (<FormTypeImageView index={index}
                            inputType={formElement.inputType}
                            name={formElement.name}
                            value={formElement.value}
                            fileName={formElement.fileName}
                            isEnabled={formElement.isEnabled}
                            errorHandler={this.handleFormError}
                            validations={formElement.validations}
                            key={index}
                            ref={this._formElementsRef[index]} />);
            }

            return (<React.StrictMode></React.StrictMode>);
        });

        return (
            <React.StrictMode>
                <div className="content-wrapper">
                    <BreadcrumbView navigation={this.props.navigation} resourceHome={this.props.resourceHome} />
                    <section className="content">
                        <div className="row">
                            <div className="col-md-12">
                                <div className="box box-info">
                                    <div className="box-header with-border">
                                        <h3 className="box-title">{this.props.title}</h3>
                                    </div>
                                    <form className="form-horizontal" onSubmit={this.handleForm}>
                                        <div className="box-body">
                                            {formElements}
                                        </div>
                                        <div className="box-footer">
                                            <button type="submit" className="btn btn-info pull-right">{this.props.submitButtonName}</button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </section>
                </div>
            </React.StrictMode>
        );
    }
}

export default FormView;