using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;
using Guru.DependencyInjection.Abstractions;

using Butterfly.ServiceModel;
using Butterfly.Configuration;

namespace Butterfly.ArticleManagement
{
    [DI(typeof(IArticleHandler), Lifetime = Lifetime.Singleton)]
    public class ArticleHandler : IArticleHandler
    {
        private readonly IFileManager _FileManager;

        private readonly IJsonFormatter _JsonFormatter;

        public ArticleHandler(IFileManager fileManager, IJsonFormatter jsonFormatter)
        {
            _FileManager = fileManager;
            _JsonFormatter = jsonFormatter;
        }

        public ApiResponse GetArticleById(ApiRequest request)
        {
            var item = request.Items.FirstOrDefault(x => x.Key == "id");
            if (item == null || !item.Value.HasValue())
            {
                return new ApiResponse()
                {
                    Message = "article id is not specified.",
                };
            }

            return new ApiResponse()
            {
                Succeeded = true,
                Data = _FileManager.Many<Article>().FirstOrDefault(x => x.Id == item.Value)
            };
        }

        public ApiResponse GetArticlesByPage(ApiRequest request)
        {
            if (request.Paging == null || request.Paging.PageNumber <= 0 || request.Paging.PageSize <= 0)
            {
                return new ApiResponse()
                {
                    Message = "paging is not valid in request.",
                };
            }

            var articles = _FileManager.Many<Article>();

            return new ApiResponse()
            {
                Succeeded = true,
                Data = articles.ToList().OrderByDescending(x => x.CreationDate)
                    .Skip(request.Paging.PageSize * (request.Paging.PageNumber - 1))
                    .Take(request.Paging.PageSize).Select(x => new Article()
                    {
                        Id = x.Id,
                        CreationDate = x.CreationDate,
                        Title = x.Title,
                        Abstract = x.Abstract,
                    }).ToArray(),
                Paging = new Paging()
                {
                    PageNumber = request.Paging.PageNumber,
                    PageSize = request.Paging.PageSize,
                    TotalPages = articles.Length / request.Paging.PageSize + ((articles.Length % request.Paging.PageSize) > 0 ? 1 : 0),
                },
            };
        }

        public ApiResponse GetSearchResult(ApiRequest request)
        {
            var item = request.Items.FirstOrDefault(x => x.Key == "keywords");
            if (item == null || !item.Value.HasValue())
            {
                return new ApiResponse()
                {
                    Data = new Article[0],
                    Succeeded = true,
                };
            }

            var fields = item.Value.SplitByChar(' ');
            if (!fields.HasLength())
            {
                return new ApiResponse()
                {
                    Data = new Article[0],
                    Succeeded = true,
                };
            }

            return new ApiResponse()
            {
                Data = _FileManager.Many<Article>().Where(x => fields.Exists(y => x.Title.ContainsIgnoreCase(y) || x.Content.ContainsIgnoreCase(y)))
                    .Select(x => new Article()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Abstract = x.Abstract.Length > 50 ? (x.Abstract.Substring(0, 50) + "...") : x.Abstract,
                    })
                    .Take(10).ToArray(),
                Succeeded = true,
            };
        }

        public ApiResponse OperateArticle(ApiRequest request)
        {
            var apiKey = request.Items.FirstOrDefault(x => x.Key.EqualsIgnoreCase("apikey"))?.Value;
            if (!apiKey.HasValue() || !apiKey.EqualsIgnoreCase(_FileManager.Single<IApiConfiguration>().ApiKey))
            {
                return new ApiResponse()
                {
                    Message = "api key is not valid.",
                };
            }

            var action = request.Items.FirstOrDefault(x => x.Key.EqualsIgnoreCase("action"))?.Value;
            if (action.EqualsIgnoreCase("create"))
            {
                var article = (request as ApiRequest<Article>)?.Body;
                if (article == null)
                {
                    return new ApiResponse()
                    {
                        Message = "request data is empty.",
                    };
                }

                var random = Path.GetRandomFileName();
                article.Id = random.Substring(0, random.IndexOf('.'));
                article.Abstract = GetAbstract(article.Content ?? string.Empty);
                article.CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                article.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                _JsonFormatter.WriteObject(article, $"./data/{article.Id}.json".FullPath());

                return new ApiResponse()
                {
                    Succeeded = true,
                    Data = new Article()
                    {
                        Id = article.Id,
                    },
                };
            }
            else if (action.EqualsIgnoreCase("modify"))
            {
                var article = (request as ApiRequest<Article>)?.Body;
                if (article == null)
                {
                    return new ApiResponse()
                    {
                        Message = "request data is empty.",
                    };
                }

                var exists = _FileManager.Many<Article>().FirstOrDefault(x => x.Id.EqualsIgnoreCase(article.Id));
                if (exists == null)
                {
                    return new ApiResponse()
                    {
                        Message = "article does not exist.",
                    };
                }

                var clone = exists.DeepCopy<Article>();
                clone.Title = article.Title;
                clone.Content = article.Content;
                clone.Abstract = GetAbstract(article.Content ?? string.Empty);
                clone.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                _JsonFormatter.WriteObject(clone, $"./data/{article.Id}.json".FullPath());

                return new ApiResponse()
                {
                    Succeeded = true,
                    Data = new Article()
                    {
                        Id = article.Id,
                    },
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Message = "action is not valid.",
                };
            }
        }

        private string GetAbstract(string content)
        {
            var a = WebUtility.HtmlDecode(Regex.Replace(content, "<(.|\n)*?>", string.Empty));
            if (a.Length > 300)
            {
                a = a.Substring(0, 300) + "...";
            }
            return a;
        }
    }
}