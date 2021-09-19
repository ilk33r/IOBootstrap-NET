import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import FormType from "../interfaces/FormType";

type FormViewErrorHandler = (errorTitle: string, errorMessage: string) => void;
type FormViewSuccessHandler = (values: string[]) => void;

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
