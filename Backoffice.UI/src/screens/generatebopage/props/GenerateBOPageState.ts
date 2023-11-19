import GenerateBOPageResponseModel from "../models/GenerateBOPageResponseModel";

class GenerateBOPageState {

    boPageDataResponse: GenerateBOPageResponseModel | null;

    constructor(boPageDataResponse: GenerateBOPageResponseModel | null) {
        this.boPageDataResponse = boPageDataResponse;
    }
}

export default GenerateBOPageState;
