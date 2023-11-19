import React from "react";
import CodeBlockViewProps from "../props/CodeBlockViewProps";
import View from "../../../presentation/views/View";

class CodeBlockView extends View<CodeBlockViewProps, {}> {

    render() {
        const descriptions = this.props.descriptions.map(description => {
            return (<small>{description}</small>)
        })

        return (
            <React.StrictMode>
                <div className="col-md-6">
                    <div className="box box-solid">
                        <div className="box-header with-border">
                            <i className="fa fa-code"></i>
                            <h3 className="box-title">{this.props.title}</h3>
                        </div>
                        <div className="box-body">
                            <blockquote>
                                <p>{this.props.sectionTitle}</p>
                                {descriptions}
                            </blockquote>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default CodeBlockView;
  