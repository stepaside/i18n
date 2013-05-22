using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace i18n.Web.Http
{
    public abstract class i18nApiController : ApiController
    {
        private LanguageItem[] _userLanguages = null;

        public string _(string text)
        {
            // Lookup resource.
            LanguageTag lt;
            text = LocalizedApplication.Current.TextLocalizerForApp.GetText(text, GetRequestUserLanguages(this.Request), out lt) ?? text;
            return HttpUtility.HtmlDecode(text);
        }

        private LanguageItem[] GetRequestUserLanguages(HttpRequestMessage request)
        {
            if (_userLanguages == null)
            {
                // Construct UserLanguages list and cache it for the rest of the request.
                _userLanguages = LanguageItem.ParseHttpLanguageHeader(request.Headers.AcceptLanguage.ToString(), LocalizedApplication.Current.DefaultLanguageTag);
            }
            return _userLanguages;
        }
    }
}
