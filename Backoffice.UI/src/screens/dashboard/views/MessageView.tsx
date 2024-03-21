import { View } from "iobootstrap-ui-base";
import MessageViewProps from "../props/MessageViewProps";
import React from "react";

class MessageView extends View<MessageViewProps, {}> {

    render() {
        const message = (this.props.messageModel.message === undefined) ? "" : this.props.messageModel.message.replace(/\n/g, "<br />");
        const createDate = (this.props.messageModel.messageCreateDate === undefined) ? new Date() : new Date(this.props.messageModel.messageCreateDate);
        const iconClass = (this.props.isReaded) ? "fa fa-star-o text-yellow" : "fa fa-star text-yellow"
        return (
            <React.StrictMode>
                <tr>
                    <td className="mailbox-star"><a href="#unstar" className="messageReadControl"><i className={iconClass}></i></a></td>
                    <td className="mailbox-subject"><div dangerouslySetInnerHTML={{__html: message}}></div></td>
                    <td className="mailbox-date">{createDate.toLocaleDateString()}</td>
                </tr>
            </React.StrictMode>
        );
    }
}

export default MessageView;
