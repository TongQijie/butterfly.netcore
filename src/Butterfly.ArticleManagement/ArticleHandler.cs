using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;

using Guru.Markdown;
using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.Formatter.Abstractions;

using Butterfly.ServiceModel;
using Butterfly.Configuration;
using Guru.DependencyInjection.Attributes;

namespace Butterfly.ArticleManagement
{
    [Injectable(typeof(IArticleHandler), Lifetime.Singleton)]
    public class ArticleHandler : IArticleHandler
    {
        private readonly IJsonFormatter _JsonFormatter;

        public ArticleHandler(IJsonFormatter jsonFormatter)
        {
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
                Data = ContainerManager.Default.Resolve<ArticleCollection>().FirstOrDefault(x => x.Id == item.Value)
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

            var articles = ContainerManager.Default.Resolve<ArticleCollection>();

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
                    TotalPages = articles.Count / request.Paging.PageSize + ((articles.Count % request.Paging.PageSize) > 0 ? 1 : 0),
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
                Data = ContainerManager.Default.Resolve<ArticleCollection>()
                    .Select(x => new
                    {
                        Weight = fields.Count(y => x.Title.ContainsIgnoreCase(y)) * 3 +
                                 fields.Count(y => WebUtility.HtmlDecode(Regex.Replace(x.Content, "<(.|\n)*?>", string.Empty)).ContainsIgnoreCase(y)),
                        Article = new Article()
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Abstract = x.Abstract.Length > 50 ? (x.Abstract.Substring(0, 50) + "...") : x.Abstract,
                        }
                    })
                    .Where(x => x.Weight > 0).OrderByDescending(x => x.Weight)
                    .Select(x => x.Article).Take(10).ToArray(),
                Succeeded = true,
            };
        }

        public ApiResponse OperateArticle(ApiRequest request)
        {
            var apiKey = request.Items.FirstOrDefault(x => x.Key.EqualsIgnoreCase("apikey"))?.Value;
            if (!apiKey.HasValue() || !apiKey.EqualsIgnoreCase(ContainerManager.Default.Resolve<IApiConfiguration>().ApiKey))
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
                
                article.CreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                article.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (article.Format == "markdown")
                {
                    var html = MarkdownToHtml(article.Content);
                    article.Title = GetTitle(html);
                    article.Abstract = GetAbstract(html);
                }
                else
                {
                    article.Abstract = GetAbstract(article.Content ?? string.Empty);
                }

                _JsonFormatter.WriteObject(new ArticleCollection() { article }, $"./data/{article.Id}.json".FullPath());

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

                var exists = ContainerManager.Default.Resolve<ArticleCollection>().FirstOrDefault(x => x.Id.EqualsIgnoreCase(article.Id));
                if (exists == null)
                {
                    return new ApiResponse()
                    {
                        Message = "article does not exist.",
                    };
                }

                var clone = exists.DeepCopy<Article>();
                clone.Content = article.Content;
                clone.ModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (article.Format == "markdown")
                {
                    var html = MarkdownToHtml(article.Content);
                    clone.Title = GetTitle(html);
                    clone.Abstract = GetAbstract(html);
                }
                else
                {
                    clone.Title = article.Title;
                    clone.Abstract = GetAbstract(article.Content ?? string.Empty);
                }

                _JsonFormatter.WriteObject(new ArticleCollection() { clone }, $"./data/{article.Id}.json".FullPath());

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

        private string MarkdownToHtml(string markdown)
        {
            return new MarkdownUtils().Transform(markdown);
        }

        private string GetTitle(string content)
        {
            var match = Regex.Match(content.Trim(), @"^<h1>.+</h1>");
            if (match != null)
            {
                return RemoveHtmlTags(match.Value);
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetAbstract(string content)
        {
            var a = RemoveHtmlTags(content);
            if (a.Length > 300)
            {
                a = a.Substring(0, 300) + "...";
            }
            return a;
        }

        private string RemoveHtmlTags(string html)
        {
            return WebUtility.HtmlDecode(Regex.Replace(html, "<(.|\n)*?>", string.Empty));
        }
    }
}