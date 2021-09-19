import ClientsAddController from "../../clients/controllers/ClientsAddController";
import ClientDeleteController from "../../clients/controllers/ClientDeleteController";
import ClientListController from "../../clients/controllers/ClientListController";
import ClientSelectController from "../../clients/controllers/ClientSelectController";
import ClientUpdateController from "../../clients/controllers/ClientUpdateController";
import ConfigurationsAddController from "../../configurations/controllers/ConfigurationsAddController";
import ConfigurationsDeleteController from "../../configurations/controllers/ConfigurationsDeleteController";
import ConfigurationsListController from "../../configurations/controllers/ConfigurationsListController";
import ConfigurationsResetCacheController from "../../configurations/controllers/ConfigurationsResetCacheController";
import ConfigurationsUpdateController from "../../configurations/controllers/ConfigurationsUpdateController";
import DashboardController from "../../dashboard/controllers/DashboardController";
import ImagesAddController from "../../images/controllers/ImagesAddController";
import ImagesEditController from "../../images/controllers/ImagesEditController";
import ImagesModifyController from "../../images/controllers/ImagesModifyController";
import ImagesUploadController from "../../images/controllers/ImagesUploadController";
import MenuEditorAddController from "../../menueditor/controllers/MenuEditorAddController";
import MenuEditorDeleteController from "../../menueditor/controllers/MenuEditorDeleteController";
import MenuEditorListController from "../../menueditor/controllers/MenuEditorListController";
import MenuEditorSelectionController from "../../menueditor/controllers/MenuEditorSelectionController";
import MenuEditorUpdateController from "../../menueditor/controllers/MenuEditorUpdateController";
import MessageListController from "../../messages/controllers/MessageListController";
import MessagesAddController from "../../messages/controllers/MessagesAddController";
import MessagesDeleteController from "../../messages/controllers/MessagesDeleteController";
import MessagesUpdateController from "../../messages/controllers/MessagesUpdateController";
import NavigationProps from "../props/NavigationProps";
import NavigationState from "../props/NavigationState";
import PushNotificationDeleteController from "../../pushnotifications/controllers/PushNotificationDeleteController";
import PushNotificationListController from "../../pushnotifications/controllers/PushNotificationListController";
import PushNotificationSendController from "../../pushnotifications/controllers/PushNotificationSendController";
import React from "react";
import UsersAddController from "../../users/controllers/UsersAddController";
import UserChangePasswordController from "../../users/controllers/UserChangePasswordController";
import UsersDeleteController from "../../users/controllers/UsersDeleteController";
import UsersListController from "../../users/controllers/UsersListController";
import UsersLogoutController from "../../users/controllers/UsersLogoutController";
import UsersUpdateController from "../../users/controllers/UsersUpdateController";
import View from "../../../presentation/views/View";

class NavigationView extends View<NavigationProps, NavigationState> {

    constructor(props: NavigationProps) {
        super(props);

        this.state = new NavigationState();
    }

    private updateLocation(hash: string) {
        let hashName = hash;
        if (hash.startsWith('#!')) {
            hashName = hash.substr(2, hash.length);;

            const newState = new NavigationState();
            newState.pageHash = hashName;
            this.setState(newState);
        }
    }

    public componentDidMount?(): void {
        this.updateLocation(window.location.hash);

        const weakSelf = this;
        $(window).on("hashchange", function(e) {
            weakSelf.updateLocation(e.target.location.hash);
        });

        if (window.opener != null) {
            setTimeout(() => {
                $('.sidebar-toggle').click();
            }, 1500);
        }
    }

    render() {
        if (this.state.pageHash === "clientsList") {
            return <ClientListController />
        }

        if (this.state.pageHash === "clientsAdd") {
            return <ClientsAddController />
        }

        if (this.state.pageHash === "clientsUpdate") {
            return <ClientUpdateController />
        }

        if (this.state.pageHash === "clientsDelete") {
            return <ClientDeleteController />
        }

        if (this.state.pageHash === "clientsSelect") {
            return <ClientSelectController />
        }

        if (this.state.pageHash === "configurationsList") {
            return <ConfigurationsListController />
        }

        if (this.state.pageHash === "configurationsAdd") {
            return <ConfigurationsAddController />
        }

        if (this.state.pageHash === "configurationsUpdate") {
            return <ConfigurationsUpdateController />
        }

        if (this.state.pageHash === "configurationsDelete") {
            return <ConfigurationsDeleteController />
        }

        if (this.state.pageHash === "resetCache") {
            return <ConfigurationsResetCacheController />
        }

        if (this.state.pageHash === "menuEditorList") {
            return <MenuEditorListController />
        }

        if (this.state.pageHash === "menuEditorAdd") {
            return <MenuEditorAddController />
        }

        if (this.state.pageHash === "menuEditorSelect") {
            return <MenuEditorSelectionController />
        }

        if (this.state.pageHash === "menuEditorUpdate") {
            return <MenuEditorUpdateController />
        }

        if (this.state.pageHash === "menuEditorDelete") {
            return <MenuEditorDeleteController />
        }

        if (this.state.pageHash === "messagesList") {
            return <MessageListController />
        }

        if (this.state.pageHash === "messagesAdd") {
            return <MessagesAddController />
        }

        if (this.state.pageHash === "messagesUpdate") {
            return <MessagesUpdateController />
        }

        if (this.state.pageHash === "messagesDelete") {
            return <MessagesDeleteController />
        }

        if (this.state.pageHash === "usersList") {
            return <UsersListController />
        }

        if (this.state.pageHash === "usersAdd") {
            return <UsersAddController />
        }

        if (this.state.pageHash === "usersUpdate") {
            return <UsersUpdateController />
        }

        if (this.state.pageHash === "usersDelete") {
            return <UsersDeleteController />
        }

        if (this.state.pageHash === "userChangePassword") {
            return <UserChangePasswordController />
        }

        if (this.state.pageHash === "usersLogout") {
            return <UsersLogoutController />
        }

        if (this.state.pageHash === "pushNotificationList") {
            return <PushNotificationListController />
        }

        if (this.state.pageHash === "pushNotificationSend") {
            return <PushNotificationSendController />
        }

        if (this.state.pageHash === "pushNotificationDelete") {
            return <PushNotificationDeleteController />
        }

        if (this.state.pageHash === "imagesEdit") {
            return <ImagesEditController />
        }

        if (this.state.pageHash === "imageAdd") {
            return <ImagesAddController />
        }

        if (this.state.pageHash === "imageUploadFile") {
            return <ImagesUploadController />
        }

        if (this.state.pageHash === "imageModify") {
            return <ImagesModifyController />
        }

        return (
            <React.StrictMode>
                <DashboardController />
            </React.StrictMode>
        );
    }
}

export default NavigationView;
