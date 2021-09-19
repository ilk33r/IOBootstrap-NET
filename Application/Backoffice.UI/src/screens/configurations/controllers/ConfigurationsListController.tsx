import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import ConfigurationDeleteRequestModel from "../models/ConfigurationDeleteRequestModel";
import ConfigurationListProps from "../props/ConfigurationListProps";
import ConfigurationListResponseModel from "../models/ConfigurationListResponseModel";
import ConfigurationListState from "../props/ConfigurationListState";
import ConfigurationUpdateRequestModel from "../models/ConfigurationUpdateRequestModel";
import Controller from "../../../presentation/controllers/Controller";
import ListDataItemModel from "../../shared/models/ListDataItemModel";
import ListView from "../../shared/views/ListView";
import React from "react";

class ConfigurationsListController extends Controller<ConfigurationListProps, ConfigurationListState> {

    constructor(props: ConfigurationListProps) {
        super(props);

        this.state = new ConfigurationListState();

        this.deleteDataHandler = this.deleteDataHandler.bind(this);
        this.updateDataHandler = this.updateDataHandler.bind(this);
    }

    public componentDidMount?(): void {
        this.appContext.removeObject("configurationDeleteRequest");
        this.appContext.removeObject("configurationUpdateRequest");

        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/ListConfigurationItems`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: ConfigurationListResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new ConfigurationListState();
                newState.configurations = response.configurations;

                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    deleteDataHandler(index: number) {
        const configuration = this.state.configurations[index];
        const deleteRequestModel = new ConfigurationDeleteRequestModel();
        deleteRequestModel.configId = configuration.id;
        
        this.appContext.setObjectForKey("configurationDeleteRequest", deleteRequestModel);
        this.navigateToPage("configurationsDelete");
    }

    updateDataHandler(index: number) {
        const configuration = this.state.configurations[index];
        const updateRequestModel = new ConfigurationUpdateRequestModel();
        updateRequestModel.configId = configuration.id;
        updateRequestModel.configKey = configuration.configKey;
        updateRequestModel.strValue = configuration.configStringValue;
        updateRequestModel.intValue = configuration.configIntValue;
        
        this.appContext.setObjectForKey("configurationUpdateRequest", updateRequestModel);
        this.navigateToPage("configurationsUpdate");
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations")
        ];

        const listDataHeaders = [
            'ID',
            'Key',
            'Int Value',
            'String Value'
        ];

        const items = this.state.configurations.map(configurationModel => {
            const itemModel = new ListDataItemModel();
            const configIntValue = (configurationModel.configIntValue != null) ? configurationModel.configIntValue.toString() : "";
            const configStringValue = (configurationModel.configStringValue != null) ? configurationModel.configStringValue : "";

            itemModel.itemList = [
                configurationModel.id.toString(),
                configurationModel.configKey,
                configIntValue,
                configStringValue
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

export default ConfigurationsListController;
