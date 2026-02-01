import CodeBlockViewProps from "../props/CodeBlockViewProps";
import { BaseView, View } from "iobootstrap-ui-base";

class CodeBlockView extends View<CodeBlockViewProps, {}> {

    render() {
        const descriptions = this.props.descriptions.map(description => {
            return (<small className="d-block mb-1">{description}</small>)
        })

        return (
            <BaseView>
                <div className="col-md-6">
                    <div className="box">
                        <div className="box-header">
                            <h3><i className="fa fa-code"></i> {this.props.title}</h3>
                        </div>
                        <div className="box-body">
                            <figure>
                                <blockquote className="blockquote">
                                    <p>{this.props.sectionTitle}</p>
                                    {descriptions}
                                </blockquote>
                            </figure>
                        </div>
                    </div>
                </div>
            </BaseView>
        );
    }
}

export default CodeBlockView;
  