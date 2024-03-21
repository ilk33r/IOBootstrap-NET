import GetImagesRequestModel from "../models/GetImagesRequestModel";
import GetImagesResponseModel from "../models/GetImagesResponseModel";
import ImagesListProps from "../props/ImagesListProps";
import ImagesListState from "../props/ImagesListState";
import React from "react";
import { Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, ListDataItemModel, ListDataPaginationModel, ListView } from "iobootstrap-bo-base";

class ImagesEditController extends Controller<ImagesListProps, ImagesListState> {

    private requestModel: GetImagesRequestModel;

    constructor(props: ImagesListProps) {
        super(props);

        this.requestModel = new GetImagesRequestModel();

        this.state = new ImagesListState();

        this.updateDataHandler = this.updateDataHandler.bind(this);
        this.pageChangeHandler = this.pageChangeHandler.bind(this);
    }

    private LoadImages() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_IMAGES_CONTROLLER_NAME}/GetImages`;
        const weakSelf = this;

        this.service.post(requestPath, this.requestModel, function (response: GetImagesResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new ImagesListState();
                newState.images = response.images;
                newState.count = response.count;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("imageData");
        this.appContext.removeObject("selectedImage");

        this.requestModel.start = 0;
        this.requestModel.count = 5;
        this.LoadImages();
    }

    pageChangeHandler(start: number, length: number) {
        this.requestModel.start = start;
        this.requestModel.count = length;
        console.log("Start: " + start + " Length: " + length);
        this.LoadImages();
    }

    updateDataHandler(index: number) {
        const currentImage = this.state.images[index];
        
        this.appContext.setObjectForKey("selectedImage", currentImage);
        this.navigateToPage("imageModify");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("imagesEdit", "Images")
        ];

        const listDataHeaders = [
            'ID',
            'Image',
            'Width',
            'Height',
            'Scale'
        ];

        const items = this.state.images.map(image => {
            const itemModel = new ListDataItemModel();
            const imageHtml = `<img src="${process.env.REACT_APP_API_URL}/${process.env.REACT_APP_IMAGE_ASSETS_CONTROLLER}/Get?publicId=${encodeURIComponent(image.fileName)}" width="150" />`
            const imageId = (image.id == null) ? "" : image.id.toString();
            const imageWidth = (image.width == null) ? "" : image.width.toString();
            const imageHeight = (image.height == null) ? "" : image.height.toString();
            const imageScale = (image.scale == null) ? "" : image.scale.toString();

            itemModel.itemList = [
                imageId,
                imageHtml,
                imageWidth,
                imageHeight,
                imageScale
            ];

            return itemModel;
        });

        const pagination = new ListDataPaginationModel();
        pagination.start = this.requestModel.start;
        pagination.length = this.requestModel.count;
        pagination.count = this.state.count;
        pagination.pageClickHandler = this.pageChangeHandler;

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    listDataHeaders={listDataHeaders} 
                    items={items}
                    resourceDelete=""
                    resourceEdit="Edit"
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect=""
                    extras={null}
                    deleteDataHandler={null}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    pagination={pagination} />
            </React.StrictMode>
        );
    }
}

export default ImagesEditController;
