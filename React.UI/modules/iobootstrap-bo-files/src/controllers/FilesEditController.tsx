import GetFilesRequestModel from "../models/GetFilesRequestModel";
import GetFilesResponseModel from "../models/GetFilesResponseModel";
import FilesListProps from "../props/FilesListProps";
import FilesListState from "../props/FilesListState";
import { BOController, BreadcrumbNavigationModel, ListDataFilterTypes, ListDataHeaderModel, ListDataItemModel, ListDataPaginationModel, ListExtrasModel, ListView } from "iobootstrap-bo-base";
import { BaseResponseModel, BaseView } from "iobootstrap-ui-base";

class FilesEditController extends BOController<FilesListProps, FilesListState> {

    private requestModel: GetFilesRequestModel;

    constructor(props: FilesListProps) {
        super(props);

        this.requestModel = new GetFilesRequestModel();

        this.state = new FilesListState();

        this.updateDataHandler = this.updateDataHandler.bind(this);
        this.pageChangeHandler = this.pageChangeHandler.bind(this);
        this.downloadFileHandler = this.downloadFileHandler.bind(this);
    }

    private loadFiles() {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_FILES_CONTROLLER_NAME}/GetFiles`;
        const weakSelf = this;

        this.service.post(requestPath, this.requestModel, function (response: GetFilesResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new FilesListState();
                newState.files = response.files;
                newState.count = response.count;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("fileData");
        this.appContext.removeObject("selectedFile");

        this.requestModel.start = 0;
        this.requestModel.count = 25;
        this.loadFiles();
    }

    private pageChangeHandler(start: number, length: number) {
        this.requestModel.start = start;
        this.requestModel.count = length;
        this.loadFiles();
    }

    private updateDataHandler(index: number) {
        const currentFile = this.state.files[index];

        this.appContext.setObjectForKey("selectedFile", currentFile);
        this.navigateToPage("fileModify");
    }

    private downloadFileHandler(index: number) {
        const currentFile = this.state.files[index];
        const requestPath = `${import.meta.env.VITE_FILE_ASSETS_CONTROLLER}/Get?publicId=${currentFile.publicId.RemoveHTML()}`;
        
        this.indicatorPresenter.present();
        const weakSelf = this;

        this.service.downloadFile(requestPath, function (blob: Blob | null, response: BaseResponseModel | null) {
            weakSelf.indicatorPresenter.dismiss();
            if (response != null) {
                weakSelf.handleServiceSuccess(response)
            } else if (blob != null) {
                weakSelf.downloadFile(blob, currentFile.fileName);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("filesEdit", "Files")
        ];

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initialize("Preview"),
            ListDataHeaderModel.initializeWithFilter("File Name", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("File Type", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Description", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Additional Data", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initializeWithFilter("Created", ListDataFilterTypes.Input, null),
            ListDataHeaderModel.initialize("Created Date"),
        ];

        const items = this.state.files.map(file => {
            const itemModel = new ListDataItemModel();
            const fileId = (file.id == null) ? "" : file.id.toString();
            const preview = this.imagePreviewHtml(file.fileType, file.publicId);
            const createdDate = (file.createdDate === undefined || file.createdDate === null) ? "-" : new Date(file.createdDate).toLocaleDateString('en-US', { year: 'numeric', day: '2-digit', month: '2-digit' });

            itemModel.itemList = [
                fileId,
                preview,
                file.fileName.RemoveHTML(),
                file.fileType,
                (file.description ?? "").RemoveHTML(),
                (file.additionalData ?? "").RemoveHTML(),
                (file.createdBy ?? "").RemoveHTML(),
                createdDate
            ];

            return itemModel;
        });

        const pagination = new ListDataPaginationModel();
        pagination.start = this.requestModel.start;
        pagination.length = this.requestModel.count;
        pagination.count = this.state.count;
        pagination.pageClickHandler = this.pageChangeHandler;

        const extras = [
            new ListExtrasModel("Download", "fa-download", this.downloadFileHandler)
        ];

        return (
            <BaseView>
                <ListView navigation={navigation}
                    headers={headers}
                    items={items}
                    resourceDelete=""
                    resourceEdit="Edit"
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect=""
                    extras={extras}
                    deleteDataHandler={null}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    itemVisibleHandler={null}
                    pagination={pagination} />
            </BaseView>
        );
    }

    private imagePreviewHtml(type: string, publicId: string): string {
        if (
            type.includes("image/jpg") || 
            type.includes("image/jpe") || 
            type.includes("image/jpeg") || 
            type.includes("image/pjpeg") || 
            type.includes("image/png") || 
            type.includes("image/heic")
        ) {
            return `<img src="${import.meta.env.VITE_API_URL}/${import.meta.env.VITE_FILE_ASSETS_CONTROLLER}/Get?publicId=${publicId.RemoveHTML()}" width="150" />`;
        }

        if (
            type.includes("video/h263") || 
            type.includes("video/h264") || 
            type.includes("video/mpeg") || 
            type.includes("video/mp4") || 
            type.includes("video/quicktime")
        ) {
            const videoType = (type === "video/quicktime") ? "video/mp4" : type;
            return `<video width="150" autoPlay muted loop><source src=${import.meta.env.VITE_API_URL}/${import.meta.env.VITE_FILE_ASSETS_CONTROLLER}/Get?publicId=${publicId.RemoveHTML()} type=${videoType} /></video>`;
        }
        
        return "";
    }
}

export default FilesEditController;
