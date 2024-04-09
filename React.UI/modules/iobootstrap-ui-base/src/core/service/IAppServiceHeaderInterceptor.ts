export interface IAppServiceHeaderInterceptor {

    interceptHeaders(): Record<string, string>
}