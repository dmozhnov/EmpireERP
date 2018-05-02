using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace ERP.Wholesale.UI.Web.HtmlHelpers
{
    public static class BoxTopHelper
    {
        /// <summary>
        /// Верхний элемент бокса
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>        
        /// <returns></returns>
        public static string BoxTop(this HtmlHelper helper, string id, string roundCssName)
        {
            StringBuilder b = new StringBuilder();

            b.Append("<div id=\"" + id + "\">");
            b.AppendFormat("<b class=\"rtop\"><b class=\"r{0}1\"></b><b class=\"r{0}2\"></b><b class=\"r{0}3\"></b><b class=\"r{0}4\"></b></b><div class=\"r{0}box\">", roundCssName);

            return b.ToString();
        }
    }
}