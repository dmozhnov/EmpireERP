using System;
using System.Web.Mvc;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Manufacturer;
using ERP.Wholesale.UI.Web.Infrastructure;
using System.Web.UI;
using ERP.Utils;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ManufacturerController : WholesaleController
    {
        #region Поля

        private readonly IManufacturerPresenter manufacturerPresenter;

        #endregion

        #region Конструктор

        public ManufacturerController(IManufacturerPresenter manufacturerPresenter)
        {
            this.manufacturerPresenter = manufacturerPresenter;
        }

        #endregion

        #region Создание/Редактирование фабрики-изготовителя

        [HttpGet]
        public ActionResult Create(string producerId)
        {
            try
            {
                int? id = !String.IsNullOrEmpty(producerId) ? ValidationUtils.TryGetInt(producerId) : (int?)null;

                return View("Edit", manufacturerPresenter.Create(id, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(ManufacturerEditViewModel model)
        {
            try
            {
                var obj = manufacturerPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(short? id)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = manufacturerPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ManufacturerEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("Edit", model);
                }

                var obj = manufacturerPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(short? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                manufacturerPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = manufacturerPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowManufacturerGrid(GridState state)
        {
            try
            {
                var model = manufacturerPresenter.GetManufacturerGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ManufacturerGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор фабрики-изготовителя

        /// <summary>
        /// Список фабрик-изготовителей, связанных с производителем
        /// </summary>
        /// <param name="producerId"></param>
        /// <remarks>Возвращает грид со списком изготовителей, связанных с производителем</remarks>
        [HttpGet]
        public ActionResult SelectManufacturerOfProducer(int producerId)
        {
            try
            {
                var model = manufacturerPresenter.SelectManufacturer(producerId, UserSession.CurrentUserInfo, "only");

                return PartialView("ManufacturerSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Выбор фабрики-изготовителя для добавления в производителя
        /// </summary>
        /// <param name="producerId"></param>
        /// <remarks>Возвращает грид со списком изготовителей, НЕ СВЯЗАННЫХ с производителем</remarks>
        [HttpGet]
        public ActionResult SelectManufacturerForProducer(int producerId)
        {
            try
            {
                var model = manufacturerPresenter.SelectManufacturer(producerId, UserSession.CurrentUserInfo, "exclude");

                return PartialView("ManufacturerSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Выбор фабрики-изготовителя
        /// </summary>
        [HttpGet]
        public ActionResult SelectManufacturer()
        {
            try
            {
                var model = manufacturerPresenter.SelectManufacturer(UserSession.CurrentUserInfo);

                return PartialView("ManufacturerSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowManufacturerSelectorGrid(GridState state)
        {
            try
            {
                var model = manufacturerPresenter.GetManufacturerGrid(state);

                return PartialView("ManufacturerSelectorGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
