using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IArticleCertificateService
    {
        ArticleCertificate CheckArticleCertificateExistence(int id);
        IList<ArticleCertificate> GetFilteredList(object state);
        int Save(ArticleCertificate articleCertificate);
        void Delete(ArticleCertificate articleCertificate, User user);

        bool IsPossibilityToDelete(ArticleCertificate articleCertificate, User user, bool checkLogic = true);

        void CheckPossibilityToDelete(ArticleCertificate articleCertificate, User user, bool checkLogic = true);
    }
}
