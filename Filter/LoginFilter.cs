//<UtopiaCopyright>
//--------------------------------------------------------------
//<copyright>乌托邦网络科技有限公司 2017?-?</copyright>
//<version>V1.0</verion>
//<createdate>2013-05-20</createdate>
//<author>wanf</author>
//<email>329902077@qq.com</email>
//<log date="2013-05-20" version="1.0">创建</log>
//--------------------------------------------------------------
//</UtopiaCopyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Utopia
{
    /// <summary>
    /// 登录过滤器
    /// </summary>
    public class LoginFilter:FilterAttribute,IActionFilter
    {
        #region IActionFilter 成员

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (UserContext.CurrentUser==null)
	        {
                filterContext.Result = new ViewResult()
                {
                    ViewName = "Login"
                };
	        }
        }

        #endregion
    }
}