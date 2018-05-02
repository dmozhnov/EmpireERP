using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ISettingService
    {
        /// <summary>
        /// Получение настроек
        /// </summary>
        /// <returns></returns>
        Setting Get();
    }
}
