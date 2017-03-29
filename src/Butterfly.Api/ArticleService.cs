using Butterfly.ServiceModel;
using Butterfly.ArticleManagement;

using Guru.DependencyInjection;
using Guru.Middleware.RESTfulService;
using Butterfly.Configuration;
using System;
using System.IO;

namespace Butterfly.Api
{
    [Service("Article", Prefix = "Api")]
    public class ArticleService
    {
        private readonly IArticleHandler _Handler;

        private readonly IFileManager _FileManager;

        public ArticleService(IArticleHandler handler, IFileManager fileManager)
        {
            _Handler = handler;
            _FileManager = fileManager;
        }

        [Method(Name = "GetArticlesByPage", HttpVerb = HttpVerb.GET, Response = ContentType.Json)]
        public ApiResponse GetArticlesByPage(int pageNumber)
        {
            return _Handler.GetArticlesByPage(new ApiRequest()
            {
                Paging = new Paging() { PageNumber = pageNumber, PageSize = 10 },
            });
        }

        [Method(Name = "GetArticleById", HttpVerb = HttpVerb.GET, Response = ContentType.Json)]
        public ApiResponse GetArticleById(string articleId)
        {
            return _Handler.GetArticleById(new ApiRequest()
            {
                Items = new KeyValueItem[]
                {
                    new KeyValueItem() { Key = "id", Value = articleId }
                },
            });
        }

        [Method(Name = "GetSearchResult", HttpVerb = HttpVerb.GET, Response = ContentType.Json)]
        public ApiResponse GetSearchResult(string keywords)
        {
            return _Handler.GetSearchResult(new ApiRequest()
            {
                Items = new KeyValueItem[]
                {
                    new KeyValueItem() { Key = "keywords", Value = keywords }
                },
            });
        }

        [Method(Name = "OperateArticle", HttpVerb = HttpVerb.POST, Response = ContentType.Json)]
        public ApiResponse OperateArticle(
            [Parameter(Source = ParameterSource.Body)] Article article,
            [Parameter(Source = ParameterSource.QueryString)] string action,
            [Parameter(Source = ParameterSource.QueryString)] string apiKey)
        {
            return _Handler.OperateArticle(new ApiRequest<Article>()
            {
                Body = article,
                Items = new KeyValueItem[]
                {
                    new KeyValueItem() { Key = "action", Value = action },
                    new KeyValueItem() { Key = "apikey", Value = apiKey },
                },
            });
        }

        [Method(Name = "UploadFile", HttpVerb = HttpVerb.POST, Request = ContentType.Json, Response = ContentType.Json)]
        public ApiResponse UploadFile(
            [Parameter(Source = ParameterSource.Body)] FileEntity file,
            [Parameter(Source = ParameterSource.QueryString)] string apiKey)
        {
            if (_FileManager.Single<IApiConfiguration>().ApiKey == apiKey)
            {
                var data = Convert.FromBase64String(file.Base64);
                using (var outputStream = new FileStream(file.Path, FileMode.Create, FileAccess.Write))
                {
                    outputStream.Write(data, 0, data.Length);
                }

                return new ApiResponse()
                {
                    Succeeded = true,
                };
            }
            else
            {
                return new ApiResponse()
                {
                    Message = "appkey is not valid.",
                };
            }

        }
    }
}