type ListDataPaginationPageClickHandler = (start: number, length: number) => void;

class ListDataPaginationModel {

    start: number;
    length: number;
    count: number;
    pageClickHandler: ListDataPaginationPageClickHandler | null;

    constructor() {
        this.start = 0;
        this.length = 0;
        this.count = 0;
        this.pageClickHandler = null;
    }
}

export default ListDataPaginationModel;
