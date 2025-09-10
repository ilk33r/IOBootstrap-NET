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
        return (
            <React.StrictMode>
                <section className="container-fluid">
                    <BreadcrumbView navigation={this.props.navigation} resourceHome={this.props.resourceHome} showTitle={false} />
                    <div className="row mb-5 mt-2">
                        <div className="col-12">
                            <div className="box">
                                <div className="box-header">
                                    <h3>{this.props.title}</h3>
                                </div>
                                <div className="box-body">
                                    <p className="fs-5">{this.props.questionMessage}</p>
                                </div>
                                <div className="box-footer text-end">
                                    <button type="button" className="btn btn-danger me-2" onClick={this.handleFormSuccess}>Yes</button>
                                    <button type="button" className="btn btn-success" onClick={this.handleFormError}>NO</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                    <div className="row mb-5"></div>
                </section>
            </React.StrictMode>
        );
    }
}

export default QuestionView;
