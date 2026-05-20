window.encryptionInterceptor = async function(encryptor, request) {
    if (request.url.includes("/BOAuthentication/Authenticate")) {
        const body = JSON.parse(request.body);
        body.password = await encryptor.encrypt(body.password);

        if (body.encryptedCaptcha != null) {
            body.encryptedCaptcha = await encryptor.encrypt(body.encryptedCaptcha);
        }

        request.body = JSON.stringify(body);
        return request;
    }

    if (request.url.includes("/BOAuthentication/AuthenticateLDAP")) {
        const body = JSON.parse(request.body);
        body.password = await encryptor.encrypt(body.password);

        if (body.encryptedCaptcha != null) {
            body.encryptedCaptcha = await encryptor.encrypt(body.encryptedCaptcha);
        }

        request.body = JSON.stringify(body);
        return request;
    }

    if (request.url.includes("/BOUser/ChangePassword")) {
        const body = JSON.parse(request.body);
        body.oldPassword = await encryptor.encrypt(body.oldPassword);
        body.newPassword = await encryptor.encrypt(body.newPassword);

        request.body = JSON.stringify(body);
        return request;
    }

    return request;
};

window.decryptionInterceptor = async function name(encryptor, response) {
    if (response.url.includes("/BOAuthentication/Authenticate")) {
        encryptor.updateDefaultHeaders("X-IO-AUTHORIZATION-TOKEN", response.body.token);
        encryptor.updateDefaultHeaders("X-IO-AUTHORIZATION-TOKEN-EXTRAS", response.body.extras);
        return response;
    }

    if (response.url.includes("/BOAuthentication/AuthenticateLDAP")) {
        encryptor.updateDefaultHeaders("X-IO-AUTHORIZATION-TOKEN", response.body.token);
        encryptor.updateDefaultHeaders("X-IO-AUTHORIZATION-TOKEN-EXTRAS", response.body.extras);
        return response;
    }

    if (response.url.includes("/BOAuthentication/CheckToken")) {
        const tokenBody = response.body;
        for (let index = 0; index < tokenBody.extras.length; index++) {
            const element = tokenBody.extras[index];
            const decrypted = await encryptor.decrypt(element);

            tokenBody.extras[index] = decrypted;
        }

        console.log("CheckToken response", tokenBody);
        return response;
    }

    return response;
};