using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Utopia
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    #region 查找资料

    // IIS Express 7.5使用及配置方法
    // http://blog.sina.com.cn/s/blog_8eee76520100u6lv.html

    // Bootstrap中文网 需要jquery-1.9.1.js
    // http://www.bootcss.com/

    // jquery.validate.min.js的使用
    // http://www.cnblogs.com/easyit/articles/1733788.html

    // .NET Reflector 7.0.0.420 注册破解版
    // http://ishare.iask.sina.com.cn/f/14197647.html
    // http://ishare.iask.sina.com.cn/download/explain.php?fileid=14197647

    // Entity Framework 入门介绍
    // http://www.cnblogs.com/mvpajun/archive/2010/07/11/Shawdren.html

    // ASP.NET MVC 拦截器
    // http://www.cnblogs.com/GeneralXU/archive/2010/01/05/1639533.html

    // System.Web.HttpException: 节未定义
    // 在Views目录下又一个_ViewStart.cshtml文件，这个文件优先于同目录下任何视图的执行
    // http://www.cnblogs.com/kissdodog/archive/2013/01/07/2848881.html

    // artDialog art.dialog.open、art.dialog.tips等扩展方法需要引入jquery.artDialog.iframeTools.min.js
    // http://www.planeart.cn/demo/artDialog/

    // ASP.NET MVC 2.0 Html.EditorForModel & Html.DisplayForModel
    // http://www.it165.net/pro/html/201302/4849.html

    //css自动换行 word-break:break-all;word-wrap:break-word;
    // http://www.cnblogs.com/mofish/archive/2011/02/16/1956263.html

    #endregion

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Channel

            routes.MapRoute(
                "Channel_Login", // 频道登录页
                "Login", // 带有参数的 URL
                new { controller = "Channel", action = "Login" } // 参数默认值
            );

            routes.MapRoute(
               "Channel_Home", // 频道首页
               "Home/{userId}", // URL with parameters
               new { controller = "Channel", action = "Home" }, // Parameter defaults
               new { userId = @"(\d+)|(\{\d+\})" }
           );

            routes.MapRoute(
               "Channel_SuccessActivity", // 频道激活处理
               "SuccessActivity", // URL with parameters
               new { controller = "Channel", action = "SuccessActivity" } // Parameter defaults
           );

            routes.MapRoute(
               "Channel_HomeRight", // 频道首页右侧局部页
               "HomeRight", // URL with parameters
               new { controller = "Channel", action = "_HomeRight" } // Parameter defaults
           );

            routes.MapRoute(
               "Channel_Common", // 频道公共路由
               "{action}", // URL with parameters
               new { controller = "Channel" } // Parameter defaults
           );

            #endregion

            #region UserSpace



            #endregion

            #region ControlPanel

            routes.MapRoute(
                "ControlPanel_BuildIndex", // 后台重建索引页面
                "ControlPanel/BuildIndex", // 带有参数的 URL
                new { controller = "ControlPanel", action = "BuildIndex" } // 参数默认值
            );

            routes.MapRoute(
                "ControlPanel_BulidIndexTuCao", // 后台重建吐槽索引
                "ControlPanel/BulidIndexTuCao", // 带有参数的 URL
                new { controller = "ControlPanel", action = "BulidIndexTuCao" } // 参数默认值
            );

            routes.MapRoute(
                "ControlPanel_Login", // 后台登陆页面
                "ControlPanel/Login", // 带有参数的 URL
                new { controller = "ControlPanel", action = "Login" } // 参数默认值
            );

            routes.MapRoute(
                "ControlPanel_Common", // 后台公共路由
                "{action}", // 带有参数的 URL
                new { controller = "ControlPanel" } // 参数默认值
            );

            #endregion

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}