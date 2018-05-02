using System;
using System.Data;
using System.Web.Mvc;
using Bizpulse.Admin.Domain.Repositories;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils.Mvc;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public class RegionController : BaseAdminController
    {
        #region Свойства

        private readonly IRegionRepository regionRepository;

        #endregion

        #region Конструкторы

        public RegionController(IRegionRepository regionRepository)
        {
            this.regionRepository = regionRepository;
        }

        #endregion

        #region Методы

        #endregion

        public ActionResult GetList()
        {
            try
            {
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    var list = new
                    {
                        List = regionRepository.GetList().GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), false)
                    };

                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

    }
}
