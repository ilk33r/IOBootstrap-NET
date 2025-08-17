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

    private nextPageButtonClick(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();

        const currentPage = this.currentPage();

        if (currentPage === this.pageCount()) {
            return;
        }

        const nextPage = currentPage + 1;
        const start = (nextPage - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    private previousPageButtonClick(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();

        const currentPage = this.currentPage();

        if (currentPage === 1) {
            return;
        }

        const previousPage = currentPage - 1;
        const start = (previousPage - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    private pageButtonClick(event: React.MouseEvent<HTMLAnchorElement>, pageNumber: number) {
        event.preventDefault();

        const start = (pageNumber - 1) * this.props.length;
        const length = this.props.length;
        this.props.pageChangeHandler(start, length);
    }

    render() {
        const currentPage = this.currentPage();
        const pageCount = this.pageCount();
        const previousButtonClassName = (currentPage === 1) ? "page-item disabled" : "page-item";
        const nextButtonClassName = (currentPage === pageCount) ? "page-item disabled" : "page-item";

        const pages = this.allPages().map(page => {
            let pageClassName;
            if (page === currentPage) {
                pageClassName = "page-item active";
            } else {
                pageClassName = "page-item";
            }

            const key = "pageButton" + page.toString();
            return (
                <li className={pageClassName} key={key}>
                    <a className="page-link" href="#page" onClick={(e) => this.pageButtonClick(e, page)}>{page}</a>
                </li>
            );
        });

        return (
            <React.StrictMode>
                <nav aria-label="Page navigation">
                    <ul className="pagination pagination-wrap justify-content-center">
                        <li className={previousButtonClassName}>
                            <a className="page-link" href="#previous" onClick={this.previousPageButtonClick}>Previous</a>
                        </li>
                        {pages}
                        <li className={nextButtonClassName}>
                            <a className="page-link" href="#next" onClick={this.nextPageButtonClick}>Next</a>
                        </li>
                    </ul>
                </nav>
            </React.StrictMode>
        );
    }
}

export default PaginationView;
