import { BaseView, Validatable, View, WindowMessageModel } from "iobootstrap-ui-base";
import FormElement from "../interfaces/FormElement";
import FormTypeMultiplePopupSelectionProps from "../props/FormTypeMultiplePopupSelectionProps";
import FormViewState from "../props/FormViewState";
import $ from 'jquery';
import React from "react";

type MessageEvent = { data: WindowMessageModel; };

class FormTypeMultiplePopupSelectionView extends View<FormTypeMultiplePopupSelectionProps, FormViewState> implements FormElement, Validatable {

    private _currentSelectionItemIndex: number;
    private _formSelectedItemIds: number[];

    constructor(props: FormTypeMultiplePopupSelectionProps) {
        super(props);

        const newState = new FormViewState();
        newState.inputValue = this.props.value;

        this.state = newState;

        this._currentSelectionItemIndex = -1;
        this._formSelectedItemIds = this.props.selectedItemIds;
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

                if (weakSelf._currentSelectionItemIndex < 0) {
                    return;
                }

                if (originalEvent.data.itemID !== undefined) {
                    if (originalEvent.data.itemID == null) {
                        if (weakSelf._currentSelectionItemIndex < weakSelf._formSelectedItemIds.length) {
                            weakSelf._formSelectedItemIds.splice(weakSelf._currentSelectionItemIndex, 1);
                        } else {
                            weakSelf._formSelectedItemIds = [];
                        }
                    } else {
                        if (weakSelf._currentSelectionItemIndex < weakSelf._formSelectedItemIds.length) {
                            weakSelf._formSelectedItemIds[weakSelf._currentSelectionItemIndex] = originalEvent.data.itemID;
                        } else {
                            weakSelf._formSelectedItemIds.push(originalEvent.data.itemID);
                        }
                    }
                    
                    const closeSelection: WindowMessageModel = {
                        name: "closeSelection",
                        itemID: null, 
                        itemValue: null 
                    };
                    
                    const baseURL = new URL(import.meta.env.VITE_BACKOFFICE_PAGE_URL ?? "");
                    if (baseURL !== null && baseURL.host.length > 0) {
                        window.postMessage(closeSelection, baseURL.origin);
                    }
                }

                let inputValues = weakSelf.state.inputValue.split(';');
                if (originalEvent.data.itemValue == null) {
                    if (weakSelf._currentSelectionItemIndex < inputValues.length) {
                        inputValues.splice(weakSelf._currentSelectionItemIndex, 1);
                    } else {
                        inputValues = [];
                    }
                } else {
                    if (weakSelf._currentSelectionItemIndex < weakSelf._formSelectedItemIds.length) {
                        inputValues[weakSelf._currentSelectionItemIndex] = originalEvent.data.itemValue;
                    } else {
                        inputValues.push(originalEvent.data.itemValue);
                    }
                }

                weakSelf.setState({
                    inputValue: inputValues.join(";")
                });

                weakSelf._currentSelectionItemIndex = -1;
            }
        });
    }

    public getValue(): string | null {
        if (this._formSelectedItemIds != null) {
            return this._formSelectedItemIds.join(";");
        }

        return null;
    }

    public getBlobValue(): Blob | null {
        return null;
    }

    public setValue(value: string): void {
    }

    public validate(): boolean {
        let validated = true;
        let errorMessage = "";
        let errorTitle= "";
        const weakSelf = this;

        this.props.validations.forEach(rule => {
            if (weakSelf._formSelectedItemIds == null) {
                errorMessage = rule.errorMessage;
                errorTitle = rule.errorTitle;
                validated = false;
            } else if (!rule.validationResult(weakSelf.getValue() ?? "")) {
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

    private handleValueChange(event: React.ChangeEvent<HTMLInputElement>, index: number) {
    }

    private handleInputClick(event: React.MouseEvent<HTMLInputElement>, index: number) {
        event.preventDefault();
        this._currentSelectionItemIndex = index;

        const pageHash = `#!selection/${this.props.selectionURL}`;
        window.location.hash = pageHash;
    }

    render() {
        const formId = "formELM" + this.props.index;
        const formClass = (this.state.hasError) ? "form-control is-invalid" : "form-control";

        let inputs: React.JSX.Element[] = [];
        let inputValues = this.state.inputValue.split(';');
        for (let index = 0; index < this._formSelectedItemIds.length + 1; index++) {
            const inputId = formId + "-" + index;
            const currentInputValue = inputValues.length > index ? inputValues[index] : "";
            if (currentInputValue == "") {
                continue;
            }

            const input = (
                <BaseView key={`key-${formId}`}>
                    <div className="input-group has-validation">
                        <div className="form-floating">
                            <input 
                            type={this.props.inputType} 
                            id={inputId} 
                            className={formClass} 
                            value={currentInputValue} 
                            placeholder={this.props.name} 
                            onChange={(e) => this.handleValueChange(e, index)} 
                            onClick={(e) => this.handleInputClick(e, index)} 
                            disabled={!this.props.isEnabled} 
                            contentEditable={false} 
                            />
                            <span className="invalid-feedback">{this.state.errorMessage}</span>
                            <label htmlFor={formId}>{this.props.name}</label>
                        </div>
                    </div>
                    <br />
                </BaseView>
            );

            inputs.push(input);
        }

        const inputId = formId + "-" + this._formSelectedItemIds.length;
        const emptyInput = (
            <BaseView key={`key-${formId}`}>
                <div className="input-group has-validation">
                    <div className="form-floating">
                        <input 
                        type={this.props.inputType} 
                        id={inputId} 
                        className={formClass} 
                        value="" 
                        placeholder={this.props.name} 
                        onChange={(e) => this.handleValueChange(e, this._formSelectedItemIds.length)} 
                        onClick={(e) => this.handleInputClick(e, this._formSelectedItemIds.length)} 
                        disabled={!this.props.isEnabled} 
                        contentEditable={false} 
                        />
                        <span className="invalid-feedback">{this.state.errorMessage}</span>
                        <label htmlFor={formId}>{this.props.name}</label>
                    </div>
                </div>
                <br />
            </BaseView>
        );
        inputs.push(emptyInput);

        return(
            <BaseView>
                <div className="row mb-3">
                    <div className="col-sm-2 text-end">
                        <label htmlFor={formId} className="col-form-label my-2">
                            <strong>{this.props.name}</strong>
                        </label>
                    </div>
                    <div className="col-sm-9">
                        {inputs}
                    </div>
                </div>
            </BaseView>
        );
    }
}

export default FormTypeMultiplePopupSelectionView;