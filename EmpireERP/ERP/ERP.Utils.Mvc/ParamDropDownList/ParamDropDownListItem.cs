using System.Web.Mvc;

namespace ERP.Utils.Mvc
{
    public class ParamDropDownListItem : SelectListItem
    {
        public string Param { get; set; }

        public ParamDropDownListItem() : base()
        {
        }
    }
}
