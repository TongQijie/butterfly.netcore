using System;
using System.Linq;

using Butterfly.ServiceModel;

using Guru.ExtensionMethod;
using Guru.DependencyInjection;
using Guru.DependencyInjection.Abstractions;

namespace Butterfly.ArticleManagement
{
    [DI(typeof(IArticleHandler), Lifetime = Lifetime.Singleton)]
    public class ArticleHandler : IArticleHandler
    {
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

            var articles = (ContainerEntry.Resolve(typeof(Article)) as object[]).Select(x => x as Article);

            return new ApiResponse()
            {
                Succeeded = true,
                Data = articles.FirstOrDefault(x => x.Id == item.Value)
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

            var articles = (ContainerEntry.Resolve(typeof(Article)) as object[]).Select(x => x as Article);

            return new ApiResponse()
            {
                Succeeded = true,
                Data = articles.ToList().OrderByDescending(x => x.CreationDate)
                    .Skip(request.Paging.PageSize * (request.Paging.PageNumber - 1))
                    .Take(request.Paging.PageSize).ToArray(),
                Paging = new Paging()
                {
                    PageNumber = request.Paging.PageNumber,
                    PageSize = request.Paging.PageSize,
                    TotalPages = articles.Length / request.Paging.PageNumber + (articles.Length % request.Paging.PageNumber) > 0 ? 1 : 0,
                },
            };
        }

        public ApiResponse OperateArticle(ApiRequest request)
        {
            throw new NotImplementedException();
        }
    }
}