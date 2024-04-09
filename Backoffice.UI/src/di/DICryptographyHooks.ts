import { AppServiceHeaderAuthenticationInterceptor, DIHooks } from "iobootstrap-ui-base";

class DICryptographyHooks {

    private static appServiceHeaderInterceptor = new AppServiceHeaderAuthenticationInterceptor();

    static setup() {
        DIHooks.Instance.setSingletonForKey("appServiceHeaderInterceptor", DICryptographyHooks.appServiceHeaderInterceptor);
    }
}

export default DICryptographyHooks;