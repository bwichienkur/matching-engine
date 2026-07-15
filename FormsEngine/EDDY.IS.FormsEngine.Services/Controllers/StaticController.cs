using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.FormsEngine.Services.Caching;
using EDDY.IS.FormsEngine.Caching;

using System.Web.Optimization;
using System.Configuration;
using Newtonsoft.Json;
using EDDY.IS.FormsEngine.Core.Interfaces;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    /// <summary>
    /// Controller to manage static resources
    /// </summary>
    public class StaticController : StaticControllerBase
    {
        const string BASE_COMMON_CSS = @"~/Templates/Common/css/{0}.css";
        const string BASE_COMMON_CSS_URI = @"Templates/Common";
        const string BASE_CSS = @"~\Templates\{0}\Themes\{1}\{2}.css";
        const string BASE_CSS_URI = @"Templates/{0}/Themes/{1}";
        const string BASE_JS = @"~\Templates\{0}\js\{1}.js";
        const string CLIENT_JS = @"~\Templates\Common\js\ClientBase.js";
        const string CLIENTWIZARD_JS = @"~\Templates\Common\js\ClientWizardBase.js";
        const string GLOBAL_JS = @"~\Templates\Common\js\Global.js";
        const string DFP_JS = @"~\Templates/Common/js/External/GoogleDFP.js";

        private readonly IFlagService _flagService;

        public StaticController(IFlagService flagService)
        {
            _flagService = flagService;
        }

        [HttpGet]
        public string GetCss(string BasePath, string Theme, string FileName)
        {
            string Result = "";
            try
            {
                string physicalPath = Server.MapPath(string.Format(BASE_CSS, BasePath, Theme, FileName));

                if (!System.IO.File.Exists(physicalPath))
                {
                    Theme = "default";
                }
                physicalPath = Server.MapPath(string.Format(BASE_CSS, BasePath, Theme, FileName));

                string cssResourcePath = Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath.Replace("Static/GetCss", "");
                string cssResourceThemeImage = string.Format(BASE_CSS_URI, BasePath, Theme);
                Response.ContentType = "text/css";
                if (!string.IsNullOrWhiteSpace(FileName))
                {
                    Result = System.IO.File.ReadAllText(physicalPath).Replace("[SERVER]", cssResourcePath + cssResourceThemeImage);
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;
        }

        [HttpGet]
        public string GetCommonCss(string FileName, bool cacheBuster = false, bool minifyJavascript = true)
        {
            BundleCache bundleCache = new BundleCache();
            Response.ContentType = "text/css";
            try
            {
                string cacheKeyString = string.Format(BASE_COMMON_CSS, FileName);

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(cacheKeyString);
                }
                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    string physicalPath = Server.MapPath(string.Format(BASE_COMMON_CSS, FileName));
                    string cssResourcePath = Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath.Replace("Static/GetCommonCss", "");
                    string cssResourceThemeImage = BASE_COMMON_CSS_URI;
                    Response.ContentType = "text/css";

                    bundleCache.VirtualPath = cacheKeyString;
                    var cssSettings = new CssSettings()
                    {
                        CommentMode = CssComment.None,
                        OutputMode = OutputMode.SingleLine,
                        MinifyExpressions = true
                    };

                    Bundle bundle = new Bundle(bundleCache.VirtualPath);
                    bundle.Include(bundleCache.VirtualPath);
                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);

                    var minifiedBundle = new Minifier().MinifyStyleSheet(bundle.GenerateBundleResponse(context).Content, null);
                    minifiedBundle = minifiedBundle.Replace("[SERVERCOMMON]", Request.Url.Scheme + "://" + Request.Url.Authority + "/" + BASE_COMMON_CSS_URI);
                    bundleCache.Content = minifiedBundle.Replace("[SERVER]", cssResourcePath + cssResourceThemeImage);
                    FormsEngineCacheProxy.Cache.Set(cacheKeyString, bundleCache);

                }

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return bundleCache.Content;
        }

        [HttpGet]
        public string GetJs(string BasePath, string FileName, bool CompressJs)
        {
            string Result = "";
            try
            {
                string physicalPath = Server.MapPath(string.Format(BASE_JS, BasePath, FileName));
                Response.ContentType = "application/javascript";
                Result = GetJSFile(physicalPath, CompressJs);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        [HttpGet]
        public string GetBundledWizardJs(bool cacheBuster = true, bool minifyJavascript = true)
        {
            BundleCache bundleCache = new BundleCache();
            try
            {
                bool FEDebugMode = Request.Cookies["FE_DebugMode"] != null;
                if (FEDebugMode)
                {
                    cacheBuster = true;
                    minifyJavascript = false;
                }

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(Constants.WIZARD_BUNDLE_JAVASCRIPT);
                }

                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = Constants.WIZARD_BUNDLE_JAVASCRIPT;
                    var JsCodeSettings = new CodeSettings()
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false,
                        LocalRenaming = LocalRenaming.KeepAll,
                        PreserveFunctionNames = true,
                        MinifyCode = true
                    };
                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);
                    var bundle = new Bundle("~/Bundles/Dynamic/Wizard")
                        .Include("~/Templates/Common/js/select2.full.min.js")
                        .Include("~/Templates/Common/js/jquery.inputmask.min.js")
                        .Include("~/Templates/Common/js/Global.js")
                        .Include("~/Templates/Common/js/ClientWizardBase.js")
                        .Include("~/Templates/Common/js/Wizard_GlobalFunctions.js")
                        .Include("~/Templates/Common/js/ManagedChoice_GlobalFunctions.js")
                        .Include("~/Templates/Common/js/schoolPicker_GlobalFunctions.js")
                        .Include("~/Templates/Common/js/Wizard.js");

                    if (minifyJavascript)
                    {
                        bundleCache.Content = new Minifier().MinifyJavaScript(bundle.GenerateBundleResponse(context).Content, JsCodeSettings);
                    }
                    else
                    {
                        bundleCache.Content = bundle.GenerateBundleResponse(context).Content;
                    }
                    bundleCache.Content = bundleCache.Content.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
                    bundleCache.Content = bundleCache.Content.Replace("[METADATA]", JsonConvert.SerializeObject(FormsEngine.GetResourceMetaDataForTCPA()));
                    bundleCache.Content = bundleCache.Content.Replace("[FEATUREFLAGS]", JsonConvert.SerializeObject(_flagService.GetAllFlags()));
                    if (!FEDebugMode)
                    {
                        FormsEngineCacheProxy.Cache.Set(Constants.WIZARD_BUNDLE_JAVASCRIPT, bundleCache, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    }
                }

                Response.ContentType = "application/javascript";

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return bundleCache.Content;
        }

        [HttpGet]
        public string GetBundledQDFJs(bool cacheBuster = true, bool minifyJavascript = true)
        {
            BundleCache bundleCache = new BundleCache();
            try
            {
                bool FEDebugMode = Request.Cookies["FE_DebugMode"] != null;
                if (FEDebugMode)
                {
                    cacheBuster = true;
                    minifyJavascript = false;
                }

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(Constants.QDF_BUNDLE_JAVASCRIPT);
                }

                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = Constants.QDF_BUNDLE_JAVASCRIPT;
                    var JsCodeSettings = new CodeSettings()
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false,
                        LocalRenaming = LocalRenaming.KeepAll,
                        PreserveFunctionNames = true,
                        MinifyCode = true
                    };
                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);
                    var bundle = new Bundle("~/Bundles/Dynamic/QDF")
                        .Include("~/Templates/Common/js/select2.full.min.js")
                        .Include("~/Templates/Common/js/jquery.inputmask.min.js")
                        .Include("~/Templates/Common/js/Global.js")
                        .Include("~/Templates/Common/js/QDFBase.js")
                        .Include("~/Templates/Common/js/QDF.js");

                    if (minifyJavascript)
                    {
                        bundleCache.Content = new Minifier().MinifyJavaScript(bundle.GenerateBundleResponse(context).Content, JsCodeSettings);
                    }
                    else
                    {
                        bundleCache.Content = bundle.GenerateBundleResponse(context).Content;
                    }

                    bundleCache.Content = bundleCache.Content.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
                    if (!FEDebugMode)
                    {
                        FormsEngineCacheProxy.Cache.Set(Constants.QDF_BUNDLE_JAVASCRIPT, bundleCache, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    }
                }

                Response.ContentType = "application/javascript";

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return bundleCache.Content;
        }

        [HttpGet]
        public string GetBundledQDFPluginJs(bool cacheBuster = true, bool minifyJavascript = true)
        {
            BundleCache bundleCache = new BundleCache();
            try
            {
                bool FEDebugMode = Request.Cookies["FE_DebugMode"] != null;
                if (FEDebugMode)
                {
                    cacheBuster = true;
                    minifyJavascript = false;
                }

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(Constants.QDFPlugin_BUNDLE_JAVASCRIPT);
                }

                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = Constants.QDFPlugin_BUNDLE_JAVASCRIPT;
                    var JsCodeSettings = new CodeSettings()
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false,
                        LocalRenaming = LocalRenaming.KeepAll,
                        PreserveFunctionNames = true,
                        MinifyCode = true
                    };
                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);
                    var bundle = new Bundle("~/Bundles/Dynamic/QDFPlugin")
                        .Include("~/Templates/Common/js/select2.full.min.js")
                        .Include("~/Templates/Common/js/jquery.inputmask.min.js")
                        .Include("~/Templates/Common/js/PluginGlobal.js")
                        .Include("~/Templates/Common/js/QDFPluginDynamicControls.js")
                        .Include("~/Templates/Common/js/QDFPlugin.js");

                    if (minifyJavascript)
                    {
                        bundleCache.Content = new Minifier().MinifyJavaScript(bundle.GenerateBundleResponse(context).Content, JsCodeSettings);
                    }
                    else
                    {
                        bundleCache.Content = bundle.GenerateBundleResponse(context).Content;
                    }
                    bundleCache.Content = bundleCache.Content.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
                    if (!FEDebugMode)
                    {
                        FormsEngineCacheProxy.Cache.Set(Constants.QDF_BUNDLE_JAVASCRIPT, bundleCache, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    }
                }

                Response.ContentType = "application/javascript";

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return bundleCache.Content;
        }

        [HttpGet]
        public string GetBundledWizardCSS(string basePath, string theme, bool cacheBuster = false)
        {

            BundleCache bundleCache = new BundleCache();
            Response.ContentType = "text/css";
            try
            {
                string cacheKeyString = string.Format(Constants.WIZARD_BUNDLE_CSS_CACHE_KEY, basePath, theme);

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(cacheKeyString);
                }
                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = cacheKeyString;
                    var cssSettings = new CssSettings()
                    {
                        CommentMode = CssComment.None,
                        OutputMode = OutputMode.SingleLine,
                        MinifyExpressions = true
                    };

                    List<string> fileList = new List<string>();

                    bool.TryParse(ConfigurationManager.AppSettings.Get("CCPAMessageEnabled"), out bool ccpaMessageEnabled);
                    string ccpaCssPath = "~/Templates/Common/css/CCPA.css";
                    if (System.IO.File.Exists(Server.MapPath(ccpaCssPath)))
                    {
                        fileList.Add(ccpaCssPath);
                    }

                    string themeBaseCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "base"));

                    string themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/default.css", basePath, theme);
                    if (System.IO.File.Exists(themeBaseCSSPhysicalPath))
                    {
                        themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/Base.css", basePath, theme);
                    }
                    fileList.Add(themeBaseCSS);


                    string themeWizardCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "Wizard"));
                    string themeWizardCSS = "";
                    if (System.IO.File.Exists(themeWizardCSSPhysicalPath))
                    {
                        themeWizardCSS = string.Format("~/Templates/{0}/themes/{1}/Wizard.css", basePath, theme);
                        fileList.Add(themeWizardCSS);
                    }

                    string themeManagedChoiceCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "ManagedChoice"));
                    string themeManagedChoiceCSS = "";
                    if (System.IO.File.Exists(themeManagedChoiceCSSPhysicalPath))
                    {
                        themeManagedChoiceCSS = string.Format("~/Templates/{0}/themes/{1}/ManagedChoice.css", basePath, theme);
                        fileList.Add(themeManagedChoiceCSS);
                    }
                    string themeCrossSellAlternateLISTCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "CrossSell"));
                    string themeCrossSellCSS = "";
                    if (System.IO.File.Exists(themeCrossSellAlternateLISTCSSPhysicalPath))
                    {
                        themeCrossSellCSS = string.Format("~/Templates/{0}/themes/{1}/CrossSell.css", basePath, theme);
                        fileList.Add(themeCrossSellCSS);
                    }
                    if (System.IO.File.Exists(Server.MapPath("~/Templates/Common/css/select2.min.css")))
                    {
                        fileList.Add("~/Templates/Common/css/select2.min.css");
                    }
                    string complianceCssPath = "~/Templates/Common/css/Compliance.css";
                    if (System.IO.File.Exists(Server.MapPath(complianceCssPath)))
                    {
                        fileList.Add(complianceCssPath);
                    }
                    Bundle bundle = new Bundle(string.Format(Constants.WIZARD_BUNDLE_CSS_PATH, basePath));

                    foreach (string file in fileList)
                    {
                        bundle.Include(file);
                    }

                    string cssResourcePath = Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath.Replace("Static/GetBundledWizardCSS", "");
                    string cssResourceThemeImage = string.Format(BASE_CSS_URI, basePath, theme);


                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);

                    var minifiedBundle = new Minifier().MinifyStyleSheet(bundle.GenerateBundleResponse(context).Content, null);
                    minifiedBundle = minifiedBundle.Replace("[SERVERCOMMON]", cssResourcePath + BASE_COMMON_CSS_URI);
                    bundleCache.Content = minifiedBundle.Replace("[SERVER]", cssResourcePath + cssResourceThemeImage);
                    FormsEngineCacheProxy.Cache.Set(cacheKeyString, bundleCache);

                }





            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return bundleCache.Content;
        }


        [HttpGet]
        public string GetBundledQDFCSS(string basePath, string theme, bool cacheBuster = false)
        {

            BundleCache bundleCache = new BundleCache();
            Response.ContentType = "text/css";
            try
            {
                string cacheKeyString = string.Format(Constants.QDF_BUNDLE_CSS_CACHE_KEY, basePath, theme);

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(cacheKeyString);
                }
                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = cacheKeyString;
                    var cssSettings = new CssSettings()
                    {
                        CommentMode = CssComment.None,
                        OutputMode = OutputMode.SingleLine,
                        MinifyExpressions = true
                    };

                    List<string> fileList = new List<string>();


                    if (System.IO.File.Exists(Server.MapPath("~/Templates/Common/css/select2.min.css")))
                    {
                        fileList.Add("~/Templates/Common/css/select2.min.css");
                    }

                    if (!string.IsNullOrEmpty(theme))
                    {
                        string themeBaseCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "base"));

                        string themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/default.css", basePath, theme);
                        if (System.IO.File.Exists(themeBaseCSSPhysicalPath))
                        {
                            themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/Base.css", basePath, theme);
                        }
                        fileList.Add(themeBaseCSS);
                    }
                    string complianceCssPath = "~/Templates/Common/css/Compliance.css";
                    if (System.IO.File.Exists(Server.MapPath(complianceCssPath)))
                    {
                        fileList.Add(complianceCssPath);
                    }
                    Bundle bundle = new Bundle(string.Format(Constants.QDF_BUNDLE_CSS_PATH, basePath));

                    foreach (string file in fileList)
                    {
                        bundle.Include(file);
                    }

                    string cssResourcePath = Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath.Replace("Static/GetBundledQDFCSS", "");
                    string cssResourceThemeImage = string.Format(BASE_CSS_URI, basePath, theme);


                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);

                    var minifiedBundle = new Minifier().MinifyStyleSheet(bundle.GenerateBundleResponse(context).Content, null);
                    minifiedBundle = minifiedBundle.Replace("[SERVERCOMMON]", Request.Url.Scheme + "://" + Request.Url.Authority + "/" + BASE_COMMON_CSS_URI);
                    bundleCache.Content = minifiedBundle.Replace("[SERVER]", cssResourcePath + cssResourceThemeImage);
                    FormsEngineCacheProxy.Cache.Set(cacheKeyString, bundleCache);

                }

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return bundleCache.Content;
        }

        [HttpGet]
        public string GetBundledQDFPluginCSS(string basePath, string theme, bool cacheBuster = false)
        {

            BundleCache bundleCache = new BundleCache();
            Response.ContentType = "text/css";
            try
            {
                string cacheKeyString = string.Format(Constants.QDFPlugin_BUNDLE_CSS_CACHE_KEY, basePath, theme);

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(cacheKeyString);
                }
                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = cacheKeyString;
                    var cssSettings = new CssSettings()
                    {
                        CommentMode = CssComment.None,
                        OutputMode = OutputMode.SingleLine,
                        MinifyExpressions = true
                    };

                    List<string> fileList = new List<string>();


                    if (System.IO.File.Exists(Server.MapPath("~/Templates/Common/css/select2.min.css")))
                    {
                        fileList.Add("~/Templates/Common/css/select2.min.css");
                    }

                    if (!string.IsNullOrEmpty(theme))
                    {
                        string themeBaseCSSPhysicalPath = Server.MapPath(string.Format(BASE_CSS, basePath, theme, "base"));

                        string themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/default.css", basePath, theme);
                        if (System.IO.File.Exists(themeBaseCSSPhysicalPath))
                        {
                            themeBaseCSS = string.Format("~/Templates/{0}/themes/{1}/Base.css", basePath, theme);
                        }
                        fileList.Add(themeBaseCSS);
                    }

                    string complianceCssPath = "~/Templates/Common/css/Compliance.css";
                    if (System.IO.File.Exists(Server.MapPath(complianceCssPath)))
                    {
                        fileList.Add(complianceCssPath);
                    }

                    Bundle bundle = new Bundle(string.Format(Constants.QDF_BUNDLE_CSS_PATH, basePath));

                    foreach (string file in fileList)
                    {
                        bundle.Include(file);
                    }

                    string cssResourcePath = Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath.Replace("Static/GetBundledQDFCSS", "");
                    string cssResourceThemeImage = string.Format(BASE_CSS_URI, basePath, theme);


                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);

                    var minifiedBundle = new Minifier().MinifyStyleSheet(bundle.GenerateBundleResponse(context).Content, null);
                    minifiedBundle = minifiedBundle.Replace("[SERVERCOMMON]", Request.Url.Scheme + "://" + Request.Url.Authority + "/" + BASE_COMMON_CSS_URI);
                    bundleCache.Content = minifiedBundle.Replace("[SERVER]", cssResourcePath + cssResourceThemeImage);
                    FormsEngineCacheProxy.Cache.Set(cacheKeyString, bundleCache);

                }

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return bundleCache.Content;
        }

        [HttpGet]
        public string GetClient()
        {
            string Result = "";
            try
            {
                string physicalPath = Server.MapPath(CLIENT_JS);
                Response.ContentType = "application/javascript";
                Result = System.IO.File.ReadAllText(physicalPath);
                Result = Result.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return Result;
        }


        [HttpGet]
        public string GetWizardClient()
        {
            string Result = "";
            try
            {
                string physicalPath = Server.MapPath(CLIENTWIZARD_JS);
                Response.ContentType = "application/javascript";
                Result = System.IO.File.ReadAllText(physicalPath);
                Result = Result.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

        [HttpGet]
        public string GetGlobal(bool cacheBuster = false, bool minifyJavascript = true)
        {
            BundleCache bundleCache = new BundleCache();
            try
            {
                bool FEDebugMode = Request.Cookies["FE_DebugMode"] != null;
                if (FEDebugMode)
                {
                    cacheBuster = true;
                    minifyJavascript = false;
                }

                if (cacheBuster)
                {
                    bundleCache = null;
                }
                else
                {
                    bundleCache = FormsEngineCacheProxy.Cache.Get<BundleCache>(Constants.WIZARD_BUNDLE_GLOBAL);
                }

                if (bundleCache == null)
                {
                    bundleCache = new BundleCache();
                    bundleCache.VirtualPath = Constants.WIZARD_BUNDLE_GLOBAL;
                    var JsCodeSettings = new CodeSettings()
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false,
                        LocalRenaming = LocalRenaming.KeepAll,
                        PreserveFunctionNames = true,
                        MinifyCode = true
                    };
                    var context = new BundleContext(this.HttpContext, BundleTable.Bundles, string.Empty);
                    var bundle = new Bundle("~/Bundles/Dynamic/Global")
                        .Include("~/Templates/Common/js/Global.js");

                    if (minifyJavascript)
                    {
                        bundleCache.Content = new Minifier().MinifyJavaScript(bundle.GenerateBundleResponse(context).Content, JsCodeSettings);
                    }
                    else
                    {
                        bundleCache.Content = bundle.GenerateBundleResponse(context).Content;
                    }

                    bundleCache.Content = bundleCache.Content.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));

                    if (!FEDebugMode)
                    {
                        FormsEngineCacheProxy.Cache.Set(Constants.WIZARD_BUNDLE_GLOBAL, bundleCache, Convert.ToInt32(ConfigurationManager.AppSettings.Get("CacheTemplateMappingsTimeMinutes")));
                    }


                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            Response.ContentType = "application/javascript";
            return bundleCache.Content;
        }

        [HttpGet]
        public string GetDFPScript()
        {
            string Result = "";
            try
            {
                string physicalPath = Server.MapPath(DFP_JS);
                Response.ContentType = "application/javascript";
                Result = System.IO.File.ReadAllText(physicalPath);
                Result = Result.Replace("[SERVICEBASE]", ConfigurationManager.AppSettings.Get("FormsEngineAjaxDomain"));
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return Result;
        }

    }
}
