/* eslint-disable jsx-a11y/alt-text */
import { Validatable, View } from "iobootstrap-ui-base";
import FormElement from "../interfaces/FormElement";
import FormTypeImageProps from "../props/FormTypeImageProps";
import FormViewState from "../props/FormViewState";
import React from "react";

class FormTypeImageView extends View<FormTypeImageProps, FormViewState> implements FormElement, Validatable {

    private _formValue: string;
    private _fileValue: Blob | null;

    constructor(props: FormTypeImageProps) {
        super(props);

        const newState = new FormViewState();
        newState.imagePreviewURL = this.props.value;
        this.state = newState;

        this._formValue = this.props.value;
        this._fileValue = null;
        this.handleDeleteImage = this.handleDeleteImage.bind(this);
        this.handleValueChange = this.handleValueChange.bind(this);
    }

    public getValue(): string | null {
        return null;
    }

    public getBlobValue(): Blob | null {
        return this._fileValue;
    }

    handleDeleteImage(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();

        if (this._formValue.length > 0) {
            this._formValue = "";
            const newState = new FormViewState();
            newState.imagePreviewURL = "";

            this.setState(newState);

            if (this.props.errorHandler != null) {
                this.props.errorHandler("deleteImage", "deleteImage");
            }
        }
    }

    handleValueChange(event: React.ChangeEvent<HTMLInputElement>) {
        const files = event.target.files;

        if (files == null || files.length === 0) {
            return;
        }

        const selectedFile = files[0];
        this._fileValue = selectedFile;
        
        const fileReader = new FileReader();
        const fileType = selectedFile.type;
        const weakSelf = this;

        fileReader.onload = function() {
            const arrayBuffer = this.result;
            if (arrayBuffer == null) {
                return;
            }

            const castedBuffer = arrayBuffer as ArrayBuffer;
            const fileData = new Uint8Array(castedBuffer);
            
            let binary = "";
            const byteLength = fileData.byteLength;

            for (let i = 0; i < byteLength; i++) {
                binary += String.fromCharCode(fileData[i]);
            }

            const b64EncodedImage = btoa(binary);

            const newState = new FormViewState();
            newState.imagePreviewURL = "data:" + fileType + ";base64," + b64EncodedImage;
            weakSelf._formValue = newState.imagePreviewURL;

            weakSelf.setState(newState);
        };

        fileReader.readAsArrayBuffer(selectedFile);
    }

    validate(): boolean {
        let validated = true;
        let errorMessage = "";
        let errorTitle= "";
        const weakSelf = this;

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

        let inputDisabled = !this.props.isEnabled;
        let imageFileAddress;
        let deleteButtonClassName;

        if (this.props.value === this._formValue) {
            imageFileAddress = this.props.value;

            if (this.props.value.length > 0) {
                inputDisabled = true;
                deleteButtonClassName = "";
            } else {
                inputDisabled = false;
                deleteButtonClassName = "hidden";
            }
        } else if (this.state.imagePreviewURL.length > 0) {
            imageFileAddress = this.state.imagePreviewURL;
            inputDisabled = true;
            deleteButtonClassName = "";
        } else {
            inputDisabled = false;
            deleteButtonClassName = "hidden";
        }

        return(
            <React.StrictMode>
                <div className={areaClass}>
                    <label htmlFor={formId} className="col-sm-2 control-label">{this.props.name}</label>
                    <div className="col-sm-9">
                        <input type="file" name="file" className="form-control" onChange={this.handleValueChange} disabled={inputDisabled} />
                        <span className={errorMessageClass}>{this.state.errorMessage}</span>
                    </div>
                    <div className="col-sm-2"></div>
                    <div className="col-sm-3">
                        <img src={imageFileAddress} width="100%" />
                    </div>
                    <div className="col-sm-1">
                        <a href="#deleteImage" className={deleteButtonClassName} onClick={this.handleDeleteImage}><i className="fa fa-trash"></i> Delete</a>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default FormTypeImageView;
