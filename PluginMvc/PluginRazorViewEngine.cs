using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages.Razor;

namespace PluginMvc
{

    /// <summary>
    /// 
    /// </summary>
    public class PluginRazorViewEngine : RazorViewEngine 
    {
        private string[] _areaViewLocationFormats = new string[]
			{
                "~/Plugins/{2}/Views/{1}/{0}.cshtml",
                "~/Plugins/{2}/Views/{1}/{0}.vbhtml",
                "~/Plugins/{2}/Views/Shared/{0}.cshtml",
                "~/Plugins/{2}/Views/Shared/{0}.vbhtml",
				"~/Areas/{2}/Views/{1}/{0}.cshtml",
				"~/Areas/{2}/Views/{1}/{0}.vbhtml",
				"~/Areas/{2}/Views/Shared/{0}.cshtml",
				"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			};
        
        private string[] _pluginViewLocationFormats = new string[]
			{
                "~/Plugins/{pluginName}/Views/{1}/{0}.cshtml",
                "~/Plugins/{pluginName}/Views/{1}/{0}.vbhtml",
                "~/Plugins/{pluginName}/Views/Shared/{0}.cshtml",
                "~/Plugins/{pluginName}/Views/Shared/{0}.vbhtml",
                "~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};

        private string[] _viewLocationFormats = new string[]
			{
				"~/Views/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.vbhtml",
				"~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};

        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorViewEngine" /> class.</summary>
        public PluginRazorViewEngine()
            : this(null)
        {
        }
        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorViewEngine" /> class using the view page activator.</summary>
        /// <param name="viewPageActivator">The view page activator.</param>
        public PluginRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            base.AreaViewLocationFormats = _areaViewLocationFormats;
            base.AreaMasterLocationFormats = _areaViewLocationFormats;
            base.AreaPartialViewLocationFormats = _areaViewLocationFormats;

            base.ViewLocationFormats = _viewLocationFormats;
            base.MasterLocationFormats = _viewLocationFormats;
            base.PartialViewLocationFormats = _viewLocationFormats;

            base.FileExtensions = new string[]
			{
				"cshtml",
				"vbhtml"
			};
        }


        /// <summary>
        /// 搜索部分视图页。
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="partialViewName"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            string areaName = GetAreaName(controllerContext.RouteData);
            if (areaName != null)
            {
                this.CodeGeneration(areaName);
            }
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            string areaName = GetAreaName(controllerContext.RouteData);
            if (areaName != null)
            {
                this.CodeGeneration(areaName);
            }
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }


        protected virtual string GetAreaName(System.Web.Routing.RouteData routeData)
        {
            //if (routeData.Values.ContainsKey("pluginName"))
            //{
            //    var pluginName = routeData.GetRequiredString("pluginName");
            //    return pluginName;
            //}

            object obj2;
            if (routeData.DataTokens.TryGetValue("area", out obj2))
            {
                return (obj2 as string);
            }
            return GetAreaName(routeData.Route);
        }
        protected virtual string GetAreaName(RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as Route;
            if ((route2 != null) && (route2.DataTokens != null) && (route2.DataTokens.ContainsKey("pluginName")))
            {
                return (route2.DataTokens["pluginName"] as string);
            }
            return null;
        }

        /// <summary>
        /// 给运行时编译的页面加了引用程序集。
        /// </summary>
        /// <param name="pluginName"></param>
        private void CodeGeneration(string pluginName)
        {
            RazorBuildProvider.CodeGenerationStarted += (object sender, EventArgs e) =>
            {
                RazorBuildProvider provider = (RazorBuildProvider)sender;

                var plugin = PluginManager.GetPlugin(pluginName);

                if (plugin != null)
                {
                    //var assems = AppDomain.CurrentDomain.GetAssemblies();
                    //foreach (var assem in assems)
                    //{
                    //    provider.AssemblyBuilder.AddAssemblyReference(assem);
                    //}
                    //provider.AssemblyBuilder.AddAssemblyReference(plugin.Assembly);
                    if (plugin.Installed == false && plugin.DependentAssemblys != null)
                    {
                        foreach (var assem in plugin.DependentAssemblys)
                        {
                            provider.AssemblyBuilder.AddAssemblyReference(assem);
                        }
                    }
                    plugin.Installed = true;
                }
            };
        }

    }
}