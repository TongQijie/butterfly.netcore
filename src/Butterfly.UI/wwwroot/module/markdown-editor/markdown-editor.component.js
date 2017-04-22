angular.module('markdownEditor').component('markdownEditor', {
    // Note: The URL is relative to our `index.html` file
    templateUrl: 'module/markdown-editor/markdown-editor.template.html',
    controller: ['$routeParams', '$http', '$scope',
        function ArticleListController($routeParams, $http, $scope) {
            var self = this;

            self.save = function () {
                $('#saveButton').removeClass('btn-default').addClass('btn-warning').html('saving').prop('disabled', true);

                var action = ($scope.article.id != undefined && $scope.article.id.length > 0) ? 'modify' : 'create';

                $http.post('Api/Article/OperateArticle?action=' + action + '&apikey=' + $scope.apiKey, {
                    'id': $scope.article.id,
                    'title': $scope.article.title,
                    'format': 'markdown',
                    'content': $scope.markdownData
                }, {
                        headers: {
                            'Content-Type': 'application/json',
                            'ACCEPT': 'application/json'
                        }
                    })
                    .then(function (response) {
                        var date = new Date();
                        var timeString = ('0' + date.getHours()).slice(-2) + ':' + ('0' + date.getMinutes()).slice(-2) + ':' + ('0' + date.getSeconds()).slice(-2);
                        if (response.status == 200) {
                            if (response.data.Succeeded) {
                                $scope.article.id = response.data.Data.id;
                                $('#promptText').html('succeeded. ' + timeString);
                            }
                            else {
                                $('#promptText').html('failed. ' + response.data.Message + ' ' + timeString);
                            }
                        }
                        else {
                            $('#promptText').html('failed. ' + timeString);
                        }

                        $('#saveButton').removeClass('btn-warning').addClass('btn-default').html('save').prop('disabled', false);
                    });
            }

            $scope.$on("$locationChangeStart", function (event) {
                if (!confirm('Are you sure to leave now?')) {
                    event.preventDefault();
                }
            });

            var articleId = $routeParams.articleId;
            if (articleId == undefined) {
                $scope.article = {
                    'title': '',
                };

                $scope.apiKey = '';

                $scope.markdownData = '';
                return;
            }

            $scope.editorOptions = {

            };

            $http.get('Api/Article/GetArticleById?articleId=' + articleId)
                .then(function (response) {
                    if (response.status == 200) {
                        if (response.data.Succeeded) {
                            $scope.article = response.data.Data;
                            $scope.markdownData = response.data.Data.content;
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