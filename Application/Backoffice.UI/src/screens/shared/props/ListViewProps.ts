import BreadcrumbNavigationModel from "../models/BreadcrumbNavigationModel";
import ListExtrasModel from "../models/ListExtrasModel";
import ListDataItemModel from "../models/ListDataItemModel";
import ListDataPaginationModel from "../models/ListDataPaginationModel";

type ListViewPropsItemHandler = (index: number) => void;

interface ListViewProps {

    navigation: BreadcrumbNavigationModel[];
    listDataHeaders: string[];
    items: ListDataItemModel[];
    resourceDelete: string;
    resourceEdit: string;
    resourceHome: string;
    resourceOptions: string;
    resourceSelect: string;
    extras: ListExtrasModel[] | null;
    deleteDataHandler: ListViewPropsItemHandler | null;
    updateDataHandler: ListViewPropsItemHandler | null;
    selectDataHandler: ListViewPropsItemHandler | null;
    pagination: ListDataPaginationModel | null;
}

export default ListViewProps;
