import ListDataHeaderModel from "../models/ListDataHeaderModel";

type ListHeaderViewPropsFilterHandler = (index: number, word: string) => void;
type ListHeaderViewPropsClearFilterHandler = () => void;

interface ListHeaderViewProps {

    headers: ListDataHeaderModel[];
    filterHandler: ListHeaderViewPropsFilterHandler | null;
    clearFilterHandler: ListHeaderViewPropsClearFilterHandler | null;
}

export default ListHeaderViewProps;