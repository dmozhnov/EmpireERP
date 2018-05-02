using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IArticleGroupService
    {
        IEnumerable<ArticleGroup> GetList();

        ArticleGroup CheckArticleGroupExistence(short id, string message = "");
        void Save(ArticleGroup articleGroup);
        void Delete(ArticleGroup articleGroup, User user);
    }
}
