import DOMPurify from "dompurify";

export {};

/* tslint:disable:interface-name */
declare global {
    interface String {
        EscapeHTML(): string
        RemoveHTML(): string
    }
}

enum HTMLEscapeChars {
    '&' = '&amp;',
    '<' = '&lt;',
    '>' = '&gt;',
    '"' = '&quot;',
    "'" = '&#39;',
    "/" = '&#x2F;'
};

String.prototype.EscapeHTML = function (this: string): string {
    const htmlEscapeReg = new RegExp(`[${Object.keys(HTMLEscapeChars)}]`, "g");
    return String(this).replace(htmlEscapeReg, (tag: string) => HTMLEscapeChars[tag as keyof typeof HTMLEscapeChars] || tag);
}

String.prototype.RemoveHTML = function (this: string): string {
    const sanitized = DOMPurify.sanitize(this);
    const htmlEscapeReg = new RegExp(`[${Object.keys(HTMLEscapeChars)}]`, "g");
    return sanitized.replace(htmlEscapeReg, (tag: string) => {
        const htmlChar = HTMLEscapeChars[tag as keyof typeof HTMLEscapeChars];
        if (htmlChar !== undefined && htmlChar !== null) {
            return "";
        } else {
            return tag;
        }
    });
}
