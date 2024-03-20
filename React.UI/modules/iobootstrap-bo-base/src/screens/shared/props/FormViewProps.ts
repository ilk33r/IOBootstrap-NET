import FormType from "../interfaces/FormType";
import BreadcrumbNavigationModel from "../models/BreadcrumbNavigationModel";

type FormViewErrorHandler = (errorTitle: string, errorMessage: string) => void;
type FormViewSuccessHandler = (values: string[], blobs: Blob[]) => void;

interface FormViewProps {

    navigation: BreadcrumbNavigationModel[];
    resourceHome: string;
    title: string;
    submitButtonName: string;
    errorHandler: FormViewErrorHandler;
    successHandler: FormViewSuccessHandler;
    formElements: FormType[];
}

export default FormViewProps;
