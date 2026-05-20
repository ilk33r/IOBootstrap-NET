import GetFilesRequestModel from "../models/GetFilesRequestModel";
import GetFilesResponseModel from "../models/GetFilesResponseModel";
import FilesListProps from "../props/FilesListProps";
import FilesListState from "../props/FilesListState";
import { BOController, BreadcrumbNavigationModel, ListDataHeaderModel, ListDataItemModel, ListDataPaginationModel, ListView } from "iobootstrap-bo-base";
import { BaseView } from "iobootstrap-ui-base";

class FilesEditController extends BOController<FilesListProps, FilesListState> {

    private requestModel: GetFilesRequestModel;

    constructor(props: FilesListProps) {
        super(props);

        this.requestModel = new GetFilesRequestModel();

        this.state = new FilesListState();

        this.updateDataHandler = this.updateDataHandler.bind(this);
        this.pageChangeHandler = this.pageChangeHandler.bind(this);
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
        this.requestModel.count = 5;
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

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("filesEdit", "Files")
        ];

        const headers = [
            ListDataHeaderModel.initialize("ID"),
            ListDataHeaderModel.initialize("File Name"),
            ListDataHeaderModel.initialize("File Type"),
        ];

        const items = this.state.files.map(file => {
            const itemModel = new ListDataItemModel();
            const fileId = (file.id == null) ? "" : file.id.toString();

            itemModel.itemList = [
                fileId,
                file.fileName,
                file.fileType,
            ];

            return itemModel;
        });

        const pagination = new ListDataPaginationModel();
        pagination.start = this.requestModel.start;
        pagination.length = this.requestModel.count;
        pagination.count = this.state.count;
        pagination.pageClickHandler = this.pageChangeHandler;

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
                    extras={null}
                    deleteDataHandler={null}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    itemVisibleHandler={null}
                    pagination={pagination} />
            </BaseView>
        );
    }
}

export default FilesEditController;
