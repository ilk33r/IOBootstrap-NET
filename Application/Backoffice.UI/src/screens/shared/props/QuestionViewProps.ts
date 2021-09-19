import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";

type QuestionViewErrorHandler = () => void;
type QuestionViewSuccessHandler = () => void;

interface QuestionViewProps {

    navigation: BreadcrumbNavigationModel[];
    resourceHome: string;
    title: string;
    questionMessage: string;
    errorHandler: QuestionViewErrorHandler;
    successHandler: QuestionViewSuccessHandler;
}

export default QuestionViewProps;
