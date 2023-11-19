import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BOPageEntityModel from "./BOPageEntityModel";

class GenerateBOPageResponseModel extends BaseResponseModel {

    entityName: string;
    entityDisplayName: string;
    entityItemName: string;
    properties: BOPageEntityModel[];
    listEntityName: string;
    listEntityAPIPath: string;
    listEntityAction: string;
    listEntityDisplayName: string;

    constructor() {
        super();

        this.entityName = "";
        this.entityDisplayName = "";
        this.entityItemName = "";
        this.properties = [];
        this.listEntityName = "";
        this.listEntityAPIPath = "";
        this.listEntityAction = "";
        this.listEntityDisplayName = "";
    }
}

export default GenerateBOPageResponseModel;
