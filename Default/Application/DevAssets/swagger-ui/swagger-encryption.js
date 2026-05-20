(function () {
    const HANDSHAKE_PATH = "/Handshake";

    class Encryptor {
        constructor() {
            this.publicKeyCryptoKey = null;
            this.keyID = null;
            this.aesKey = null;
            this.sessionId = null;
            this.symmetricKey = null;
            this.aesIV = null;
            this.symmetricIV = null;
            this.nonce = null;
            this.defaultHeaders = {};
        }

        normalizeRequest(input, init) {
            const url = typeof input === "string" ? input : input?.url;
            const method = (init?.method || input?.method || "GET").toUpperCase();
            return { url, method };
        }

        isHandshakeRequest(url, method) {
            return Boolean(url && method === "GET" && url.includes(HANDSHAKE_PATH) && url.toLowerCase().includes("index"));
        }

        hexToBytes(hexString) {
            let bytes = [];
            for (let c = 0; c < hexString.length; c += 2) {
                bytes.push(parseInt(hexString.substring(c, c + 2), 16));
            }

            return bytes;
        }

        base64UrlEncode(buffer) {
            return this.base64Encode(buffer)
                .replace(/\+/g, '-')
                .replace(/\//g, '_')
                .replace(/=+$/, '');
        }

        base64Encode(buffer) {
            return btoa(Array.from(buffer, b => String.fromCharCode(b)).join(''));
        }
        
        base64Decode(string) {
            return Uint8Array.from(atob(string), c => c.charCodeAt(0));
        }

        async generateAESKeys() {
            const key = await window.crypto.subtle.generateKey(
                {
                    name: "AES-CBC",
                    length: 256
                },
                true,
                ["encrypt", "decrypt"]
            );

            return key;
        }

        async importKey(headers, body) {
            this.keyID = body.keyID;
            this.sessionId = headers["x-session-id"];
            this.aesKey = await this.generateAESKeys();
            const exponentBytes = new Uint8Array(this.hexToBytes(body.publicKeyExponent));
            const modulusBytes = new Uint8Array(this.hexToBytes(body.publicKeyModulus).slice(1));
            const exponentBase64 = this.base64UrlEncode(exponentBytes);
            const modulusBase64 = this.base64UrlEncode(modulusBytes);

            const jwk = {
                kty: "RSA",
                use: "enc",
                n: modulusBase64,
                e: exponentBase64,
            };
            this.publicKeyCryptoKey = await window.crypto.subtle.importKey(
                "jwk",
                jwk,
                {
                    name: "RSA-OAEP",
                    hash: "SHA-256",
                },
                true,
                ["encrypt"]
            );

            const exported = await window.crypto.subtle.exportKey("raw", this.aesKey);
            const exportedKeyBuffer = new Uint8Array(exported);

            const symmetricKey = await window.crypto.subtle.encrypt(
                "RSA-OAEP",
                this.publicKeyCryptoKey,
                exportedKeyBuffer
            );

            this.symmetricKey = this.base64Encode(new Uint8Array(symmetricKey));
            this.aesIV = window.crypto.getRandomValues(new Uint8Array(16));
            const symmetricIV = await window.crypto.subtle.encrypt("RSA-OAEP", this.publicKeyCryptoKey, this.aesIV);
            this.symmetricIV = this.base64Encode(new Uint8Array(symmetricIV));
        }

        getAESMessageEncoding(message) {
            const enc = new TextEncoder();
            return enc.encode(message);
        }

        updateDefaultHeaders(key, value) {
            this.defaultHeaders[key] = value;
        }

        async encrypt(plainText) {
            if (this.aesIV == null || this.aesKey == null) {
                throw new Error("Cryptography does not initialized.");
            }

            const encodedMessage = this.getAESMessageEncoding(plainText);
            const encryptedData = await window.crypto.subtle.encrypt(
                {
                    name: "AES-CBC",
                    iv: this.aesIV
                },
                this.aesKey,
                encodedMessage
            );

            const encryptedString = this.base64Encode(new Uint8Array(encryptedData));
            return encryptedString;
        }

        async decrypt(encryptedText) {
            if (this.aesIV == null || this.aesKey == null) {
                throw new Error("Cryptography does not initialized.");
            }

            const encodedMessage = this.base64Decode(encryptedText);
            const decryptedData = await window.crypto.subtle.decrypt(
                {
                    name: "AES-CBC",
                    iv: this.aesIV
                },
                this.aesKey,
                encodedMessage
            );

            const decoder = new TextDecoder();
            return decoder.decode(decryptedData);
        }

        async handleRequest(request) {
            if (this.keyID != null && (request.headers["X-KEY-ID"] == undefined || request.headers["X-KEY-ID"] == null)) {
                request.headers["X-KEY-ID"] = this.keyID;
            }

            if (this.sessionId != null && (request.headers["X-SESSION-ID"] == undefined || request.headers["X-SESSION-ID"] == null)) {
                request.headers["X-SESSION-ID"] = this.sessionId;
            }

            if (this.symmetricKey != null && (request.headers["X-SYMMETRIC-KEY"] == undefined || request.headers["X-SYMMETRIC-KEY"] == null)) {
                request.headers["X-SYMMETRIC-KEY"] = this.symmetricKey;
            }

            if (this.symmetricIV != null && (request.headers["X-SYMMETRIC-IV"] == undefined || request.headers["X-SYMMETRIC-IV"] == null)) {
                request.headers["X-SYMMETRIC-IV"] = this.symmetricIV;
            }

            if (this.nonce != null && this.aesIV != null && this.aesKey != null) {
                const encryptedNonce = await this.encrypt(this.nonce);
                request.headers["X-NONCE"] = encryptedNonce;
            }

            Object.entries(this.defaultHeaders).forEach(([key, value]) => {
                request.headers[key] = value;
            });

            return request;
        }

        async handleResponse(response, request) {
            const { url, method } = this.normalizeRequest(response?.url, request);

            if (response.headers["X-NONCE"] != undefined && response.headers["X-NONCE"] != null) {
                this.nonce = response.headers["X-NONCE"];
            }

            if (response.headers["x-nonce"] != undefined && response.headers["x-nonce"] != null) {
                this.nonce = response.headers["x-nonce"];
            }

            if (!this.isHandshakeRequest(url, method) || !response?.ok) {
                return response;
            }

            try {
                await this.importKey(response.headers, response.body);
            } catch (error) {
                console.error("Swagger public key import error:", error);
            }

            return response;
        }
    }

    const encryptor = new Encryptor();

    function installSwaggerRequestInterceptor() {
        if (!window.ui || typeof window.ui.getConfigs !== "function") {
            return false;
        }

        const configs = window.ui.getConfigs();
        if (!configs || configs.__jweInterceptorInstalled) {
            return true;
        }

        const previousInterceptor = configs.requestInterceptor;
        const previousResponseInterceptor = configs.responseInterceptor;

        configs.requestInterceptor = async function (request) {
            const req = previousInterceptor ? await previousInterceptor(request) : request;
            try {
                const updatedRequest = await encryptor.handleRequest(req);
                return await window.encryptionInterceptor(encryptor, updatedRequest);
            } catch (error) {
                console.error("Swagger JWE encryption error:", error);
                return req;
            }
        };

        configs.responseInterceptor = async function (response) {
            const res = previousResponseInterceptor ? await previousResponseInterceptor(response) : response;
            const updatedResponse = await encryptor.handleResponse(res, res?.obj?.req);
            return await window.decryptionInterceptor(encryptor, updatedResponse);
        };

        configs.showMutatedRequest = true;
        configs.__jweInterceptorInstalled = true;
        return true;
    }

    const intervalId = setInterval(() => {
        if (installSwaggerRequestInterceptor()) {
            clearInterval(intervalId);
        }
    }, 100);
})();
