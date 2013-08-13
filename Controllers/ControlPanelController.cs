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
using LuceneSearch;
using Utopia.Service;
using Lucene.Net.Documents;
using System.IO;

namespace Utopia.Controllers
{
    /// <summary>
    /// 乌托邦后台管理控制器
    /// </summary>
    public class ControlPanelController : Controller
    {
        UtopiaService utopiaService = new UtopiaService();

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 建索引页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BuildIndex()
        {
            return View();
        }

        /// <summary>
        /// 全部重建吐槽索引
        /// </summary>
        /// <returns></returns>
        public ActionResult BulidIndexTuCao()
        {
            string indexPath = HttpContext.Server.MapPath("../Index/Microblog");
            bool isCreated = true; 

            //每次取1000条建索引
            int pageIndex = 1;
            while (true)
            {
                List<Document> documents=new List<Document>();
                List<Uto_Microblog> microblogs = utopiaService.GetMicroBlogIndex(pageIndex);
                pageIndex++;
                if (microblogs != null && microblogs.Count > 0)
                {
                    foreach (var microblog in microblogs)
	                {
		                Document document = new Document();
                        document.Add(new Field("MicroblogId", microblog.MicroblogId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                        document.Add(new Field("Body", microblog.Body.ToString(), Field.Store.NO, Field.Index.ANALYZED));
                        document.Add(new Field("RepeatId", microblog.RepeatId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                        documents.Add(document);
	                }
                    LuceneIndex.BuildIndex(indexPath, isCreated, true, documents);
                }
                else
                {
                    break;
                }
            }
            
            return Json(1);
        }
    }
}
