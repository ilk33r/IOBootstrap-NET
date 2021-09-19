import FormElement from "../interfaces/FormElement";
import FormTypeTextProps from "../props/FormTypeTextProps";
import FormViewState from "../props/FormViewState";
import Validatable from "../../../presentation/inerfaces/Validatable";
import View from "../../../presentation/views/View";
import React from "react";

class FormTypeTextView extends View<FormTypeTextProps, FormViewState> implements FormElement, Validatable {

    private _formValue: string;

    constructor(props: FormTypeTextProps) {
        super(props);

        this.state = new FormViewState();

        this._formValue = this.props.value;
        this.handleValueChange = this.handleValueChange.bind(this);
    }

    public getValue(): string {
        return this._formValue;
    }

    handleValueChange(event: { target: { value: string; }; }) {
        this._formValue = event.target.value;
    }

    validate(): boolean {
        var validated = true;
        var errorMessage = "";
        var errorTitle= "";
        let weakSelf = this;

        this.props.validations.forEach(rule => {
            if (!rule.validationResult(weakSelf._formValue)) {
                errorMessage = rule.errorMessage;
                errorTitle = rule.errorTitle;
                validated = false;
            }
        });

        if (!validated) {
            const newState = new FormViewState();
            newState.hasError = true;
            newState.errorMessage = errorMessage;

            this.setState(newState);
            if (this.props.errorHandler != null) {
                this.props.errorHandler(errorTitle, errorMessage);
            }
        }

        return validated;
    }

    render() {
        const formId = "formELM" + this.props.index;
        const areaClass = (this.state.hasError) ? "form-group has-error" : "form-group";
        const errorMessageClass = (this.state.errorMessage.length > 0) ? "help-block" : "help-block hidden";

        return(
            <React.StrictMode>
                <div className={areaClass}>
                    <label htmlFor={formId} className="col-sm-2 control-label">{this.props.name}</label>
                    <div className="col-sm-9">
                        <input type={this.props.inputType} id={formId} className="form-control" defaultValue={this.props.value} placeholder={this.props.name} onChange={this.handleValueChange} disabled={!this.props.isEnabled} />
                        <span className={errorMessageClass}>{this.state.errorMessage}</span>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default FormTypeTextView;
