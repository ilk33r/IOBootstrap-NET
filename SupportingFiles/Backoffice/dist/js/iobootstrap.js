io.prototype.app.dashboard = function(e, hash) {
    window.ioinstance.service.loadLayout('dashboard', false, function () {
        window.ioinstance.layout.render();
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
                    var userLayoutData = {
                        id: user.id,
                        userName: user.userName,
                        userRole: user.userRole,
                        tokenDate: user.tokenDate,
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
        var roleList = '';
        var roleNames = ['Super Admin', 'Admin', 'User'];

        for (var i = 0; i < 3; i++) {
            if (userRole == i) {
                roleList += '<option value="' + i + '" selected="selected">' + roleNames[i] + '</option>';
            } else {
                roleList += '<option value="' + i + '">' + roleNames[i] + '</option>';
            }
        }
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

io.prototype.app.userChangePassword = function (id, userName) {
    // Show indicator
    window.ioinstance.indicator.show();
    window.ioinstance.service.loadLayout('userchangepassword', false, function () {
        window.ioinstance.layout.contentLayoutData = {
            id: id,
            userName: userName
        };

        window.ioinstance.layout.render();
        window.ioinstance.selectMenu('usersUpdate');

        $('#changePasswordForm').submit(function (e) {
            e.preventDefault();
            var callout = window.ioinstance.callout;
            var repeatedpassword = $('#repeatedpassword').val();
            var request = window.ioinstance.request.UserChangePasswordRequest;
            request.Version = window.ioinstance.version;
            request.UserName = $('#changePasswordForm').attr('data-id');
            request.OldPassword = $('#currentPassword').val();
            request.NewPassword = $('#password').val();

            $('.passwordArea').removeClass('has-error');
            $('.passwordAreaHelp').addClass('hidden');

            if (request.OldPassword.length <= 3 || request.NewPassword.length <= 3) {
                callout.show(callout.types.danger, 'Invalid password.', 'Password is too short.');
                $('.passwordArea').addClass('has-error');
                $('.passwordAreaHelp').removeClass('hidden');
                $('.passwordAreaHelp').text('Password is too short.');
                window.ioinstance.indicator.hide();
                return;
            } else if (request.NewPassword != repeatedpassword) {
                callout.show(callout.types.danger, 'Invalid password.', 'Passwords did not match.');
                $('.passwordArea').addClass('has-error');
                $('.passwordAreaHelp').removeClass('hidden');
                $('.passwordAreaHelp').text('Passwords did not match.');
                window.ioinstance.indicator.hide();
                return;
            }

            window.ioinstance.indicator.show();
            window.ioinstance.service.post('backoffice/users/password/change', request, function (status, response, error) {
                if (status && response.status.success) {
                    callout.show(callout.types.success, 'User password has been changed successfully.', '');
                    window.ioinstance.app.usersList(null, 'usersList');
                } else {
                    callout.show(callout.types.danger, 'Invalid password.', 'Current password is incorrect.');
                    $('.passwordArea').addClass('has-error');
                    $('.passwordAreaHelp').removeClass('hidden');
                    $('.passwordAreaHelp').text('Current password is incorrect.');
                    window.ioinstance.indicator.hide();
                }
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