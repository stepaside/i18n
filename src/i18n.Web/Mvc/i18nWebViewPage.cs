using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace i18n.Web.Mvc.Mvc
{
    public abstract class i18nWebViewPage<T> : WebViewPage<T>
    {
        /// <summary>
        /// Returns markup where the passed value string is wrapped with double quotes,
        /// with optional support for generating an HTML attribute="value" combination.
        /// </summary>
        /// <remarks>
        /// Workaround for limitation writing HTML attributes whose values are translatable strings,
        /// as discussed in issue #8.
        /// The problem is that xgettext, when set to c# mode, does not detect the _() function when
        /// used within double quotes. Thus, <img alt="@_("logo")"> doesn't get picked up.
        /// Using this overload of the _() helper avoids the problem by encoding the quotes within
        /// the method.
        /// </remarks>
        /// <param name="value">String which is to be wrapped in double quotes and also translatable
        /// (and picked up by xgettext and so exported to the POT file).</param>
        /// <param name="attrname">Optional name of an HTML attribute when producing markup for an HTML attribute e.g. alt.
        /// Null or empty string if just the quoted value string to be output.</param>
        /// <returns>Raw html markup string of the form "value" or attrname="value".</returns>
        /// <example>
        /// The following Razor HTML syntax:
        ///       <img @_("Our logo", "alt") src="...">...</img>
        /// will output as:
        ///       <img alt="Our logo" src="...">...</img>
        /// </example>
        /// <example>
        /// The following Razor Javascript syntax:
        ///     <script type="text/javascript">
        ///           $(function () {
        ///             alert(@_("Hello there", ""));
        ///           });
        ///     </script>
        /// will output as:
        ///     <script type="text/javascript">
        ///           $(function () {
        ///             alert("Hello there");
        ///           });
        ///     </script>
        /// </example>
        /// <seealso cref="https://github.com/danielcrenna/i18n/issues/8"/>
        public IHtmlString _(string value, string attr)
        {
            value = this.Context.GetText(value);
            string raw = string.IsNullOrEmpty(attr) ? string.Format("\"{0}\"", value) : string.Format("{0}=\"{1}\"", attr, value);
            return new System.Web.HtmlString(raw);
        }

        public IHtmlString _(string text)
        {
            return new MvcHtmlString(Context.GetText(text));
        }
    }

    public abstract class i18nWebViewPage : WebViewPage
    {
        public IHtmlString _(string text)
        {
            return new MvcHtmlString(Context.GetText(text));
        }
    }
}
