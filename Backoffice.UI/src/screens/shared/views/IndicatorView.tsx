import IndicatorProps from '../props/IndicatorProps';
import IndicatorState from '../props/IndicatorState';
import IndicatorViewPresenter from '../../../presentation/inerfaces/IndicatorViewPresenter';
import View from '../../../presentation/views/View';
import React from 'react';

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
        const indicatorClassName = (this.state.isVisible) ? "" : "hidden";
        return (
            <React.StrictMode>
                <div id="pageIndicator" className={indicatorClassName}>
                    <div className="overlay">
                        <i className="fa fa-spinner fa-spin fa-3x"></i>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default IndicatorView;
