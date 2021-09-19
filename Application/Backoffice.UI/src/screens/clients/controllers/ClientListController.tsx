import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import ClientDeleteRequestModel from "../models/ClientDeleteRequestModel";
import ClientListProps from "../props/ClientListProps";
import ClientListState from "../props/ClientListState";
import ClientUpdateRequestModel from "../models/ClientUpdateRequestModel";
import Controller from "../../../presentation/controllers/Controller";
import ListClientsResponseModel from "../models/ListClientsResponseModel";
import ListDataItemModel from "../../shared/models/ListDataItemModel";
import ListView from "../../shared/views/ListView";
import React from "react";

class ClientListController extends Controller<ClientListProps, ClientListState> {

    constructor(props: ClientListProps) {
        super(props);

        this.state = new ClientListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("clientDeleteRequest");
        this.appContext.removeObject("clientUpdateRequest");

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

    deleteDataHandler(index: number) {
        const client = this.state.clientList[index];
        const deleteRequestModel = new ClientDeleteRequestModel();
        deleteRequestModel.ClientId = client.id;
        
        this.appContext.setObjectForKey("clientDeleteRequest", deleteRequestModel);
        this.navigateToPage("clientsDelete");
    }

    updateDataHandler(index: number) {
        const client = this.state.clientList[index];
        const updateRequestModel = new ClientUpdateRequestModel();
        updateRequestModel.ClientId = client.id;
        updateRequestModel.ClientDescription = client.clientDescription;
        updateRequestModel.IsEnabled = (client.isEnabled) ? 1 : 0;
        updateRequestModel.RequestCount = client.requestCount;
        updateRequestModel.MaxRequestCount = client.maxRequestCount;
        
        this.appContext.setObjectForKey("clientUpdateRequest", updateRequestModel);
        this.navigateToPage("clientsUpdate");
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
                    deleteDataHandler={this.deleteDataHandler}
                    updateDataHandler={this.updateDataHandler}
                    selectDataHandler={null}
                    pagination={null} />
            </React.StrictMode>
        );
    }
}

export default ClientListController;