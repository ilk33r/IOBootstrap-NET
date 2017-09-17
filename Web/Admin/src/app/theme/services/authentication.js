/**
 * Created by ilkerozcan on 17/09/2017.
 */
(function () {
    'use strict';

    angular.module('BlurAdmin.theme')
        .service('authentication', authenticationService);

    /** @ngInject */
    function authenticationService($q) {
        return {
            loadImg: function (src) {
                var d = $q.defer();
                var img = new Image();
                img.src = src;
                img.onload = function(){
                    d.resolve();
                };
                return d.promise;
            }
        }
    }

})();