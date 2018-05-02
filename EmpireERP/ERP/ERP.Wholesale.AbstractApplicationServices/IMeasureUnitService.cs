using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IMeasureUnitService
    {
        MeasureUnit CheckMeasureUnitExistence(short id);
        IList<MeasureUnit> GetFilteredList(object state);
        short Save(MeasureUnit measureUnit);
        void Delete(MeasureUnit measureUnit, User user);

        bool IsPossibilityToDelete(MeasureUnit measureUnit, User user, bool checkLogic = true);

        void CheckPossibilityToDelete(MeasureUnit measureUnit, User user, bool checkLogic = true);
    }
}
