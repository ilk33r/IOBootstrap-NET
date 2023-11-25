import BaseRequestModel from "../../../common/models/BaseRequestModel";
import BOPageEntityModel from "./BOPageEntityModel";

class GenerateBOPageFilesRequestModel extends BaseRequestModel {

    entityName: string;
    entityDisplayName: string;
    entityItemName: string;
    properties: BOPageEntityModel[];
    listEntityName: string;
    listEntityAPIPath: string;
    listEntityDisplayName: string;

    constructor() {
        super();

        this.entityName = "";
        this.entityDisplayName = "";
        this.entityItemName = "";
        this.properties = [];
        this.listEntityName = "";
        this.listEntityAPIPath = "";
        this.listEntityDisplayName = "";
    }
}

export default GenerateBOPageFilesRequestModel;