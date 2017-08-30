using Guru.DependencyInjection.Attributes;
using System.Collections.Generic;

namespace Butterfly.ServiceModel
{
    [StaticFile(typeof(ArticleCollection), "./data/*.json", Format = "json", MultiFiles = true)]
    public class ArticleCollection : List<Article>
    {
    }
}
