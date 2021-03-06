var io = function (authorization, appName, backOfficePath, clientId, clientSecret, debug, layoutApiUrl, layoutApiPath, serviceApiUrl, version) {
    'use strict';
    this.authorization = authorization;
    this.appName = appName;
    this.backOfficePath = backOfficePath;
    this.log.debug = debug;
    this.layoutApiUrl = layoutApiUrl;
    this.layoutApiPath = layoutApiPath;
    this.serviceApiUrl = serviceApiUrl;
    this.token = localStorage.getItem('token');
    this.userRole = -1;
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
    callout: {},
    indicator: {},
    layout: {},
    log: {},
    request: {},
    resources: {},
    response: {},
    service: {},
    ui: {},
    userRoles: {},
    validationRuleTypes: {
        matchRule: 'MatchRule',
        minLength: 'MinLengthRule'
    },
    selectedMenuItem: null,
    openedWindow: null,
    initialize: function () {
        var that = this;
        this.layout.footerLayoutData = {
            version: that.version
        };

        $(window).on('hashchange', function(e) {
            var hash = e.target.location.hash;
            window.ioinstance.setHash(hash, e);
        });

        $(window).on('message', function (e) {
            if (window.ioinstance.openedWindow != null) {
                window.ioinstance.openedWindow.close();
            }
            window.ioinstance.log.call('Message received');
            if (e.originalEvent.data.startsWith('{') || e.originalEvent.data.startsWith('[')) {
                var message = JSON.parse(e.originalEvent.data);
                window.ioinstance.callMessage(message);
            }
        });

        this.inited();
    },
    inited: function() {},
    callMessage: function (message) {
        var methodName = message.actionName;
        var method = this.app[methodName];
        if (typeof method === "function") {
            method(message.data, methodName);
        }
    },
    checkToken: function () {
        var request = this.request.CheckTokenRequest;
        request.Token = this.token;
        let requestURLFormat = '%s/CheckToken';
        let requestURL = requestURLFormat.format(IOGlobal.authenticationControllerName);
        this.service.post(requestURL, request, function(status, response, error) {
            // Check response
            if (status && response.status.success) {
                localStorage.setItem('userName', response.userName);
                window.ioinstance.userRole = response.userRole;
                if (window.ioinstance.checkHash(window.location.hash)) {
                    window.ioinstance.showMasterPage(function () {
                        window.ioinstance.setHash(window.location.hash, null)
                    });
                } else {
                    window.ioinstance.showDashboard();
                }
            } else {
                window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName });
            }
        });
    },
    checkHash: function (hash) {
        if (hash.startsWith('#!')) {
            let methodName = hash.substr(2, hash.length);
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
        let request = this.request.AuthenticationRequest;
        let userName = $('#inputEmail3').val();
        request.UserName = userName;
        request.Password = $('#inputPassword3').val();
        window.location.hash = '';
        let requestURLFormat = '%s/Authenticate';
        let requestURL = requestURLFormat.format(IOGlobal.authenticationControllerName);
        this.service.post(requestURL, request, function (status, response, error) {
            // Check response
            if (status && response.status.success) {
                localStorage.setItem('token', response.token);
                localStorage.setItem('userName', response.userName);
                window.ioinstance.token = response.token;
                window.ioinstance.userRole = response.userRole;
                window.ioinstance.showDashboard();
            } else {
                var layoutData = {
                    appName: window.ioinstance.appName,
                    hasErrorClass: 'has-error',
                    hasMessageClass: '',
                    errorMessage: (response != undefined && response.status != undefined) ? response.status.message : "An unkown error occured."
                };
                window.ioinstance.showLogin(layoutData);
            }
        });
    },
    messagesHtml: function(callback) {
        // Call messages list
        let requestURLFormat = '%s/ListMessages';
        let requestURL = requestURLFormat.format(IOGlobal.messagesControllerName);

        this.service.get(requestURL, function(status, response, error) {
            if (status && response.status.success) {
                window.ioinstance.service.loadLayoutText('dashboardmessage', function (layout) {
                    var readedMessagesJSON = localStorage.getItem('readedMessages');
                    var readedMessages = null;
                    if (readedMessagesJSON != null && readedMessagesJSON.length > 0) {
                        readedMessages = JSON.parse(readedMessagesJSON);
                        var ts = Math.round((new Date()).getTime() / 1000);

                        for (var index in readedMessages) {
                            var message = readedMessages[index];
                            // Check message marked as 30 days
                            if (message.date + 2592000 < ts) {
                                readedMessages.splice(index, 1);
                            }
                        }
                    }

                    var messagesHtml = '';
                    var messagesLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                    for (var index in response.messages) {
                        var message = response.messages[index];
                        var messageId = message.id;
                        var messageText = message.message.replace(/\n/g, "<br />");
                        var startStatus = 'fa-star';
                        if (readedMessages != null) {
                            var readedMessageObject = readedMessages.find(function(element) {
                                return element.id == messageId;
                            });

                            if (readedMessageObject != undefined && readedMessageObject != null) {
                                startStatus = 'fa-star-o';
                            }
                        }

                        var createDate = new Date(message.messageCreateDate);
                        var messagesLayoutData = {
                            id: messageId,
                            message: messageText,
                            date: createDate.toLocaleDateString(),
                            startStatus: startStatus
                        };

                        messagesHtml += window.ioinstance.layout.renderLayout(layout, messagesLayoutData, messagesLayoutProperties);
                    }

                    callback(messagesHtml);
                });
            } else {
                window.ioinstance.indicator.hide();
                window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
            }
        });
    },
    openWindow: function (hash) {
        var url = this.layoutApiUrl;
        if (this.layoutApiPath.length > 0) {
            url += "/" + this.layoutApiPath;
        }
        url += '#!' + hash;
        this.openedWindow = window.open(url, hash, 'width=1224,height=640,top=60,left=60,menubar=0,status=0,titlebar=0');
    },
    showDashboard: function () {
        // Load dashboard
        this.showMasterPage(function () {
            window.ioinstance.app.dashboard(null, null);
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
        this.layout.menuLayoutData.userName = localStorage.getItem('userName');
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
    progressView: $("#progressModal"),
    progressViewProgress: $('#progressModalProgress'),
    progressViewTitle: $('#progressModalTitle'),
    show: function () {
        this.indicator.show();
    },
    hide: function () {
        this.indicator.hide();
    },
    showProgressModal: function (title) {
        this.setProgress(0);
        this.progressViewTitle.html(title);
        this.progressView.modal({backdrop: 'static', keyboard: false});
    },
    setProgress: function (value) {
        this.progressViewProgress.width(value + '%');
    },
    hideProgressModal: function () {
        this.progressView.modal('hide');
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
            var paramsJson = params.b64decode();
            var method = window.ioinstance.app[actionName];

            if (typeof method === "function") {
                var paramsArray = JSON.parse(paramsJson);
                method.apply(null, paramsArray);
            }
        });

        $('a[data-type="selection"]').click(function (e) {
            e.preventDefault();
            var element = $(this);
            var actionName = element.attr('data-method');
            var params = element.attr('data-params');
            var paramsJson = params.b64decode();
            var messageData = {
                'actionName': actionName,
                'data': JSON.parse(paramsJson)
            };

            window.opener.postMessage(JSON.stringify(messageData), '*');
        });

        if (window.opener != null) {
            $('.sidebar-toggle').click();
        }

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

        let requestURLFormat = '%s/ListMenuItems';
        let requestURL = requestURLFormat.format(IOGlobal.menuControllerName);

        window.ioinstance.service.get(requestURL, function (status, response, error) {
            // Check response
            if (status && response.status.success) {
                window.ioinstance.layout.loadMenuChildLayouts(function (parentmenuitemLayout, parentmenuwithchilditemLayout, childmenuitemLayout) {
                    window.ioinstance.layout.prepareMenuLayout(response.items, parentmenuitemLayout, parentmenuwithchilditemLayout, childmenuitemLayout);
                });
            } else {
                window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName});
            }
        });
    },
    loadMenuChildLayouts: function(callback) {
        window.ioinstance.service.loadLayoutText('parentmenuitem', function (parentmenuitemLayout) {
            window.ioinstance.service.loadLayoutText('parentmenuwithchilditem', function (parentmenuwithchilditemLayout) {
                window.ioinstance.service.loadLayoutText('childmenuitem', function (childmenuitemLayout) {
                    callback(parentmenuitemLayout, parentmenuwithchilditemLayout, childmenuitemLayout);
                });
            });
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
    prepareMenuLayout: function(menuItems, parentmenuitemLayout, parentmenuwithchilditemLayout, childmenuitemLayout) {
        window.ioinstance.service.loadLayoutText('menu', function (menuLayout) {
            var menuLayoutContent = '';
            var parentmenuitemLayoutProperties = window.ioinstance.layout.parseLayoutProperties(parentmenuitemLayout);
            var parentmenuwithchilditemLayoutProperties = window.ioinstance.layout.parseLayoutProperties(parentmenuwithchilditemLayout);
            var childmenuitemLayoutProperties = window.ioinstance.layout.parseLayoutProperties(childmenuitemLayout);

            for (var parentMenuItemIndex in menuItems) {
                var menuItem = menuItems[parentMenuItemIndex];

                if (menuItem.childItems.length > 0) {
                    var childMenuItems = '';

                    for (var childMenuItemIndex in menuItem.childItems) {
                        var childMenuItem = menuItem.childItems[childMenuItemIndex];

                        var childmenuitemLayoutData = {
                            action: childMenuItem.action,
                            cssClass: childMenuItem.cssClass,
                            name: childMenuItem.name
                        };
                        childMenuItems += window.ioinstance.layout.renderLayout(childmenuitemLayout, childmenuitemLayoutData, childmenuitemLayoutProperties);
                    }

                    var parentmenuitemLayoutData = {
                        childMenuItems: childMenuItems,
                        cssClass: menuItem.cssClass,
                        name: menuItem.name
                    };
                    menuLayoutContent += window.ioinstance.layout.renderLayout(parentmenuwithchilditemLayout, parentmenuitemLayoutData, parentmenuwithchilditemLayoutProperties);
                } else {
                    var parentmenuitemLayoutData = {
                        action: menuItem.menuItem,
                        cssClass: menuItem.cssClass,
                        name: menuItem.name
                    };
                    menuLayoutContent += window.ioinstance.layout.renderLayout(parentmenuitemLayout, parentmenuitemLayoutData, parentmenuitemLayoutProperties);
                }
            }

            window.ioinstance.layout.menuLayoutData.menuContent = menuLayoutContent;
            window.ioinstance.layout.menuLayoutProperties = window.ioinstance.layout.parseLayoutProperties(menuLayout);
            window.ioinstance.layout.menuLayout = menuLayout;
        });
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

io.prototype.pushNotificationCategories = {
    getCategoryList: function () {
        return [ new window.ioinstance.ui.formDataOptions('-', '') ];
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
    Clone: function (object) {
        return Object.assign({}, object);
    }
};

io.prototype.resources = {
    allResources: [],
    get: function (resourceKey) {
        for (var i = 0; i < this.allResources.length; i++) {
            let resource = this.allResources[i];
            if (resource.resourceKey == resourceKey) {
                return resource.resourceValue;
            }
        }

        return resourceKey;
    },
    getResources: function (resources, callback) {
        var requestResources = [];

        for (var i = 0; i < resources.length; i++) {
            let currentResource = resources[i];
            if (!this.resourceExists(currentResource)) {
                requestResources.push(currentResource);
            }
        }

        if (requestResources.length == 0) {
            callback();
            return;
        }

        let request = {
            ResourceKeys: requestResources
        }

        let requestURLFormat = '%s/GetResources';
        let requestURL = requestURLFormat.format(IOGlobal.resourcesControllerName);
        window.ioinstance.service.post(requestURL, request, function(status, response, error) {
            if (status && response.status.success) {
                for (var j = 0; j < response.resources.length; j++) {
                    let currentResource = response.resources[j];
                    if (!window.ioinstance.resources.resourceExists(currentResource.resourceKey)) {
                        window.ioinstance.resources.allResources.push(currentResource);
                    }
                }
                callback();
                return;
            }

            window.ioinstance.resources.allResources = [];
            callback();
        });
    },
    resourceExists: function(key) {
        for (var i = 0; i < this.allResources.length; i++) {
            let currentResource = this.allResources[i];
            if (currentResource.resourceKey == key) {
                return true;
            }
        }

        return false;
    }
}

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

io.prototype.service = {
    dataTypes: {
        text: 'text',
        json: 'json'
    },
    types: {
        get: 'get',
        post: 'post'
    },
    loadedLayoutData: function (name, value) {
        this.name = name;
        this.value = value;
    },
    loadedLayouts: [],
    call: function (isLayout, path, type, data, dataType, callback) {
        var url = '%s/%s';
        var baseUrl = (isLayout) ? window.ioinstance.layoutApiUrl : window.ioinstance.serviceApiUrl;
        var version = (isLayout) ? '?v=' + window.ioinstance.version : '';
        url = url.format(baseUrl, path) + version;
        var postData = (data != null) ? JSON.stringify(data) : '';
        $.ajax({
            url: url,
            crossDomain: true,
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
                if (typeof data === 'object' && (data.status.code === 401 || data.status.code === 403)) {
                    window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName});
                    return;
                } else if (typeof data === 'object' && data.status.code !== 200) {
                    callback(false, data, { message: data.status.message, detailedMessage: data.status.detailedMessage });
                    return;
                }
                callback(true, data, null);
            },
            error: function (request, status, error) {
                let io = window.ioinstance;
                io.indicator.hide();
                var responeData = (dataType === io.service.dataTypes.json) ? request.responseJSON : request.responseText;
                if (typeof responeData === 'object' && responeData.Status.Code !== 200) {
                    window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName});
                }
                callback(false, responeData, error);
            }
        });
    },
    get: function (path, callback) {
        this.call(false, path, this.types.get, null, this.dataTypes.json, callback);
    },
    layoutFromCache: function(layoutName) {
        for (var i = 0; i < this.loadedLayouts.length; i++) {
            var layoutData = this.loadedLayouts[i];
            if (layoutData.name === layoutName) {
                return layoutData.value;
            }
        }

        return null;
    },
    loadLayout: function (layoutName, isBaseLayout, callback) {
        var cachedLayout = this.layoutFromCache(layoutName);
        if (cachedLayout != null) {
            if (isBaseLayout) {
                window.ioinstance.layout.setBaseLayout(cachedLayout, callback);
            } else {
                window.ioinstance.layout.setContentLayout(cachedLayout, callback);
            }

            return;
        }

        var path = '%s/dist/layouts/%s.html';
        this.call(true, path.format(window.ioinstance.backOfficePath, layoutName), this.types.get, null, this.dataTypes.text, function (status, response, error) {
            if (status) {
                var layoutData = new window.ioinstance.service.loadedLayoutData(layoutName, response);
                window.ioinstance.service.loadedLayouts.push(layoutData);

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
        var cachedLayout = this.layoutFromCache(layoutName);
        if (cachedLayout != null) {
            callback(cachedLayout);
            return;
        }

        var path = '%s/dist/layouts/%s.html';
        this.call(true, path.format(window.ioinstance.backOfficePath, layoutName), this.types.get, null, this.dataTypes.text, function (status, response, error) {
            if (status) {
                var layoutData = new window.ioinstance.service.loadedLayoutData(layoutName, response);
                window.ioinstance.service.loadedLayouts.push(layoutData);

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

io.prototype.service.loadedLayoutData.prototype = {
    name: '',
    value: '',
};

io.prototype.ui = {
    resourcesModel: {
        edit: '',
        delete: '',
        home: '',
        options: '',
        select: ''
    },
    breadcrumb: function (activeNavigationId, activeNavigationName, breadcrumbNavigations) {
        this.activeNavigationId = activeNavigationId;
        this.activeNavigationName = activeNavigationName;
        this.breadcrumbNavigations = breadcrumbNavigations;
    },
    breadcrumbNavigation: function (navigationId, navigationName) {
        this.navigationId = navigationId;
        this.navigationName = navigationName;
    },
    createListParams: function(hash, breadcrumb, listDataHeaders, listData, onRendered) {
        this.hash = hash;
        this.breadcrumb = breadcrumb;
        this.listDataHeaders = listDataHeaders;
        this.listData = listData;
        this.onRendered = onRendered;
    },
    extraParam: function (name, icon, methodName, methodArguments) {
        this.name = name;
        this.icon = icon;
        this.methodName = methodName;
        this.methodArguments = methodArguments;
    },
    formData: function (formDataType, id, name, requestKey) {
        this.formDataType = formDataType;
        this.id = id;
        this.name = name;
        this.requestKey = requestKey;
        this.value = '';
        this.options = [];
        this.validations = [];
        this.params = '';
        this.methodName = '';
        this.enabled = true
    },
    formDataOptions: function (name, value) {
        this.name = name;
        this.value = value;
    },
    formDataTypes: {
        date: 'DateType',
        number: 'NumberType',
        password: 'PasswordType',
        popupSelection: 'PopupSelectionType',
        select: 'SelectType',
        textArea: 'TextAreaType',
        text: 'TextType'
    },
    pagination: function (start, length, count) {
        this.start = start;
        this.length = length;
        this.count = count;
    },
    loadedFormDatas: null,
    checkFormDatasLoaded: function(callback) {
        var layoutHtml = '';
        for (var key in this.loadedFormDatas) {
            var layoutData = this.loadedFormDatas[key];
            if (layoutData == null) {
                setTimeout(function () {
                    window.ioinstance.ui.checkFormDatasLoaded(callback);
                }, 500);

                break
            } else {
                layoutHtml += layoutData;
            }
        }

        callback(layoutHtml);
    },
    createBreadcrumb: function (breadcrumb, callback) {
        window.ioinstance.service.loadLayoutText('breadcrumbLayout', function (layout) {
            var breadcrumbHtml = '';

            for (var i = 0; i < breadcrumb.breadcrumbNavigations.length; i++) {
                var breadcrumbLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
                var breadcrumbLayoutData = breadcrumb.breadcrumbNavigations[i];

                breadcrumbHtml += window.ioinstance.layout.renderLayout(layout, breadcrumbLayoutData, breadcrumbLayoutProperties);
            }

            callback(breadcrumbHtml);
        });
    },
    createForm: function (hash, breadcrumb, formName, formDataArray, submitButtonName, onRendered, callback) {
        let io = window.ioinstance;

        // Show indicator
        io.indicator.show();
        io.selectMenu(hash);

        // Load all form data
        this.loadAllFormData(formDataArray);

        // Create breadcrumb
        this.createBreadcrumb(breadcrumb, function (breadcrumbHtml) {
            // Load all form data
            window.ioinstance.ui.checkFormDatasLoaded(function (formLayoutHtml) {
                // Load form layout
                window.ioinstance.service.loadLayout('formLayout', false, function () {
                    // Prepare form layout
                    window.ioinstance.layout.contentLayoutData = {
                        activeNavigationName: breadcrumb.activeNavigationName,
                        activeNavigationId: breadcrumb.activeNavigationId,
                        breadcrumbLayout: breadcrumbHtml,
                        formName: formName,
                        formData: formLayoutHtml,
                        submitButtonName: submitButtonName
                    };

                    // Render layout
                    window.ioinstance.layout.render();
                    window.ioinstance.selectMenu(hash);

                    onRendered();

                    window.ioinstance.ui.listenPopupSelect(formDataArray);
                    window.ioinstance.ui.listenFormSubmit(formName, formDataArray, callback);
                });
            });
        });
    },
    createFormWithDateType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithDateLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message'
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithNumberType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithNumberLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message'
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithPasswordType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithPasswordLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message'
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithPopupSelectionType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithPopupSelectionLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message',
                formDataParams: formData.params,
                formDataMethodName: formData.methodName
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithSelectType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithSelectLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var optionsHtml = '';

            for (var i = 0; i < formData.options.length; i++) {
                var option = formData.options[i];
                if (option.value === formData.value) {
                    optionsHtml += '<option value="' + option.value + '" selected="selected">' + option.name + '</option>';
                } else {
                    optionsHtml += '<option value="' + option.value + '">' + option.name + '</option>';
                }
            }

            var formLayoutData = {
                formDataId: formData.id,
                formDataName: formData.name,
                options: optionsHtml,
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithTextAreaType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithTextAreaLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message'
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createFormWithTextType: function (formData, callback) {
        window.ioinstance.service.loadLayoutText('formWithTextLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            let enabledHtml = (formData.enabled) ? '' : 'disabled="disabled"';
            var formLayoutData = {
                formDataIdArea: formData.id + 'Area',
                formDataId: formData.id,
                formDataName: formData.name,
                formDataValue: formData.value,
                formDataIdMessage: formData.id + 'Message',
                formDataEnabled: enabledHtml
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
            callback(formHtml);
        });
    },
    createList: function (createListParams) {
        let resources = this.resourcesModel;
        resources.edit = 'BackOffice.Edit';
        resources.delete = 'BackOffice.Delete';
        resources.home = 'BackOffice.Home';
        resources.options = 'BackOffice.Options';
        resources.select = 'BackOffice.Select';
        createListParams.resourcesKeys = resources;

        this.createListWithResourceKeys(createListParams);
    },
    createListWithResourceKeys: function (createListParams) {
        let io = window.ioinstance;

        let requestResourceKeys = [
            createListParams.resourcesKeys.edit,
            createListParams.resourcesKeys.delete,
            createListParams.resourcesKeys.home,
            createListParams.resourcesKeys.options,
            createListParams.resourcesKeys.select
        ];

        io.resources.getResources(requestResourceKeys, function() {
            let resources = Object.assign({}, io.resourcesModel);
            resources.edit = io.resources.get(createListParams.resourcesKeys.edit);
            resources.delete = io.resources.get(createListParams.resourcesKeys.delete);
            resources.home = io.resources.get(createListParams.resourcesKeys.home);
            resources.options = io.resources.get(createListParams.resourcesKeys.options);
            resources.select = io.resources.get(createListParams.resourcesKeys.select);
            createListParams.resources = resources;

            io.ui.createListData(createListParams);
        });
    },
    createListData: function (createListParams) {
        let io = window.ioinstance;

        // Show indicator
        io.indicator.show();
        io.selectMenu(createListParams.hash);

        // Create breadcrumb
        this.createBreadcrumb(createListParams.breadcrumb, function (breadcrumbHtml) {
            // Load item layout
            io.ui.createListItem(createListParams, function (listDataItemsHtml, listDataHeadersHtml) {
                if (createListParams.pagination == null) {
                    io.ui.createListDataHtml(createListParams, breadcrumbHtml, listDataItemsHtml, listDataHeadersHtml, null);
                } else {
                    io.ui.createPagination(createListParams.pagination, function (paginationHtml) {
                        io.ui.createListDataHtml(createListParams, breadcrumbHtml, listDataItemsHtml, listDataHeadersHtml, paginationHtml);
                    });
                }
            });
        });
    },
    createListDataHtml: function (createListParams, breadcrumbHtml, listDataItemsHtml, listDataHeadersHtml, paginationHtml) {
        $('.paginationPage').unbind('click');
        $('#paginationNext').unbind('click');
        $('#paginationPrevious').unbind('click');

        // Load form layout
        window.ioinstance.service.loadLayout('listLayout', false, function () {
            // Prepare form layout
            window.ioinstance.layout.contentLayoutData = {
                activeNavigationName: createListParams.breadcrumb.activeNavigationName,
                activeNavigationId: createListParams.breadcrumb.activeNavigationId,
                breadcrumbLayout: breadcrumbHtml,
                listData: listDataItemsHtml,
                headersHtml: listDataHeadersHtml,
                resourcesOptions: createListParams.resources.options,
                resourcesHome: createListParams.resources.home,
                paginationHtml: (paginationHtml != null) ? paginationHtml : ''
            };

            // Render layout
            window.ioinstance.layout.render();
            window.ioinstance.selectMenu(createListParams.hash);

            if (paginationHtml != null) {
                
                $('.paginationPage').click(function (e) {
                    e.preventDefault();
                    let currentPageNumber = parseInt($(this).attr('data-dt-idx'));
                    createListParams.onPaged(currentPageNumber);
                });

                $('#paginationNext').click(function (e) {
                    e.preventDefault();
                    let currentPageNumber = parseInt($(this).attr('data-dt-idx'));
                    if (!$(this).parent().hasClass('disabled')) {
                        createListParams.onPaged(currentPageNumber);
                    }
                });

                $('#paginationPrevious').click(function (e) {
                    e.preventDefault();
                    let currentPageNumber = parseInt($(this).attr('data-dt-idx'));
                    if (!$(this).parent().hasClass('disabled')) {
                        createListParams.onPaged(currentPageNumber);
                    }
                });
            }

            createListParams.onRendered();
        });
    },
    createListItem: function (createListParams, callback) {
        window.ioinstance.service.loadLayoutText('listItemLayout', function (layout) {
            var itemsHtml = '';
            var itemsLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

            for (var index in createListParams.listData) {
                var listDataItems = createListParams.listData[index];
                var itemUpdateParams = (createListParams.updateParams != null && createListParams.updateParams.length > index) ? createListParams.updateParams[index] : [];
                var itemDeleteParams = (createListParams.deleteParams != null && createListParams.deleteParams.length > index) ? createListParams.deleteParams[index] : [];
                var itemSelectParams = (createListParams.selectParams != null && createListParams.selectParams.length > index) ? createListParams.selectParams[index] : [];
                var itemExtraParams = (createListParams.extraParams != null && createListParams.extraParams.length > index) ? createListParams.extraParams[index] : [];
                var singleItemHtml = '';

                for (var itemIndex in listDataItems) {
                    singleItemHtml += '<td><div class="breakWord">' + listDataItems[itemIndex] + '</div></td>';
                }

                var updateParamsJSONString = '';
                if (itemUpdateParams.length > 0) {
                    updateParamsJSONString = JSON.stringify(itemUpdateParams);
                } else {
                    updateParamsJSONString = '[]';
                }

                var deleteParamsJSONString = '';
                if (itemDeleteParams.length > 0) {
                    deleteParamsJSONString = JSON.stringify(itemDeleteParams);
                } else {
                    deleteParamsJSONString = '[]';
                }

                var selectParamsJSONString = '';
                if (itemSelectParams.length > 0) {
                    selectParamsJSONString = JSON.stringify(itemSelectParams);
                } else {
                    selectParamsJSONString = '[]';
                }

                var updateParamsHtml = updateParamsJSONString.b64encode();
                var deleteParamsHtml = deleteParamsJSONString.b64encode();
                var selectParamsHtml = selectParamsJSONString.b64encode();
                var updateIsHidden = (createListParams.updateMethodName != null && createListParams.updateMethodName !== '') ? '' : 'hidden';
                var deleteIsHidden = (createListParams.deleteMethodName != null && createListParams.deleteMethodName !== '') ? '' : 'hidden';
                var selectIsHidden = (createListParams.selectMethodName != null && createListParams.selectMethodName !== '') ? '' : 'hidden';
                var hasRowClass = (createListParams.hasRowClasses != null && createListParams.hasRowClasses.length > index) ? createListParams.hasRowClasses[index] : false;
                var extraParamsHtml = '';

                if (itemExtraParams != null && itemExtraParams.length > 0) {
                    for (var extraParamIndex in itemExtraParams) {
                        var extraParam = itemExtraParams[extraParamIndex];
                        var extraParamsB64 = JSON.stringify(extraParam.methodArguments).b64encode();
                        extraParamsHtml += '<a class="btn btn-app" href="#!' + extraParam.methodName + '" data-type="action" data-method="' + extraParam.methodName + '" data-params="' + extraParamsB64 + '">';
                        extraParamsHtml += '<i class="fa ' + extraParam.icon + '"></i> ' + extraParam.name + '</a>';
                    }
                }

                var itemsLayoutData = {
                    rowClass: (hasRowClass) ? 'childmenu' : '',
                    singleItemHtml: singleItemHtml,
                    updateIsHidden: updateIsHidden,
                    updateMethodName: (createListParams.updateMethodName != null) ? createListParams.updateMethodName : '',
                    updateParamsHtml: updateParamsHtml,
                    deleteIsHidden: deleteIsHidden,
                    deleteMethodName: (createListParams.deleteMethodName != null) ? createListParams.deleteMethodName : '',
                    deleteParamsHtml: deleteParamsHtml,
                    selectIsHidden:  selectIsHidden,
                    selectMethodName: (createListParams.selectMethodName != null) ? createListParams.selectMethodName : '',
                    selectParamsHtml: selectParamsHtml,
                    extraParams: extraParamsHtml,
                    resourceEdit: createListParams.resources.edit,
                    resourcesDelete: createListParams.resources.delete,
                    resourcesSelect: createListParams.resources.select
                };

                itemsHtml += window.ioinstance.layout.renderLayout(layout, itemsLayoutData, itemsLayoutProperties);
            }

            var headersHtml = '';
            for (let headerItem in createListParams.listDataHeaders) {
                headersHtml += '<th>' + createListParams.listDataHeaders[headerItem] + '</th>';
            }

            callback(itemsHtml, headersHtml);
        });
    },
    createPagination: function (paginationObject, successHandler) {
        let pageCount = Math.ceil(paginationObject.count / paginationObject.length);
        let currentPage = Math.floor(paginationObject.start / paginationObject.length) + 1;

        window.ioinstance.service.loadLayoutText('listPaginationLayout', function (layout) {
            let itemsLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
            var pagesHtml = '';
            for (var i = 1; i <= pageCount; i++) {
                if (i == currentPage) {
                    pagesHtml += '<li class="paginate_button active"><a href="#" data-dt-idx="' + i + '" class="paginationPage" tabindex="0">' + i + '</a></li>';
                } else {
                    pagesHtml += '<li class="paginate_button"><a href="#" data-dt-idx="' + i + '" class="paginationPage" tabindex="0">' + i + '</a></li>';
                }
            }

            let itemsLayoutData = {
                previousPageIsDisabled: (currentPage == 1) ? 'disabled' : '',
                nextPageIsDisabled: (currentPage == pageCount) ? 'disabled' : '',
                previousPage: (currentPage - 1),
                nextPage: (currentPage + 1),
                pages: pagesHtml
            };
            let itemsHtml = window.ioinstance.layout.renderLayout(layout, itemsLayoutData, itemsLayoutProperties);
            successHandler(itemsHtml);
        });
    },
    createPopupSelection: function (hash, breadcrumb, listDataHeaders, listData, selectMethodName, selectionParams, hasRowClasses, onRendered) {
        let resources = this.resourcesModel;
        resources.edit = 'BackOffice.Edit';
        resources.delete = 'BackOffice.Delete';
        resources.home = 'BackOffice.Home';
        resources.options = 'BackOffice.Options';
        resources.select = 'BackOffice.Select';

        this.createPopupSelectionWithResourceKeys(hash, breadcrumb, listDataHeaders, listData, selectMethodName, selectionParams, hasRowClasses, resources, onRendered);
    },
    createPopupSelectionWithResourceKeys: function (hash, breadcrumb, listDataHeaders, listData, selectMethodName, selectionParams, hasRowClasses, resourceKeys, onRendered) {
        let io = window.ioinstance;

        let requestResourceKeys = [
            resourceKeys.edit,
            resourceKeys.delete,
            resourceKeys.home,
            resourceKeys.options,
            resourceKeys.select
        ];

        io.resources.getResources(requestResourceKeys, function() {
            let resources = Object.assign({}, io.resourcesModel);
            resources.edit = io.resources.get(resourceKeys.edit);
            resources.delete = io.resources.get(resourceKeys.delete);
            resources.home = io.resources.get(resourceKeys.home);
            resources.options = io.resources.get(resourceKeys.options);
            resources.select = io.resources.get(resourceKeys.select);

            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, onRendered);
            createListParams.hasRowClasses = hasRowClasses;
            createListParams.resources = resources;
            createListParams.selectMethodName = selectMethodName;
            createListParams.selectParams = selectionParams;

            io.ui.createListData(createListParams);
        });
    },
    getPopupSelectionValue: function(id) {
        return parseInt($('#' + id).attr('data-params'));
    },
    listenFormSubmit: function (formName, formDataArray, callback) {
        $('#' + formName).submit(function (e) {
            e.preventDefault();

            var request = {};
            for (var i = 0; i < formDataArray.length; i++) {
                var formData = formDataArray[i];

                if (!window.ioinstance.ui.validateFormData(formData.validations)) {
                    window.ioinstance.indicator.hide();
                    return;
                }

                if (formData.requestKey != null) {
                    request[formData.requestKey] = $('#' + formData.id).val();
                }
            }

            callback(request)
        });
    },
    listenPopupSelect: function (formDataArray) {
        for (var index in formDataArray) {
            var formData = formDataArray[index];

            if (formData.formDataType === window.ioinstance.ui.formDataTypes.popupSelection) {
                this.listenPopupSelectClick(formData.id, formData.methodName);
            }
        }
    },
    listenPopupSelectClick: function (id, methodName) {
        $('#' + id).click(function (e) {
            e.preventDefault();

            // Client select window
            window.ioinstance.openWindow(methodName);
        }).change(function (e) {
            var thisElm = $(this);
            if (thisElm.val().length === 0) {
                thisElm.attr('data-params', '');
            }
        });
    },
    loadAllFormData: function (formDataArray) {
        this.loadedFormDatas = {};

        for (var i = 0; i < formDataArray.length; i++) {
            var formData = formDataArray[i];
            var methodName = 'createFormWith' + formData.formDataType;
            var method = this[methodName];
            if (typeof method === "function") {
                let formDataName = formData.name;
                this.loadedFormDatas[formDataName] = null;
                method(formData, function (layout) {
                    window.ioinstance.ui.loadedFormDatas[formDataName] = layout;
                });
            } else {
                window.ioinstance.log.call('Form method ' + methodName + ' could not found.');
            }
        }
    },
    validateFormData: function (validations) {
        for (var i = 0; i < validations.length; i++) {
            var validation = validations[i];
            if (!validation.validate()) {
                return false;
            }
        }

        return true;
    }
};

io.prototype.ui.breadcrumb.prototype = {
    activeNavigationName: '',
    activeNavigationId: '',
    breadcrumbNavigations: []
};

io.prototype.ui.breadcrumbNavigation.prototype = {
    navigationId: '',
    navigationName: ''
};

io.prototype.ui.createListParams.prototype = {
    hash: '',
    breadcrumb: null,
    listDataHeaders: [],
    listData: [],
    updateMethodName: null,
    updateParams: null,
    deleteMethodName: null,
    deleteParams: null,
    hasRowClasses: null,
    resources: {},
    resourcesKeys: {},
    pagination: null,
    selectMethodName: null,
    selectParams: {},
    extraParams: {},
    onPaged: null,
    onRendered: null
};

io.prototype.ui.extraParam.prototype = {
    name: '',
    icon: '',
    methodName: '',
    methodArguments: []
};

io.prototype.ui.formData.prototype = {
    formDataType: '',
    id: '',
    methodName: '',
    name: '',
    options: null,
    params: '',
    requestKey: '',
    value: '',
    validations: null
};

io.prototype.ui.formDataOptions.prototype = {
    name: '',
    value: ''
};

io.prototype.ui.pagination.prototype = {
    start: 0,
    length: 0,
    count: 0
};

io.prototype.userRoles = {
    superAdmin: 0,
    admin: 1,
    user: 2,
    getRoleName: function (roleId) {
        switch (roleId) {
            case 0:
                return 'Super Admin';
            case 1:
                return 'Admin';
            case 2:
                return 'User';
            default:
                return 'Undefined';
        }
    },
    getRoleList: function () {
        var roleList = [];

        for (var i = 0; i < 3; i++) {
            var roleName = this.getRoleName(i);
            roleList.push(new window.ioinstance.ui.formDataOptions(roleName, i));
        }

        return roleList;
    }
};

var ioValidation = function(ruleType, errorMessage, validatableId, alternateMessage) {
    this.ruleName = ruleType;
    this.alternateMessage = alternateMessage;
    this.errorMessage = errorMessage;
    this.validatableId = validatableId;
};

ioValidation.prototype = {
    alternateMessage: '',
    errorMessage: '',
    length: 0,
    otherInput: '',
    ruleName: '',
    validatableId: '',
    constructor: ioValidation,
    validate: function () {
        var methodName = 'validate' + this.ruleName;
        var method = this[methodName];
        if (typeof method === "function") {
            return method(this);
        }

        window.ioinstance.log.call('Validation rule ' + this.ruleName + ' could not found.');
        return false;
    },
    validateMatchRule: function (self) {
        var validatableId = self.validatableId;
        var validatable = $('#' + validatableId);
        var value = validatable.val();

        let callout = window.ioinstance.callout;

        var areaElement = $('.' + validatableId + 'Area');
        areaElement.removeClass('has-error');

        var messageElement = $('.' + validatableId + 'Message');
        messageElement.addClass('hidden');

        var otherElementVal = $('#' + self.otherInput).val();

        if (value !== otherElementVal) {
            callout.show(callout.types.danger, self.errorMessage, self.alternateMessage);
            areaElement.addClass('has-error');
            messageElement.removeClass('hidden');
            messageElement.text(self.errorMessage);

            return false;
        }

        return true;
    },
    validateMinLengthRule: function (self) {
        var validatableId = self.validatableId;
        var validatable = $('#' + validatableId);
        var value = validatable.val();

        let callout = window.ioinstance.callout;

        var areaElement = $('.' + validatableId + 'Area');
        areaElement.removeClass('has-error');

        var messageElement = $('.' + validatableId + 'Message');
        messageElement.addClass('hidden');

        if (self.length > value.length) {
            callout.show(callout.types.danger, self.errorMessage, self.alternateMessage);
            areaElement.addClass('has-error');
            messageElement.removeClass('hidden');
            messageElement.text(self.errorMessage);

            return false;
        }

        return true;
    }
};

String.prototype.b64encode = function() {
    return btoa(unescape(encodeURIComponent(this)));
};

String.prototype.b64decode = function() {
    return decodeURIComponent(escape(atob(this)));
};

/**
 * Format strings like prinf
 * "<h1>%s</h1><p>%s</p>".format("Header", "Just a test!");
 */
if (!String.prototype.format) {
    String.prototype.format = function()  {
        var newStr = this, i = 0;
        while (/%s/.test(newStr)) {
            newStr = newStr.replace('%s', arguments[i++])
        }

        return newStr;
    }
}

if (!String.prototype.escapeHtml) {
    String.prototype.escapeHtml = function()  {
        return this
            .replace(/\"/g, "&quot;")
            .replace(/\'/g, "&apos;")
            .replace(/\n/g, "\\n")
            .replace(/\r/g, "\\r");
    }
}

if (!String.prototype.unEscapeHtml) {
    String.prototype.unEscapeHtml = function()  {
        return this
            .replace(/\&quot\;/g, "\"")
            .replace(/\&apos\;/g, "\'")
            .replace(/\\\n/g, "\n")
            .replace(/\\r/g, "\r");
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
       IOGlobal.layoutPath,
       IOGlobal.serviceApiUrl,
       IOGlobal.version);

    // Check token
    if (window.ioinstance.token != null) {
        window.ioinstance.checkToken();
    } else {
        // Get dashboard
        window.ioinstance.showLogin({hasErrorClass: '', hasMessageClass: 'hidden', appName: window.ioinstance.appName});
    }
});