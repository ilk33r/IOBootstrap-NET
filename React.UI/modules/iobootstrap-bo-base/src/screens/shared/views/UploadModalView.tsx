import { UploadModalViewPresenter, View } from "iobootstrap-ui-base";
import UploadModalViewProps from "../props/UploadModalViewProps";
import UploadModalViewState from "../props/UploadModalViewState";
import React from "react";

class UploadModalView extends View<UploadModalViewProps, UploadModalViewState> implements UploadModalViewPresenter {

    constructor(props: UploadModalViewProps) {
        super(props);

        this.state = new UploadModalViewState();
    }

    public present(): void {
        const newState = new UploadModalViewState();
        newState.progress = 0;

        this.setState(newState);

        if (this.props.presentHandler != null) {
            this.props.presentHandler();
        }
    }

    public setProgress(progress: number): void {
        const newState = new UploadModalViewState();
        newState.progress = progress;

        this.setState(newState);
    }

    public dismiss(): void {
        if (this.props.dismissHandler != null) {
            this.props.dismissHandler();
        }
    }

    render() {
        const progressStyle = {
            width: this.state.progress.toString() + '%'
        };

        return (
            <React.StrictMode>
                <div id="uploadModal" className="modal fade" role="dialog">
                     <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-body">
                                <div className="progress">
                                    <div className="progress-bar" data-valuenow="60" data-valuemin="0" data-valuemax="100" style={progressStyle}>
                                        {this.state.progress} %
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default UploadModalView;