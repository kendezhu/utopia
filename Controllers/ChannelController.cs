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
using Utopia.Service;
using Utopia;
using Utopia.Models;
using System.IO;
using System.Text;
using LuceneSearch;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace Utopia.Controllers
{
    /// <summary>
    /// 乌托邦频道控制器
    /// </summary> 
    public class ChannelController : Controller
    {
        private UtopiaService utopiaService = new UtopiaService();

        #region 登录/退出/注册


        //登陆/注册 页面
        public ActionResult Login()
        {
            if (UserContext.CurrentUser != null)
            {
                return Redirect(UrlHelper.Instance().ChannelHome() + "/" + UserContext.CurrentUser.UserId);
            }

            return View();
        }

        //登陆
        [HttpPost]
        public ActionResult Login(string email, string password, bool rememberme = false)
        {
            Uto_User user = utopiaService.GetUserByEmail(email);

            if (user != null && user.Password == password)
            {
                UserContext.AddUserToCookie(email, password, rememberme);
                return Json(user.UserId);
            }
            else
            {
                return Json(0);
            }
        }

        //注册
        [HttpPost]
        public ActionResult Register(Uto_User user)
        {
            //查看有无重复邮箱
            if (utopiaService.GetUserByEmail(user.Email)!=null)
	        {
                return Json(0);
	        }
            else
            {
                //用户的激活码用于邮箱激活
                user.ActivityCode = Guid.NewGuid().ToString();
                user.IsActivity = "0";
                user.DateCreated = DateTime.Now;
                utopiaService.RegisterUser(user);

                UserContext.AddUserToCookie(user.Email, user.Password, false);
                Utility.SendMail(user);

                return Json(user.UserId);
            }
        }

        //处理帐号激活
        public ActionResult SuccessActivity(long userId, string activityCode)
        {
            Uto_User user = utopiaService.GetUserById(userId);

            ViewData["activityStatus"] = false;
            if (user.ActivityCode == activityCode)
            {
                user.IsActivity = "1";
                utopiaService.UpdateUser(user);
                ViewData["activityStatus"] = true;
            }

            return View();
        }

        //退出
        [HttpPost]
        public ActionResult LoginOut()
        {
            UserContext.ClearUserFromCookie();
            return Json(1);
        }

        #endregion

        #region 频道呈现

        //频道首页
        [LoginFilter()]
        public ActionResult Home(long userId = 0)
        {
            //头部选中项
            ViewData["headerSelected"] = "Home";

            Uto_User user = utopiaService.GetUserById(userId);
            ViewBag.userName = user == null ? string.Empty : user.Username;

            return View();
        }

        //频道页LeftSide的头像等区域
        public ActionResult _ChannelUserInfo()
        {
            //头部选中项
            ViewData["headerSelected"] = "Home";

            //获取当前访问地址中的UserId，从而获取该用户
            ViewBag.User = Url.UrlUser();

            return View();
        }

        #region 频道首页右侧的局部页

        //频道首页右侧的局部页 NavStr为导航枚举
        public ActionResult _HomeRight(EnumClass.ChannelHomeRightNav NavEnum = EnumClass.ChannelHomeRightNav.MyWord, long urlUserId = 0, int pageIndex = 1, int pageSize = 10)
        {
            string ViewName = null;
            switch (NavEnum)
            {
                case EnumClass.ChannelHomeRightNav.MySale:
                    ViewName = "_MySale";
                    break;
                case EnumClass.ChannelHomeRightNav.MyShareMemory:
                    ViewName = "_MyShareMemory";
                    break;
                case EnumClass.ChannelHomeRightNav.MyFood:
                    ViewName = "_MyFood";
                    break;
                case EnumClass.ChannelHomeRightNav.TheirFood:
                    ViewName = "_TheirFood";
                    break;
                case EnumClass.ChannelHomeRightNav.TheirSale:
                    ViewName = "_TheirSale";
                    break;
                case EnumClass.ChannelHomeRightNav.TheirShareMemory:
                    ViewName = "_TheirShareMemory";
                    break;
                case EnumClass.ChannelHomeRightNav.TheirWord:
                    ViewName = "Microblog/_TheirWord";
                    break;
                default:
                    ViewBag.MyWord = MyWord(urlUserId, pageIndex, pageSize);
                    ViewName = "Microblog/_MyWord";
                    break;
            }
            return View(ViewName);
        }

        //我的吐槽业务逻辑
        public List<Uto_Microblog> MyWord(long urluserId, int pageIndex = 1, int pageSize = 10)
        {
            //当前URL地址栏的用户
            ViewBag.UrlUser = utopiaService.GetUserById(urluserId);
            //当前URL地址栏的用户的吐槽总数
            ViewBag.TotalRecord = utopiaService.GetTotalUserMicroBlogCount(urluserId);

            //插入表情
            GetEmotionStr();

            //插入图片
            string uploadUrl = UrlHelper.Instance().ChannelUploadPic();
            string picStr = @"<form target='hidden_frame' name='tucaoPicForm' id='tucaoPicForm' method='post' enctype='MULTIPART/FORM-DATA' action='" + uploadUrl + "'>" +
                               @"<input name='tucaoPicFile' id='tucaoPicFile' type='file' accept='image/gif,image/jpeg,image/jpg,image/png'/>
                              <button id='tucaoPicSubmit' class='btn btn-primary' type='button'>上传</button>
                              <span class='muted' id='tucaoPicInfo' style='margin-left:15px'>还可以传<span style='margin-left:2px;margin-right:2px;' id='tucaoPicNum'>4</span>张</span>
                              <iframe name='hidden_frame' id='hidden_frame' style='display:none'></iframe>
                              <div style='margin-top:4px' id='tucaoPicYuLan'></div>
                            </form>";
            ViewBag.Picture = picStr.ToString();

            //at用户
            StringBuilder atUserStr = new StringBuilder("<ul id='atUserUl' class='nav nav-list'>");
            atUserStr.Append("<li><div style='margin-bottom:6px' class='input-prepend'><span class='add-on'>@</span><input class='span12' id='atUserTxt' type='text' placeholder='用户名'></div><li>");
            atUserStr.Append("</ul>");

            ViewBag.atUser = atUserStr.ToString();

            return utopiaService.GetUserMicroBlog(urluserId, pageIndex, pageSize);
        }

        #endregion

        #endregion       

        #region 吐槽

        //发布吐槽
        [HttpPost]
        public ActionResult PublishMyWord(string microblogBody, string attachmentIds = null)
        {
            Microblog microblog = Microblog.New();
            microblog.Body = microblogBody;
            Uto_Microblog dbMicroblog = microblog.AsDbMicroblog();

            utopiaService.PublishMicroBlog(dbMicroblog);

            //更新附件BelongId
            if (!string.IsNullOrEmpty(attachmentIds))
            {
                IEnumerable<long> longattachmentIds = attachmentIds.Split(',').Where(n => !string.IsNullOrEmpty(n)).Take(4).Select(n => long.Parse(n));
                foreach (var attachmentId in longattachmentIds)
                {
                    Uto_Attachment attachment = utopiaService.GetAttachmentById(attachmentId);
                    attachment.BelongId = dbMicroblog.MicroblogId;
                    utopiaService.UpdateAttachment(attachment);
                    //表示该吐槽有附件
                    dbMicroblog.AttachmentId = 1;
                    utopiaService.UpdateMicroBlog(dbMicroblog);
                }
            }

            //解析其中的@用户 然后发送有人@你的消息
            List<long> userIds = Utility.atUserIds(microblogBody).Select(n => long.Parse(n)).ToList();
            foreach (var userId in userIds)
            {
                Message message = Message.New();
                message.IsIgnore = false;
                message.UserId = userId;
                message.BelongId = dbMicroblog.MicroblogId;
                message.ChildrenBelongId = 0;
                message.DateCreated = DateTime.Now;
                message.Type = "microblogAtUser"; //微博@用户
                Uto_Message dbMessage = message.AsDbMessage();
                utopiaService.SendMessage(dbMessage);
            }

            //吐槽增量索引
            List<Document> documents = new List<Document>();
            Document document = new Document();
            document.Add(new Field("MicroblogId", dbMicroblog.MicroblogId.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("Body", dbMicroblog.Body.ToString(), Field.Store.NO, Field.Index.ANALYZED));
            document.Add(new Field("RepeatId", dbMicroblog.RepeatId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            documents.Add(document);
            string indexPath = HttpContext.Server.MapPath("~/Index/Microblog");
            LuceneIndex.BuildIndex(indexPath,false,true,documents);

            return Json(1);
        }

        //删除吐槽
        [HttpPost]
        public ActionResult DeleteMyWord(long microblogId)
        {
            //吐槽删除索引
            string indexPath = HttpContext.Server.MapPath("~/Index/Microblog");
            Term term=new Term("MicroblogId",microblogId.ToString());
            LuceneIndex.DeleteIndex(indexPath, term);
            return Json(utopiaService.DeleteMicroBlog(microblogId));
        }

        //转发吐槽
        [HttpPost]
        public ActionResult RepeatMyWord(string repeatContent,long microblogId)
        {
            //原始微博转发数+1
            Uto_Microblog oriMicroblog = utopiaService.GetMicroBlogById(microblogId);
            oriMicroblog.RepeatCount++;
            utopiaService.UpdateMicroBlog(oriMicroblog);

            //产生新的转发微博
            Uto_Microblog microblog = new Uto_Microblog();
            microblog.Body = oriMicroblog.Body;
            microblog.DateCreated = DateTime.Now;
            microblog.RepeatId = oriMicroblog.RepeatId > 0 ? oriMicroblog.RepeatId : microblogId;
            microblog.UserId = UserContext.CurrentUser.UserId;
            microblog.RepeatContent = repeatContent;
            microblog.AttachmentId = 0;
            utopiaService.PublishMicroBlog(microblog);

            return Json(1);
        }

        //吐槽详细页
        [LoginFilter()]
        public ActionResult MicroblogDetail(long microblogId)
        {
            List<Uto_Microblog> microblogs=new List<Uto_Microblog>();
            microblogs.Add(utopiaService.GetMicroBlogById(microblogId));
            return View("Microblog/MicroblogDetail", utopiaService.AnalyzeBody(microblogs).First());
        }

        //上传吐槽配图
        [HttpPost]
        public void UploadPic()
        {
            HttpPostedFileBase postFile = Request.Files["tucaoPicFile"];

            string attachmentUrl = string.Empty;
            long attachmentId = 0;
            if (postFile != null && postFile.ContentLength > 0)
            {
                Utopia.Utility.SavePic(postFile, "Microblog", out attachmentUrl, out attachmentId);
            }

            Response.Write("<script>parent.uploadCallback('" + attachmentUrl + "^" + attachmentId + "')</script>");
        }

        #endregion

        #region 消息

        /// <summary>
        /// 有无新消息
        /// </summary>
        public ActionResult IsHasNewMessage(long newMessageId)
        {
            bool flag = utopiaService.IsHasNewMessage(newMessageId, UserContext.CurrentUser.UserId);
            if (flag)
                return Json(1);
            else
                return Json(0);
        }

        /// <summary>
        /// 某人的消息提醒弹出页(各种消息的前10条 点更多..到消息页面)
        /// </summary>
        public ActionResult _Message()
        {
            return View("Message/_Message");
        }

        /// <summary>
        /// @用户消息
        /// </summary>
        /// <returns></returns>
        public ActionResult _MessageAtUser(long userId)
        {
            List<Message> messages = utopiaService.GetMessageByUserId(userId,"microblogAtUser").Select(n => n.AsMessage()).ToList();
            return View("Message/_MessageAtUser", messages);
        }

        /// <summary>
        /// 微博回复消息
        /// </summary>
        /// <returns></returns>
        public ActionResult _MessageMicroblogComment(long userId)
        {
            List<Message> messages = utopiaService.GetMessageByUserId(userId, "microblogComment").Select(n => n.AsMessage()).ToList();
            return View("Message/_MessageMicroblogComment", messages);
        }

        #endregion

        #region 评论

        /// <summary>
        /// 评论页面 all是否显示全部
        /// </summary>
        public ActionResult _Comment(long belongId,long userId,string type,bool all=false,int pageIndex=1,int pageSize=10)
        {
            GetEmotionStr();
            long commentCount = 0;
            List<Uto_Comment> comments = utopiaService.GetCommentByBelongId(belongId, type,out commentCount,all,pageSize,pageIndex);
            ViewBag.belongId = belongId;
            ViewBag.userId = userId;
            ViewBag.type = type;
            ViewBag.commentCount = commentCount;
            ViewBag.all = all;
            return View("Comment/_Comment",comments);
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddComment(string body,long belongId,long userId,string type)
        {
            Uto_Comment comment = new Uto_Comment();
            comment.Body = body;
            comment.UserId = userId;
            comment.SendUserId = UserContext.CurrentUser.UserId;
            comment.DateCreated = DateTime.Now;
            comment.Type = type;
            comment.BelongId = belongId;
            utopiaService.AddComment(comment);

            //更新被评论数
            UpdateCommentCount(belongId, type,true);

            //发出评论通知
            Message message = Message.New();
            message.IsIgnore = false;
            message.UserId = userId;
            message.BelongId = belongId;
            message.ChildrenBelongId = comment.CommentId;
            message.DateCreated = DateTime.Now;
            message.Type = type;
            Uto_Message dbMessage = message.AsDbMessage();
            utopiaService.SendMessage(dbMessage);

            return Json(1);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteComment(long commentId)
        {
            Uto_Comment comment = utopiaService.GetCommentById(commentId);
            if (comment.SendUserId==UserContext.CurrentUser.UserId)
            {
                utopiaService.DeleteComment(comment.CommentId);

                //更新评论数
                UpdateCommentCount(comment.BelongId,comment.Type,false);

                //删除评论通知
                utopiaService.DeleteMessage(null, null, comment.CommentId,comment.Type);
                
                return Json(1);
            }
            return Json(0);
        }

        #endregion

        #region 搜索

        /// <summary>
        /// 搜索结果页面
        /// </summary>
        public ActionResult SearchResult(string keyword)
        {
            ViewBag.keyword = keyword;
            ViewBag.title = "'" + keyword + "' 的搜索结果";
            return View();
        }

        /// <summary>
        /// 搜索微博
        /// </summary>
        public ActionResult _SearchMicroblog(string keyword, int pageIndex = 1, int pageSize = 4)
        {
            string indexPath = HttpContext.Server.MapPath("~/Index/Microblog");

            int totalRecord = 0;
            string totalTime = null;

            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(new TermQuery(new Term("RepeatId", "0")), BooleanClause.Occur.MUST);
            string[] panguKeywords = LuceneIndex.GetKeyWordSplitBySpace(keyword, true).Split(',');
            foreach (var item in panguKeywords)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    booleanQuery.Add(new TermQuery(new Term("Body",item)),BooleanClause.Occur.MUST);
                }
            }

            List<Document> documents = LuceneIndex.QuerySearch(indexPath, booleanQuery, null, null, pageIndex, pageSize, out totalRecord, out totalTime);

            List<Uto_Microblog> microblogs = new List<Uto_Microblog>();
            int currentIndex = (pageIndex - 1) * pageSize;
            int maxIndex = pageIndex * pageSize - 1;
            foreach (var document in documents)
            {
                microblogs.Add(utopiaService.GetMicroBlogById(long.Parse(document.Get("MicroblogId"))));
            }
            microblogs = utopiaService.AnalyzeBody(microblogs);

            ViewBag.keyword = keyword;
            ViewBag.totalRecord = totalRecord;
            ViewBag.totalTime = totalTime;

            return View("Microblog/_SearchMicroblog", microblogs);
        }

        #endregion

        #region PrivateMethod

        /// <summary>
        /// 表情字符串
        /// </summary>
        /// <param name="isTuC">是否是吐槽那里的表情</param>
        private void GetEmotionStr()
        {
            List<string> fileNames = Utility.GetFileNameOfDirectory(Server.MapPath("~/Content/Emotion"));
            StringBuilder emotionStr = new StringBuilder("<ul class='inline'>");
            foreach (var fileName in fileNames)
            {
                emotionStr.Append("<li>");
                emotionStr.Append("<img style='cursor:pointer;' class='emotionGif' id='" + fileName.TrimEnd(new char[] { '.', 'g', 'i', 'f' }) + "' src='../../Content/Emotion/" + fileName + "'/>");
                emotionStr.Append("</li>");
            }
            ViewBag.Emotion = emotionStr.ToString();
        }

        /// <summary>
        /// 更新评论数
        /// </summary>
        /// <param name="belongId">被评论内容ID</param>
        /// <param name="type">评论类型</param>
        /// <param name="isPlus">++还是--</param>
        private void UpdateCommentCount(long belongId, string type,bool isPlus)
        {
            switch (type.Trim())
            {
                //微博评论
                case "microblogComment":
                    Uto_Microblog microblog = utopiaService.GetMicroBlogById(belongId);
                    if (!isPlus && microblog.CommentCount > 0)
                    {
                        microblog.CommentCount--;
                    }
                    else
                    {
                        microblog.CommentCount++;
                    }
                    utopiaService.UpdateMicroBlog(microblog);
                    break;
                //商品评论
                case "shopComment":
                    break;
                //午饭评论
                case "foodComment":
                    break;
                // 分享评论
                case "shareComment":
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region PublicMethod

        //@用户-查找
        [HttpPost]
        public ActionResult atUser(string userName)
        {
            Uto_User currentUser = UserContext.CurrentUser;
            List<Uto_User> users = utopiaService.GetUsersByUserName(userName).Where(n => n.Email != currentUser.Email).Take(20).ToList();
            return Json(users.Select(n => new { id = n.UserId, name = n.Username, sex = (n.Sex.Trim() == "1") ? "男" : "女" }));
        }

        #endregion
    }
}
