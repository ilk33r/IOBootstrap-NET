import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import ClientListProps from "../props/ClientListProps";
import ClientListState from "../props/ClientListState";
import Controller from "../../../presentation/controllers/Controller";
import ListClientsResponseModel from "../models/ListClientsResponseModel";
import ListDataItemModel from "../../shared/models/ListDataItemModel";
import ListView from "../../shared/views/ListView";
import React from "react";
import WindowMessageModel from "../../../common/models/WindowMessageModel";

class ClientSelectController extends Controller<ClientListProps, ClientListState> {

    constructor(props: ClientListProps) {
        super(props);

        this.state = new ClientListState();

        this.selectDataHandler = this.selectDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONTROLLER_NAME}/ListClients`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: ListClientsResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new ClientListState();
                newState.clientList = response.clientList;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    selectDataHandler(index: number) {
        const client = this.state.clientList[index];
        const selectedClient: WindowMessageModel = { itemID: client.id, itemValue: client.clientDescription };
        this.postMessage(selectedClient);
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("clientsList", "Clients")
        ];

        const listDataHeaders = [
            'ID',
            'Description',
            'Enabled',
            'Request Count',
            'Max Request Count',
            'Client ID',
            'Secret'
        ];

        const items = this.state.clientList.map(clientModel => {
            const itemModel = new ListDataItemModel();
            const clientIsEnabled = (clientModel.isEnabled) ? "YES" : "NO";

            itemModel.itemList = [
                clientModel.id.toString(),
                clientModel.clientDescription,
                clientIsEnabled,
                clientModel.requestCount.toString(),
                clientModel.maxRequestCount.toString(),
                clientModel.clientID,
                clientModel.clientSecret
            ];

            return itemModel;
        });

        return (
            <React.StrictMode>
                <ListView navigation={navigation} 
                    listDataHeaders={listDataHeaders} 
                    items={items}
                    resourceDelete="Delete"
                    resourceEdit="Edit"
                    resourceHome="Home"
                    resourceOptions="Options"
                    resourceSelect=""
                    extras={null}
                    deleteDataHandler={null}
                    updateDataHandler={null}
                    selectDataHandler={this.selectDataHandler}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default ClientSelectController;
