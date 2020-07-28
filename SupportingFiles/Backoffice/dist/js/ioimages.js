io.prototype.ui.formDataTypes.image = 'ImageType';
io.prototype.ui.formDataTypes.multipleImage = 'MultipleImageType';

io.prototype.request.GetImagesRequestModel = {
    Culture: 0,
    Version: '',
    Start: 0,
    Count: 10
};

io.prototype.request.SaveImageRequestModel = {
    Culture: 0,
    Version: '',
    FileData: null,
    Sizes: null
};

io.prototype.ui.createFormWithImageType = function (formData, callback) {
    window.ioinstance.service.loadLayoutText('formWithImageLayout', function (layout) {
        var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);
        let imagePreview = (formData.imagePreview === undefined) ? '' : formData.imagePreview;
        var formLayoutData = {
            formDataIdArea: formData.id + 'Area',
            formDataId: formData.id,
            formDataName: formData.name,
            formDataImageId: formData.imageId,
            formDataUrl: formData.imageUrl,
            formDataImagePreview: imagePreview,
            formDataHasImage: (formData.hasImage) ? '' : 'hidden',
            formDataIdMessage: formData.id + 'Message',
            formDataDisabled: (formData.hasImage) ? 'disabled="disabled"' : ''
        };

        var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
        callback(formHtml);
    });
};

io.prototype.ui.createFormWithMultipleImageType = function (formData, callback) {
    window.ioinstance.service.loadLayoutText('formWithMultipleImageLayout', function (layout) {
        var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

        var formLayoutData = {
            formDataId: formData.id,
            formDataName: formData.name,
        };

        var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);
        callback(formHtml);
    });
};

io.prototype.app.generateFileVariant = function (fileName, width, height, scale, keepRatio) {
    let variants = {
        FileName: fileName,
        Width: width,
        Height: height,
        Scale: scale,
        KeepRatio: keepRatio
    };

    return Object.assign({}, variants);
};

io.prototype.app.getUploadedImageIds = function(imageId) {
    let imageIdString = $('#' + imageId + 'ImageId').val();
    let imageIdList = JSON.parse(imageIdString.unEscapeHtml());
    return imageIdList;
};

io.prototype.ui.listenMultipleImageSelect = function (id, name, tokens, fileVariants, images) {
    $('#' + id).click(function (e) {
        e.preventDefault();
        var formArea = $('#' + id + 'MultipleImagesArea');
        var index = parseInt(formArea.attr('data-index'));
        formArea.attr('data-index', index + 1);

        window.ioinstance.indicator.show();
        window.ioinstance.service.loadLayoutText('formWithImageLayout', function (layout) {
            var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

            var formLayoutData = {
                formDataId: id + "-" + index,
                formDataName: name + " " + index,
                formDataImageId: '',
                formDataUrl: tokens.authentication.storageBaseUrl,
                formDataImagePreview: '',
                formDataHasImage: 'hidden'
            };

            var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);

            $('#' + id + 'MultipleImageForm').append(formHtml);
            window.ioinstance.app.prepareImageInputForId(fileVariants, formLayoutData.formDataId);

            // Show indicator
            window.ioinstance.indicator.hide();
        });
    });

    if (images != null && images.length > 0) {
        var formArea = $('#' + id + 'MultipleImagesArea');

        for (var index in images) {
            formArea.attr('data-index', index + 1);

            window.ioinstance.indicator.show();
            window.ioinstance.service.loadLayoutText('formWithImageLayout', function (layout) {
                var formLayoutProperties = window.ioinstance.layout.parseLayoutProperties(layout);

                var currentImage = images[index];
                var imageVariantFile = window.ioinstance.app.imageVariantFromJson(currentImage.imageVariations);

                var formLayoutData = {
                    formDataId: id + "-" + index,
                    formDataName: name + " " + index,
                    formDataImageId: currentImage.imageVariations.escapeHtml(),
                    formDataUrl: tokens.authentication.storageBaseUrl,
                    formDataImagePreview: tokens.authentication.storageBaseUrl + imageVariantFile,
                    formDataHasImage: ''
                };

                var formHtml = window.ioinstance.layout.renderLayout(layout, formLayoutData, formLayoutProperties);

                $('#' + id + 'MultipleImageForm').append(formHtml);
                window.ioinstance.app.prepareImageInputForId(fileVariants, formLayoutData.formDataId);

                // Show indicator
                window.ioinstance.indicator.hide();
            });
        }
    }

};

io.prototype.app.prepareImageInputForId = function(fileVariants, inputId) {
    var image = $('#' + inputId);
    var imageId = inputId + 'ImageId';
    var imageUrl = inputId + 'Url';
    var imagePreview = inputId + 'ImagePreview';
    var deleteButton = inputId + 'ImageDelete';

    $(image).on('change', function (e) {
        window.ioinstance.indicator.showProgressModal('Uploading ...');

        var file = e.target.files[0];
        window.ioinstance.app.readImageFileAndUpload(file, fileVariants, inputId, imageId, imageUrl, imagePreview, deleteButton);
    });

    $('#' + deleteButton).click(function (e) {
        e.preventDefault();
        window.ioinstance.app.deleteImages(imageId, imagePreview, deleteButton, inputId);
    });
};

io.prototype.app.readImageFileAndUpload = function(file, fileVariants, inputId, imageIdContext, imageUrlContext, imageViewId, deleteButtonId) {
    var reader = new FileReader();
    var fileType = file.type;
    reader.onload = function() {
        var arrayBuffer = this.result;
        var fileData = new Uint8Array(arrayBuffer);

        var binary = '';
        var len = fileData.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(fileData[i]);
        }

        var b64encoded = btoa(binary);
        var imageData = 'data:' + fileType + ';base64,' + b64encoded;
        window.ioinstance.indicator.setProgress(30);
        window.ioinstance.app.uploadFile(imageData, fileVariants, inputId, imageIdContext, imageUrlContext, imageViewId, deleteButtonId);
    };

    window.ioinstance.indicator.setProgress(10);
    reader.readAsArrayBuffer(file);
};

io.prototype.app.deleteImages = function(imageId, imagePreview, deleteButton, inputId) {
    let imageIdString = $('#' + imageId).val();
    let imageIdList = JSON.parse(imageIdString.unEscapeHtml());

    let request = {
        ImagesIdList: imageIdList
    };

    let requestURLFormat = '%s/DeleteImages';
    let requestURL = requestURLFormat.format(IOGlobal.imagesControllerName);
    window.ioinstance.service.post(requestURL, request, function (status, response, error) {
        if (status && response.status.success) {
            $('#' + imageId).val('');
            $('#' + imagePreview).attr('src', '');
            $('#' + deleteButton).addClass('hidden');
            $('#' + inputId).attr('disabled', false);
        } else {
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, response.status.message, response.status.detailedMessage);
        }

        window.ioinstance.indicator.hideProgressModal();
    });
}

io.prototype.app.uploadFile = function(fileBase64, fileVariants, inputId, imageIdContext, imageUrlContext, imageViewId, deleteButtonId) {
    var request = window.ioinstance.request.SaveImageRequestModel;
    request.Version = window.ioinstance.version;
    request.FileData = fileBase64;
    request.Sizes = fileVariants;

    var currentProgress = 30;
    var progressInterval = setInterval(function () {
        currentProgress += 1;
        if (currentProgress <= 100) {
            window.ioinstance.indicator.setProgress(currentProgress);
        }
    }, 2000);

    let requestURLFormat = '%s/SaveImages';
    let requestURL = requestURLFormat.format(IOGlobal.imagesControllerName);
    window.ioinstance.service.post(requestURL, request, function (status, response, error) {
        if (status && response.status.success) {
            var fileIds = [];
            for (fileIdx in response.files) {
                let file = response.files[fileIdx];
                fileIds.push(file.id);
            }
            var fileDataString = JSON.stringify(fileIds);
            var imageData = response.files[0];
            var imageBaseUrl = $('#' + imageUrlContext).val();
            var imageUrl = imageBaseUrl + imageData.fileName;
            $('#' + imageIdContext).val(fileDataString.escapeHtml());
            $('#' + imageViewId).attr('src', imageUrl);
            $('#' + deleteButtonId).removeClass('hidden');
            $('#' + inputId).attr('disabled', true);
        } else {
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, response.status.message, response.status.detailedMessage);
        }

        clearInterval(progressInterval);
        window.ioinstance.indicator.hideProgressModal();
    });
};

io.prototype.app.imageAdd = function (e, hash) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('imagesEdit', 'Images');
    var formBreadcrumb = new io.ui.breadcrumb('imageAdd', 'Add an image', [ breadcrumbNavigation ]);

    var width = new io.ui.formData(io.ui.formDataTypes.number, 'width', 'Width', 'Width');
    var height = new io.ui.formData(io.ui.formDataTypes.number, 'height', 'Height', 'Height');
    var scale = new io.ui.formData(io.ui.formDataTypes.number, 'scale', 'Scale', 'Scale');
    var keepRatio = new io.ui.formData(io.ui.formDataTypes.select, 'keepRatio', 'KeepRatio', 'KeepRatio');
    keepRatio.options = [
        new io.ui.formDataOptions('No', 0),
        new io.ui.formDataOptions('YES', 1)
    ];

    var formDatas = [
        width,
        height,
        scale,
        keepRatio
    ];

    io.ui.createForm(hash, formBreadcrumb, 'addImageForm', formDatas, 'Next', function () {
        },
        function (request) {
            request.Width = parseInt(request.Width);
            request.Height = parseInt(request.Height);
            request.Scale = parseInt(request.Scale);
            request.KeepRatio = (parseInt(request.KeepRatio) == 1) ? true : false;
            io.app.imageAddUploadFile(request);
        });
};

io.prototype.app.imageAddUploadFile = function (request) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('imagesEdit', 'Images');
    var formBreadcrumb = new io.ui.breadcrumb('imageAdd', 'Add an image', [ breadcrumbNavigation ]);

    var image = new io.ui.formData(io.ui.formDataTypes.image, 'image', 'Image', 'FileData');
    image.imageUrl = IOGlobal.storageBaseUrl;
    image.imageId = '0';
    var formDatas = [
        image
    ];

    io.ui.createForm('imageAdd', formBreadcrumb, 'addImageForm', formDatas, 'Next', function () {
        let imageFileVariant = io.app.generateFileVariant('1x', request.Width, request.Height, request.Scale, request.KeepRatio);
        window.ioinstance.app.prepareImageInputForId([imageFileVariant], 'image');
        },
        function (request) {
            io.app.imagesEdit(null, 'imagesEdit');
        });
};

io.prototype.app.imagesEdit = function (e, hash) {
    window.ioinstance.app.imagesEditPaging(e, hash, 0, 5);
};

io.prototype.app.imagesEditPaging = function (e, hash, start, length) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumb = new io.ui.breadcrumb('imagesEdit', 'Images', []);

    let requestURLFormat = '%s/GetImages';
    let requestURL = requestURLFormat.format(IOGlobal.imagesControllerName);

    var request = window.ioinstance.request.GetImagesRequestModel;
    request.Version = window.ioinstance.version;
    request.Start = start;
    request.Count = length;

    io.service.post(requestURL, request, function(status, response, error) {
        if (status && response.status.success) {
            let listData = [];
            let updateParams = [];

            for (let index in response.images) {
                let image = response.images[index];
                let imageUrl = IOGlobal.storageBaseUrl + image.fileName;
                let imageHtmlFormat = '<img src="%s" width="150" />'
                
                let itemListData = [
                    image.id,
                    image.fileName,
                    image.width,
                    image.height,
                    image.scale,
                    imageHtmlFormat.format(imageUrl)
                ];

                listData.push(itemListData);

                let itemUpdateData = [
                    image.id,
                    image.fileName
                ];

                updateParams.push(itemUpdateData);
            }

            let listDataHeaders = [
                'ID',
                'File Name',
                'Width',
                'Height',
                'Scale',
                'Image'
            ];
            
            let createListParams = new io.ui.createListParams(hash, breadcrumb, listDataHeaders, listData, function () {
            });
            createListParams.updateMethodName = 'imagesUpdate';
            createListParams.updateParams = updateParams;
            createListParams.pagination = new io.ui.pagination(request.Start, request.Count, response.count);
            createListParams.onPaged = function (pageNumber) {
                let newStart = length * (pageNumber - 1);
                io.app.imagesEditPaging(e, hash, newStart, length);
            };

            io.ui.createList(createListParams);
        } else {
            window.ioinstance.indicator.hide();
            window.ioinstance.callout.show(window.ioinstance.callout.types.danger, 'An error occured.', '');
        }
    });
};

io.prototype.app.imagesUpdate = function (id, imageName) {
    let io = window.ioinstance;

    // Show indicator
    io.indicator.show();

    let breadcrumbNavigation = new io.ui.breadcrumbNavigation('imagesEdit', 'Images');
    var formBreadcrumb = new io.ui.breadcrumb('imageAdd', 'Update an image', [ breadcrumbNavigation ]);

    var image = new io.ui.formData(io.ui.formDataTypes.image, 'image', 'Image', 'FileData');
    image.imageUrl = IOGlobal.storageBaseUrl;
    var fileIds = [id];
    var fileDataString = JSON.stringify(fileIds);
    image.imageId = fileDataString.escapeHtml();
    image.imagePreview = IOGlobal.storageBaseUrl + imageName;
    image.hasImage = true;

    var formDatas = [
        image
    ];

    io.ui.createForm('imagesUpdate', formBreadcrumb, 'updateImageForm', formDatas, 'Next', function () {
        let imageFileVariant = io.app.generateFileVariant('1x', 0, 0, 1, true);
        window.ioinstance.app.prepareImageInputForId([imageFileVariant], 'image');
        },
        function (request) {
            io.app.imagesEdit(null, 'imagesEdit');
        });
};