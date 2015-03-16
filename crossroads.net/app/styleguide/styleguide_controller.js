'use strict()';
(function () {
    module.exports = function ($scope, $location, $anchorScroll, growl) {
        var _this = this;

        _this.scrollTo = function (id) {
            $location.hash(id);
            console.log($location.hash());
            $anchorScroll();
        }

        _this.addWarnMessage = function () {
            growl.addWarnMessage("GROWL");
        }
    };
})();