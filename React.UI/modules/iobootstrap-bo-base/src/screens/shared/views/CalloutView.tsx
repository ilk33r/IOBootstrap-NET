import CalloutProps from '../props/CalloutProps';
import CalloutState from '../props/CalloutState';
import React from 'react';
import { BaseView, CalloutViewPresenter, View } from 'iobootstrap-ui-base';

class CalloutView extends View<CalloutProps, CalloutState> implements CalloutViewPresenter {

    constructor(props: CalloutProps) {
        super(props);

        this.state = new CalloutState();
        this.handleDismissButton = this.handleDismissButton.bind(this);
    }

    public show(type: string, title: string, message: string): void {
        const state = new CalloutState();
        state.className = "alert alert-dismissible fade show " + type;
        state.title = title;
        state.message = message;

        this.setState(state);

        const weakSelf = this;
        setTimeout(function () {
            weakSelf.dismiss();
        }, 7000);
    }

    public dismiss(): void {
        const state = new CalloutState();
        state.className = "alert alert-dismissible d-none";
        state.title = "";
        state.message = "";

        this.setState(state);
    }

    private handleDismissButton(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        this.dismiss();
    }

    render() {   
        const titleClassName = (this.state.title.length > 0) ? "d-block" : "d-none";
        return (
            <BaseView>
                <div id="callout" className={this.state.className} role="alert">
                    <div className="hstack gap-3">
                        <div>
                            <i className="fas fa-triangle-exclamation" aria-hidden="true"></i>
                            <i className="fas fa-circle-exclamation" aria-hidden="true"></i>
                            <i className="fas fa-circle-check" aria-hidden="true"></i>
                            <i className="fas fa-circle-info" aria-hidden="true"></i>
                        </div>
                        <div>
                            <strong className={titleClassName}>{this.state.title}</strong>
                            <p className="mb-0">{this.state.message}</p>
                        </div>
                        <button type="button" className="btn-close" aria-label="Close" onClick={this.handleDismissButton}></button>
                    </div>
                </div>
            </BaseView>
        );
    }
}

export default CalloutView;
