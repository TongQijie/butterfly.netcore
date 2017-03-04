angular.module('articleList').component('articleList', {
    // Note: The URL is relative to our `index.html` file
    templateUrl: 'module/article-list/article-list.template.html',
    controller: ['$routeParams', '$http', '$location', '$anchorScroll', '$scope', '$timeout',
        function ArticleListController($routeParams, $http, $location, $anchorScroll, $scope, $timeout) {
            var self = this;

            $scope.keywords = '';
            var pending = false;
            var timer;

            $scope.keywordChanged = function () {
                if (pending) {
                    $timeout.cancel(timer);
                }

                if ($scope.keywords.length == 0) {
                    self.searchResult = [];
                    return;
                }

                timer = $timeout(function () {
                    console.log("search: " + $scope.keywords);

                    $http.get('Api/Article/GetSearchResult?keywords=' + $scope.keywords)
                        .then(function (response) {
                            if (response.status == 200) {
                                if (response.data.Succeeded) {
                                    self.searchResult = response.data.Data;
                                }
                                else {
                                    // business error
                                }
                            }
                            else {
                                // network error
                            }
                        });
                }, 1500);

                pending = true;
            }

            $scope.resultItemClicked = function (a) {
                $location.path("/article/" + a.id);
            }

            $scope.$on(
                "$destroy",
                function (event) {
                    $timeout.cancel(timer);
                }
            );

            var pageNumber = Number($routeParams.pageNumber);
            if (pageNumber == NaN) {
                return;
            }

            $http.get('Api/Article/GetArticlesByPage?PageNumber=' + pageNumber, {
                headers: {
                    'Content-Type': 'application/json',
                    'ACCEPT': 'application/json'
                }
            }).then(function (response) {
                if (response.status == 200) {
                    if (response.data.Succeeded == true) {
                        self.articles = response.data.Data;
                        self.paging = response.data.Paging;
                    }
                    else {
                        // business error
                    }
                }
                else {
                    // network error
                }
            });
        }]
});