using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utopia.Models
{
    /// <summary>
    /// 微博实体类
    /// </summary>
    public class Microblog
    {
        Utopia.Service.UtopiaService utopiaService = new Service.UtopiaService();

        public static Microblog New()
        {
            return new Microblog{
                DateCreated=DateTime.Now,
                UserId=UserContext.CurrentUser.UserId
            };
        }

        #region 持久化数据

        public long MicroblogId { get; protected set; }

        public long UserId { get; set; }

        public string Body { get; set; }

        public DateTime DateCreated { get; set; }

        public long RepeatId { get; set; }

        public string RepeatContent { get; set; }

        public long AttachmentId { get; set; }

        #endregion

        #region 导航属性

        /// <summary>
        /// 微博的作者
        /// </summary>
        public Uto_User MicroBlogUser
        {
            get {
                return utopiaService.GetUserById(this.UserId);
            }
        }

        /// <summary>
        /// 获取转发微博的原始作者
        /// </summary>
        public Uto_User OriMicroBlogUser
        {
            get
            {
                return utopiaService.GetUserById(utopiaService.GetMicroBlogById(this.RepeatId).UserId);
            }
        }

        #endregion

        public Uto_Microblog AsDbMicroblog()
        {
            return new Uto_Microblog
            {
                UserId = this.UserId,
                Body = this.Body,
                DateCreated = this.DateCreated,
                RepeatId=this.RepeatId,
                RepeatContent = this.RepeatContent,
                AttachmentId = this.AttachmentId
            };
        }
    }

    public static class Uto_MicroblogExtexse
    {
        public static Utopia.Service.UtopiaService utopiaService = new Utopia.Service.UtopiaService();

        public static Microblog AsMicroblog(this Uto_Microblog DbMicroblog)
        {
            return new Microblog
            {
                UserId = DbMicroblog.UserId,
                Body = DbMicroblog.Body,
                DateCreated = DbMicroblog.DateCreated,
                RepeatId = DbMicroblog.RepeatId,
                RepeatContent = DbMicroblog.RepeatContent,
                AttachmentId = DbMicroblog.AttachmentId
            };
        }

        /// <summary>
        /// 某吐槽的附件(图片)
        /// </summary>
        public static List<Uto_Attachment> MicroblogAttachments(this Uto_Microblog DbMicroblog)
        {
            if (DbMicroblog.RepeatId>0)
            {
                DbMicroblog = utopiaService.GetMicroBlogById(DbMicroblog.RepeatId);
            }
            if (DbMicroblog.AttachmentId==1)
            {
                using (UtopiaEntities db = new UtopiaEntities())
                {
                    return db.Uto_Attachment.Where(n => n.BelongId == DbMicroblog.MicroblogId).ToList<Uto_Attachment>();
                }   
            }
            return null;
        }

        /// <summary>
        /// 前4个转发内容(共5个)，这里不需要解析
        /// </summary>
        public static string FrontRepeatContent(this Uto_Microblog microblog)
        {
            string repeatContent = null;

            if (microblog.RepeatId>0)
            {
                List<string> repeatContentList = utopiaService.GetMicroBlogById(microblog.MicroblogId).RepeatContent.Split(';').ToList();
                repeatContentList = repeatContentList.Take(4).ToList();


                foreach (var item in repeatContentList)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        repeatContent += item + ";";   
                    }
                }   
            }

            return repeatContent;
        }
    }
}