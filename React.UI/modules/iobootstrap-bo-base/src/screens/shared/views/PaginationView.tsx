import { View } from "iobootstrap-ui-base";
import PaginationViewProps from "../props/PaginationViewProps";
import PaginationViewState from "../props/PaginationViewState";
import React from "react";

class PaginationView extends View<PaginationViewProps, PaginationViewState> {

    constructor(props: PaginationViewProps) {
        super(props);

        this.state = new PaginationViewState();

        this.nextPageButtonClick = this.nextPageButtonClick.bind(this);
        this.previousPageButtonClick = this.previousPageButtonClick.bind(this);
        this.pageButtonClick = this.pageButtonClick.bind(this);
    }

    private allPages(): number[] {
        const pageCount = this.pageCount();
        let allPages: number[] = [];

        for (let i = 1; i <= pageCount; i++) {
            allPages.push(i);
        }

        return allPages;
    }

    private currentPage(): number {
        return (this.props.length === 0) ? 1 : Math.floor(this.props.start / this.props.length) + 1;
    }

    private pageCount(): number {
        return (this.props.length === 0) ? 0 : Math.ceil(this.props.count / this.props.length);
    }

    nextPageButtonClick(e: { preventDefault: () => void; }) {
        e.preventDefault();

        const currentPage = this.currentPage();

        if (currentPage === this.pageCount()) {
            return;
        }

        const nextPage = currentPage + 1;
        const start = (nextPage - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    previousPageButtonClick(e: { preventDefault: () => void; }) {
        e.preventDefault();

        const currentPage = this.currentPage();

        if (currentPage === 1) {
            return;
        }

        const previousPage = currentPage - 1;
        const start = (previousPage - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    pageButtonClick(e: { preventDefault: () => void; }, pageNumber: number) {
        e.preventDefault();

        const start = (pageNumber - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    render() {
        const currentPage = this.currentPage();
        const pageCount = this.pageCount();
        const previousButtonClassName = (currentPage === 1) ? "paginate_button previous disabled" : "paginate_button previous";
        const nextButtonClassName = (currentPage === pageCount) ? "paginate_button next disabled" : "paginate_button next";

        const pages = this.allPages().map(page => {
            let pageClassName;
            if (page === currentPage) {
                pageClassName = "paginate_button active";
            } else {
                pageClassName = "paginate_button";
            }

            const key = "pageButton" + page.toString();
            return (
                <li className={pageClassName} key={key}>
                    <a href="#page" className="paginationPage" onClick={(e) => this.pageButtonClick(e, page)}>{page}</a>
                </li>
            );
        });

        return (
            <React.StrictMode>
                <div className="row">
                    <div className="col-sm-5"></div>
                    <div className="col-sm-7">
                        <div className="dataTables_paginate paging_simple_numbers">
                            <ul className="pagination">
                                <li className={previousButtonClassName}>
                                    <a onClick={this.previousPageButtonClick} href="#previous">Previous</a>
                                </li>
                                {pages}
                                <li className={nextButtonClassName}>
                                    <a onClick={this.nextPageButtonClick} href="#next">Next</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </React.StrictMode>
        );
    }
}

export default PaginationView;
