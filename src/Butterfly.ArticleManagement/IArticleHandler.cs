using Butterfly.ServiceModel;

namespace Butterfly.ArticleManagement
{
    public interface IArticleHandler
    {
        ApiResponse GetArticlesByPage(ApiRequest request);

        ApiResponse GetArticleById(ApiRequest request);

        ApiResponse OperateArticle(ApiRequest request);
    }
}