import { Validatable, View, WindowMessageModel } from "iobootstrap-ui-base";
import FormElement from "../interfaces/FormElement";
import FormTypePopupSelectionProps from "../props/FormTypePopupSelectionProps";
import FormViewState from "../props/FormViewState";
import React from "react";

type MessageEvent = { data: WindowMessageModel; };

class FormTypePopupSelectionView extends View<FormTypePopupSelectionProps, FormViewState> implements FormElement, Validatable {

    private _formSelectedItemId: number | null;

    constructor(props: FormTypePopupSelectionProps) {
        super(props);

        const newState = new FormViewState();
        newState.inputValue = this.props.value;

        this.state = newState;

        this._formSelectedItemId = this.props.selectedItemId;
        this.handleValueChange = this.handleValueChange.bind(this);
        this.handleInputClick = this.handleInputClick.bind(this);
    }

    public componentDidMount?() {
        const weakSelf = this;
        $(window).on("message", function (e) {            
            if (e.originalEvent !== undefined) {
                const originalEvent = e.originalEvent as unknown as MessageEvent;
                
                if (originalEvent === undefined || originalEvent.data.name === undefined) {
                    return;
                }

                if (originalEvent.data.name !== undefined && originalEvent.data.name !== "itemSelected") {
                    return;
                }

                if (originalEvent.data.itemID !== undefined) {
                    if (originalEvent.data.itemID == null) {
                        weakSelf._formSelectedItemId = null;
                    } else {
                        weakSelf._formSelectedItemId = originalEvent.data.itemID;
                    }
                    
                    
                    const closeSelection: WindowMessageModel = {
                        name: "closeSelection",
                        itemID: null, 
                        itemValue: null 
                    };
                    
                    const baseURL = new URL(process.env.REACT_APP_BACKOFFICE_PAGE_URL ?? "");
                    if (baseURL !== null && baseURL.host.length > 0) {
                        window.postMessage(closeSelection, baseURL.origin);
                    }
                }

                weakSelf.setState({
                    inputValue: originalEvent.data.itemValue ?? ""
                });
            }
        });
    }

    public getValue(): string | null {
        if (this._formSelectedItemId != null) {
            return this._formSelectedItemId.toString();
        }

        return null;
    }

    public getBlobValue(): Blob | null {
        return null;
    }

    private handleValueChange(event: { target: { value: string; }; }) {
    }

    private handleInputClick(event: { preventDefault: () => void; }) {
        const pageHash = `#!selection/${this.props.selectionURL}`;
        window.location.hash = pageHash;
    }

    validate(): boolean {
        let validated = true;
        let errorMessage = "";
        let errorTitle= "";
        const weakSelf = this;

        this.props.validations.forEach(rule => {
            if (weakSelf._formSelectedItemId == null) {
                errorMessage = rule.errorMessage;
                errorTitle = rule.errorTitle;
                validated = false;
            } else if (!rule.validationResult(weakSelf._formSelectedItemId.toString())) {
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
            <React.StrictMode>
                <div className="row mb-3">
                    <div className="col-sm-2 text-end">
                        <label htmlFor={formId} className="col-form-label my-2">
                            <strong>{this.props.name}</strong>
                        </label>
                    </div>
                    <div className="col-sm-9">
                        <div className="input-group has-validation">
                            <div className="form-floating">
                                <input type={this.props.inputType} id={formId} className={formClass} value={this.state.inputValue} placeholder={this.props.name} onChange={this.handleValueChange} onClick={this.handleInputClick} disabled={!this.props.isEnabled} contentEditable={false} />
                                <span className="invalid-feedback">{this.state.errorMessage}</span>
                                <label htmlFor={formId}>{this.props.name}</label>
                            </div>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default FormTypePopupSelectionView;
