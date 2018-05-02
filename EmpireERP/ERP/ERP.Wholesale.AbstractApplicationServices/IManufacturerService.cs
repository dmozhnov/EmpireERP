using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IManufacturerService : IBaseDictionaryService<Manufacturer>
    {        
        /// <summary>
        /// Проверка, является ли указанная фабрика-изготовитель производителем
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <returns></returns>
        bool IsProducer(Manufacturer manufacturer);

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя при удалении производителя, которым она является
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <param name="producer">Производитель, которым является данный изготовитель</param>
        bool IsPossibilityToDeleteManufacturerAndProducer(Manufacturer manufacturer, Producer producer);

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя при удалении производителя, которым она является
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        /// <param name="producer">Производитель, которым является данный изготовитель</param>
        void CheckPossibilityToDeleteManufacturerAndProducer(Manufacturer manufacturer, Producer producer);

        /// <summary>
        /// Проверка на возможность удаления фабрики-изготовителя
        /// </summary>
        /// <param name="manufacturer">Фабрика-изготовитель</param>
        void CheckPossibilityToDeleteManufacturer(Manufacturer manufacturer);
    }
}
