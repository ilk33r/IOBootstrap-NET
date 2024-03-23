import QuestionViewProps from "../props/QuestionViewProps";
import React from "react";
import { View } from "iobootstrap-ui-base";
import BreadcrumbView from "./BreadcrumbView";

class QuestionView extends View<QuestionViewProps, {}> {

    constructor(props: QuestionViewProps) {
        super(props);

        this.handleFormSuccess = this.handleFormSuccess.bind(this);
        this.handleFormError = this.handleFormError.bind(this);
    }

    handleFormSuccess(e: { }) {
        this.props.successHandler();
    }

    handleFormError(e: { }) {
        this.props.errorHandler();
    }

    render() {
        const noButtonStyle = {
            marginRight: '5px'
        };

        return (
            <React.StrictMode>
                <div className="content-wrapper">
                    <BreadcrumbView navigation={this.props.navigation} resourceHome={this.props.resourceHome} />
                    <section className="content">
                        <div className="row">
                            <div className="col-md-12">
                                <div className="box box-info">
                                    <div className="box-header with-border">
                                        <h3 className="box-title">{this.props.title}</h3>
                                    </div>
                                    <div className="box-body">
                                        {this.props.questionMessage}
                                    </div>
                                    <div className="box-footer">
                                        <button type="button" className="btn btn-danger pull-right" onClick={this.handleFormSuccess}>Yes</button>
                                        <button type="button" className="btn btn-success pull-right" style={noButtonStyle} onClick={this.handleFormError}>NO</button>
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

export default QuestionView;
