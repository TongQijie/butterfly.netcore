using Butterfly.ServiceModel;
using Butterfly.ArticleManagement;

using Guru.Middleware.RESTfulService;

namespace Butterfly.Api
{
    [Service("Article", Prefix = "Api")]
    public class ArticleService
    {
        private readonly IArticleHandler _Handler;

        public ArticleService(IArticleHandler handler)
        {
            _Handler = handler;
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
    }
}