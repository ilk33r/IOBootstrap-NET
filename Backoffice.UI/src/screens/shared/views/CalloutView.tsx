import CalloutProps from '../props/CalloutProps';
import CalloutState from '../props/CalloutState';
import CalloutViewPresenter from '../../../presentation/inerfaces/CalloutViewPresenter';
import React from 'react';
import View from '../../../presentation/views/View';

class CalloutView extends View<CalloutProps, CalloutState> implements CalloutViewPresenter {

    constructor(props: CalloutProps) {
        super(props);

        this.state = new CalloutState();
    }

    public show(type: string, title: string, message: string): void {
        const state = new CalloutState();
        state.className = "callout fade in " + type;
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
        state.className = "callout fade out hidden";
        state.title = "";
        state.message = "";

        this.setState(state);
    }

    render() {        
        return (
            <React.StrictMode>
                <div id="callout" className={this.state.className}>
                    <h4>{this.state.title}</h4>
                    <p>{this.state.message}</p>
                </div>
            </React.StrictMode>
        );
    }
}

export default CalloutView;
