using System;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using Bizpulse.Admin.Domain.Repositories;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Utils.Mvc;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public class CityController : BaseAdminController
    {
        #region Свойства

        private readonly ICityRepository cityRepository;

        #endregion

        #region Конструкторы

        public CityController(ICityRepository cityRepository)
        {
            this.cityRepository = cityRepository;
        }

        #endregion

        #region Методы

        public ActionResult GetList(string regionId)
        {
            try
            {
                var _regionId = ValidationUtils.TryGetShort(regionId, "Неверное значение кода региона.");
                
                using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
                {
                    var list = new
                    {
                        List = cityRepository.GetByRegionId(_regionId).OrderBy(x => x.SortOrder).ThenBy(x => x.Name) 
                            .GetComboBoxItemList(s => s.Name, s => s.Id.ToString(), false, false)
                    };

                    return Json(list, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
