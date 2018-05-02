using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Infrastructure.Repositories.Criteria
{
    public interface IQuery
    {
        Type GetParameterType();
    }
}
