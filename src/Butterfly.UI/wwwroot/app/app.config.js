angular.module('blogApp').config(['$locationProvider', '$routeProvider',
    function config($locationProvider, $routeProvider) {
        $routeProvider.
          when('/page/:pageNumber', {
              template: '<article-list></article-list>'
          }).
          when('/article/:articleId', {
              template: '<article-detail></article-detail>'
          }).
          when('/editor/:articleId?', {
              template: '<article-editor></article-editor>'
          }).
          when('/auth/:id?', {
              template: '<auth></auth>'
          }).
          otherwise('/page/1');

        if (window.history && window.history.pushState) {
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: false
            });
        }
    }
]);