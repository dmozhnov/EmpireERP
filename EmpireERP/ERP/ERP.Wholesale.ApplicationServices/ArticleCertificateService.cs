using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ArticleCertificateService : IArticleCertificateService
    {
        #region Поля

        private readonly IArticleCertificateRepository articleCertificateRepository;

        #endregion

        #region Конструктор
        
        public ArticleCertificateService(IArticleCertificateRepository articleCertificateRepository)
        {
            this.articleCertificateRepository = articleCertificateRepository;
        } 

        #endregion

        #region Методы

        /// <summary>
        /// Получение сертификата товара по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArticleCertificate CheckArticleCertificateExistence(int id)
        {
            var articleCertificate = articleCertificateRepository.GetById(id);
            ValidationUtils.NotNull(articleCertificate, "Сертификат товара не найден. Возможно, он был удален.");

            return articleCertificate;
        }

        public IList<ArticleCertificate> GetFilteredList(object state)
        {
            return articleCertificateRepository.GetFilteredList(state);
        }

        public int Save(ArticleCertificate articleCertificate)
        {
            articleCertificateRepository.Save(articleCertificate);

            return articleCertificate.Id;
        }

        public void Delete(ArticleCertificate articleCertificate, User user)
        {
            CheckPossibilityToDelete(articleCertificate, user);

            articleCertificateRepository.Delete(articleCertificate);
        }

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion        

        #region Удаление

        public bool IsPossibilityToDelete(ArticleCertificate articleCertificate, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToDelete(articleCertificate, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(ArticleCertificate articleCertificate, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, Permission.ArticleCertificate_Delete);

            if (checkLogic)
            {
                var article = articleCertificateRepository.Query<Article>()
                    .Where(x => x.Certificate == articleCertificate).FirstOrDefault<Article>();
                if (article != null)
                {
                    throw new Exception(String.Format("Невозможно удалить сертификат товара, т.к. он используется в номенклатуре товара «{0}».", article.FullName));
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
