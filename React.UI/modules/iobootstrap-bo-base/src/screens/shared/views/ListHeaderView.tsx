import { View } from "iobootstrap-ui-base";
import ListHeaderViewProps from "../props/ListHeaderViewProps";
import ListDataFilterTypes from "../models/ListDataFilterTypes";
import React from "react";
import ListHeaderViewState from "../props/ListHeaderViewState";

class ListHeaderView extends View<ListHeaderViewProps, ListHeaderViewState> {

    constructor(props: ListHeaderViewProps) {
        super(props);

        this.state = new ListHeaderViewState();
        this.clearFilterClicked = this.clearFilterClicked.bind(this);
        this.onSelectionChange = this.onSelectionChange.bind(this);
        this.onTextChange = this.onTextChange.bind(this);
    }

    private clearFilterClicked(event: React.MouseEvent<HTMLButtonElement>) {
        event.preventDefault();

        if (this.props.clearFilterHandler != null) {
            this.props.clearFilterHandler();
        }

        this.setState({
            filterIndex: null,
            filterWord: null
        });
    }

    private onSelectionChange(event: React.ChangeEvent<HTMLSelectElement>, index: number) {
        event.preventDefault();

        const selectionValue = event.target.value;
        if (this.props.filterHandler != null) {
            this.props.filterHandler(index, selectionValue);
        }

        this.setState({
            filterIndex: index,
            filterWord: selectionValue
        });
    }

    private onTextChange(event: React.ChangeEvent<HTMLInputElement>, index: number) {
        event.preventDefault();

        const inputValue = event.target.value;
        if (inputValue.length <= 2) {
            return
        }

        if (this.props.filterHandler != null) {
            this.props.filterHandler(index, inputValue);
        }

        this.setState({
            filterIndex: index,
            filterWord: inputValue
        });
    }
    
    render() {
        const weakSelf = this;
        const headers = this.props.headers.map((header, headerIndex) => {
            const key = "headerIndex" + headerIndex;
            let headerNode: React.ReactNode;

            if (header.filterType == ListDataFilterTypes.None) {
                headerNode = (
                    <React.StrictMode>
                        {header.title}
                    </React.StrictMode>
                );
            } else {
                let formNode: React.ReactNode;

                if (header.filterType == ListDataFilterTypes.Input) {
                    formNode = (
                        <form className="dropdown-menu p-4">
                            <div className="mb-3">
                                <input type="text" className="form-control" onChange={(e) => this.onTextChange(e, headerIndex)}/>
                            </div>
                            <button type="button" className="btn btn-secondary btn-sm" onClick={(e) => this.clearFilterClicked(e)}>Reset</button>
                        </form>
                    );
                } else if (header.filterType == ListDataFilterTypes.Select) {
                    const customFilters = header.customFilters ?? [];
                    const uniqueFilters = [...new Set(customFilters)]
                    const selectionNode = uniqueFilters.map((filter, index) => {
                        const key = `filter-${headerIndex}-${index}`;
                        return (<option key={key} value={filter}>{filter}</option>)
                    });

                    formNode = (
                        <form className="dropdown-menu p-4" onSubmit={(e) => e.preventDefault() }>
                            <div className="mb-3">
                                <select className="form-control" defaultValue={weakSelf.state.filterWord ?? ""} onChange={(e) => this.onSelectionChange(e, headerIndex)}>
                                    {selectionNode}
                                </select>
                            </div>
                            <button type="button" className="btn btn-secondary btn-sm" onClick={(e) => this.clearFilterClicked(e)}>Reset</button>
                        </form>
                    );
                } else {
                    formNode = (<React.StrictMode></React.StrictMode>);
                }

                let buttonBadgeNode: React.ReactNode;
                if (this.state.filterIndex != null && this.state.filterIndex == headerIndex) {
                    buttonBadgeNode = (
                        <span className="position-absolute top-0 start-100 translate-middle p-2 bg-danger border border-light rounded-circle">
                            <span className="visually-hidden">New alerts</span>
                        </span>
                    );
                } else {
                    buttonBadgeNode = (<React.StrictMode></React.StrictMode>);
                }

                headerNode = (
                    <React.StrictMode>
                        {header.title} <button type="button" className="btn btn-outline-info btn-sm dropdown-toggle position-relative" data-bs-toggle="dropdown" aria-expanded="false" data-bs-auto-close="outside">{buttonBadgeNode}</button>
                        {formNode}
                    </React.StrictMode>
                );
            }

            return (
                <th key={key}>
                    <div className="hstack gap-2">
                        {headerNode}
                    </div>
                </th>
            );
        });

        return headers;
    }
}

export default ListHeaderView;