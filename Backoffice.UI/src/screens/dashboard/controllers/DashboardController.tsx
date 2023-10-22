import BoxView from "../views/BoxView";
import Controller from "../../../presentation/controllers/Controller";
import DashboardProps from "../props/DashboardProps";
import DashboardState from "../props/DashboardState";
import MessageView from "../views/MessageView";
import MessagesResponseModel from "../models/MessagesResponseModel";
import React from "react";

class DashboardController extends Controller<DashboardProps, DashboardState> {

    private _isMounted: boolean = false;

    constructor(props: DashboardProps) {
        super(props);

        this.state = new DashboardState();
    }

    public componentDidMount?(): void {
        this._isMounted = true;
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MESSAGES_CONTROLLER_NAME}/ListMessages`;
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
                <div className="content-wrapper">
                    <section className="content">
                        <div className="row">
                            <BoxView backgroundStyle="bg-aqua" 
                            title="Products"
                            value="0"
                            iconName="fa-shopping-bag" />
                            <BoxView backgroundStyle="bg-yellow" 
                            title="User Registrations"
                            value="0"
                            iconName="fa-user-check" />
                        </div>
                        <div className="row">
                            <div className="col-md-6">
                                <div className="box box-primary">
                                    <div className="box-header with-border">
                                        <h3 className="box-title">Announcements</h3>
                                    </div>
                                    <div className="box-body no-padding">
                                        <div className="table-responsive mailbox-messages">
                                            <table className="table table-hover table-striped">
                                                <tbody>{messages}</tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                </div>
            </React.StrictMode>
        );
    }
}

export default DashboardController;
