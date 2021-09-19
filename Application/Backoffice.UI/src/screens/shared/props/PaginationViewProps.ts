type PaginationViewPageClickHandler = (start: number, length: number) => void;

interface PaginationViewProps {

    start: number;
    length: number;
    count: number;
    pageChangeHandler: PaginationViewPageClickHandler;
}

export default PaginationViewProps;
