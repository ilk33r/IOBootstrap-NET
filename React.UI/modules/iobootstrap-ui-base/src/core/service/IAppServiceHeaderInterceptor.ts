export interface IAppServiceHeaderInterceptor {

    interceptRequestHeaders(): Promise<Record<string, string>>
    interceptResponseHeaders(headers: Headers): void
}