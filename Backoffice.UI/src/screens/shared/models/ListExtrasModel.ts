type ListExtrasItemHandler = (index: number) => void;

class ListExtrasModel {

    name: string;
    icon: string;
    itemSelectionHandler: ListExtrasItemHandler;

    constructor(name: string, icon: string, handler: ListExtrasItemHandler) {
        this.name = name;
        this.icon = icon;
        this.itemSelectionHandler = handler;
    }
}

export default ListExtrasModel;
