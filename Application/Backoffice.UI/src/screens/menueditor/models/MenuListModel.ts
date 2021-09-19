class MenuListModel {

    id: number;
    menuOrder: number;
    requiredRole: number;
    action: string;
    cssClass: string;
    name: string;
    childItems: MenuListModel[];
    
    constructor() {
        this.id = 0;
        this.menuOrder = 0;
        this.requiredRole = 0;
        this.action = "";
        this.cssClass = "";
        this.name = "";
        this.childItems = [];
    }

}

export default MenuListModel;
