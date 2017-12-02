using Butterfly.ServiceModel;
using Butterfly.ArticleManagement;

using Guru.DependencyInjection;
using Butterfly.Configuration;
using System;
using System.IO;
using Guru.AspNetCore.Attributes;

namespace Butterfly.Api
{
    [Api("Article")]
    public class ArticleService
    {
        private readonly IArticleHandler _Handler;

        public ArticleService(IArticleHandler handler)
        {
            _Handler = handler;
        }

        [ApiMethod("GetArticlesByPage")]
        public ApiResponse GetArticlesByPage(int pageNumber)
        {
            return _Handler.GetArticlesByPage(new ApiRequest()
            {
                Paging = new Paging() { PageNumber = pageNumber, PageSize = 10 },
            });
        }

        [ApiMethod( "GetArticleById")]
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

        [ApiMethod("GetSearchResult")]
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

        [ApiMethod("OperateArticle")]
        public ApiResponse OperateArticle(Article article, string action, string apiKey)
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

        [ApiMethod("UploadFile")]
        public ApiResponse UploadFile(FileEntity file, string apiKey)
        {
            if (ContainerManager.Default.Resolve<IApiConfiguration>().ApiKey == apiKey)
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