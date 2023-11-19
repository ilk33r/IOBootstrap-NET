import BOPagePropertyType from "../enumerations/BOPagePropertyType";
import BOPageEntityCustomEnumTypeModel from "./BOPageEntityCustomEnumTypeModel";

class BOPageEntityModel {

    propertyName: string;
    propertyJsonKey: string;
    type: BOPagePropertyType;
    nullable: boolean;
    enumTypeName: string | null;
    enumType: BOPageEntityCustomEnumTypeModel[] | null;

    constructor() {
        this.propertyName = "";
        this.propertyJsonKey = "";
        this.type = BOPagePropertyType.String;
        this.nullable = false;
        this.enumTypeName = null;
        this.enumType = null;
    }
}

export default BOPageEntityModel;