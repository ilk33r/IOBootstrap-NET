import BreadcrumbNavigationModel from "../models/BreadcrumbNavigationModel";
import ListDataItemModel from "../models/ListDataItemModel";
import ListDataPaginationModel from "../models/ListDataPaginationModel";
import ListExtrasModel from "../models/ListExtrasModel";

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
