using System;
using System.Web;

namespace YAF.Classes
{
    #region Using

    using YAF.Types.Extensions;

    #endregion

    public class SitecoreUrlBuilder : BaseUrlBuilder
    {
        #region Public Methods

        public override string BuildUrl(string url)
        {
            // escape & to &amp;
            url = url.Replace("&", "&amp;");

            var path = HttpContext.Current.Request.Url.LocalPath;
            // return URL to current script with URL from parameter as script's parameter
            //return "{0}{1}?{2}".FormatWith(path, Config.ForceScriptName ?? ScriptName, url);
            return "{0}?{1}".FormatWith(path, url);
        }

      
        public override string BuildUrl(object boardSettings, string url)
        {
            return this.BuildUrl(url);
        }

        #endregion
    }
}