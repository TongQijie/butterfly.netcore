angular.module('articleDetail').component('articleDetail', {
    // Note: The URL is relative to our `index.html` file
    templateUrl: 'module/article-detail/article-detail.template.html',
    controller: ['$routeParams', '$http', '$sce', '$scope',
        function ArticleDetailController($routeParams, $http, $sce, $scope) {
            var self = this;

            $http.get('Api/Article/GetArticleById?articleId=' + $routeParams.articleId)
                .then(function (response) {
                    if (response.status == 200) {
                        if (response.data.Succeeded) {
                            self.article = response.data.Data;
                            if (response.data.Data.format == "markdown") {
                                self.editorUrl = './markdown/' + response.data.Data.id;
                                var converter = new showdown.Converter({ tables: true });
                                $scope.html = converter.makeHtml(response.data.Data.content).replace('<table>', '<table class="table table-striped">');
                            }
                            else {
                                self.editorUrl = './editor/' + response.data.Data.id;
                                $scope.html = response.data.Data.content;
                            }
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