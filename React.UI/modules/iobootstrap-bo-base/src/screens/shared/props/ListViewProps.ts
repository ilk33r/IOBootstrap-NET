import BreadcrumbNavigationModel from "../models/BreadcrumbNavigationModel";
import ListDataHeaderModel from "../models/ListDataHeaderModel";
import ListDataItemModel from "../models/ListDataItemModel";
import ListDataPaginationModel from "../models/ListDataPaginationModel";
import ListExtrasModel from "../models/ListExtrasModel";

type ListViewPropsItemHandler = (index: number) => void;
type ListViewVisibleItemHandler = (listIndex: number, itemIndex: number) => boolean;

interface ListViewProps {

    navigation: BreadcrumbNavigationModel[];
    headers: ListDataHeaderModel[];
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
    itemVisibleHandler: ListViewVisibleItemHandler | null;
    pagination: ListDataPaginationModel | null;
}

export default ListViewProps;
