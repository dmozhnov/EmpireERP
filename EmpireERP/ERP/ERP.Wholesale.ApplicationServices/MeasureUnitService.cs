using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class MeasureUnitService : IMeasureUnitService
    {
        #region Поля

        private readonly IMeasureUnitRepository measureUnitRepository;

        #endregion

        #region Конструктор
        
        public MeasureUnitService(IMeasureUnitRepository measureUnitRepository)
        {
            this.measureUnitRepository = measureUnitRepository;                        
        } 
        #endregion

        #region Методы

        /// <summary>
        /// Получение единицы измерения по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MeasureUnit CheckMeasureUnitExistence(short id)
        {
            var measureUnit = measureUnitRepository.GetById(id);
            ValidationUtils.NotNull(measureUnit, "Единица измерения не найдена. Возможно, она была удалена.");

            return measureUnit;
        }

        public IList<MeasureUnit> GetFilteredList(object state)
        {
            return measureUnitRepository.GetFilteredList(state);
        }

        public short Save(MeasureUnit measureUnit)
        {
            CheckMeasureUnitFullNameUniqueness(measureUnit);
            CheckMeasureUnitShortNameUniqueness(measureUnit);
            CheckMeasureUnitNumericCodeUniqueness(measureUnit);
                        
            measureUnitRepository.Save(measureUnit);                        

            return measureUnit.Id;
        }

        public void Delete(MeasureUnit measureUnit, User user)
        {
            CheckPossibilityToDelete(measureUnit, user);

            measureUnitRepository.Delete(measureUnit);
        }

        #region Вспомогательные методы

        /// <summary>
        /// Проверка полного названия единицы измерения на уникальность.
        /// </summary>
        /// <param name="measureUnit">Единица измерения.</param>
        private void CheckMeasureUnitFullNameUniqueness(MeasureUnit measureUnit)
        {
            int count = measureUnitRepository.Query<MeasureUnit>().Where(x => x.FullName == measureUnit.FullName && x.Id != measureUnit.Id).Count();
            if (count > 0)
            {
                throw new Exception("Единица измерения с таким полным наименованием уже существует.");
            }
        }

        /// <summary>
        /// Проверка краткого названия единицы измерения на уникальность.
        /// </summary>
        /// <param name="measureUnit">Единица измерения.</param>
        private void CheckMeasureUnitShortNameUniqueness(MeasureUnit measureUnit)
        {
            int count = measureUnitRepository.Query<MeasureUnit>().Where(x => x.ShortName == measureUnit.ShortName && x.Id != measureUnit.Id).Count();
            if (count > 0)
            {
                throw new Exception("Единица измерения с таким кратким наименованием уже существует.");
            }
        }

        /// <summary>
        /// Проверка цифрового кода единицы измерения на уникальность.
        /// </summary>
        /// <param name="measureUnit">Единица измерения.</param>
        private void CheckMeasureUnitNumericCodeUniqueness(MeasureUnit measureUnit)
        {
            int count = measureUnitRepository.Query<MeasureUnit>().Where(x => x.NumericCode == measureUnit.NumericCode && x.Id != measureUnit.Id).Count();
            if (count > 0)
            {
                throw new Exception("Единица измерения с таким цифровым кодом уже существует.");
            }
        }

        #endregion

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

        public bool IsPossibilityToDelete(MeasureUnit measureUnit, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToDelete(measureUnit, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(MeasureUnit measureUnit, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, Permission.MeasureUnit_Delete);

            if (checkLogic)
            {
                if (measureUnitRepository.Query<Article>().Where(x => x.MeasureUnit == measureUnit).Count() > 0)
                {
                    throw new Exception("Невозможно удалить единицу измерения, т.к. она используется в номенклатурах товаров.");
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
