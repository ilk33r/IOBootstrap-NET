import { BaseView, View } from 'iobootstrap-ui-base';

class FooterView extends View<{}, {}> {

    render() {        
        return (
            <BaseView>
                <footer className="container-fluid bg-white border-top p-3">
                    <div className="row">
                        <div className="col-md-8 col-sm-12">
                            <strong>&copy; 2026</strong>
                        </div>
                        <div className="col-md-4 col-sm-12 text-md-end text-sm-start">
                            <strong>Version</strong> {import.meta.env.VITE_VERSION}
                        </div>
                    </div>
                </footer>
            </BaseView>
        );
    }
}

export default FooterView;
