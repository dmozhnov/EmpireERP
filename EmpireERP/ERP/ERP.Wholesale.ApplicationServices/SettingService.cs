using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.AbstractApplicationServices;

namespace ERP.Wholesale.ApplicationServices
{
    public class SettingService : ISettingService
    {
        #region Поля

        private readonly ISettingRepository settingRepository;

        #endregion

        #region Конструктор

        public SettingService(ISettingRepository settingRepository)
        {
            this.settingRepository = settingRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение настроек
        /// </summary>
        /// <returns></returns>
        public Setting Get()
        {
            return settingRepository.Get();
        }

        #endregion
    }
}
