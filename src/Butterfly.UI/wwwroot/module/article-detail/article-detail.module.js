angular.module('articleDetail', ['hljs']).directive('highlight', [
    function () {
        return {
            replace: false,
            scope: {
                'ngModel': '='
            },
            link: function (scope, element, attributes) {
                scope.$watch('ngModel', function (newVal, oldVal) {
                    if (newVal !== oldVal) {
                        element.html(scope.ngModel);
                        var items = element[0].querySelectorAll('pre');
                        angular.forEach(items, function (item) {
                            hljs.highlightBlock(item);
                        });
                    }
                });    
            }
        };
    }
]);