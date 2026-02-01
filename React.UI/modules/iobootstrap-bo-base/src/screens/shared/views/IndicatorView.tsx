import IndicatorProps from '../props/IndicatorProps';
import IndicatorState from '../props/IndicatorState';
import { BaseView, IndicatorViewPresenter, View } from "iobootstrap-ui-base";

class IndicatorView extends View<IndicatorProps, IndicatorState> implements IndicatorViewPresenter {

    constructor(props: IndicatorProps) {
        super(props);

        this.state = new IndicatorState();
    }

    public present(): void {
        const newState = new IndicatorState();
        newState.isVisible = true;

        this.setState(newState);
    }

    public dismiss(): void {
        const newState = new IndicatorState();
        newState.isVisible = false;

        this.setState(newState);
    }

    render() {
        const indicatorClassName = (this.state.isVisible) ? "" : "d-none";
        return (
            <BaseView>
                <div id="pageIndicator" className={indicatorClassName}>
                    <div className="overlay"></div>
                    <div className="spinner-container text-center">
                        <div className="spinner-border text-light" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    </div>
                </div>
            </BaseView>
        );
    }
}

export default IndicatorView;
