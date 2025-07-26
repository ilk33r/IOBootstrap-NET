import { View } from "iobootstrap-ui-base";
import UserLoginInformationProps from "../props/UserLoginInformationProps";

class UserLoginInformationView extends View<UserLoginInformationProps, {}> {

    render() {
        return (
            <section className="content">
                <div className="row">
                    <div className="col-md-12">
                        <div id="userInfo" className="editor" contentEditable>
                            <table width="100%" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', color: '#000000', backgroundColor: 'rgb(244, 244, 244)'}}>
                                <tbody>
                                    <tr>
                                        <td align="center">
                                            <table width="600" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', maxWidth: '600px', backgroundColor: '#ffffff', border: '4px solid #000000', borderRadius: '16px', overflow: 'hidden', boxShadow: 'rgba(0, 0, 0, 0.1) 0px 4px 8px'}}>
                                                <tbody>
                                                    <tr>
                                                        <td style={{backgroundColor: '#11BFDE', color: '#ffffff', textAlign: 'center', paddingTop: '15px', paddingBottom: '15px', fontSize: '24px', fontWeight: 'bold', borderTopLeftRadius: '16px', borderTopRightRadius: '16px'}}>
                                                            {process.env.REACT_APP_APP_NAME} LOGIN
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style={{padding: '10px', textAlign: 'center'}}>
                                                            <p style={{textAlign: 'left'}}>Your account has been created.</p>
                                                            <p style={{textAlign: 'left'}}><b>{process.env.REACT_APP_APP_NAME}</b> login information is below.</p>
                                                            <table width="100%" cellPadding={0} cellSpacing={0} border={0} style={{margin: '0px auto', border: '2px solid #000000', padding: '15px', borderRadius: '8px', backgroundColor: 'rgb(249, 249, 249)'}}>
                                                                <tbody>
                                                                    <tr>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b>URL</b>
                                                                        </td>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b><a href={process.env.REACT_APP_BACKOFFICE_PAGE_URL} target="_blank">{process.env.REACT_APP_APP_NAME} Login</a></b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b>User Name</b>
                                                                        </td>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b>{this.props.userName}</b>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b>Password</b>
                                                                        </td>
                                                                        <td width="50%" style={{padding: '10px', border: '1px solid #000000'}}>
                                                                            <b>{this.props.temporaryPassword}</b>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </section>
        );
    }
}

export default UserLoginInformationView;
