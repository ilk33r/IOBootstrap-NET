import React from 'react';

class View<TProps, TState> extends React.Component<TProps, TState> {

    public navigateToPageWithState(url: string) {
        var pushChangeEvent = new CustomEvent("onpushstate", {
            detail: {
                url
            }
        });
        history.pushState({}, "", url);

        setTimeout(function () {
            window.dispatchEvent(pushChangeEvent);
        }, 350);
    }
}

export default View;