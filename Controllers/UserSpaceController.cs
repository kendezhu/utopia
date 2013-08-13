//<UtopiaCopyright>
//--------------------------------------------------------------
//<copyright>乌托邦网络科技有限公司 2017?-?</copyright>
//<version>V1.0</verion>
//<createdate>2013-05-02</createdate>
//<author>wanf</author>
//<email>329902077@qq.com</email>
//<log date="2013-05-02" version="1.0">创建</log>
//--------------------------------------------------------------
//</UtopiaCopyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Utopia.Controllers
{
    /// <summary>
    /// 乌托邦用户空间控制器
    /// </summary>
    public class UserSpaceController : Controller
    {
        //
        // GET: /UserSpace/

        public ActionResult Index()
        {
            return View();
        }

    }
}
