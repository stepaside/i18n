using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i18n.Web.Mvc
{
    public class Controller : System.Web.Mvc.Controller
    {
        public string _(string text)
        {
            return HttpContext.GetText(text);
        }
    }
}
