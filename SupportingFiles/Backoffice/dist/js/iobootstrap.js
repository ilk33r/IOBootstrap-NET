io.prototype.inited = function() {
};

io.prototype.request.ClientDeleteRequest = {
    Culture: 0,
    Version: '',
    ClientId: 0
};

io.prototype.request.ConfigurationDeleteRequest = {
    Culture: 0,
    Version: '',
    ConfigId: 0
};

io.prototype.request.MenuDeleteRequestModel = {
    Culture: 0,
    Version: '',
    ID: 0
};

io.prototype.request.MessageDeleteRequestModel = {
    Culture: 0,
    Version: '',
    MessageId: 0
};

io.prototype.request.PushNotificationMessageDeleteRequest = {
    Culture: 0,
    Version: '',
    ID: 0
};

io.prototype.request.ResourceDeleteRequestModel = {
    Culture: 0,
    Version: '',
    ID: 0
};

io.prototype.request.UserDeleteRequest = {
    Culture: 0,
    Version: '',
    UserId: 0
};

io.prototype.app.dashboard = function(e, hash) {
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('dashboard', false, function () {
        window.ioinstance.messagesHtml(function (messages) {
            window.ioinstance.layout.contentLayoutData = {
                messages: messages
            };
            window.ioinstance.layout.render();
            window.ioinstance.indicator.hide();
        });
    });
};

io.prototype.app.clientsList = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('clientsList', 'Clients', []);
    let requestURLFormat = '%s/ListClients';
    let requestURL = requestURLFormat.format(IOGlobal.backOfficeControllerName);

    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var updateParams = [];
            var deleteParams = [];

            for (var index in response.clientList) {
                var client = response.clientList[index];

                var isEnabled = (client.isEnabled === 1) ? 'YES' : 'NO';
                var itemListData = [
                    client.id,
                    client.clientDescription,
                    isEnabled,
                    client.requestCount,
                    client.maxRequestCount,
                    client.clientID,
                    client.clientSecret
                ];

                listData.push(itemListData);

                var itemUpdateData = [
                    client.id,
                    client.clientDescription,
                    client.isEnabled,
                    client.requestCount,
                    client.maxRequestCount
                ];

                updateParams.push(itemUpdateData);
                deleteParams.push([client.id]);
            }

            let listDataHeaders = [
                'ID',
                'Description',
                'Enabled',
                'Request Count',
                'Max Request Count',
                'Client ID',
                'Secret'
            ];

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'clientsUpdate';
            createListParams.updateParams = updateParams;
            createListParams.deleteMethodName = 'clientDelete';
            createListParams.deleteParams = deleteParams;

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.clientsUpdate = function (id, clientDescription, isEnabled, requestCount, maxRequestCount) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('clientsList', 'Clients');
    var formBreadcrumb = new io.ui.breadcrumb('clientsUpdate', 'Update client.', [ breadcrumbNavigation ]);

    var isEnabledFormData = new io.ui.formData(io.ui.formDataTypes.select, 'clientIsEnabled', 'Is Enabled', 'IsEnabled');
    isEnabledFormData.value = isEnabled;
    isEnabledFormData.options = [
        new io.ui.formDataOptions('NO', 0),
        new io.ui.formDataOptions('YES', 1)
    ];

    var clientDescriptionFormData = new io.ui.formData(io.ui.formDataTypes.text, 'clientDescription', 'Description', 'ClientDescription');
    var clientDescriptionValidation = new ioValidation(io.validationRuleTypes.minLength, 'Client name is too sort.', 'clientDescription', 'Invalid client name.');
    clientDescriptionValidation.length = 1;
    clientDescriptionFormData.validations = [ clientDescriptionValidation ];
    clientDescriptionFormData.value = clientDescription;

    var requestCountFormData = new io.ui.formData(io.ui.formDataTypes.number, 'clientRequestCount', 'Request Count', 'RequestCount');
    requestCountFormData.value = requestCount;

    var maxRequestCountFormData = new io.ui.formData(io.ui.formDataTypes.number, 'clientMaxRequestCount', 'Max Request Count', 'MaxRequestCount');
    maxRequestCountFormData.value = maxRequestCount;

    var formDatas = [
        isEnabledFormData,
        clientDescriptionFormData,
        requestCountFormData,
        maxRequestCountFormData
    ];

    io.ui.createForm('clientsUpdate', formBreadcrumb, 'updateClientForm', formDatas, 'Update', function () {
    }, function (request) {

        request.ClientId = id;
        request.IsEnabled = parseInt(request.IsEnabled);
        request.RequestCount = parseInt(request.RequestCount);
        request.MaxRequestCount = parseInt(request.MaxRequestCount);

        let requestURLFormat = '%s/UpdateClient';
        let requestURL = requestURLFormat.format(IOGlobal.backOfficeControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;

            if (status && response.status.success) {
                callout.show(callout.types.success, 'Client has been updated successfully.', '');
                window.location.hash = '';
                window.ioinstance.app.clientsList(null, 'clientsList');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }
        });
    });
};

io.prototype.app.clientDelete = function (id) {
    var answer = confirm("Are you sure want to delete this client ?");
    if (answer) {
        var request = window.ioinstance.request.ClientDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ClientId = id;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteClient';
        let requestURL = requestURLFormat.format(IOGlobal.backOfficeControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Client has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.clientsList(null, 'clientsList');
        });
    }
};

io.prototype.app.clientsAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('clientsList', 'Clients');
    var formBreadcrumb = new io.ui.breadcrumb('clientsAdd', 'Add a client.', [ breadcrumbNavigation ]);

    var clientDescriptionFormData = new io.ui.formData(io.ui.formDataTypes.text, 'clientDescription', 'Description', 'ClientDescription');
    var clientDescriptionValidation = new ioValidation(io.validationRuleTypes.minLength, 'Client name is too sort.', 'clientDescription', 'Invalid client name.');
    clientDescriptionValidation.length = 1;
    clientDescriptionFormData.validations = [ clientDescriptionValidation ];

    var requestCountFormData = new io.ui.formData(io.ui.formDataTypes.number, 'clientRequestCount', 'Request Count', 'RequestCount');

    var formDatas = [
        clientDescriptionFormData,
        requestCountFormData
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addClientForm', formDatas, 'Save', function () {
    }, function (request) {
        request.RequestCount = parseInt(request.RequestCount);
        let requestURLFormat = '%s/AddClient';
        let requestURL = requestURLFormat.format(IOGlobal.backOfficeControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;

            if (status && response.status.success) {
                callout.show(callout.types.success, 'Client has been added successfully.', '');
                window.location.hash = '';
                window.ioinstance.app.clientsList(null, 'clientsList');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }
        });
    });
};

io.prototype.app.clientsSelect = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('clientsList', 'Clients', []);
    let requestURLFormat = '%s/ListClients';
    let requestURL = requestURLFormat.format(IOGlobal.backOfficeControllerName);

    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var selectionParams = [];

            for (var index in response.clientList) {
                var client = response.clientList[index];

                var isEnabled = (client.isEnabled === 1) ? 'YES' : 'NO';
                var itemListData = [
                    client.id,
                    client.clientDescription,
                    isEnabled,
                    client.requestCount,
                    client.maxRequestCount,
                    client.clientID,
                    client.clientSecret
                ];

                listData.push(itemListData);

                var itemUpdateData = [
                    client.id,
                    client.clientDescription,
                    client.isEnabled,
                    client.requestCount,
                    client.maxRequestCount
                ];

                selectionParams.push([client.id, client.clientDescription]);
            }

            let listDataHeaders = [
                'ID',
                'Description',
                'Enabled',
                'Request Count',
                'Max Request Count',
                'Client ID',
                'Secret'
            ];

            io.ui.createPopupSelection(hash, breadcrumb, listDataHeaders, listData, 'clientSelectItem', selectionParams, null, function () {
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.clientSelectItem = function (e, hash) {
    var client = $('#client');
    client.attr('data-params', e[0]);
    client.val(e[1]);
};

io.prototype.app.configurationDelete = function(id) {
    var answer = confirm("Are you sure want to delete this configuration ?");
    if (answer) {
        var request = window.ioinstance.request.ConfigurationDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ConfigId = id;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteConfigItem';
        let requestURL = requestURLFormat.format(IOGlobal.configurationControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Configuration has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.configurationsList(null, 'configurationsList');
        });
    }
};

io.prototype.app.configurationsAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('configurationsList', 'Configurations');
    var formBreadcrumb = new io.ui.breadcrumb('configurationsAdd', 'Add a configuration parameter.', [ breadcrumbNavigation ]);

    var configKeyFormData = new io.ui.formData(io.ui.formDataTypes.text, 'configKey', 'Config Key', 'ConfigKey');
    var configKeyValidation = new ioValidation(io.validationRuleTypes.minLength, 'Config key is too sort.', 'configKey', 'Invalid config key.');
    configKeyValidation.length = 3;
    configKeyFormData.validations = [ configKeyValidation ];

    var integerValueFormData = new io.ui.formData(io.ui.formDataTypes.number, 'intValue', 'Integer Value', 'IntValue');
    var stringValueFormData = new io.ui.formData(io.ui.formDataTypes.textArea, 'strValue', 'String Value', 'StrValue');

    var formDatas = [
        configKeyFormData,
        integerValueFormData,
        stringValueFormData
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addConfigurationForm', formDatas, 'Save', function () {
    }, function (request) {
        request.IntValue = parseInt(request.IntValue);

        let requestURLFormat = '%s/AddConfigItem';
        let requestURL = requestURLFormat.format(IOGlobal.configurationControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;

            if (status && response.status.success) {
                callout.show(callout.types.success, 'Configuration parameter has been added successfully.', '');
                window.location.hash = '';
                window.ioinstance.app.configurationsList(null, 'configurationsList');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }
        });
    });
};

io.prototype.app.configurationsList = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('configurationsList', 'Configurations', []);

    let requestURLFormat = '%s/ListConfigurationItems';
    let requestURL = requestURLFormat.format(IOGlobal.configurationControllerName);
    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var updateParams = [];
            var deleteParams = [];

            for (var index in response.configurations) {
                var configuration = response.configurations[index];

                var itemListData = [
                    configuration.id,
                    configuration.configKey,
                    configuration.configIntValue,
                    configuration.configStringValue
                ];

                listData.push(itemListData);

                var itemUpdateData = [
                    configuration.id,
                    configuration.configKey,
                    configuration.configIntValue,
                    configuration.configStringValue
                ];

                updateParams.push(itemUpdateData);
                deleteParams.push([configuration.id]);
            }

            let listDataHeaders = [
                'ID',
                'Key',
                'Int Value',
                'String Value'
            ];

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'configurationUpdate';
            createListParams.updateParams = updateParams;
            createListParams.deleteMethodName = 'configurationDelete';
            createListParams.deleteParams = deleteParams;

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.configurationUpdate = function (id, key, intValue, stringValue) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('configurationsList', 'Configurations');
    var formBreadcrumb = new io.ui.breadcrumb('configurationUpdate', 'Update configuration parameter.', [ breadcrumbNavigation ]);

    var configKeyFormData = new io.ui.formData(io.ui.formDataTypes.text, 'configKey', 'Config Key', 'ConfigKey');
    var configKeyValidation = new ioValidation(io.validationRuleTypes.minLength, 'Config key is too sort.', 'configKey', 'Invalid config key.');
    configKeyValidation.length = 3;
    configKeyFormData.validations = [ configKeyValidation ];
    configKeyFormData.value = key;

    var integerValueFormData = new io.ui.formData(io.ui.formDataTypes.number, 'intValue', 'Integer Value', 'IntValue');
    integerValueFormData.value = intValue;

    var stringValueFormData = new io.ui.formData(io.ui.formDataTypes.textArea, 'strValue', 'String Value', 'StrValue');
    stringValueFormData.value = stringValue;

    var formDatas = [
        configKeyFormData,
        integerValueFormData,
        stringValueFormData
    ];

    io.ui.createForm('configurationUpdate', formBreadcrumb, 'updateConfigurationForm', formDatas, 'Update', function () {
    }, function (request) {
        request.ConfigId = id;
        request.IntValue = parseInt(request.IntValue);

        let requestURLFormat = '%s/UpdateConfigItem';
        let requestURL = requestURLFormat.format(IOGlobal.configurationControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;

            if (status && response.status.success) {
                callout.show(callout.types.success, 'Configuration parameter has been updated successfully.', '');
                window.location.hash = '';
                window.ioinstance.app.configurationsList(null, 'configurationsList');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }
        });
    });
};

io.prototype.app.deleteItem = function(url, request, alertMessageResourceKey, successMessageResourceKey, callback) {
    let io = window.ioinstance;

    let resources = [
        alertMessageResourceKey,
        successMessageResourceKey
    ];

    io.indicator.show();

    io.resources.getResources(resources, function() {
        var answer = confirm(io.resources.get(alertMessageResourceKey));
        if (answer) {
            io.service.post(url, request, function (status, response, error) {
                let callout = io.callout;
                if (status && response.status.success) {
                    callout.show(callout.types.success, io.resources.get(successMessageResourceKey), '');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                }

                io.indicator.hide();
                callback();
            });
        } else {
            io.indicator.hide();
        }
    });
};

io.prototype.app.menuEditorList = function(e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('menuEditorList', 'Menu Editor', []);

    let requestURLFormat = '%s/ListMenuItems';
    let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);

    // Call client list
    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var updateParams = [];
            var deleteParams = [];
            var hasRowClasses = [];

            for (var index in response.items) {
                var menu = response.items[index];

                var roleName = window.ioinstance.userRoles.getRoleName(menu.requiredRole);
                var itemListData = [
                    menu.id,
                    menu.name,
                    menu.action,
                    menu.cssClass,
                    roleName,
                    menu.menuOrder,
                ];

                listData.push(itemListData);

                var itemUpdateData = [
                    menu.id,
                    menu.name,
                    menu.action,
                    menu.cssClass,
                    menu.requiredRole,
                    menu.menuOrder,
                    '',
                    ''
                ];

                updateParams.push(itemUpdateData);
                deleteParams.push([menu.id]);
                hasRowClasses.push(false);

                var childMenuItems = menu.childItems;
                if (childMenuItems != null && childMenuItems.length > 0) {
                    for (var childIndex in childMenuItems) {
                        var childMenu = childMenuItems[childIndex];
                        var childMenuRoleName = window.ioinstance.userRoles.getRoleName(childMenu.requiredRole);
                        var childItemListData = [
                            childMenu.id,
                            childMenu.name,
                            childMenu.action,
                            childMenu.cssClass,
                            childMenuRoleName,
                            childMenu.menuOrder,
                        ];

                        listData.push(childItemListData);

                        var itemUpdateData = [
                            childMenu.id,
                            childMenu.name,
                            childMenu.action,
                            childMenu.cssClass,
                            childMenu.requiredRole,
                            childMenu.menuOrder,
                            menu.name,
                            menu.id
                        ];

                        updateParams.push(itemUpdateData);
                        deleteParams.push([childMenu.id]);
                        hasRowClasses.push(true);
                    }
                }
            }

            let listDataHeaders = [
                'ID',
                'Name',
                'Action',
                'Css Class',
                'Role',
                'Order'
            ];

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'menuEditorUpdate';
            createListParams.updateParams = updateParams;
            createListParams.deleteMethodName = 'menuEditorDelete';
            createListParams.deleteParams = deleteParams;
            createListParams.hasRowClasses = hasRowClasses;

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.menuEditorAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('menuEditorList', 'Menu Editor');
    var formBreadcrumb = new io.ui.breadcrumb('menuEditorAdd', 'Add a menu item', [ breadcrumbNavigation ]);

    var nameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuName', 'Name', 'Name');
    var nameValidation = new ioValidation(io.validationRuleTypes.minLength, 'Name is too short.', 'menuName', 'Invalid menu name.');
    nameValidation.length = 1;
    nameFormData.validations = [ nameValidation ];

    var actionFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuAction', 'Action', 'Action');
    var actionValidation = new ioValidation(io.validationRuleTypes.minLength, 'Action is too short.', 'menuAction', 'Invalid menu action.');
    actionValidation.length = 1;
    actionFormData.validations = [ actionValidation ];

    var cssClassNameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuCss', 'CSS Class Name', 'CssClass');

    var rolesOptions = window.ioinstance.userRoles.getRoleList();
    var rolesFormData = new io.ui.formData(io.ui.formDataTypes.select, 'role', 'Required Role', 'RequiredRole');
    rolesFormData.options = rolesOptions;

    var menuOrderFormData = new io.ui.formData(io.ui.formDataTypes.number, 'menuOrder', 'Menu Order', 'MenuOrder');
    var parentMenuFormData = new io.ui.formData(io.ui.formDataTypes.popupSelection, 'parentMenu', 'Parent Menu', 'ParentEntityID');
    parentMenuFormData.params = '';
    parentMenuFormData.methodName = 'menuSelect';

    var formDatas = [
        nameFormData,
        actionFormData,
        cssClassNameFormData,
        rolesFormData,
        menuOrderFormData,
        parentMenuFormData
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addMenuForm', formDatas, 'Save', function () {
        },
        function (request) {

            request.MenuOrder = parseInt(request.MenuOrder);
            request.RequiredRole = parseInt(request.RequiredRole);
            if (request.ParentEntityID === '') {
                request.ParentEntityID = null;
            } else {
                request.ParentEntityID = window.ioinstance.ui.getPopupSelectionValue('parentMenu');
            }

            let requestURLFormat = '%s/AddMenuItem';
            let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Menu has been added successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.menuEditorList(null, 'menuEditorList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.menuEditorDelete = function (id) {
    var answer = confirm("Are you sure want to delete this menu ?");
    if (answer) {
        var request = window.ioinstance.request.MenuDeleteRequestModel;
        request.Version = window.ioinstance.version;
        request.ID = id;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteMenuItem';
        let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Menu has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.menuEditorList(null, 'menuEditorList');
        });
    }
};

io.prototype.app.menuEditorSelect = function(e, hash) {
    var parentMenu = $('#parentMenu');
    parentMenu.attr('data-params', e[0]);
    parentMenu.val(e[1]);
};

io.prototype.app.menuEditorUpdate = function(id, name, action, cssClass, userRoleRaw, menuOrder, parentMenuName, parentMenuId) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('menuEditorList', 'Menu Editor');
    var formBreadcrumb = new io.ui.breadcrumb('menuEditorUpdate', 'Update Menu Item', [ breadcrumbNavigation ]);

    var nameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuName', 'Name', 'Name');
    var nameValidation = new ioValidation(io.validationRuleTypes.minLength, 'Name is too short.', 'menuName', 'Invalid menu name.');
    nameValidation.length = 1;
    nameFormData.validations = [ nameValidation ];
    nameFormData.value = name;

    var actionFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuAction', 'Action', 'Action');
    var actionValidation = new ioValidation(io.validationRuleTypes.minLength, 'Action is too short.', 'menuAction', 'Invalid menu action.');
    actionValidation.length = 1;
    actionFormData.validations = [ actionValidation ];
    actionFormData.value = action;

    var cssClassNameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'menuCss', 'CSS Class Name', 'CssClass');
    cssClassNameFormData.value = cssClass;

    var rolesOptions = window.ioinstance.userRoles.getRoleList();
    var rolesFormData = new io.ui.formData(io.ui.formDataTypes.select, 'role', 'Required Role', 'RequiredRole');
    rolesFormData.value = userRoleRaw;
    rolesFormData.options = rolesOptions;

    var menuOrderFormData = new io.ui.formData(io.ui.formDataTypes.number, 'menuOrder', 'Menu Order', 'MenuOrder');
    menuOrderFormData.value = menuOrder;

    var parentMenuFormData = new io.ui.formData(io.ui.formDataTypes.popupSelection, 'parentMenu', 'Parent Menu', 'ParentEntityID');
    parentMenuFormData.params = parentMenuId;
    parentMenuFormData.methodName = 'menuSelect';
    parentMenuFormData.value = parentMenuName;

    var formDatas = [
        nameFormData,
        actionFormData,
        cssClassNameFormData,
        rolesFormData,
        menuOrderFormData,
        parentMenuFormData
    ];

    io.ui.createForm('menuEditorUpdate', formBreadcrumb, 'updateMenuForm', formDatas, 'Update', function () {
        },
        function (request) {
            request.ID = id;
            request.MenuOrder = parseInt(request.MenuOrder);
            request.RequiredRole = parseInt(request.RequiredRole);
            if (request.ParentEntityID === '') {
                request.ParentEntityID = null;
            } else {
                request.ParentEntityID = window.ioinstance.ui.getPopupSelectionValue('parentMenu');
            }

            let requestURLFormat = '%s/UpdateMenuItem';
            let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Menu has been updated successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.menuEditorList(null, 'menuEditorList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.menuSelect = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('menuEditorList', 'Menu Editor', []);

    let requestURLFormat = '%s/ListMenuItems';
    let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);

    // Call client list
    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var hasRowClasses = [];
            var selectionParams = [];

            for (var index in response.items) {
                var menu = response.items[index];

                var roleName = window.ioinstance.userRoles.getRoleName(menu.requiredRole);
                var itemListData = [
                    menu.id,
                    menu.name,
                    menu.action,
                    menu.cssClass,
                    roleName,
                    menu.menuOrder,
                ];

                listData.push(itemListData);
                selectionParams.push([menu.id, menu.name]);
                hasRowClasses.push(false);

                var childMenuItems = menu.childItems;
                if (childMenuItems != null && childMenuItems.length > 0) {
                    for (var childIndex in childMenuItems) {
                        var childMenu = childMenuItems[childIndex];
                        var childMenuRoleName = window.ioinstance.userRoles.getRoleName(childMenu.requiredRole);
                        var childItemListData = [
                            childMenu.id,
                            childMenu.name,
                            childMenu.action,
                            childMenu.cssClass,
                            childMenuRoleName,
                            childMenu.menuOrder,
                        ];

                        listData.push(childItemListData);
                        selectionParams.push([childMenu.id, childMenu.name]);
                        hasRowClasses.push(true);
                    }
                }
            }

            let listDataHeaders = [
                'ID',
                'Name',
                'Action',
                'Css Class',
                'Role',
                'Order'
            ];

            io.ui.createPopupSelection(hash, breadcrumb, listDataHeaders, listData, 'menuEditorSelect', selectionParams, hasRowClasses, function () {
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.messageDelete = function (id) {
    var answer = confirm("Are you sure want to delete this message ?");
    if (answer) {
        var request = window.ioinstance.request.MessageDeleteRequestModel;
        request.Version = window.ioinstance.version;
        request.MessageId = id;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteMessagesItem';
        let requestURL = requestURLFormat.format(IOGlobal.messagesControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Message has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.messagesList(null, 'messagesList');
        });
    }
};

io.prototype.app.messagesAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('messagesList', 'Messages');
    var formBreadcrumb = new io.ui.breadcrumb('messagesAdd', 'Add a new message', [ breadcrumbNavigation ]);

    var messageFormData = new io.ui.formData(io.ui.formDataTypes.textArea, 'message', 'Message', 'Message');
    var messageValidation = new ioValidation(io.validationRuleTypes.minLength, 'Message is too short.', 'message', 'Invalid message.');
    messageValidation.length = 3;
    messageFormData.validations = [ messageValidation ];

    var startDateFormData = new io.ui.formData(io.ui.formDataTypes.date, 'startDate', 'Start Date', 'MessageStartDate');
    var endDateFormData = new io.ui.formData(io.ui.formDataTypes.date, 'endDate', 'End Date', 'MessageEndDate');

    var formDatas = [
        messageFormData,
        startDateFormData,
        endDateFormData
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addMessageForm', formDatas, 'Save', function () {
        },
        function (request) {
            let requestURLFormat = '%s/AddMessagesItem';
            let requestURL = requestURLFormat.format(IOGlobal.messagesControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Message has been added successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.messagesList(null, 'messagesList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
    });
};

io.prototype.app.messagesList = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('messagesList', 'Messages', []);
    let requestURLFormat = '%s/ListAllMessages';
    let requestURL = requestURLFormat.format(IOGlobal.messagesControllerName);

    // Call client list
    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            var listData = [];
            var updateParams = [];
            var deleteParams = [];

            for (var index in response.messages) {
                var message = response.messages[index];
                var createDate = new Date(message.messageCreateDate);
                var startDate = new Date(message.messageStartDate);
                var endDate = new Date(message.messageEndDate);

                var itemListData = [
                    message.id,
                    message.message.replace(/\n/g, "<br />"),
                    createDate.toLocaleDateString(),
                    startDate.toLocaleDateString(),
                    endDate.toLocaleDateString()
                ];

                listData.push(itemListData);

                var startDateMonthValue = startDate.getMonth() + 1;
                var startDateMonth = (startDateMonthValue < 10) ? '0' + startDateMonthValue : startDateMonthValue;

                var startDateDayValue = startDate.getDay() + 1;
                var startDateDay = (startDateDayValue < 10) ? '0' + startDateDayValue : startDateDayValue;

                var endDateMonthValue = endDate.getMonth() + 1;
                var endDateMonth = (endDateMonthValue < 10) ? '0' + endDateMonthValue : endDateMonthValue;

                var endDateDayValue = endDate.getDay() + 1;
                var endDateDay = (endDateDayValue < 10) ? '0' + endDateDayValue : endDateDayValue;

                var startDateString = startDate.getFullYear() + '-' + startDateMonth + '-' + startDateDay;
                var endDateString = endDate.getFullYear() + '-' + endDateMonth + '-' + endDateDay;

                var itemUpdateData = [
                    message.id,
                    message.message,
                    startDateString,
                    endDateString
                ];

                updateParams.push(itemUpdateData);
                deleteParams.push([message.id]);
            }

            let listDataHeaders = [
                'ID',
                'Message',
                'Create Date',
                'Start Date',
                'End Date'
            ];

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'messageUpdate';
            createListParams.updateParams = updateParams;
            createListParams.deleteMethodName = 'messageDelete';
            createListParams.deleteParams = deleteParams;

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.messageUpdate = function (id, message, startDate, endDate) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('messagesList', 'Messages');
    var formBreadcrumb = new io.ui.breadcrumb('messagesAdd', 'Add a new message', [ breadcrumbNavigation ]);

    var messageFormData = new io.ui.formData(io.ui.formDataTypes.textArea, 'message', 'Message', 'Message');
    var messageValidation = new ioValidation(io.validationRuleTypes.minLength, 'Message is too short.', 'message', 'Invalid message.');
    messageValidation.length = 3;
    messageFormData.validations = [ messageValidation ];
    messageFormData.value = message;

    var startDateFormData = new io.ui.formData(io.ui.formDataTypes.date, 'startDate', 'Start Date', 'MessageStartDate');
    startDateFormData.value = startDate;
    var endDateFormData = new io.ui.formData(io.ui.formDataTypes.date, 'endDate', 'End Date', 'MessageEndDate');
    endDateFormData.value = endDate;

    var formDatas = [
        messageFormData,
        startDateFormData,
        endDateFormData
    ];

    io.ui.createForm('messagesUpdate', formBreadcrumb, 'addMessageForm', formDatas, 'Save', function () {
        },
        function (request) {
            request.MessageId = id;
            let requestURLFormat = '%s/UpdateMessagesItem';
            let requestURL = requestURLFormat.format(IOGlobal.messagesControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Message has been updated successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.messagesList(null, 'messagesList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
    });
};

io.prototype.app.usersList = function (e, hash) {
    let io = window.ioinstance;

    let resources = [
        'BackOffice.Edit',
        'BackOffice.Delete',
        'BackOffice.Options',
        'BackOffice.Select',
        'BackOffice.Home',
        'BackOffice.Users',
        'BackOffice.ChangePassword',
        'BackOffice.ID',
        'BackOffice.Name',
        'BackOffice.Role',
        'BackOffice.LastLoginDate',
        'BackOffice.Error'
    ];

    // Show indicator
    io.indicator.show();

    io.resources.getResources(resources, function() {
        let breadcrumb = new io.ui.breadcrumb('usersList', io.resources.get('BackOffice.Users'), []);

        let requestURLFormat = '%s/ListUsers';
        let requestURL = requestURLFormat.format(IOGlobal.userControllerName);
        io.service.get(requestURL, function(status, response, error) {
            if (status && response.status.success) {
                var listData = [];
                var updateParams = [];
                var deleteParams = [];
                var extraParams = [];

                for (var index in response.users) {
                    var user = response.users[index];
    
                    var roleName = window.ioinstance.userRoles.getRoleName(user.userRole);
                    var itemListData = [
                        user.id,
                        user.userName,
                        roleName,
                        user.tokenDate
                    ];
    
                    listData.push(itemListData);
    
                    var itemUpdateData = [
                        user.id,
                        user.userName,
                        user.userRole
                    ];
    
                    updateParams.push(itemUpdateData);
                    deleteParams.push([user.id]);
    
                    let itemExtraParams = [
                        new io.ui.extraParam(io.resources.get('BackOffice.ChangePassword'), 'fa-key', 'userChangePassword', [null, 'usersUpdate', user.id, user.userName])
                    ];
    
                    extraParams.push(itemExtraParams);
                }

                let listDataHeaders = [
                    io.resources.get('BackOffice.ID'),
                    io.resources.get('BackOffice.Name'),
                    io.resources.get('BackOffice.Role'),
                    io.resources.get('BackOffice.LastLoginDate')
                ];

                let resources = io.ui.resourcesModel;
                resources.edit = io.resources.get('BackOffice.Edit');
                resources.delete = io.resources.get('BackOffice.Delete');
                resources.home = io.resources.get('BackOffice.Home');
                resources.options = io.resources.get('BackOffice.Options');
                resources.select = io.resources.get('BackOffice.Select');

                let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
                });
                createListParams.updateMethodName = 'userUpdate';
                createListParams.updateParams = updateParams;
                createListParams.deleteMethodName = 'userDelete';
                createListParams.deleteParams = deleteParams;
                createListParams.resources = resources;
                createListParams.extraParams = extraParams;
    
                io.ui.createListData(createListParams);
            } else {
                window.ioinstance.indicator.hide();
                window.ioinstance.callout.show(window.ioinstance.callout.types.danger, io.resources.get('BackOffice.Error'), '');
            }
        });
    });
};

io.prototype.app.userUpdate = function (id, userName, userRole) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('usersList', 'Users');
    var formBreadcrumb = new io.ui.breadcrumb('userUpdate', 'Update user', [ breadcrumbNavigation ]);

    var userNameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'userName', 'User Name', 'UserName');
    var userNameValidation = new ioValidation(io.validationRuleTypes.minLength, 'User name is too short.', 'userName', 'Invalid user name.');
    userNameValidation.length = 3;
    userNameFormData.validations = [ userNameValidation ];
    userNameFormData.value = userName;

    var roleFormData = new io.ui.formData(io.ui.formDataTypes.select, 'role', 'Role', 'UserRole');
    roleFormData.value = userRole;
    roleFormData.options = io.userRoles.getRoleList();

    var formDatas = [
        userNameFormData,
        roleFormData
    ];

    io.ui.createForm('userUpdate', formBreadcrumb, 'updateUserForm', formDatas, 'Save', function () {
        },
        function (request) {

            request.UserId = id;
            request.UserRole = parseInt(request.UserRole);

            let requestURLFormat = '%s/UpdateUser';
            let requestURL = requestURLFormat.format(IOGlobal.userControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User has been updated successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.usersList(null, 'usersList');
                } else if (response.status.code === 700) {
                    var helpText = 'User ' + request.UserName + ' is exists.';
                    callout.show(callout.types.danger, 'Invalid username.', helpText);
                    window.ioinstance.indicator.hide();
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.userChangePassword = function (e, hash, id, userName) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('usersList', 'Users');
    var formBreadcrumb = new io.ui.breadcrumb('userUpdate', 'Change password', [ breadcrumbNavigation ]);

    var currentPasswordFormData = new io.ui.formData(io.ui.formDataTypes.password, 'currentPassword', 'Current Password', 'OldPassword');
    var currentPasswordMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Password is too short.', 'currentPassword', 'Invalid password.');
    currentPasswordMinLengthValidation.length = 3;
    currentPasswordFormData.validations = [ currentPasswordMinLengthValidation ];

    var passwordFormData = new io.ui.formData(io.ui.formDataTypes.password, 'password', 'Password', 'NewPassword');
    var passwordMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Password is too short.', 'password', 'Invalid password.');
    passwordMinLengthValidation.length = 1;
    passwordFormData.validations = [ passwordMinLengthValidation ];

    var passwordRepeatFormData = new io.ui.formData(io.ui.formDataTypes.password, 'repeatedPassword', 'Password (Repeat)', 'PasswordRepeated');
    var passwordRepeatMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Password is too short.', 'repeatedPassword', 'Invalid password.');
    passwordRepeatMinLengthValidation.length = 3;
    var passwordMatchValidation = new ioValidation(io.validationRuleTypes.matchRule, 'Passwords did not match.', 'repeatedPassword', 'Invalid password.');
    passwordMatchValidation.otherInput = 'password';
    passwordRepeatFormData.validations = [ passwordRepeatMinLengthValidation, passwordMatchValidation ];

    var currentPasswordIsHidden = (window.ioinstance.userRole === window.ioinstance.userRoles.superAdmin) ? 'hidden' : '';
    var formDatas = [];

    if (!currentPasswordIsHidden) {
        formDatas.push(currentPasswordFormData);
    }

    formDatas.push(passwordFormData);
    formDatas.push(passwordRepeatFormData);

    io.ui.createForm(hash, formBreadcrumb, 'changePassword', formDatas, 'Save', function () {
        },
        function (request) {

            request.UserName = userName || localStorage.getItem('userName');
            let requestURLFormat = '%s/ChangePassword';
            let requestURL = requestURLFormat.format(IOGlobal.userControllerName);

            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User password has been changed successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.usersList(null, 'usersList');
                } else {
                    callout.show(callout.types.danger, 'Invalid password.', 'Current password is incorrect.');
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.userDelete = function (id) {
    var answer = confirm("Are you sure want to delete this user ?");
    if (answer) {
        var request = window.ioinstance.request.UserDeleteRequest;
        request.Version = window.ioinstance.version;
        request.UserId = id;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteUser';
        let requestURL = requestURLFormat.format(IOGlobal.userControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'User has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.usersList(null, 'usersList');
        });
    }
};

io.prototype.app.usersLogout = function (e, hash) {
    localStorage.removeItem('userName');
    localStorage.removeItem('token');
    window.ioinstance.token = '';
    window.ioinstance.userRole = -1;
    window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName });
};

io.prototype.app.usersAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('usersList', 'Users');
    var formBreadcrumb = new io.ui.breadcrumb('usersAdd', 'Add a new user', [ breadcrumbNavigation ]);

    var userNameFormData = new io.ui.formData(io.ui.formDataTypes.text, 'userName', 'User Name', 'UserName');
    var userNameValidation = new ioValidation(io.validationRuleTypes.minLength, 'User name is too short.', 'userName', 'Invalid user name.');
    userNameValidation.length = 3;
    userNameFormData.validations = [ userNameValidation ];

    var passwordFormData = new io.ui.formData(io.ui.formDataTypes.password, 'password', 'Password', 'Password');
    var passwordMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Password is too short.', 'password', 'Invalid password.');
    passwordMinLengthValidation.length = 3;
    passwordFormData.validations = [ passwordMinLengthValidation ];

    var passwordRepeatFormData = new io.ui.formData(io.ui.formDataTypes.password, 'repeatedPassword', 'Password (Repeat)', 'PasswordRepeated');
    var passwordRepeatMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Password is too short.', 'repeatedPassword', 'Invalid password.');
    passwordRepeatMinLengthValidation.length = 3;
    var passwordMatchValidation = new ioValidation(io.validationRuleTypes.matchRule, 'Passwords did not match.', 'repeatedPassword', 'Invalid password.');
    passwordMatchValidation.otherInput = 'password';
    passwordRepeatFormData.validations = [ passwordRepeatMinLengthValidation, passwordMatchValidation ];

    var roleFormData = new io.ui.formData(io.ui.formDataTypes.select, 'role', 'Role', 'UserRole');
    roleFormData.options = io.userRoles.getRoleList();

    var formDatas = [
        userNameFormData,
        passwordFormData,
        passwordRepeatFormData,
        roleFormData
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addUserForm', formDatas, 'Save', function () {
    },
    function (request) {
        request.UserRole = parseInt(request.UserRole);

        let requestURLFormat = '%s/AddUser';
        let requestURL = requestURLFormat.format(IOGlobal.userControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;

            if (status && response.status.success) {
                callout.show(callout.types.success, 'User has been added successfully.', '');
                window.location.hash = '';
                window.ioinstance.app.usersList(null, 'usersList');
            } else if (response.status.code === 700) {
                var helpText = 'User ' + request.UserName + ' is exists.';
                callout.show(callout.types.danger, 'Invalid username.', helpText);
                window.ioinstance.indicator.hide();
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }
        });
    });
};

io.prototype.app.pushNotificationList = function (e, hash) {
    let io = window.ioinstance;

    let resources = [
        'BackOffice.PushNotificationMessages',
        'BackOffice.Sending',
        'BackOffice.Completed',
        'BackOffice.ID',
        'BackOffice.Client',
        'BackOffice.Date',
        'BackOffice.Category',
        'BackOffice.Data',
        'BackOffice.Message',
        'BackOffice.Title',
        'BackOffice.Status',
        'BackOffice.SendedDevices',
        'BackOffice.Error'
    ];

    // Show indicator
    io.indicator.show();

    io.resources.getResources(resources, function() {
        let breadcrumb = new io.ui.breadcrumb('pushNotificationList', io.resources.get('BackOffice.PushNotificationMessages'), []);

        let requestURLFormat = '%s/ListMessages';
        let requestURL = requestURLFormat.format(IOGlobal.pushNotificationsControllerName);

        io.service.get(requestURL, function(status, response, error) {
            if (status && response.status.success) {
                var listData = [];
                var deleteParams = [];

                for (var index in response.messages) {
                    var message = response.messages[index];
    
                    let completed = (message.isCompleted === 0) ? io.resources.get('BackOffice.Sending') : io.resources.get('BackOffice.Completed');
                    let notificationDate = new Date(message.notificationDate);
                    let clientDescription = (message.client != null) ? message.client.clientDescription : "";

                    var itemListData = [
                        message.id,
                        clientDescription,
                        notificationDate.toLocaleDateString(),
                        message.notificationCategory,
                        message.notificationData,
                        message.notificationMessage,
                        message.notificationTitle,
                        completed
                        // message.sendedDevices
                    ];
    
                    listData.push(itemListData);
                    deleteParams.push([message.id]);
                }

                let listDataHeaders = [
                    io.resources.get('BackOffice.ID'),
                    io.resources.get('BackOffice.Client'),
                    io.resources.get('BackOffice.Date'),
                    io.resources.get('BackOffice.Category'),
                    io.resources.get('BackOffice.Data'),
                    io.resources.get('BackOffice.Message'),
                    io.resources.get('BackOffice.Title'),
                    io.resources.get('BackOffice.Status')
                    // io.resources.get('BackOffice.SendedDevices')
                ];

                let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
                });
                createListParams.deleteMethodName = 'pushNotificationMessageDelete';
                createListParams.deleteParams = deleteParams;

                io.ui.createList(createListParams);
            } else {
                window.ioinstance.indicator.hide();
                window.ioinstance.callout.show(window.ioinstance.callout.types.danger, io.resources.get('BackOffice.Error'), '');
            }
        });
    });
};

io.prototype.app.pushNotificationMessageDelete = function (id) {
    var answer = confirm("Are you sure want to delete this message ?");
    if (answer) {
        var request = window.ioinstance.request.PushNotificationMessageDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ID = id;
        window.ioinstance.indicator.show();

        let requestURLFormat = '%s/DeleteMessage';
        let requestURL = requestURLFormat.format(IOGlobal.pushNotificationsControllerName);

        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Message has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.pushNotificationList(null, 'pushNotificationList');
        });
    }
};

io.prototype.app.pushNotificationSend = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('sendNotification', 'Send Push Notification', []);

    var deviceTypeFormData = new io.ui.formData(io.ui.formDataTypes.select, 'deviceType', 'Device Type', 'DeviceType');
    deviceTypeFormData.value = 2;
    deviceTypeFormData.options = [
        new io.ui.formDataOptions('Android', 0),
        new io.ui.formDataOptions('iOS', 1),
        new io.ui.formDataOptions('Generic', 2)
    ];

    var clientFormData = new io.ui.formData(io.ui.formDataTypes.popupSelection, 'client', 'Client', 'ClientId');
    clientFormData.params = '';
    clientFormData.methodName = 'clientsSelect';
    // var clientMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Client is not selected.', 'client', 'Invalid client.');
    // clientMinLengthValidation.length = 1;
    // clientFormData.validations = [ clientMinLengthValidation ];

    var categoryFormData = new io.ui.formData(io.ui.formDataTypes.select, 'notificationCategory', 'Category', 'NotificationCategory');;
    categoryFormData.options = io.pushNotificationCategories.getCategoryList();

    var titleFormData = new io.ui.formData(io.ui.formDataTypes.text, 'notificationTitle', 'Title', 'NotificationTitle');
    var messageFormData = new io.ui.formData(io.ui.formDataTypes.text, 'notificationMessage', 'Message', 'NotificationMessage');
    var messageMinLengthValidation = new ioValidation(io.validationRuleTypes.minLength, 'Message is too short.', 'notificationMessage', 'Invalid notification message.');
    messageMinLengthValidation.length = 3;
    messageFormData.validations = [ messageMinLengthValidation ];

    var dataFormData = new io.ui.formData(io.ui.formDataTypes.text, 'notificationData', 'Data', 'NotificationData');

    var formDatas = [
        deviceTypeFormData,
        clientFormData,
        categoryFormData,
        titleFormData,
        messageFormData,
        dataFormData
    ];

    io.ui.createForm(hash, breadcrumb, 'sendNotificationForm', formDatas, 'Save', function () {
        },
        function (request) {
            request.ClientId = window.ioinstance.ui.getPopupSelectionValue('client');
            request.DeviceType = parseInt(request.DeviceType);

            let requestURLFormat = '%s/SendNotification';
            let requestURL = requestURLFormat.format(IOGlobal.pushNotificationsControllerName);

            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Push notification has been send successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.pushNotificationList(null, 'pushNotificationList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.resourceAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('resourcesList', 'Resources');
    var formBreadcrumb = new io.ui.breadcrumb('resourceAdd', 'Add a resource', [ breadcrumbNavigation ]);

    var resourceKey = new io.ui.formData(io.ui.formDataTypes.text, 'resourceKey', 'Key', 'ResourceKey');
    var keyValidation = new ioValidation(io.validationRuleTypes.minLength, 'Key is too short.', 'resourceKey', 'Invalid resource key.');
    keyValidation.length = 1;
    resourceKey.validations = [ keyValidation ];

    var resourceValue = new io.ui.formData(io.ui.formDataTypes.text, 'resourceValue', 'Value', 'ResourceValue');
    var valueValidation = new ioValidation(io.validationRuleTypes.minLength, 'Value is too short.', 'resourceValue', 'Invalid resource value.');
    valueValidation.length = 1;
    resourceValue.validations = [ valueValidation ];

    var formDatas = [
        resourceKey,
        resourceValue
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addResourceForm', formDatas, 'Save', function () {
        },
        function (request) {

            if (request.ParentEntityID === '') {
                request.ParentEntityID = null;
            } else {
                request.ParentEntityID = window.ioinstance.ui.getPopupSelectionValue('parentMenu');
            }

            let requestURLFormat = '%s/AddResource';
            let requestURL = requestURLFormat.format(IOGlobal.resourcesControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Resource has been added successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.resourcesList(null, 'resourceList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.resourceDelete = function (resourceID) {
    var answer = confirm("Are you sure want to delete this resource ?");
    if (answer) {
        var request = window.ioinstance.request.ResourceDeleteRequestModel;
        request.Version = window.ioinstance.version;
        request.ID = resourceID;
        window.ioinstance.indicator.show();
        let requestURLFormat = '%s/DeleteResource';
        let requestURL = requestURLFormat.format(IOGlobal.resourcesControllerName);
        window.ioinstance.service.post(requestURL, request, function (status, response, error) {
            let callout = window.ioinstance.callout;
            if (status && response.status.success) {
                callout.show(callout.types.success, 'Resource has been deleted successfully.', '');
            } else {
                callout.show(callout.types.danger, error.message, error.detailedMessage);
                window.ioinstance.indicator.hide();
            }

            window.ioinstance.app.resourcesList(null, 'resourcesList');
        });
    }
}

io.prototype.app.resourcesList = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('resourcesList', 'Resources', []);

    let requestURLFormat = '%s/GetAllResources';
    let requestURL = requestURLFormat.format(IOGlobal.resourcesControllerName);
    io.service.get(requestURL, function(status, response, error) {
        if (status && response.status.success) {
            let listData = [];
            let updateParams = [];
            let deleteParams = [];

            for (let index in response.resources) {
                var resource = response.resources[index];

                var itemListData = [
                    resource.resourceID,
                    resource.resourceKey,
                    resource.resourceValue
                ];

                listData.push(itemListData);

                var itemUpdateData = [
                    resource.resourceID,
                    resource.resourceKey,
                    resource.resourceValue
                ];

                updateParams.push(itemUpdateData);
                deleteParams.push([resource.resourceID]);
            }

            let listDataHeaders = [
                'ID',
                'Key',
                'Value'
            ];

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'resourceUpdate';
            createListParams.updateParams = updateParams;
            createListParams.deleteMethodName = 'resourceDelete';
            createListParams.deleteParams = deleteParams;

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.resourceUpdate = function (resourceID, resourceKeyData, resourceValueData) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('resourcesList', 'Resources');
    var formBreadcrumb = new io.ui.breadcrumb('resourceUpdate', 'Update a resource', [ breadcrumbNavigation ]);

    var resourceKey = new io.ui.formData(io.ui.formDataTypes.text, 'resourceKey', 'Key', 'ResourceKey');
    var keyValidation = new ioValidation(io.validationRuleTypes.minLength, 'Key is too short.', 'resourceKey', 'Invalid resource key.');
    keyValidation.length = 1;
    resourceKey.value = resourceKeyData;
    resourceKey.validations = [ keyValidation ];

    var resourceValue = new io.ui.formData(io.ui.formDataTypes.text, 'resourceValue', 'Value', 'ResourceValue');
    var valueValidation = new ioValidation(io.validationRuleTypes.minLength, 'Value is too short.', 'resourceValue', 'Invalid resource value.');
    valueValidation.length = 1;
    resourceValue.value = resourceValueData;
    resourceValue.validations = [ valueValidation ];

    var formDatas = [
        resourceKey,
        resourceValue
    ];

    io.ui.createForm('resourceUpdate', formBreadcrumb, 'addMenuForm', formDatas, 'Save', function () {
        },
        function (request) {
            request.ResourceID = resourceID;

            let requestURLFormat = '%s/UpdateResource';
            let requestURL = requestURLFormat.format(IOGlobal.resourcesControllerName);
            window.ioinstance.service.post(requestURL, request, function (status, response, error) {
                let callout = window.ioinstance.callout;

                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Resource has been updated successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.resourcesList(null, 'resourceList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
};

io.prototype.app.resetCache = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    let requestURLFormat = '%s/resetCache';
    let requestURL = requestURLFormat.format(IOGlobal.configurationControllerName);
    window.ioinstance.service.get(requestURL, function (status, response, error) {
        let callout = window.ioinstance.callout;
        if (status && response.status.success) {
            callout.show(callout.types.success, 'Application has been cleaned.', '');
        } else {
            callout.show(callout.types.danger, error.message, error.detailedMessage);
        }

        window.location.hash = "";
        window.location.reload();
    });
};