using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace i18n.Web.UI
{
    public static class ControlExtensions
    {
        public static string _(this Page page, string text)
        {
            return HttpContext.Current.GetText(text);
        }

        public static string _(this MasterPage masterPage, string text)
        {
            return HttpContext.Current.GetText(text);
        }

        public static string _(this UserControl userControl, string text)
        {
            return HttpContext.Current.GetText(text);
        }
    }
}
