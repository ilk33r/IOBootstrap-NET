import BOPagePropertyType from "../enumerations/BOPagePropertyType";
import BOPageEntityCustomEnumTypeModel from "./BOPageEntityCustomEnumTypeModel";

class BOPageEntityModel {

    propertyName: string;
    propertyJsonKey: string;
    type: BOPagePropertyType;
    nullable: boolean;
    stringLength: number | null;
    enumTypeName: string | null;
    enumType: BOPageEntityCustomEnumTypeModel[] | null;

    constructor() {
        this.propertyName = "";
        this.propertyJsonKey = "";
        this.type = BOPagePropertyType.String;
        this.nullable = false;
        this.stringLength = null;
        this.enumTypeName = null;
        this.enumType = null;
    }
}

export default BOPageEntityModel;