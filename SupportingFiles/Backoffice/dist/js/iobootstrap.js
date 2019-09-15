io.prototype.inited = function() {
};

io.prototype.request.ClientAddRequest = {
    Culture: 0,
    Version: '',
    ClientDescription: '',
    RequestCount: 0
};

io.prototype.request.ClientDeleteRequest = {
    Culture: 0,
    Version: '',
    ClientId: 0
};

io.prototype.request.ClientUpdateRequest = {
    Culture: 0,
    Version: '',
    ClientId: 0,
    ClientDescription: '',
    IsEnabled: 0,
    RequestCount: 0,
    MaxRequestCount: 0
};

io.prototype.request.ConfigurationAddRequest = {
    Culture: 0,
    Version: '',
    ConfigKey: '',
    IntValue: 0,
    StrValue: ''
};

io.prototype.request.ConfigurationDeleteRequest = {
    Culture: 0,
    Version: '',
    ConfigId: 0
};

io.prototype.request.ConfigurationUpdateRequest = {
    Culture: 0,
    Version: '',
    ConfigId: 0,
    ConfigKey: '',
    IntValue: 0,
    StrValue: ''
};

io.prototype.request.MenuUpdateRequestModel = {
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

io.prototype.request.SendNotificationRequest = {
    Culture: 0,
    Version: '',
    ClientId: 0,
    DeviceType: 0,
    NotificationCategory: '',
    NotificationData: '',
    NotificationMessage: '',
    NotificationTitle: ''
};

io.prototype.request.UserAddRequest = {
    Culture: 0,
    Version: '',
    UserName: '',
    Password: '',
    UserRole: 0
};

io.prototype.request.UserChangePasswordRequest = {
    Culture: 0,
    Version: '',
    UserName: '',
    OldPassword: '',
    NewPassword: ''
};

io.prototype.request.UserDeleteRequest = {
    Culture: 0,
    Version: '',
    UserId: 0
};

io.prototype.request.UserUpdateRequest = {
    Culture: 0,
    Version: '',
    UserId: 0,
    UserName: '',
    UserRole: 0
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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/clients/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('client', function (layout) {
                var clientsHtml = '';
                var clientLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.clientList) {
                    var client = response.clientList[index];
                    var clientLayoutData = {
                        id: client.id,
                        clientID: client.clientID,
                        clientSecret: client.clientSecret,
                        clientDescription: client.clientDescription,
                        isEnabled: (client.isEnabled == 1) ? 'YES' : 'NO',
                        requestCount: client.requestCount,
                        maxRequestCount: client.maxRequestCount
                    };

                    clientsHtml += window.ioinstance.layout.renderLayout(layout, clientLayoutData, clientLayoutProperties);
                }

                window.ioinstance.service.loadLayout('clientslist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        clients: clientsHtml
                    };
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);
                });
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.clientUpdate = function (id, clientDescription, isEnabled, requestCount, maxRequestCount) {
    // Show indicator
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('clientupdate', false, function () {
        var options = '';

        if (isEnabled == 'YES') {
            options += '<option value="0">NO</option>';
            options += '<option value="1" selected="selected">YES</option>';
        } else {
            options += '<option value="0" selected="selected">NO</option>';
            options += '<option value="1">YES</option>';
        }

        window.ioinstance.layout.contentLayoutData = {
            id: id,
            clientDescription: clientDescription,
            requestCount: requestCount,
            maxRequestCount: maxRequestCount,
            options: options
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('clientsUpdate');

        $('#updateClientForm').submit(function (e) {
            e.preventDefault();
            var request = window.ioinstance.request.ClientUpdateRequest;
            request.Version = window.ioinstance.version;
            request.ClientId = parseInt($('#updateClientForm').attr('data-id'));
            request.ClientDescription = $('#clientDescription').val();
            request.IsEnabled = parseInt($('#clientIsEnabled').val());
            request.RequestCount = parseInt($('#clientRequestCount').val());
            request.MaxRequestCount = parseInt($('#clientMaxRequestCount').val());

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/clients/update', request, function (status, response, error) {
                var callout = window.ioinstance.callout;
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Client has been updated successfully.', '');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }

                window.ioinstance.app.clientsList(null, 'clientsList');
            });
        });
    });
};

io.prototype.app.clientsUpdate = function (e, hash) {
    window.ioinstance.app.clientsList(e, hash);
};

io.prototype.app.clientDelete = function (id) {
    var answer = confirm("Are you sure want to delete this client ?");
    if (answer) {
        var request = window.ioinstance.request.ClientDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ClientId = id;
        window.ioinstance.indicator.show();
        window.ioinstance.service.post('backoffice/clients/delete', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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

io.prototype.app.clientsDelete = function (e, hash) {
    window.ioinstance.app.clientsList(e, hash);
};

io.prototype.app.clientsAdd = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('clientsadd', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#addClientForm').submit(function (e) {
            e.preventDefault();
            var request = window.ioinstance.request.ClientAddRequest;
            request.Version = window.ioinstance.version;
            request.ClientDescription = $('#clientDescription').val();
            request.RequestCount = $('#clientRequestCount').val();
            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/clients/add', request, function (status, response, error) {
                var callout = window.ioinstance.callout;
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Client has been added successfully.', '');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }

                window.ioinstance.app.clientsList(null, 'clientsList');
            });
        });
    });
};

io.prototype.app.clientsSelect = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/clients/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('clientselectitem', function (layout) {
                var clientsHtml = '';
                var clientLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.clientList) {
                    var client = response.clientList[index];
                    var clientLayoutData = {
                        id: client.id,
                        clientID: client.clientID,
                        clientSecret: client.clientSecret,
                        clientDescription: client.clientDescription,
                        isEnabled: (client.isEnabled == 1) ? 'YES' : 'NO',
                        requestCount: client.requestCount,
                        maxRequestCount: client.maxRequestCount
                    };

                    clientsHtml += window.ioinstance.layout.renderLayout(layout, clientLayoutData, clientLayoutProperties);
                }

                window.ioinstance.service.loadLayout('clientslist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        clients: clientsHtml
                    };
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);
                });
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.clientSelectItem = function (e, hash) {
    var client = $('#client');
    client.attr('data-clientId', e[0]);
    client.val(e[1]);
};

io.prototype.app.configurationDelete = function(id) {
    var answer = confirm("Are you sure want to delete this configuration ?");
    if (answer) {
        var request = window.ioinstance.request.ConfigurationDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ConfigId = id;
        window.ioinstance.indicator.show();
        window.ioinstance.service.post('backoffice/configurations/delete', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('configurationsadd', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#addConfigurationForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.ConfigurationAddRequest;
            request.Version = window.ioinstance.version;
            request.ConfigKey = $('#configKey').val();
            request.StrValue = $('#strValue').val();
            request.IntValue = parseInt($('#intValue').val());

            $('.configKeyArea').removeClass('has-error');
            $('.configKeyAreaHelp').addClass('hidden');

            if (request.ConfigKey.length <= 3) {
                callout.show(callout.types.danger, 'Invalid configuration key.', 'Configuration key is too short.');
                $('.configKeyArea').addClass('has-error');
                $('.configKeyAreaHelp').removeClass('hidden');
                $('.configKeyAreaHelp').text('Configuration key is too short.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/configurations/add', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Configuration has been added successfully.', '');
                    window.ioinstance.app.configurationsList(null, 'configurationsList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.configurationsList = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/configurations/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('configuration', function (layout) {
                var configurationHtml = '';
                var configurationLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.configurations) {
                    var configuration = response.configurations[index];
                    var configurationLayoutData = {
                        id: configuration.id,
                        configKey: configuration.configKey,
                        configIntValue: configuration.configIntValue,
                        configStringValue: configuration.configStringValue,
                        configEscapedStringValue: configuration.configStringValue.escapeHtml()
                    };

                    configurationHtml += window.ioinstance.layout.renderLayout(layout, configurationLayoutData, configurationLayoutProperties);
                }

                window.ioinstance.service.loadLayout('configurationlist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        configurations: configurationHtml
                    };
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);
                });
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.configurationUpdate = function (id, key, intValue, stringValue) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu('configurationsList');

    io.service.loadLayout('configurationsupdate', false, function () {
        window.ioinstance.layout.contentLayoutData = {
            id: id,
            configKey: key,
            configIntValue: intValue,
            configStrValue: stringValue.unEscapeHtml()
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('configurationsList');

        $('#updateConfigurationForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.ConfigurationUpdateRequest;
            request.Version = window.ioinstance.version;
            request.ConfigId = parseInt($(this).attr('data-configId'));
            request.ConfigKey = $('#configKey').val();
            request.StrValue = $('#strValue').val();
            request.IntValue = parseInt($('#intValue').val());

            $('.configKeyArea').removeClass('has-error');
            $('.configKeyAreaHelp').addClass('hidden');

            if (request.ConfigKey.length <= 3) {
                callout.show(callout.types.danger, 'Invalid configuration key.', 'Configuration key is too short.');
                $('.configKeyArea').addClass('has-error');
                $('.configKeyAreaHelp').removeClass('hidden');
                $('.configKeyAreaHelp').text('Configuration key is too short.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/configurations/update', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Configuration has been updated successfully.', '');
                    window.ioinstance.app.configurationsList(null, 'configurationsList');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.menuEditorList = function(e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumb = new io.ui.breadcrumb('menuEditorList', 'Menu Editor', []);

    // Call client list
    io.service.get('backoffice/menu/list', function(status, response, error) {
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

            var listDataHeaders = [
                'ID',
                'Name',
                'Action',
                'Css Class',
                'Role',
                'Order',
                'Options'
            ];

            io.ui.createList(hash, breadcrumb, listDataHeaders, listData, 'menuEditorUpdate', updateParams, 'menuEditorDelete', deleteParams, hasRowClasses, function () {
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.menuEditorAdd = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumbNavigation = new io.ui.breadcrumbNavigation('menuEditorList', 'Menu Editor');
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

            if (request.ParentEntityID === '') {
                request.ParentEntityID = null;
            } else {
                request.ParentEntityID = parseInt($('#parentMenu').attr('data-params'));
            }

            window.ioinstance.service.post('backoffice/menu/add', request, function (status, response, error) {
                var callout = window.ioinstance.callout;

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
        var request = window.ioinstance.request.MenuUpdateRequestModel;
        request.Version = window.ioinstance.version;
        request.ID = id;
        window.ioinstance.indicator.show();
        window.ioinstance.service.post('backoffice/menu/delete', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumbNavigation = new io.ui.breadcrumbNavigation('menuEditorList', 'Menu Editor');
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

            if (request.ParentEntityID === '') {
                request.ParentEntityID = null;
            } else {
                request.ParentEntityID = parseInt($('#parentMenu').attr('data-params'));
            }

            window.ioinstance.service.post('backoffice/menu/update', request, function (status, response, error) {
                var callout = window.ioinstance.callout;

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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumb = new io.ui.breadcrumb('menuEditorList', 'Menu Editor', []);

    // Call client list
    io.service.get('backoffice/menu/list', function(status, response, error) {
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

            var listDataHeaders = [
                'ID',
                'Name',
                'Action',
                'Css Class',
                'Role',
                'Order',
                'Options'
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
        window.ioinstance.service.post('backoffice/messages/delete', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumbNavigation = new io.ui.breadcrumbNavigation('messagesList', 'Messages');
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
            window.ioinstance.service.post('backoffice/messages/add', request, function (status, response, error) {
                var callout = window.ioinstance.callout;

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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumb = new io.ui.breadcrumb('messagesList', 'Messages', []);

    // Call client list
    io.service.get('backoffice/messages/listall', function(status, response, error) {
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

            var listDataHeaders = [
                'ID',
                'Message',
                'Create Date',
                'Start Date',
                'End Date',
                'Options'
            ];

            io.ui.createList(hash, breadcrumb, listDataHeaders, listData, 'messageUpdate', updateParams, 'messageDelete', deleteParams, null, function () {
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.messageUpdate = function (id, message, startDate, endDate) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    var breadcrumbNavigation = new io.ui.breadcrumbNavigation('messagesList', 'Messages');
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
            window.ioinstance.service.post('backoffice/messages/update', request, function (status, response, error) {
                var callout = window.ioinstance.callout;

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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/users/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('user', function (layout) {
                var usersHtml = '';
                var userLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.users) {
                    var user = response.users[index];
                    var roleName = window.ioinstance.userRoles.getRoleName(user.userRole);
                    var userLayoutData = {
                        id: user.id,
                        userName: user.userName,
                        userRole: roleName,
                        tokenDate: user.tokenDate
                    };

                    usersHtml += window.ioinstance.layout.renderLayout(layout, userLayoutData, userLayoutProperties);
                }

                window.ioinstance.service.loadLayout('userslist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        users: usersHtml
                    };
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);
                });
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.userUpdate = function (id, userName, userRole) {
    // Show indicator
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('userupdate', false, function () {
        var roleList = window.ioinstance.userRoles.getRoleSelection(userRole);
        window.ioinstance.layout.contentLayoutData = {
            id: id,
            userName: userName,
            userRole: userRole,
            roleList: roleList
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('usersUpdate');

        $('#updateUserForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.UserUpdateRequest;
            request.Version = window.ioinstance.version;
            request.UserId = parseInt($(this).attr('data-id'));
            request.UserName = $('#userName').val();
            request.UserRole = parseInt($('#userRole').val());

            if (request.UserName.length <= 3) {
                callout.show(callout.types.danger, 'Invalid username.', 'User name is too sort.');
                $('.userNameArea').addClass('has-error');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/users/update', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User has been updated successfully.', '');
                    window.ioinstance.app.usersList(null, 'usersList');
                } else if (response.status.code === window.ioinstance.response.StatusCodes.USER_EXISTS) {
                    var helpText = 'User ' + request.UserName + ' is exists.';
                    callout.show(callout.types.danger, 'Invalid username.', helpText);
                    $('.userNameArea').addClass('has-error');
                    window.ioinstance.indicator.hide();
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.usersUpdate = function (e, hash) {
    window.ioinstance.app.usersList(e, hash);
};

io.prototype.app.userChangePassword = function (e, hash, id, userName) {
    // Show indicator
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('userchangepassword', false, function () {
        var currentPasswordIsHidden = (window.ioinstance.userRole == window.ioinstance.userRoles.superAdmin) ? 'hidden' : '';
        window.ioinstance.layout.contentLayoutData = {
            userName: userName || localStorage.getItem('userName'),
            currentPasswordIsHidden: currentPasswordIsHidden
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('usersUpdate');
        var changePasswordForm = $('#changePasswordForm');

        changePasswordForm.submit(function (e) {
            e.preventDefault();

            var passwordArea = $('.passwordArea');
            var passwordAreaHelp = $('.passwordAreaHelp');

            var callout = window.ioinstance.callout;
            var repeatedpassword = $('#repeatedpassword').val();
            var request = window.ioinstance.request.UserChangePasswordRequest;
            request.Version = window.ioinstance.version;
            request.UserName = changePasswordForm.attr('data-id');
            request.OldPassword = $('#currentPassword').val();
            request.NewPassword = $('#password').val();

            passwordArea.removeClass('has-error');
            passwordAreaHelp.addClass('hidden');

            if (request.NewPassword.length <= 3) {
                callout.show(callout.types.danger, 'Invalid password.', 'Password is too short.');
                passwordArea.addClass('has-error');
                passwordAreaHelp.removeClass('hidden');
                passwordAreaHelp.text('Password is too short.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.NewPassword != repeatedpassword) {
                callout.show(callout.types.danger, 'Invalid password.', 'Passwords did not match.');
                passwordArea.addClass('has-error');
                passwordAreaHelp.removeClass('hidden');
                passwordAreaHelp.text('Passwords did not match.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/users/password/change', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User password has been changed successfully.', '');
                } else {
                    callout.show(callout.types.danger, 'Invalid password.', 'Current password is incorrect.');
                    passwordArea.addClass('has-error');
                    passwordAreaHelp.removeClass('hidden');
                    passwordAreaHelp.text('Current password is incorrect.');
                }
                window.ioinstance.indicator.hide();
            });
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
        window.ioinstance.service.post('backoffice/users/delete', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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

io.prototype.app.usersDelete = function (e, hash) {
    window.ioinstance.app.usersList(e, hash);
};

io.prototype.app.usersAdd = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('usersadd', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#addUserForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var repeatedpassword = $('#repeatedpassword').val();
            var request = window.ioinstance.request.UserAddRequest;
            request.Version = window.ioinstance.version;
            request.UserName = $('#userName').val();
            request.Password = $('#password').val();
            request.UserRole = parseInt($('#role').val());

            var passwordArea = $('.passwordArea');
            passwordArea.removeClass('has-error');
            var passwordAreaHelp = $('.passwordAreaHelp');
            passwordAreaHelp.addClass('hidden');

            if (request.Password.length <= 3) {
                callout.show(callout.types.danger, 'Invalid password.', 'Password is too short.');
                passwordArea.addClass('has-error');
                passwordAreaHelp.removeClass('hidden');
                passwordAreaHelp.text('Password is too short.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.Password != repeatedpassword) {
                callout.show(callout.types.danger, 'Invalid password.', 'Passwords did not match.');
                passwordArea.addClass('has-error');
                passwordAreaHelp.removeClass('hidden');
                passwordAreaHelp.text('Passwords did not match.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.UserName.length <= 3) {
                callout.show(callout.types.danger, 'Invalid username.', 'User name is too sort.');
                $('.userNameArea').addClass('has-error');
                passwordAreaHelp.removeClass('hidden');
                passwordAreaHelp.text('User name is too sort.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/users/add', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User has been added successfully.', '');
                    window.ioinstance.app.usersList(null, 'usersList');
                } else if (response.status.code === window.ioinstance.response.StatusCodes.USER_EXISTS) {
                    var helpText = 'User ' + request.UserName + ' is exists.';
                    callout.show(callout.types.danger, 'Invalid username.', helpText);
                    $('.userNameArea').addClass('has-error');
                    passwordAreaHelp.removeClass('hidden');
                    passwordAreaHelp.text(helpText);
                    window.ioinstance.indicator.hide();
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.pushNotificationList = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/pushnotificationbackoffice/listMessages', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('pushnotificationmessage', function (layout) {
                var messageHtml = '';
                var messageLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.messages) {
                    var message = response.messages[index];
                    var notificationDate = new Date(message.notificationDate);
                    var completed = (message.isCompleted == 0) ? 'Sending' : 'Completed';
                    var messageLayoutData = {
                        id: message.id,
                        client: message.client.clientDescription,
                        category: message.notificationCategory,
                        data: message.notificationData,
                        date: notificationDate.toLocaleDateString(),
                        message: message.notificationMessage,
                        title: message.notificationTitle,
                        completed: completed,
                        sendedDevice: message.sendedDevices
                    };

                    messageHtml += window.ioinstance.layout.renderLayout(layout, messageLayoutData, messageLayoutProperties);
                }

                window.ioinstance.service.loadLayout('pushnotificationmessagelist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        messages: messageHtml
                    };
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);
                });
            });
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.pushNotificationMessageDelete = function (id) {
    var answer = confirm("Are you sure want to delete this message ?");
    if (answer) {
        var request = window.ioinstance.request.PushNotificationMessageDeleteRequest;
        request.Version = window.ioinstance.version;
        request.ID = id;
        window.ioinstance.indicator.show();
        window.ioinstance.service.post('backoffice/pushnotificationbackoffice/deleteMessage', request, function (status, response, error) {
            var callout = window.ioinstance.callout;
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
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('pushnotificationsend', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#client').click(function (e) {
            e.preventDefault();

            // Client select window
            window.ioinstance.openWindow('clientsSelect');
        }).change(function (e) {
            if ($(this).val().length == 0) {
                $(this).attr('data-clientId', '0');
            }
        });

        $('#sendNotificationForm').submit(function (e) {
            e.preventDefault();

            var request = window.ioinstance.request.SendNotificationRequest;
            request.Version = window.ioinstance.version;
            request.ClientId = parseInt($('#client').attr('data-clientId'));
            request.DeviceType = parseInt($('#deviceType').val());
            request.NotificationCategory = $('#notificationCategory').val();
            request.NotificationData = $('#notificationData').val();
            request.NotificationMessage = $('#notificationMessage').val();
            request.NotificationTitle = $('#notificationTitle').val();

            window.ioinstance.service.post('backoffice/pushnotificationbackoffice/sendnotification', request, function (status, response, error) {
                var callout = window.ioinstance.callout;
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Push notification has been send successfully.', '');
                    window.ioinstance.app.sendNotification(null, 'sendNotification');
                } else {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.restartApp = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    window.ioinstance.service.get("backoffice/configurations/restartApp", function (status, response, error) {
        var callout = window.ioinstance.callout;
        if (status && response.status.success) {
            callout.show(callout.types.success, 'Application restarted has been send successfully.', '');
        } else {
            callout.show(callout.types.danger, error.message, error.detailedMessage);
        }

        window.location.hash = "";
        window.location.reload();
    });
};