using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Utopia
{
    public class UrlHelper
    {
        #region Instance

        private static volatile UrlHelper _instance = null;
        private static readonly object lockObject = new object();
        private static System.Web.Mvc.UrlHelper urlHelper = null;

        /// <summary>
        /// 创建主页实体
        /// </summary>
        /// <returns></returns>
        public static UrlHelper Instance()
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new UrlHelper();
                    }
                }
            }
            urlHelper = new System.Web.Mvc.UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));
            return _instance;
        }

        private UrlHelper()
        {
        }

        #endregion Instance

        #region Channel

        /// <summary>
        /// 频道登陆Get
        /// </summary>
        public string ChannelLoginGet()
        {
            return urlHelper.Action("Login", "Channel");
        }

        /// <summary>
        /// 频道登陆Post
        /// </summary>
        public string ChannelLoginPost()
        {
            return urlHelper.Action("Login", "Channel");
        }

        /// <summary>
        /// 频道注册Post
        /// </summary>
        public string ChannelRegisterPost()
        {
            return urlHelper.Action("Register", "Channel");
        }

        /// <summary>
        /// 频道处理帐号激活
        /// </summary>
        public string ChannelActivity()
        {
            return urlHelper.Action("SuccessActivity", "Channel");
        }

        /// <summary>
        /// 频道首页
        /// </summary>
        public string ChannelHome()
        {
            return urlHelper.Action("Home", "Channel");
        }

        /// <summary>
        /// 频道退出
        /// </summary>
        /// <returns></returns>
        public string ChannelLoginOut()
        {
            return urlHelper.Action("LoginOut", "Channel");
        }

        /// <summary>
        /// 频道首页右侧局部页
        /// </summary>
        /// <returns></returns>
        public string ChannelHomeRight(EnumClass.ChannelHomeRightNav NavEnum,long urlUserId=0)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("NavEnum", NavEnum);
            rvd.Add("urlUserId", urlUserId);
            return urlHelper.Action("_HomeRight", "Channel", rvd);
        }

        /// <summary>
        /// 频道发布吐槽
        /// </summary>
        /// <returns></returns>
        public string ChannelPublishMyWord()
        {
            return urlHelper.Action("PublishMyWord", "Channel");
        }

        /// <summary>
        /// 频道删除吐槽
        /// </summary>
        /// <returns></returns>
        public string ChannelDeleteMyWord(long microblogId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("microblogId", microblogId);
            return urlHelper.Action("DeleteMyWord", "Channel",rvd);
        }

        /// <summary>
        /// 频道转发吐槽
        /// </summary>
        /// <returns></returns>
        public string ChannelRepeatMyWord()
        {
            return urlHelper.Action("RepeatMyWord", "Channel");
        }

         /// <summary>
        /// 频道吐槽详细
        /// </summary>
        /// <returns></returns>
        public string ChannelMicroblogDetail(long microblogId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("microblogId", microblogId);
            return urlHelper.Action("MicroblogDetail", "Channel", rvd);
        }

        /// <summary>
        /// 频道上传图片
        /// </summary>
        /// <returns></returns>
        public string ChannelUploadPic()
        {
            return urlHelper.Action("UploadPic", "Channel");
        }

        /// <summary>
        /// @用户
        /// </summary>
        /// <returns></returns>
        public string ChannelAtUser()
        {
            return urlHelper.Action("atUser", "Channel");
        }

        /// <summary>
        /// 有无最新消息
        /// </summary>
        /// <returns></returns>
        public string IsHasNewMessage()
        {
            return urlHelper.Action("IsHasNewMessage", "Channel");
        }

        /// <summary>
        /// 最新消息弹出页
        /// </summary>
        public string _Message()
        {
            return urlHelper.Action("_Message", "Channel");
        }

        /// <summary>
        /// @用户消息局部页
        /// </summary>
        public string _MessageAtUser(long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            return urlHelper.Action("_MessageAtUser", "Channel", rvd);
        }

        /// <summary>
        /// 微博回复局部页
        /// </summary>
        public string _MessageMicroblogComment(long userId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("userId", userId);
            return urlHelper.Action("_MessageMicroblogComment", "Channel", rvd);
        }

        /// <summary>
        /// 评论局部页面
        /// </summary>
        public string _Comment(long belongId,long userId,string type,bool all=false)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("belongId", belongId);
            rvd.Add("userId", userId);
            rvd.Add("type", type);
            rvd.Add("all", all);
            return urlHelper.Action("_Comment", "Channel", rvd);
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        public string AddComment(long belongId, long userId, string type)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("belongId", belongId);
            rvd.Add("userId", userId);
            rvd.Add("type", type);
            return urlHelper.Action("AddComment", "Channel", rvd);
        }

        //删除评论
        public string DeleteComment(long commentId)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("commentId", commentId);
            return urlHelper.Action("DeleteComment", "Channel", rvd);
        }

        //搜索结果页
        public string SearchResult()
        {
            return urlHelper.Action("SearchResult", "Channel");
        }

        //搜索微博局部页
        public string _SearchMicroblog(string keyword)
        {
            RouteValueDictionary rvd = new RouteValueDictionary();
            rvd.Add("keyword", keyword);
            return urlHelper.Action("_SearchMicroblog", "Channel", rvd);
        }

        #endregion

        #region UserSpace



        #endregion

        #region ControlPanel

        /// <summary>
        /// 后台微博索引
        /// </summary>
        public string ControlPanelBuildIndexTuCao()
        {
            return urlHelper.Action("BulidIndexTuCao", "ControlPanel");
        }

        #endregion
    }
}