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

io.prototype.request.MenuAddRequestModel = {
    Culture: 0,
    Version: '',
    Action: '',
    CssClass: '',
    Name: '',
    MenuOrder: 0,
    RequiredRole: 0,
    ParentEntityID: null
};

io.prototype.request.MenuUpdateRequestModel = {
    Culture: 0,
    Version: '',
    ID: 0,
    Action: '',
    CssClass: '',
    Name: '',
    MenuOrder: 0,
    RequiredRole: 0,
    ParentEntityID: null
};

io.prototype.request.MessageAddRequestModel = {
    Culture: 0,
    Version: '',
    Message: '',
    MessageStartDate: '',
    MessageEndDate: ''
};

io.prototype.request.MessageDeleteRequestModel = {
    Culture: 0,
    Version: '',
    MessageId: 0
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

io.prototype.request.SendNotificationRequest = {
    Culture: 0,
    Version: '',
    DeviceType: 0,
    NotificationCategory: '',
    NotificationData: '',
    NotificationMessage: '',
    NotificationTitle: ''
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
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }

                window.ioinstance.app.clientsList(null, 'clientsList');
            });
        });
    });
};

io.prototype.app.menuEditorList = function(e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call menu list
    io.service.get('backoffice/menu/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('menueditormenu', function (layout) {
                var menuEditorHtml = '';
                var menuEditorLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

                for (var index in response.items) {
                    var menu = response.items[index];
                    var roleName = window.ioinstance.userRoles.getRoleName(menu.requiredRole);
                    var menuLayoutData = {
                        rowClass: '',
                        id: menu.id,
                        name: menu.name,
                        action: menu.action,
                        cssClass: menu.cssClass,
                        menuOrder: menu.menuOrder,
                        userRole: roleName,
                        userRoleRaw: menu.requiredRole,
                        parentMenuName: '',
                        parentMenuId: 0
                    };

                    menuEditorHtml += window.ioinstance.layout.renderLayout(layout, menuLayoutData, menuEditorLayoutProperties);

                    var childMenuItems = menu.childItems;
                    if (childMenuItems != null && childMenuItems.length > 0) {
                        for (var childIndex in childMenuItems) {
                            var childMenu = childMenuItems[childIndex];
                            var childMenuRoleName = window.ioinstance.userRoles.getRoleName(childMenu.requiredRole);
                            var childMenuLayoutData = {
                                rowClass: 'childmenu',
                                id: childMenu.id,
                                name: childMenu.name,
                                action: childMenu.action,
                                cssClass: childMenu.cssClass,
                                menuOrder: childMenu.menuOrder,
                                userRole: childMenuRoleName,
                                userRoleRaw: childMenu.requiredRole,
                                parentMenuName: menu.name,
                                parentMenuId: menu.id
                            };

                            menuEditorHtml += window.ioinstance.layout.renderLayout(layout, childMenuLayoutData, menuEditorLayoutProperties);
                        }
                    }
                }

                window.ioinstance.service.loadLayout('menueditorlist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        menu: menuEditorHtml
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

io.prototype.app.menuEditorAdd = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('menueditoradd', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#parentMenu').click(function (e) {
            e.preventDefault();

            // Client select window
            window.ioinstance.openWindow('menuSelect');
        }).change(function (e) {
            if ($(this).val().length == 0) {
                $(this).attr('data-parentMenuId', '0');
            }
        });

        $('#addMenuForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.MenuAddRequestModel;
            request.Version = window.ioinstance.version;
            request.Name = $('#menuName').val();
            request.Action = $('#menuAction').val();
            request.CssClass = $('#menuCss').val();
            request.MenuOrder = parseInt($('#menuOrder').val());
            request.RequiredRole = parseInt($('#role').val());
            request.ParentEntityID = parseInt($('#parentMenu').attr('data-parentMenuId'));

            var menuNameArea = $('.menuNameArea').removeClass('has-error');
            var menuNameAreaHelp = $('.menuNameAreaHelp').addClass('hidden');

            var menuActionArea = $('.menuActionArea').removeClass('has-error');
            var menuActionAreaHelp = $('.menuActionAreaHelp').addClass('hidden');

            if (request.Name.length < 1) {
                callout.show(callout.types.danger, 'Invalid name.', '');
                menuNameArea.addClass('has-error');
                menuNameAreaHelp.removeClass('hidden');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.Action.length < 1) {
                callout.show(callout.types.danger, 'Invalid action.', '');
                menuActionArea.addClass('has-error');
                menuActionAreaHelp.removeClass('hidden');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/menu/add', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Menu has been added successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.menuEditorList(null, 'menuEditorList');
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.menuEditorSelect = function(e, hash) {
    var parentMenu = $('#parentMenu');
    parentMenu.attr('data-parentMenuId', e[0]);
    parentMenu.val(e[1]);
};

io.prototype.app.menuEditorUpdate = function(id, name, action, cssClass, userRoleRaw, menuOrder, parentMenuName, parentMenuId) {
    // Show indicator
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('menueditorupdate', false, function () {
        var roleList = window.ioinstance.userRoles.getRoleSelection(userRoleRaw);
        window.ioinstance.layout.contentLayoutData = {
            id: id,
            name: name,
            action: action,
            cssClass: cssClass,
            userRoleRaw: userRoleRaw,
            menuOrder: menuOrder,
            parentMenuName: parentMenuName,
            parentMenuId: parentMenuId,
            roleList: roleList
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('menuEditorList');

        $('#parentMenu').click(function (e) {
            e.preventDefault();

            // Client select window
            window.ioinstance.openWindow('menuSelect');
        }).change(function (e) {
            if ($(this).val().length == 0) {
                $(this).attr('data-parentMenuId', '0');
            }
        });

        $('#updateMenuForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.MenuUpdateRequestModel;
            request.ID = parseInt($(this).attr('data-menuId'));
            request.Version = window.ioinstance.version;
            request.Name = $('#menuName').val();
            request.Action = $('#menuAction').val();
            request.CssClass = $('#menuCss').val();
            request.MenuOrder = parseInt($('#menuOrder').val());
            request.RequiredRole = parseInt($('#role').val());
            request.ParentEntityID = parseInt($('#parentMenu').attr('data-parentMenuId'));

            var menuNameArea = $('.menuNameArea').removeClass('has-error');
            var menuNameAreaHelp = $('.menuNameAreaHelp').addClass('hidden');

            var menuActionArea = $('.menuActionArea').removeClass('has-error');
            var menuActionAreaHelp = $('.menuActionAreaHelp').addClass('hidden');

            if (request.Name.length < 1) {
                callout.show(callout.types.danger, 'Invalid name.', '');
                menuNameArea.addClass('has-error');
                menuNameAreaHelp.removeClass('hidden');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.Action.length < 1) {
                callout.show(callout.types.danger, 'Invalid action.', '');
                menuActionArea.addClass('has-error');
                menuActionAreaHelp.removeClass('hidden');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/menu/update', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Menu has been updated successfully.', '');
                    window.location.hash = '';
                    window.ioinstance.app.menuEditorList(null, 'menuEditorList');
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.menuSelect = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/menu/list', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('menueditormenuselectitem', function (layout) {
                var menuEditorHtml = '';
                var menuEditorLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

                for (var index in response.items) {
                    var menu = response.items[index];
                    var roleName = window.ioinstance.userRoles.getRoleName(menu.requiredRole);
                    var menuLayoutData = {
                        rowClass: '',
                        id: menu.id,
                        name: menu.name,
                        action: menu.action,
                        cssClass: menu.cssClass,
                        menuOrder: menu.menuOrder,
                        userRole: roleName,
                        userRoleRaw: menu.requiredRole,
                        parentMenuName: '',
                        parentMenuId: 0
                    };

                    menuEditorHtml += window.ioinstance.layout.renderLayout(layout, menuLayoutData, menuEditorLayoutProperties);

                    var childMenuItems = menu.childItems;
                    if (childMenuItems != null && childMenuItems.length > 0) {
                        for (var childIndex in childMenuItems) {
                            var childMenu = childMenuItems[childIndex];
                            var childMenuRoleName = window.ioinstance.userRoles.getRoleName(childMenu.requiredRole);
                            var childMenuLayoutData = {
                                rowClass: 'childmenu',
                                id: childMenu.id,
                                name: childMenu.name,
                                action: childMenu.action,
                                cssClass: childMenu.cssClass,
                                menuOrder: childMenu.menuOrder,
                                userRole: childMenuRoleName,
                                userRoleRaw: childMenu.requiredRole,
                                parentMenuName: menu.name,
                                parentMenuId: menu.id
                            };

                            menuEditorHtml += window.ioinstance.layout.renderLayout(layout, childMenuLayoutData, menuEditorLayoutProperties);
                        }
                    }
                }

                window.ioinstance.service.loadLayout('menueditorlist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        menu: menuEditorHtml
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
    io.selectMenu(hash);

    io.service.loadLayout('messagesadd', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#addMessageForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var request = window.ioinstance.request.MessageAddRequestModel;
            request.Version = window.ioinstance.version;
            request.Message = $('#message').val();
            request.MessageStartDate = $('#startDate').val();
            request.MessageEndDate = $('#endDate').val();

            if (request.Message.length <= 3) {
                callout.show(callout.types.danger, 'Invalid message.', 'Message is too short.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/messages/add', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'Message has been added successfully.', '');
                    window.ioinstance.app.messagesList(null, 'messagesList');
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.messagesList = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    // Call client list
    io.service.get('backoffice/messages/listall', function(status, response, error) {
        if (status && response.status.success) {
            window.ioinstance.service.loadLayoutText('message', function (layout) {
                var messagesHtml = '';
                var messageLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                for (var index in response.messages) {
                    var message = response.messages[index];
                    var createDate = new Date(message.messageCreateDate);
                    var startDate = new Date(message.messageStartDate);
                    var endDate = new Date(message.messageEndDate);
                    var messageLayoutData = {
                        id: message.id,
                        message: message.message.replace(/\\n/g, "<br />"),
                        createDate: createDate.toLocaleDateString(),
                        startDate: startDate.toLocaleDateString(),
                        endDate: endDate.toLocaleDateString()
                    };

                    messagesHtml += window.ioinstance.layout.renderLayout(layout, messageLayoutData, messageLayoutProperties);
                }

                window.ioinstance.service.loadLayout('messageslist', false, function () {
                    window.ioinstance.layout.contentLayoutData = {
                        messages: messagesHtml
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

            $('.passwordArea').removeClass('has-error');
            $('.passwordAreaHelp').addClass('hidden');

            if (request.Password.length <= 3) {
                callout.show(callout.types.danger, 'Invalid password.', 'Password is too short.');
                $('.passwordArea').addClass('has-error');
                $('.passwordAreaHelp').removeClass('hidden');
                $('.passwordAreaHelp').text('Password is too short.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.Password != repeatedpassword) {
                callout.show(callout.types.danger, 'Invalid password.', 'Passwords did not match.');
                $('.passwordArea').addClass('has-error');
                $('.passwordAreaHelp').removeClass('hidden');
                $('.passwordAreaHelp').text('Passwords did not match.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.UserName.length <= 3) {
                callout.show(callout.types.danger, 'Invalid username.', 'User name is too sort.');
                $('.userNameArea').addClass('has-error');
                $('.userNameAreaHelp').removeClass('hidden');
                $('.userNameAreaHelp').text('User name is too sort.');
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
                    $('.userNameAreaHelp').removeClass('hidden');
                    $('.userNameAreaHelp').text(helpText);
                    window.ioinstance.indicator.hide();
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};

io.prototype.app.sendNotification = function (e, hash) {
    var io = window.ioinstance;

    // Show indicator
    io.indicator.show();
    io.selectMenu(hash);

    io.service.loadLayout('sendnotification', false, function () {
        window.ioinstance.layout.render();
        window.ioinstance.selectMenu(hash);

        $('#sendNotificationForm').submit(function (e) {
            e.preventDefault();

            var request = window.ioinstance.request.SendNotificationRequest;
            request.Version = window.ioinstance.version;
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
                } else  {
                    callout.show(callout.types.danger, error.message, error.detailedMessage);
                    window.ioinstance.indicator.hide();
                }
            });
        });
    });
};