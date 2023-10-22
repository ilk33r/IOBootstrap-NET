/* eslint-disable jsx-a11y/anchor-is-valid */
import ListViewProps from "../props/ListViewProps";
import React from "react";
import View from "../../../presentation/views/View";
import BreadcrumbView from "./BreadcrumbView";
import PaginationView from "./PaginationView";

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
        const updateClass = (this.props.updateDataHandler != null) ? "btn btn-app" : "btn btn-app hidden";
        const deleteClass = (this.props.deleteDataHandler != null) ? "btn btn-app" : "btn btn-app hidden";
        const selectionClass = (this.props.selectDataHandler != null) ? "btn btn-app" : "btn btn-app hidden";

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
                return (<td key={key}><div className="breakWord" dangerouslySetInnerHTML={{__html: itemColumn}}></div></td>);
            });

            const rowClass = (listItem.isEven) ? "childmenu" : "";
            const optionsColumnKey = "itemOptions" + itemIndex;
            const itemKey = "itemIndex" + itemIndex.toString();

            let itemExtras;
            if (this.props.extras != null && this.props.extras.length > 0) {
                itemExtras = this.props.extras.map(itemExtra => {
                    const iconClassName = "fa " + itemExtra.icon;
                    const key = "itemExtraKey" + itemIndex.toString() + itemExtra.name;
                    return (
                        <a className="btn btn-app" key={key} onClick={() => itemExtra.itemSelectionHandler(itemIndex)}>
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
                    <td key={optionsColumnKey}>
                        <a className={updateClass} onClick={(e) => this.handleItemUpdateClick(e, itemIndex)}>
                            <i className="fa fa-edit"></i> {this.props.resourceEdit}
                        </a>
                        <a className={deleteClass} onClick={(e) => this.handleItemDeleteClick(e, itemIndex)}>
                            <i className="fa fa-trash"></i> {this.props.resourceDelete}
                        </a>
                        <a className={selectionClass} onClick={(e) => this.handleItemSelect(e, itemIndex)}>
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
                <div className="content-wrapper">
                    <BreadcrumbView navigation={this.props.navigation} resourceHome={this.props.resourceHome} />
                    <section className="content">
                        <div className="row">
                            <div className="col-xs-12">
                                <div className="box">
                                    <div className="box-body">
                                        <table className="table table-bordered table-hover menuTable">
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
                        {pagination}
                    </section>
                </div>
            </React.StrictMode>
        );
    }
}

export default ListView;
