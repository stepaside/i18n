using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace i18n.Web.Mvc
{
    public class i18nController : Controller
    {
        public string _(string text)
        {
            return HttpContext.GetText(text);
        }
    }
}
