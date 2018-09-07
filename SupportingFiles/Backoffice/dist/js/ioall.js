var io = function (authorization, appName, backOfficePath, clientId, clientSecret, debug, layoutApiUrl, serviceApiUrl, version) {
    'use strict';
    this.authorization = authorization;
    this.appName = appName;
    this.backOfficePath = backOfficePath;
    this.log.debug = debug;
    this.layoutApiUrl = layoutApiUrl;
    this.serviceApiUrl = serviceApiUrl;
    this.token = localStorage.getItem('token');
    this.version = version;

    // Setup client info
    this.clientID = clientId;
    this.clientSecret = clientSecret;

    // Initialize layout
    this.layout.initialize();

    // Dismiss callout
    this.callout.dismiss();

    // Show indicator
    this.indicator.show();

    // Initialize app
    this.initialize();
};

io.prototype = {
    constructor: io,
    app: {},
    indicator: {},
    layout: {},
    log: {},
    request: {},
    service: {},
    selectedMenuItem: null,
    initialize: function () {
        this.layout.footerLayoutData = {
            version: IOGlobal.version
        };

        $(window).on('hashchange', function(e) {
            var hash = e.target.location.hash;
            window.ioinstance.setHash(hash, e);
        });
    },
    checkToken: function () {
        var request = this.request.CheckTokenRequest;
        request.Token = this.token;
        this.service.post('backoffice/users/password/checktoken', request, function(status, response, error) {
            // Check response
            if (status && response.status.success) {
                if (window.ioinstance.checkHash(window.location.hash)) {
                    window.ioinstance.showMasterPage(function () {
                        window.ioinstance.setHash(window.location.hash, null)
                    });
                } else {
                    window.ioinstance.showDashboard();
                }
            } else {
                window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName,});
            }
        });
    },
    checkHash: function (hash) {
        if (hash.startsWith('#!')) {
            var methodName = hash.substr(2, hash.length);
            var method = this.app[methodName];
            if (typeof method === "function") {
                return true;
            }
        }

        return false;
    },
    setHash: function (hash, event) {
        if (hash.startsWith('#!')) {
            var methodName = hash.substr(2, hash.length);
            var method = this.app[methodName];
            if (typeof method === "function") {
                method(event, methodName);
            }
        }
    },
    loginApp: function () {
        var request = this.request.AuthenticationRequest;
        var userName = $('#inputEmail3').val();
        localStorage.setItem("userName", userName);
        request.UserName = userName;
        request.Password = $('#inputPassword3').val();
        this.service.post('backoffice/users/password/authenticate', request, function (status, response, error) {
            // Check response
            if (status && response.status.success) {
                localStorage.setItem('token', response.token);
                window.ioinstance.showDashboard();
            } else {
                var layoutData = {
                    appName: window.ioinstance.appName,
                    hasErrorClass: 'has-error',
                    hasMessageClass: '',
                    errorMessage: response.status.message
                };
                window.ioinstance.showLogin(layoutData);
            }
        });
    },
    showDashboard: function () {
        // Load dashboard
        this.showMasterPage(function () {
            window.ioinstance.service.loadLayout('dashboard', false, function () {
                window.ioinstance.layout.render();
            });
        });
    },
    setHeaderData: function () {
        // Set header data
        this.layout.headerLayoutData = {
            appName: window.ioinstance.appName,
            userName: localStorage.getItem('userName')
        };
    },
    showLogin: function (data) {
        // Load login page
        this.service.loadLayout('login', true, function () {
            window.ioinstance.layout.baseLayoutData = data;
            window.ioinstance.layout.render();
            $('#loginForm').submit(function (e) {
                e.preventDefault();
                window.ioinstance.indicator.show();
                window.ioinstance.loginApp();
            });
        });
    },
    showMasterPage: function (callback) {
        this.service.loadLayout('base', true, function () {
            window.ioinstance.setHeaderData();
            window.ioinstance.setMenuData();
            callback();
        });
    },
    setMenuData: function () {
        // Set header data
        this.layout.menuLayoutData = {
            userName: localStorage.getItem('userName')
        };
    },
    selectMenu: function (id) {
        if (this.selectedMenuItem != null) {
            this.selectedMenuItem.removeClass('active');
        }

        this.selectedMenuItem = $('#menu' + id);
        this.selectedMenuItem.addClass('active');
        this.selectedMenuItem.parent().parent().children('a').click();
    }
};

io.prototype.callout = {
    types: {
        info: 'callout-info',
        danger: 'callout-danger',
        success: 'callout-success',
        warning: 'callout-warning'
    },
    callout: $('#callout'),
    calloutTimeout: null,
    currentType: null,
    dismiss: function () {
        this.callout.removeClass(this.types.info);
        this.callout.removeClass(this.types.danger);
        this.callout.removeClass(this.types.success);
        this.callout.removeClass(this.types.warning);
        this.callout.fadeOut();
    },
    show: function (type, title, message) {
        this.dismiss();
        this.callout.addClass(type);
        $('#calloutTitle').html(title);
        $('#calloutMessage').html(message);
        this.callout.fadeIn();

        this.calloutTimeout = setTimeout(function () {
            window.ioinstance.callout.dismiss();
        }, 5000);
    }
};

io.prototype.indicator = {
    indicator: $('#pageIndicator'),
    show: function () {
        this.indicator.show();
    },
    hide: function () {
        this.indicator.hide();
    }
};

io.prototype.layout = {
    baseLayout: '',
    baseLayoutCallback: null,
    baseLayoutData: {},
    baseLayoutProperties: [],
    headerLayout: '',
    headerLayoutData: {},
    headerLayoutProperties: [],
    menuLayout: '',
    menuLayoutData: {},
    menuLayoutProperties: [],
    contentLayout: '',
    contentLayoutData: {},
    contentLayoutProperties: [],
    footerLayout: '',
    footerLayoutData: {},
    footerLayoutProperties: [],
    pageContent: null,
    initialize: function () {
        this.pageContent = $('#pagecontent');
    },
    checkBaseLayoutLoaded: function () {
        if (this.headerLayout != null && this.menuLayout != null && this.footerLayout != null) {
            this.baseLayoutCallback();
            return;
        }

        setTimeout(function () {
            window.ioinstance.layout.checkBaseLayoutLoaded();
        }, 250);
    },
    listenActions: function () {
        $('a[data-type="action"]').click(function (e) {
            e.preventDefault();
            var element = $(this);
            var actionName = element.attr('data-method');
            var params = element.attr('data-params');
            var method = window.ioinstance.app[actionName];

            if (typeof method === "function") {
                var paramsArray = JSON.parse(params);
                method.apply(null, paramsArray);
            }
        });

        $(document).tree();
    },
    loadHeader: function () {
        this.headerLayout = null;
        window.ioinstance.service.loadLayoutText('header', function (layout) {
            window.ioinstance.layout.headerLayout = layout;
            window.ioinstance.layout.headerLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
        });
    },
    loadMenu: function () {
        this.menuLayout = null;
        window.ioinstance.service.loadLayoutText('menu', function (layout) {
            window.ioinstance.layout.menuLayout = layout;
            window.ioinstance.layout.menuLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
        });
    },
    loadFooter: function () {
        this.footerLayout = null;
        window.ioinstance.service.loadLayoutText('footer', function (layout) {
            window.ioinstance.layout.footerLayout = layout;
            window.ioinstance.layout.footerLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
        });
    },
    setBaseLayout: function(layout, callback) {
        this.baseLayout = layout;
        this.baseLayoutCallback = callback;
        this.baseLayoutProperties = this.parseLayoutProperties(layout);

        // Loop throught layout properties
        for (var index in this.baseLayoutProperties) {
            var property = this.baseLayoutProperties[index];

            if (property == 'header') {
                this.loadHeader();
            } else if (property == 'menu') {
                this.loadMenu();
            } else if (property == 'footer') {
                this.loadFooter();
            }
        }

        this.checkBaseLayoutLoaded();
    },
    setContentLayout: function(layout, callback) {
        this.contentLayout = layout;
        this.contentLayoutProperties = this.parseLayoutProperties(layout);
        callback()
    },
    parseLayoutProperties: function (layout) {
        var layoutRegex = /\$\{([a-zA-Z0-9\.]+)\}/gm;
        var response;
        var layoutProperties = [];

        while ((response = layoutRegex.exec(layout)) !== null) {
            layoutProperties.push(response[1]);
        }

        return layoutProperties;
    },
    render: function () {
        var baseLayout = this.baseLayout;

        // Loop throught layout properties
        for (var index in this.baseLayoutProperties) {
            var renderedLayout = '';
            var property = this.baseLayoutProperties[index];

            if (property == 'header') {
                renderedLayout = window.ioinstance.layout.renderLayout(window.ioinstance.layout.headerLayout,
                    window.ioinstance.layout.headerLayoutData,
                    window.ioinstance.layout.headerLayoutProperties);
            } else if (property == 'menu') {
                renderedLayout = window.ioinstance.layout.renderLayout(window.ioinstance.layout.menuLayout,
                    window.ioinstance.layout.menuLayoutData,
                    window.ioinstance.layout.menuLayoutProperties);
            } else if (property == 'content') {
                renderedLayout = window.ioinstance.layout.renderLayout(window.ioinstance.layout.contentLayout,
                    window.ioinstance.layout.contentLayoutData,
                    window.ioinstance.layout.contentLayoutProperties);
            } else if (property == 'footer') {
                renderedLayout = window.ioinstance.layout.renderLayout(window.ioinstance.layout.footerLayout,
                    window.ioinstance.layout.footerLayoutData,
                    window.ioinstance.layout.footerLayoutProperties);
            } else if (window.ioinstance.layout.baseLayoutData[property] != undefined) {
                renderedLayout = window.ioinstance.layout.baseLayoutData[property];
            }

            baseLayout = baseLayout.replace('${' + property + '}', renderedLayout);
        }

        this.pageContent.html(baseLayout);
        this.listenActions();
        window.ioinstance.indicator.hide();
    },
    renderLayout: function (layout, data, properties) {
        var renderedLayout = layout;

        // Loop throught layout properties
        for (var index in properties) {
            var property = properties[index];

            if (data[property] != undefined) {
                renderedLayout = renderedLayout.replace('${' + property + '}', data[property]);
            }
        }

        return renderedLayout;
    }
};

io.prototype.log = {
    debug: false,
    call: function (message) {
        if (this.debug) {
            console.log(message);
        }
    }
};

io.prototype.request = {
    AuthenticationRequest: {
        Culture: 0,
        Version: '',
        UserName: '',
        Password: ''
    },
    CheckTokenRequest: {
        Culture: 0,
        Version: '',
        Token: ''
    },
    ClientAddRequest: {
        Culture: 0,
        Version: '',
        ClientDescription: '',
        RequestCount: 0
    },
    ClientDeleteRequest: {
        Culture: 0,
        Version: '',
        ClientId: 0
    },
    ClientUpdateRequest: {
        Culture: 0,
        Version: '',
        ClientId: 0,
        ClientDescription: '',
        IsEnabled: 0,
        RequestCount: 0,
        MaxRequestCount: 0
    },
    UserAddRequest: {
        Culture: 0,
        Version: '',
        UserName: '',
        Password: '',
        UserRole: 0
    },
    UserChangePasswordRequest: {
        Culture: 0,
        Version: '',
        UserName: '',
        OldPassword: '',
        NewPassword: ''
    },
    UserDeleteRequest: {
        Culture: 0,
        Version: '',
        UserId: 0
    },
    SendNotificationRequest: {
        Culture: 0,
        Version: '',
        DeviceType: 0,
        NotificationCategory: '',
        NotificationData: '',
        NotificationMessage: '',
        NotificationTitle: ''
    }
};

io.prototype.response = {
    StatusCodes: {
        OK: 0,
        ENDPOINT_FAILURE: 1,
        BAD_REQUEST: 2,
        INVALID_CLIENTS: 3,
        INVALID_CREDIENTALS: 4,
        UNSUPPORTED_VERSION: 5,
        USER_EXISTS: 6
    }
};

io.prototype.deviceTypes = {
    Android: 0,
    iOS: 1,
    Generic: 2,
    Unkown: 999
};

io.prototype.service = {
    dataTypes: {
        text: 'text',
        json: 'json'
    },
    types: {
        get: 'get',
        post: 'post'
    },
    call: function (isLayout, path, type, data, dataType, callback) {
        var url = '%s/%s';
        var baseUrl = (isLayout) ? window.ioinstance.layoutApiUrl : window.ioinstance.serviceApiUrl;
        url = url.format(baseUrl, path);
        var postData = (data != null) ? JSON.stringify(data) : '';
        $.ajax({
            url: url,
            type: type,
            contentType: 'application/json',
            data: postData,
            headers: {
                'X-IO-AUTHORIZATION': window.ioinstance.authorization,
                'X-IO-AUTHORIZATION-TOKEN': (window.ioinstance.token == null) ? '' : window.ioinstance.token,
                'X-IO-CLIENT-ID': window.ioinstance.clientID,
                'X-IO-CLIENT-SECRET': window.ioinstance.clientSecret
            },
            dataType: dataType,
            success: function (data) {
                callback(true, data, null);
            },
            error: function (error) {
                var responeData = (dataType == window.ioinstance.service.dataTypes.json) ? JSON.parse(error.responseText) : error.responseText;
                callback(true, responeData, error);
            }
        });
    },
    get: function (path, callback) {
        this.call(false, path, this.types.get, null, this.dataTypes.json, callback);
    },
    loadLayout: function (layoutName, isBaseLayout, callback) {
        var path = '%s/dist/layouts/%s.html';
        this.call(true, path.format(window.ioinstance.backOfficePath, layoutName), this.types.get, null, this.dataTypes.text, function (status, response, error) {
            if (status) {
                if (isBaseLayout) {
                    window.ioinstance.layout.setBaseLayout(response, callback);
                } else {
                    window.ioinstance.layout.setContentLayout(response, callback);
                }
            } else {
                var logMessage = 'loadLayout %s returns error. Code: %s, message: %s';
                window.ioinstance.log.call(logMessage.format(layoutName, error.status, error.statusText));
                callback();
            }
        });
    },
    loadLayoutText: function (layoutName, callback) {
        var path = '%s/dist/layouts/%s.html';
        this.call(true, path.format(window.ioinstance.backOfficePath, layoutName), this.types.get, null, this.dataTypes.text, function (status, response, error) {
            if (status) {
                callback(response);
            } else {
                var logMessage = 'loadLayout %s returns error. Code: %s, message: %s';
                window.ioinstance.log.call(logMessage.format(layoutName, error.status, error.statusText));
                callback(null);
            }
        });
    },
    post: function (path, request, callback) {
        request.Version = window.ioinstance.version;
        this.call(false, path, this.types.post, request, this.dataTypes.json, callback);
    }
};

/**
 * Format strings like prinf
 * "<h1>%s</h1><p>%s</p>".format("Header", "Just a test!");
 */
if (!String.prototype.format)  {
    String.prototype.format = function()  {
        var newStr = this, i = 0;
        while (/%s/.test(newStr)) {
            newStr = newStr.replace('%s', arguments[i++])
        }

        return newStr;
    }
}

$(document).ready(function () {
   'use strict';
   window.ioinstance = new io(IOGlobal.authorization,
       IOGlobal.appName,
       IOGlobal.backOfficePath,
       IOGlobal.clientId,
       IOGlobal.clientSecret,
       IOGlobal.isDebug,
       IOGlobal.layoutApiUrl,
       IOGlobal.serviceApiUrl,
       IOGlobal.version);

    // Check token
    if (window.ioinstance.token != null) {
        window.ioinstance.checkToken();
    } else {
        // Get dashboard
        window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName,});
    }
});