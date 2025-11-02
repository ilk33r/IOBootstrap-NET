/* eslint-disable jsx-a11y/anchor-is-valid */
import ListViewProps from "../props/ListViewProps";
import React from "react";
import { View } from "iobootstrap-ui-base";
import PaginationView from "./PaginationView";
import BreadcrumbView from "./BreadcrumbView";
import ListHeaderView from "./ListHeaderView";
import ListViewState from "../props/ListViewState";

class ListView extends View<ListViewProps, ListViewState> {

    constructor(props: ListViewProps) {
        super(props);

        this.state = new ListViewState();

        this.handleItemDeleteClick = this.handleItemDeleteClick.bind(this);
        this.handleItemSelect = this.handleItemSelect.bind(this);
        this.handleItemUpdateClick = this.handleItemUpdateClick.bind(this);
        this.handlePagination = this.handlePagination.bind(this);
        this.handleFilter = this.handleFilter.bind(this);
        this.handleClearFilter = this.handleClearFilter.bind(this);
    }

    private handleItemDeleteClick(e: {}, itemIndex: number) {
        if (this.props.deleteDataHandler != null) {
            const realIndex = this.state.filteredItemIndexes != null ? this.state.filteredItemIndexes[itemIndex] : itemIndex;
            this.props.deleteDataHandler(realIndex);
        }
    }

    private handleItemSelect(e: {}, itemIndex: number) {
        if (this.props.selectDataHandler != null) {
            const realIndex = this.state.filteredItemIndexes != null ? this.state.filteredItemIndexes[itemIndex] : itemIndex;
            this.props.selectDataHandler(realIndex);
        }
    }

    private handleItemUpdateClick(e: {}, itemIndex: number) {
        if (this.props.updateDataHandler != null) {
            const realIndex = this.state.filteredItemIndexes != null ? this.state.filteredItemIndexes[itemIndex] : itemIndex;
            this.props.updateDataHandler(realIndex);
        }
    }

    private handlePagination(start: number, length: number) {
        if (this.props.pagination != null && this.props.pagination.pageClickHandler != null) {
            this.props.pagination.pageClickHandler(start, length);
        }
    }

    private handleFilter(index: number, word: string) {
        let filteredItemIndexes: number[] = [];
        const filteredItems = this.props.items.filter((it, idx) => {
            const filterStatus = it.itemList[index].toLowerCase().includes(word.toLocaleLowerCase());
            
            if (filterStatus) {
                filteredItemIndexes.push(idx);
            }
            
            return filterStatus;
        });

        this.setState({
            items: filteredItems,
            filteredItemIndexes: filteredItemIndexes
        });
    }

    private handleClearFilter() {
        this.setState({
            items: null,
            filteredItemIndexes: null
        });
    }

    render() {
        const updateClass = (this.props.updateDataHandler != null) ? "btn btn-square edit" : "btn btn-square edit d-none";
        const deleteClass = (this.props.deleteDataHandler != null) ? "btn btn-square delete" : "btn btn-square delete d-none";
        const selectionClass = (this.props.selectDataHandler != null) ? "btn btn-square select" : "btn btn-square select d-none";

        const footers = this.props.headers.map((footer, footerIndex) => {
            const key = "footerIndex" + footerIndex;
            return (<th key={key}>{footer.title}</th>);
        });

        const listItems = this.state.items ?? this.props.items;
        const listData = listItems.map((listItem, itemIndex) => {
            const listDataColumn = listItem.itemList.map((itemColumn, columnIndex) => {
                const key = "columnIndex" + columnIndex.toString();
                const textWrap = (listItem.textWraps.length > columnIndex) ? listItem.textWraps[columnIndex] : true;

                if (textWrap) {
                    return (<td key={key}><div className="text-wrap text-break" dangerouslySetInnerHTML={{__html: itemColumn}}></div></td>);
                } else {
                    return (<td key={key}><div dangerouslySetInnerHTML={{__html: itemColumn}}></div></td>);
                }
            });

            const rowClass = (listItem.isChild) ? "table-warning" : "";
            const optionsColumnKey = "itemOptions" + itemIndex;
            const itemKey = "itemIndex" + itemIndex.toString();

            let itemUpdateClass = updateClass;
            let itemDeleteClass = deleteClass;
            let itemSelectionClass = selectionClass;
            if (this.props.itemVisibleHandler != null) {
                if (!this.props.itemVisibleHandler(itemIndex, 0)) {
                    itemUpdateClass = "btn btn-square edit d-none";
                }
                
                if (!this.props.itemVisibleHandler(itemIndex, 1)) {
                    itemDeleteClass = "btn btn-square delete d-none";
                }

                if (!this.props.itemVisibleHandler(itemIndex, 2)) {
                    itemSelectionClass = "btn btn-square select d-none";
                }
            }

            let itemExtras;
            if (this.props.extras != null && this.props.extras.length > 0) {
                itemExtras = this.props.extras.map((itemExtra, itemExtraIndex) => {
                    const iconClassName = "fa " + itemExtra.icon;
                    const key = "itemExtraKey" + itemIndex.toString() + itemExtra.name;
                    let itemExtraClass = "btn btn-square general";
                    if (this.props.itemVisibleHandler != null) {
                        if (!this.props.itemVisibleHandler(itemIndex, itemExtraIndex + 3)) {
                            itemExtraClass = "btn btn-square general d-none";
                        }
                    }

                    return (
                        <a className={itemExtraClass} key={key} onClick={() => itemExtra.itemSelectionHandler(itemIndex)}>
                            <i className={iconClassName}></i><span className="btn-label">{itemExtra.name}</span>
                        </a>
                    );
                });
            } else {
                itemExtras = (<React.StrictMode></React.StrictMode>);
            }

            const optionsTextWrap = (listItem.textWraps.length > listItem.itemList.length) ? listItem.textWraps[listItem.itemList.length] : false;
            const optionsTextWrapClassName = (optionsTextWrap) ? "" : "text-nowrap";
            return (
                <tr className={rowClass} key={itemKey}>
                    {listDataColumn}
                    <td key={optionsColumnKey} className={optionsTextWrapClassName}>
                        <div className="cell-actions">
                            <a className={itemUpdateClass} onClick={(e) => this.handleItemUpdateClick(e, itemIndex)}>
                                <i className="fa fa-edit"></i><span className="btn-label">{this.props.resourceEdit}</span>
                            </a>
                            <a className={itemDeleteClass} onClick={(e) => this.handleItemDeleteClick(e, itemIndex)}>
                                <i className="fa fa-trash"></i><span className="btn-label">{this.props.resourceDelete}</span>
                            </a>
                            <a className={itemSelectionClass} onClick={(e) => this.handleItemSelect(e, itemIndex)}>
                                <i className="fa fa-check"></i><span className="btn-label">{this.props.resourceSelect}</span>
                            </a>
                            {itemExtras}
                        </div>
                    </td>
                </tr>
            );
        });

        let pagination;
        if (this.props.pagination == null) {
            pagination = (<React.StrictMode></React.StrictMode>);
        } else {
            pagination = (<PaginationView start={this.props.pagination.start}
                            length={this.props.pagination.length}
                            count={this.props.pagination.count}
                            pageChangeHandler={this.handlePagination} />);
        }

        return (
            <React.StrictMode>
                <section className="container-fluid">
                    <BreadcrumbView navigation={this.props.navigation} resourceHome={this.props.resourceHome} showTitle={true} />
                    <section className="content">
                        <div className="row">
                            <div className="col-12">
                                <div className="box">
                                    <div className="box-body">
                                        <table className="table table-bordered table-hover table-striped table-sticky">
                                            <thead>
                                                <tr>
                                                    <ListHeaderView 
                                                    headers={this.props.headers}
                                                    filterHandler={this.handleFilter}
                                                    clearFilterHandler={this.handleClearFilter}
                                                    />
                                                    <th key="headerOptions">{this.props.resourceOptions}</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {listData}
                                            </tbody>
                                            <tfoot>
                                                <tr>
                                                    {footers}
                                                    <th key="footerOptions">{this.props.resourceOptions}</th>
                                                </tr>
                                            </tfoot>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                    {pagination}
                </section>
            </React.StrictMode>
        );
    }
}

export default ListView;
