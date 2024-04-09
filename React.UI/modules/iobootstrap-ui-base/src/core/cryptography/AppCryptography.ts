export interface AppEncryptedData {
    symmetricKey: string;
    symmetricIV: string;
    encrypted: string;
}

class AppCryptography {

    private aesKey: CryptoKey | null;
    private aesIV: Uint8Array | null;
    private cryptoKey: CryptoKey | null;
    private symmetricKey: string | null;

    private static _instance: AppCryptography;

    constructor() {
        this.aesKey = null;
        this.aesIV = null;
        this.cryptoKey = null;
        this.symmetricKey = null;
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public get initilized(): boolean {
        return this.symmetricKey != null;
    }

    public async initialize(exponent: string, modulus: string) {
        const exponentBytes = new Uint8Array(this.hexToBytes(exponent));
        const modulusBytes = new Uint8Array(this.hexToBytes(modulus).slice(1));
        const exponentBase64 = this.base64UrlEncode(exponentBytes);
        const modulusBase64 = this.base64UrlEncode(modulusBytes);

        this.aesKey = await this.generateAESKeys();
        
        const jwk: JsonWebKey = {
            "kty": "RSA",
            "use": "enc",
            "n": modulusBase64,
            "e": exponentBase64,
        };
        this.cryptoKey = await window.crypto.subtle.importKey(
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

        const symmetricKey = await window.crypto.subtle.encrypt("RSA-OAEP", this.cryptoKey, exportedKeyBuffer);
        this.symmetricKey = this.base64Encode(new Uint8Array(symmetricKey));
    }

    public async encrypt(plainText: string): Promise<AppEncryptedData> {
        if (this.cryptoKey == null || this.aesKey == null) {
            throw new Error("Cryptography does not initialized.")
        }

        this.aesIV = window.crypto.getRandomValues(new Uint8Array(16));

        const symmetricIV = await window.crypto.subtle.encrypt("RSA-OAEP", this.cryptoKey, this.aesIV);
        const symmetricIVBase64 = this.base64Encode(new Uint8Array(symmetricIV));

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
        
        return {
            symmetricKey: this.symmetricKey ?? "",
            symmetricIV: symmetricIVBase64,
            encrypted: encryptedString
        };
    }

    public async decrypt(encryptedText: string): Promise<string> {
        if (this.aesKey == null || this.aesIV == null) {
            throw new Error("Cryptography does not initialized.")
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

    private async generateAESKeys(): Promise<CryptoKey> {
        const key = await window.crypto.subtle.generateKey(
            {
                name: "AES-CBC",
                length: 256,
            },
            true,
            ["encrypt", "decrypt"],
        );
        
        return key;
    }

    private hexToBytes(hexString: string): any[] {
        let bytes = [];
        for (let c = 0; c < hexString.length; c += 2) {
            bytes.push(parseInt(hexString.substr(c, 2), 16));
        }

        return bytes;
    }

    public base64Encode(buffer: Uint8Array): string {
        return btoa(Array.from(buffer, b => String.fromCharCode(b)).join(''));
    }

    public base64Decode(string: string): Uint8Array {
        return Uint8Array.from(atob(string), c => c.charCodeAt(0));
    }

    private base64UrlEncode(buffer: Uint8Array): string {
        return this.base64Encode(buffer)
            .replace(/\+/g, '-')
            .replace(/\//g, '_')
            .replace(/=+$/, '');
    }

    private getAESMessageEncoding(message: string): Uint8Array {
        const enc = new TextEncoder();
        return enc.encode(message);
    }
}

export default AppCryptography;
