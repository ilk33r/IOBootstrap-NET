import ListDataFilterTypes from "./ListDataFilterTypes";

class ListDataHeaderModel {

    customFilters: string[] | null;
    filterType: ListDataFilterTypes;
    title: string;

    constructor(title: string, filterType: ListDataFilterTypes, customFilters: string[] | null) {
        this.title = title;
        this.filterType = filterType;
        this.customFilters = customFilters;
    }

    public static initialize(title: string): ListDataHeaderModel {
        const response = new ListDataHeaderModel(title, ListDataFilterTypes.None, null);
        return response;
    }

    public static initializeWithFilter(title: string, filterType: ListDataFilterTypes, customFilters: string[] | null): ListDataHeaderModel {
        const response = new ListDataHeaderModel(title, filterType, customFilters);
        return response;
    }
}

export default ListDataHeaderModel;