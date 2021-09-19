import FormElement from "../interfaces/FormElement";
import FormTypePopupSelectionProps from "../props/FormTypePopupSelectionProps";
import FormViewState from "../props/FormViewState";
import Validatable from "../../../presentation/inerfaces/Validatable";
import View from "../../../presentation/views/View";
import React from "react";
import WindowMessageModel from "../../../common/models/WindowMessageModel";

type MessageEvent = { data: WindowMessageModel; };

class FormTypePopupSelectionView extends View<FormTypePopupSelectionProps, FormViewState> implements FormElement, Validatable {

    private _formValue: string;
    private _formSelectedItemId: number;
    private _openedWindow: WindowProxy | null;

    constructor(props: FormTypePopupSelectionProps) {
        super(props);

        const newState = new FormViewState();
        newState.inputValue = this.props.value;

        this.state = newState;

        this._formValue = this.props.value;
        this._formSelectedItemId = this.props.selectedItemId;
        this._openedWindow = null;
        this.handleValueChange = this.handleValueChange.bind(this);
        this.handleInputClick = this.handleInputClick.bind(this);
    }

    componentDidMount?() {
        const weakSelf = this;
        $(window).on("message", function (e) {
            if (weakSelf._openedWindow !== null) {
                weakSelf._openedWindow.close();
            }
            
            if (e.originalEvent !== undefined) {
                const originalEvent = e.originalEvent as unknown as MessageEvent;
                weakSelf._formSelectedItemId = originalEvent.data.itemID;

                const newState = new FormViewState();
                newState.inputValue = originalEvent.data.itemValue;
                
                weakSelf.setState(newState);
            }
        });
    }

    public getValue(): string {
        return this._formSelectedItemId.toString();
    }

    handleValueChange(event: { target: { value: string; }; }) {
        this._formValue = event.target.value;
    }

    handleInputClick(event: { preventDefault: () => void; }) {
        const pageURL = `${process.env.REACT_APP_BACKOFFICE_PAGE_URL}#!${this.props.selectionURL}`;
        const pageHash = `#!${this.props.selectionURL}`;
        this._openedWindow = window.open(pageURL, pageHash, 'width=1224,height=640,top=60,left=60,menubar=0,status=0,titlebar=0');
    }

    validate(): boolean {
        let validated = true;
        let errorMessage = "";
        let errorTitle= "";
        const weakSelf = this;

        this.props.validations.forEach(rule => {
            if (!rule.validationResult(weakSelf._formSelectedItemId.toString())) {
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
                        <input type={this.props.inputType} id={formId} className="form-control" value={this.state.inputValue} placeholder={this.props.name} onChange={this.handleValueChange} onClick={this.handleInputClick} disabled={!this.props.isEnabled} contentEditable={false} />
                        <span className={errorMessageClass}>{this.state.errorMessage}</span>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default FormTypePopupSelectionView;
