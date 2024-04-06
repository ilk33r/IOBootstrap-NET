import { View } from "iobootstrap-ui-base";
import SelectionWrapperProps from "../props/SelectionWrapperProps";
import SelectionWrapperState from "../props/SelectionWrapperState";
import React from "react";
import NavigationView from "./NavigationView";

class SelectionWrapperView extends View<SelectionWrapperProps, SelectionWrapperState> {

    constructor(props: SelectionWrapperProps) {
        super(props);

        this.state = new SelectionWrapperState();

        this.handleClose = this.handleClose.bind(this);
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

    render() {
        if (this.props.selectionHash != null) {
            return (
                <React.StrictMode>
                    <div className="wrapper selectionContent">
                        <div className="overlayClick" onClick={this.handleClose}>
                            <button type="button" className="close" onClick={this.handleClose}><span aria-hidden="true">&times;</span></button>
                        </div>
                        <div className="overlay">
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
