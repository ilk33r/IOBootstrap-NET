/* eslint-disable no-eval */
import React from "react";
import ModalInputState from "../props/ModalInputState";
import { View } from "iobootstrap-ui-base";
import { ModalInputViewHandler } from "../../../presentation/interfaces/ModalInputViewHandler";
import ModalInputViewPresenter from "../../../presentation/interfaces/ModalInputViewPresenter";
import ModalInputViewProps from "../props/ModalInputViewProps";

class ModalInputView extends View<ModalInputViewProps, ModalInputState> implements ModalInputViewPresenter {

    private inputViewHandler: ModalInputViewHandler | undefined;

    private _formValue: string;

    constructor(props: ModalInputViewProps) {
        super(props);

        this._formValue = "";
        this.state = new ModalInputState();
        this.handleValueChange = this.handleValueChange.bind(this);
        this.handleCancel = this.handleCancel.bind(this);
        this.handleDeactivate = this.handleDeactivate.bind(this);
    }

    public show(handler: ModalInputViewHandler): void {
        this.inputViewHandler = handler;
        
        if (this.props.presentHandler != null) {
            this.props.presentHandler();
        }
    }

    handleValueChange(event: { target: { value: string; }; }) {
        this._formValue = event.target.value;
    }

    handleCancel() {
        if (this.props.dismissHandler != null) {
            this.props.dismissHandler();
        }

        if (this.inputViewHandler) {
            this.inputViewHandler(null);
        }
    }

    handleDeactivate() {
        if (!this._formValue) {
            return;
        }

        if (this.props.dismissHandler != null) {
            this.props.dismissHandler();
        }

        if (this.inputViewHandler) {
            this.inputViewHandler(this._formValue);
        }
    }

    render() {        
        return (
            <React.StrictMode>
                <div className="modal fade" id="inputModal" aria-labelledby="inputModalLabel" aria-hidden="true">
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h3 className="modal-title" id="inputModalLabel">Deactivate Device</h3>
                                <button type="button" className="close" aria-label="Close" onClick={this.handleCancel}>
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div className="modal-body">
                                <div className="form-group">
                                    <label className="col-form-label">Reason:</label>
                                    <textarea className="form-control" id="deactivate-reason-text" maxLength={255} onChange={this.handleValueChange}></textarea>
                                </div>
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" onClick={this.handleCancel}>Close</button>
                                <button type="button" className="btn btn-primary" onClick={this.handleDeactivate}>Deactivate</button>
                            </div>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default ModalInputView;
