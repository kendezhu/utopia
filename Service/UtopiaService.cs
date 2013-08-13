using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Utopia.Service
{
    /// <summary>
    /// Service业务逻辑类
    /// </summary>
    public class UtopiaService
    {
        #region Channel

        #region 用户
 
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="user"></param>
        public void RegisterUser(Uto_User user)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                db.Uto_User.AddObject(user);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public void UpdateUser(Uto_User user)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_User userDb = db.Uto_User.Single(n => n.UserId == user.UserId);
                userDb.Username = user.Username;
                userDb.Password = user.Password;
                userDb.Sex = user.Sex;
                userDb.IsActivity = user.IsActivity;
                db.SaveChanges();
            }
        }

        public Uto_User GetUserByEmail(string email)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_User user = db.Uto_User.SingleOrDefault<Uto_User>(n => n.Email == email);
                return user;
            }
        }

        public Uto_User GetUserById(long userId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_User user = db.Uto_User.SingleOrDefault<Uto_User>(n => n.UserId == userId);
                return user;
            }
        }

        public List<Uto_User> GetUsersByUserName(string userName)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                List<Uto_User> users = db.Uto_User.Where<Uto_User>(n => n.Username.Contains(userName)).ToList();
                return users;
            }
        }

        #endregion

        #region 吐槽

        public void PublishMicroBlog(Uto_Microblog microblog)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                db.Uto_Microblog.AddObject(microblog);
                db.SaveChanges();
            }
        }

        public void UpdateMicroBlog(Uto_Microblog microblog)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_Microblog microblogDb = db.Uto_Microblog.Single(n => n.MicroblogId == microblog.MicroblogId);
                microblogDb.AttachmentId = microblog.AttachmentId;
                microblogDb.Body = microblog.Body;
                microblogDb.DateCreated = microblog.DateCreated;
                microblogDb.RepeatId = microblog.RepeatId;
                microblogDb.RepeatContent = microblog.RepeatContent;
                microblogDb.UserId = microblog.UserId;
                microblogDb.CommentCount = microblog.CommentCount;
                db.SaveChanges();
            }
        }

        public int DeleteMicroBlog(long microblogId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_Microblog microblog = db.Uto_Microblog.Single(n => n.MicroblogId == microblogId);
                if (UserContext.CurrentUser.UserId == microblog.UserId && microblog != null)
                {
                    db.Uto_Microblog.DeleteObject(microblog);
                    //删除旗下的图片附件记录及附件本身
                    IEnumerable<Uto_Attachment> attachments = db.Uto_Attachment.Where(n => n.BelongId == microblogId);
                    foreach (var attachment in attachments)
                    {
                        db.Uto_Attachment.DeleteObject(attachment);
                        string path = HttpContext.Current.Server.MapPath(attachment.Url.Replace("../../", "~/"));
                        File.Delete(path);
                    }
                    //删除旗下所有微博回复
                    IEnumerable<Uto_Comment> comments = db.Uto_Comment.Where(n => n.BelongId == microblogId && n.Type == "microblogComment");
                    foreach (var comment in comments)
                    {
                        db.Uto_Comment.DeleteObject(comment);
                    }
                    //删除旗下所有微博消息（@用户、回复）
                    IEnumerable<Uto_Message> messages = db.Uto_Message.Where(n => n.BelongId == microblogId && (n.Type == "microblogAtUser" || n.Type == "microblogComment"));
                    foreach (var message in messages)
                    {
                        db.Uto_Message.DeleteObject(message);
                    }
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int GetTotalUserMicroBlogCount(long userId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                return db.Uto_Microblog.Where(n => n.UserId == userId).Count();
            }
        }

        public List<Uto_Microblog> GetUserMicroBlog(long userId, int pageIndex = 1, int pageSize = 10)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                string sql = @"select top " + pageSize.ToString() + " * from uto_microblog where microblogId not in (select top ((" + pageIndex.ToString() + "-1)*" + pageSize.ToString() + ") microblogId from uto_microblog where userid="+userId.ToString()+" order by microblogId desc) and userId=" + userId.ToString() + " order by microblogId desc";
                List<Uto_Microblog> microblogs = db.ExecuteStoreQuery<Uto_Microblog>(sql).ToList();
                //解析吐槽Body及吐槽repeatContent
                microblogs = AnalyzeBody(microblogs);
                return microblogs;
            }
        }

        /// <summary>
        /// 解析吐槽Body
        /// </summary>
        public List<Uto_Microblog> AnalyzeBody(List<Uto_Microblog> microblogs)
        {
            for (int i = 0; i < microblogs.Count; i++)
            {
                microblogs.ElementAt(i).Body = Utility.AnalyzeBody(microblogs.ElementAt(i).Body);
                if (microblogs.ElementAt(i).RepeatId > 0)
                {
                    microblogs.ElementAt(i).RepeatContent = Utility.AnalyzeBody(microblogs.ElementAt(i).RepeatContent);
                }
            }
            return microblogs;
        }

        public Uto_Microblog GetMicroBlogById(long microblogId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                return db.Uto_Microblog.Where(n => n.MicroblogId == microblogId).First();
            }
        }

        public List<Uto_Microblog> GetMicroBlogIndex(int pageIndex = 1, int pageSize = 1000)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                string sql = @"select top " + pageSize.ToString() + " * from uto_microblog where microblogId not in (select top ((" + pageIndex.ToString() + "-1)*" + pageSize.ToString() + ") microblogId from uto_microblog order by microblogId desc) order by microblogId desc";
                List<Uto_Microblog> microblogs = db.ExecuteStoreQuery<Uto_Microblog>(sql).ToList();

                return microblogs;
            }
        }

        #endregion

        #region 附件

        public void AddAttachment(Uto_Attachment attachment)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                attachment.DateCreated = DateTime.Now;
                db.Uto_Attachment.AddObject(attachment);
                db.SaveChanges();
            }
        }

        public void UpdateAttachment(Uto_Attachment attachment)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_Attachment attachmentDb = db.Uto_Attachment.Single(n => n.AttachmentId == attachment.AttachmentId);
                attachmentDb.BelongId = attachment.BelongId;
                attachmentDb.DateCreated = DateTime.Now;
                attachmentDb.Url = attachment.Url;
                attachmentDb.UserId = attachment.UserId;
                attachmentDb.Type = attachment.Type;
                attachmentDb.Width = attachment.Width;
                attachmentDb.Height = attachment.Height;

                db.SaveChanges();
            }
        }

        public Uto_Attachment GetAttachmentById(long attachmentId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_Attachment attachmentDb = db.Uto_Attachment.Single(n => n.AttachmentId == attachmentId);
                return attachmentDb;
            }
        }

        #endregion

        #region 消息

        public void SendMessage(Uto_Message message)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                db.Uto_Message.AddObject(message);
                db.SaveChanges();
            }
        }

        public List<Uto_Message> GetMessageByUserId(long userId,string type)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                return db.Uto_Message.Where(n => n.UserId == userId&&n.IsIgnore==false&&n.Type==type).OrderByDescending(n=>n.MessageId).Take(10).ToList();
            }
        }

        public bool IsHasNewMessage(long newMessageId,long userId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                return db.Uto_Message.Where(n=>n.UserId==userId&&n.IsIgnore==false).Any(n => n.MessageId > newMessageId);
            }
        }

        public void DeleteMessage(long? messageId,long? belongId,long? childrenBelongId,string type)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                if (messageId.HasValue)
                {
                    Uto_Message message = db.Uto_Message.First(n=>n.MessageId==messageId.Value&&n.Type==type);
                    db.Uto_Message.DeleteObject(message);
                }
                if (belongId.HasValue)
                {
                    IEnumerable<Uto_Message> messages = db.Uto_Message.Where(n => n.BelongId == belongId.Value && n.Type == type);
                    foreach (var message in messages)
                    {
                        db.Uto_Message.DeleteObject(message);   
                    }
                }
                if (childrenBelongId.HasValue)
                {
                    Uto_Message message = db.Uto_Message.First(n => n.ChildrenBelongId == childrenBelongId.Value && n.Type == type);
                    db.Uto_Message.DeleteObject(message);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region 评论

        public List<Uto_Comment> GetCommentByBelongId(long belongId,string type,out long commentCount,bool all=false,int pageSize=10,int pageIndex=1)
        {

            using (UtopiaEntities db = new UtopiaEntities())
            {
                List<Uto_Comment> comments = db.Uto_Comment.Where(n => n.BelongId == belongId && n.Type == type).OrderBy(n=>n.CommentId).ToList();
                commentCount = comments.Count;
                if (!all)
                {
                    comments = comments.Take(10).ToList();
                }
                else
                {
                    string sql = @"select top " + pageSize.ToString() + " * from uto_comment where commentId not in (select top ((" + pageIndex.ToString() + "-1)*" + pageSize.ToString() + ") commentId from uto_comment where belongid=" + belongId.ToString() + " and type='"+type+"' order by commentId desc) and belongid=" + belongId.ToString() + " and type='"+type+"' order by commentId desc";
                    comments = db.ExecuteStoreQuery<Uto_Comment>(sql).ToList();
                }
                //解析评论Body
                for (int i = 0; i < comments.Count; i++)
                {
                    comments.ElementAt(i).Body = Utility.AnalyzeBody(comments.ElementAt(i).Body);
                }
                return comments;
            }
        }

        public Uto_Comment GetCommentById(long commentId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                return db.Uto_Comment.Where(n => n.CommentId == commentId).First();
            }
        }

        public void AddComment(Uto_Comment comment)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                db.Uto_Comment.AddObject(comment);
                db.SaveChanges();
            }
        }

        public void DeleteComment(long commentId)
        {
            using (UtopiaEntities db = new UtopiaEntities())
            {
                Uto_Comment comment = db.Uto_Comment.First(n => n.CommentId == commentId);
                db.Uto_Comment.DeleteObject(comment);
                db.SaveChanges();
            }
        }

        #endregion

        #endregion
    }
}