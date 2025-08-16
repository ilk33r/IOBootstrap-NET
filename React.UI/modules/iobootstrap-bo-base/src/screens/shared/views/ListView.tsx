/* eslint-disable jsx-a11y/anchor-is-valid */
import ListViewProps from "../props/ListViewProps";
import React from "react";
import { View } from "iobootstrap-ui-base";
import PaginationView from "./PaginationView";
import BreadcrumbView from "./BreadcrumbView";

class ListView extends View<ListViewProps, {}> {

    constructor(props: ListViewProps) {
        super(props);

        this.handleItemDeleteClick = this.handleItemDeleteClick.bind(this);
        this.handleItemSelect = this.handleItemSelect.bind(this);
        this.handleItemUpdateClick = this.handleItemUpdateClick.bind(this);
        this.handlePagination = this.handlePagination.bind(this);
    }

    handleItemDeleteClick(e: {}, itemIndex: number) {
        if (this.props.deleteDataHandler != null) {
            this.props.deleteDataHandler(itemIndex);
        }
    }

    handleItemSelect(e: {}, itemIndex: number) {
        if (this.props.selectDataHandler != null) {
            this.props.selectDataHandler(itemIndex);
        }
    }

    handleItemUpdateClick(e: {}, itemIndex: number) {
        if (this.props.updateDataHandler != null) {
            this.props.updateDataHandler(itemIndex);
        }
    }

    handlePagination(start: number, length: number) {
        if (this.props.pagination != null && this.props.pagination.pageClickHandler != null) {
            this.props.pagination.pageClickHandler(start, length);
        }
    }

    render() {
        const updateClass = (this.props.updateDataHandler != null) ? "btn btn-square" : "btn btn-square d-none";
        const deleteClass = (this.props.deleteDataHandler != null) ? "btn btn-square" : "btn btn-square d-none";
        const selectionClass = (this.props.selectDataHandler != null) ? "btn btn-square" : "btn btn-square d-none";

        const headers = this.props.listDataHeaders.map((header, headerIndex) => {
            const key = "headerIndex" + headerIndex;
            return (<th key={key}>{header}</th>);
        });

        const footers = this.props.listDataHeaders.map((footer, footerIndex) => {
            const key = "footerIndex" + footerIndex;
            return (<th key={key}>{footer}</th>);
        });

        const listData = this.props.items.map((listItem, itemIndex) => {

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
                    itemUpdateClass = "btn btn-square d-none";
                }
                
                if (!this.props.itemVisibleHandler(itemIndex, 1)) {
                    itemDeleteClass = "btn btn-square d-none";
                }

                if (!this.props.itemVisibleHandler(itemIndex, 2)) {
                    itemSelectionClass = "btn btn-square d-none";
                }
            }

            let itemExtras;
            if (this.props.extras != null && this.props.extras.length > 0) {
                itemExtras = this.props.extras.map((itemExtra, itemExtraIndex) => {
                    const iconClassName = "fa " + itemExtra.icon;
                    const key = "itemExtraKey" + itemIndex.toString() + itemExtra.name;
                    let itemExtraClass = "btn btn-square";
                    if (this.props.itemVisibleHandler != null) {
                        if (!this.props.itemVisibleHandler(itemIndex, itemExtraIndex + 3)) {
                            itemExtraClass = "btn btn-square d-none";
                        }
                    }

                    return (
                        <a className={itemExtraClass} key={key} onClick={() => itemExtra.itemSelectionHandler(itemIndex)}>
                            <i className={iconClassName}></i> {itemExtra.name}
                        </a>
                    );
                });
            } else {
                itemExtras = (<React.StrictMode></React.StrictMode>);
            }

            return (
                <tr className={rowClass} key={itemKey}>
                    {listDataColumn}
                    <td key={optionsColumnKey} className="text-nowrap">
                        <a className={itemUpdateClass} onClick={(e) => this.handleItemUpdateClick(e, itemIndex)}>
                            <i className="fa fa-edit"></i> {this.props.resourceEdit}
                        </a>
                        <a className={itemDeleteClass} onClick={(e) => this.handleItemDeleteClick(e, itemIndex)}>
                            <i className="fa fa-trash"></i> {this.props.resourceDelete}
                        </a>
                        <a className={itemSelectionClass} onClick={(e) => this.handleItemSelect(e, itemIndex)}>
                            <i className="fa fa-check"></i> {this.props.resourceSelect}
                        </a>
                        {itemExtras}
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
                            <div className="col-xs-12">
                                <div className="box">
                                    <div className="box-body">
                                        <table className="table table-bordered table-hover table-striped">
                                            <thead>
                                                <tr>
                                                    {headers}
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
