import BoxView from "../views/BoxView";
import DashboardProps from "../props/DashboardProps";
import DashboardState from "../props/DashboardState";
import MessageView from "../views/MessageView";
import MessagesResponseModel from "../models/MessagesResponseModel";
import React from "react";
import { BOController } from "iobootstrap-bo-base";

class DashboardController extends BOController<DashboardProps, DashboardState> {

    private _isMounted: boolean = false;

    constructor(props: DashboardProps) {
        super(props);

        this.state = new DashboardState();
    }

    public componentDidMount?(): void {
        this._isMounted = true;
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/ListMessages`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: MessagesResponseModel) {
            if (weakSelf.handleServiceSuccess(response) && weakSelf._isMounted) {
                const newState = new DashboardState();
                newState.messages = (response.messages !== undefined) ? response.messages : [];

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    public componentWillUnmount?(): void {
        this._isMounted = false;
    }

    render() {
        const messages = this.state.messages.map(message => {
            return (<MessageView messageModel={message} isReaded={false} key={message.id} />)
        });
        return (
            <React.StrictMode>
                <section className="container-fluid">
                    <section className="content">
                        <div className="row">
                            <BoxView backgroundStyle="bg-info text-white" 
                            title="Products"
                            value="0"
                            iconName="fa-shopping-bag" />
                            <BoxView backgroundStyle="bg-warning text-white" 
                            title="User Registrations"
                            value="0"
                            iconName="fa-user-check" />
                        </div>
                        <div className="row">
                            <div className="col-md-6">
                                <div className="box">
                                    <div className="box-header">
                                        <h3>Announcements</h3>
                                    </div>
                                    <div className="box-body">
                                        <div className="hstack gap-3">
                                            <table className="table table-bordered table-hover table-striped ann-table">
                                                <tbody>{messages}</tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                </section>
            </React.StrictMode>
        );
    }
}

export default DashboardController;
