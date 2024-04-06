import DashboardController from "../../dashboard/controllers/DashboardController";
import NavigationProps from "../props/NavigationProps";
import NavigationState from "../props/NavigationState";
import React from "react";
import { View } from "iobootstrap-ui-base";
import { UserChangePasswordController, UsersAddController, UsersDeleteController, UsersListController, UsersLogoutController, UsersUpdateController } from "iobootstrap-bo-users";
import { ConfigurationsAddController, ConfigurationsDeleteController, ConfigurationsListController, ConfigurationsResetCacheController, ConfigurationsUpdateController } from "iobootstrap-bo-configurations";
import { MessageListController, MessagesAddController, MessagesDeleteController, MessagesUpdateController } from "iobootstrap-bo-messages";
import { MenuEditorAddController, MenuEditorDeleteController, MenuEditorListController, MenuEditorSelectionController, MenuEditorUpdateController } from "iobootstrap-bo-menueditor";
import { PushNotificationDeleteController, PushNotificationListController, PushNotificationSendController } from "iobootstrap-bo-pushnotifications";
import { GenerateBOPageController } from "iobootstrap-bo-generatebopage";
import { ImagesAddController, ImagesEditController, ImagesModifyController } from "iobootstrap-bo-images";

class NavigationView extends View<NavigationProps, NavigationState> {

    constructor(props: NavigationProps) {
        super(props);

        this.state = new NavigationState();
    }

    render() {
        if (this.props.pageHash === "clientsList") {
            return <ClientListController />
        }

        if (this.props.pageHash === "clientsAdd") {
            return <ClientsAddController />
        }

        if (this.props.pageHash === "clientsUpdate") {
            return <ClientUpdateController />
        }

        if (this.props.pageHash === "clientsDelete") {
            return <ClientDeleteController />
        }

        if (this.props.pageHash === "selection/clientsSelect") {
            return <ClientSelectController />
        }

        if (this.props.pageHash === "configurationsList") {
            return <ConfigurationsListController />
        }

        if (this.props.pageHash === "configurationsAdd") {
            return <ConfigurationsAddController />
        }

        if (this.props.pageHash === "configurationsUpdate") {
            return <ConfigurationsUpdateController />
        }

        if (this.props.pageHash === "configurationsDelete") {
            return <ConfigurationsDeleteController />
        }

        if (this.props.pageHash === "resetCache") {
            return <ConfigurationsResetCacheController />
        }

        if (this.props.pageHash === "menuEditorList") {
            return <MenuEditorListController />
        }

        if (this.props.pageHash === "menuEditorAdd") {
            return <MenuEditorAddController />
        }

        if (this.props.pageHash === "selection/menuEditorSelect") {
            return <MenuEditorSelectionController />
        }

        if (this.props.pageHash === "menuEditorUpdate") {
            return <MenuEditorUpdateController />
        }

        if (this.props.pageHash === "menuEditorDelete") {
            return <MenuEditorDeleteController />
        }

        if (this.props.pageHash === "messagesList") {
            return <MessageListController />
        }

        if (this.props.pageHash === "messagesAdd") {
            return <MessagesAddController />
        }

        if (this.props.pageHash === "messagesUpdate") {
            return <MessagesUpdateController />
        }

        if (this.props.pageHash === "messagesDelete") {
            return <MessagesDeleteController />
        }

        if (this.props.pageHash === "usersList") {
            return <UsersListController />
        }

        if (this.props.pageHash === "usersAdd") {
            return <UsersAddController />
        }

        if (this.props.pageHash === "usersUpdate") {
            return <UsersUpdateController />
        }

        if (this.props.pageHash === "usersDelete") {
            return <UsersDeleteController />
        }

        if (this.props.pageHash === "userChangePassword") {
            return <UserChangePasswordController />
        }

        if (this.props.pageHash === "usersLogout") {
            return <UsersLogoutController />
        }

        if (this.props.pageHash === "pushNotificationList") {
            return <PushNotificationListController />
        }

        if (this.props.pageHash === "pushNotificationSend") {
            return <PushNotificationSendController />
        }

        if (this.props.pageHash === "pushNotificationDelete") {
            return <PushNotificationDeleteController />
        }

        if (this.props.pageHash === "imagesEdit") {
            return <ImagesEditController />
        }

        if (this.props.pageHash === "imageAdd") {
            return <ImagesAddController />
        }

        if (this.props.pageHash === "imageModify") {
            return <ImagesModifyController />
        }

        if (this.props.pageHash === "actionGenerateBOPage") {
            return <GenerateBOPageController />
        }
        
        return (
            <React.StrictMode>
                <DashboardController />
            </React.StrictMode>
        );
    }
}

export default NavigationView;
