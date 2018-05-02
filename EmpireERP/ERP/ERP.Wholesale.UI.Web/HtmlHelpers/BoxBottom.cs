using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;


namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    /// <summary>
    /// Нижний элемент бокса
    /// </summary>
    public static class BoxBottomHelper
    {
        public static string BoxBottom(this HtmlHelper helper, string roundCssName)
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("</div><b class=\"rbottom\"><b class=\"r{0}4\"></b><b class=\"r{0}3\"></b><b class=\"r{0}2\"></b><b class=\"r{0}1\"></b></b></div>", roundCssName);

            return b.ToString();
        }
    }
}