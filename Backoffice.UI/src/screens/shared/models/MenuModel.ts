class MenuModel {

    action: string | undefined;
    cssClass: string | undefined;
    name: string | undefined;
    isExpanded: boolean;
    childItems: MenuModel[] | undefined | null;

    constructor() {
        this.isExpanded = false;
    }
}

export default MenuModel;
