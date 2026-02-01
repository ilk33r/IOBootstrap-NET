import { BaseView, Validatable, View } from "iobootstrap-ui-base";
import FormElement from "../interfaces/FormElement";
import FormTypeTextProps from "../props/FormTypeTextProps";
import FormViewState from "../props/FormViewState";

class FormTypeTextView extends View<FormTypeTextProps, FormViewState> implements FormElement, Validatable {

    private _formValue: string;

    constructor(props: FormTypeTextProps) {
        super(props);

        this.state = new FormViewState();

        this._formValue = this.props.value;
        this.handleValueChange = this.handleValueChange.bind(this);
    }

    public getValue(): string | null {
        return this._formValue;
    }

    public getBlobValue(): Blob | null {
        return null;
    }

    handleValueChange(event: { target: { value: string; }; }) {
        this._formValue = event.target.value;
        
        if (this.props.changeHandler != null) {
            this.props.changeHandler(this.props.index, this._formValue);
        }
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
        const formClass = (this.state.hasError) ? "form-control is-invalid" : "form-control";

        return(
            <BaseView>
                <div className="row mb-3">
                    <div className="col-sm-2 text-end">
                        <label htmlFor={formId} className="col-form-label my-2">
                            <strong>{this.props.name}</strong>
                        </label>
                    </div>
                    <div className="col-sm-9">
                        <div className="input-group has-validation">
                            <div className="form-floating">
                                <input type={this.props.inputType} id={formId} className={formClass} defaultValue={this.props.value} placeholder={this.props.name} onChange={this.handleValueChange} disabled={!this.props.isEnabled} />
                                <span className="invalid-feedback">{this.state.errorMessage}</span>
                                <label htmlFor={formId}>{this.props.name}</label>
                            </div>
                        </div>
                    </div>
                </div>
            </BaseView>
        );
    }
}

export default FormTypeTextView;
