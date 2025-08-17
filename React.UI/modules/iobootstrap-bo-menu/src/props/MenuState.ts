import MenuModel from "../models/MenuModel";

class MenuState {

    menuItems: MenuModel[];
    isMediumDevice: boolean;

    constructor() {
        this.menuItems = [];
        this.isMediumDevice = false;
    }
}

export default MenuState;
