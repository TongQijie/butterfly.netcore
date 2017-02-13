angular.module('articleEditor').component('articleEditor', {
    // Note: The URL is relative to our `index.html` file
    templateUrl: 'module/article-editor/article-editor.template.html',
    controller: ['$routeParams', '$http', '$scope',
        function ArticleListController($routeParams, $http, $scope) {
            var self = this;

            self.save = function () {
                $('#saveButton').removeClass('btn-default').addClass('btn-warning').html('saving').prop('disabled', true);

                var action = ($scope.article.id != undefined && $scope.article.id.length > 0) ? 'modify' : 'create';

                $http.post('Article/Operate?action=' + action + '&key=' + $scope.apiKey, {
                    'id': $scope.article.id,
                    'title': $scope.article.title,
                    'abstract': $scope.article.abstract,
                    'content': $scope.htmlVariable
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
                        if (response.data.error == undefined || response.data.error == false) {
                            $scope.article.id = response.data.data.id;
                            $('#promptText').html('succeeded. ' + timeString);
                        }
                        else {
                            $('#promptText').html('failed. ' + response.data.msg + ' ' + timeString);
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
                    'abstract': '',
                };

                $scope.apiKey = '';

                $scope.htmlVariable = '';
                return;
            }

            $http.get('as/gabi?id=' + articleId)
              .then(function (response) {
                  if (response.status == 200) {
                      if (response.data.error == undefined || response.data.error == false) {
                          $scope.article = response.data.data;
                          $scope.htmlVariable = response.data.data.content;
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