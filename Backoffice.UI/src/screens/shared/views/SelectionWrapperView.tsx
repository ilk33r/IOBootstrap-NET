import { View, WindowMessageModel } from "iobootstrap-ui-base";
import SelectionWrapperProps from "../props/SelectionWrapperProps";
import SelectionWrapperState from "../props/SelectionWrapperState";
import React from "react";
import NavigationView from "./NavigationView";

class SelectionWrapperView extends View<SelectionWrapperProps, SelectionWrapperState> {

    constructor(props: SelectionWrapperProps) {
        super(props);

        this.state = new SelectionWrapperState();

        this.handleClose = this.handleClose.bind(this);
        this.closeButtonClicked = this.closeButtonClicked.bind(this);
        this.trashButtonClicked = this.trashButtonClicked.bind(this);
    }

    public componentDidMount(): void {
        const weakSelf = this;
        $(window).on("message", function (e) {            
            if (e.originalEvent !== undefined) {
                const originalEvent = e.originalEvent as unknown as MessageEvent;

                if (originalEvent === undefined || originalEvent.data.name === undefined) {
                    return;
                }

                if (originalEvent.data.name === "closeSelection") {
                    weakSelf.handleClose();
                }
            }
        });
    }

    private handleClose() {
        window.location.hash = "#!" + this.props.pageHash;
    }

    private closeButtonClicked(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();
        this.handleClose();
    }

    private trashButtonClicked(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();
        
        const windowMessage: WindowMessageModel = {
            name: "itemSelected",
            itemID: null, 
            itemValue: null
        };

        const baseURL = new URL(process.env.REACT_APP_POST_MESSAGE_URL ?? "");
        if (baseURL !== null && baseURL.host.length > 0) {
            window.postMessage(windowMessage, baseURL.origin);
            this.handleClose();
        }
    }
    
    render() {
        if (this.props.selectionHash != null) {
            return (
                <React.StrictMode>
                    <div className="selection-content container-fluid">
                        <div className="overlay" onClick={this.handleClose}></div>
                        <a className="icon-link icon-link-hover link-underline-opacity-0 link-light fs-3 close" onClick={this.closeButtonClicked} href="#root">
                            <i className="fas fa-circle-xmark" aria-hidden="true"></i>
                        </a>
                        <a className="icon-link icon-link-hover link-underline-opacity-0 link-light fs-3 delete" onClick={this.trashButtonClicked} href="#root">
                            <i className="fas fa-trash" aria-hidden="true"></i>
                        </a>
                        <div className="content">
                            <NavigationView pageHash={this.props.selectionHash} />
                        </div>
                    </div>
                </React.StrictMode>
            );
        }

        return (
            <React.StrictMode>
            </React.StrictMode>
        );
    }
}

export default SelectionWrapperView;
