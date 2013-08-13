using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utopia.Models
{
    /// <summary>
    /// 消息实体
    /// </summary>
    public class Message
    {
        Utopia.Service.UtopiaService utopiaService = new Service.UtopiaService();

        public static Message New()
        {
            return new Message
            {
                DateCreated = DateTime.Now,
                SendUserId = UserContext.CurrentUser.UserId
            };
        }

        #region 持久化数据

        public long MessageId { get; set; }

        /// <summary>
        /// 消息类型(是@用户、回复、有人买了你的东西、有人订了你的饭等)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否忽略 (是1 否0)
        /// </summary>
        public bool IsIgnore { get; set; }

        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 属于哪个项的ID（如，微博ID，商品ID，午餐ID等）
        /// </summary>
        public long BelongId { get; set; }

        /// <summary>
        /// 子属ID 比如这条消息是由微博下的一条回复产生的，则这个ID就是这条回复的ID
        /// </summary>
        public long ChildrenBelongId { get; set; }

        /// <summary>
        /// 接受人ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 发送人ID
        /// </summary>
        public long SendUserId { get; set; }

        #endregion

        #region 导航属性

        /// <summary>
        /// 接受人
        /// </summary>
        public Uto_User User
        {
            get
            {
                return utopiaService.GetUserById(this.UserId);
            }
        }

        /// <summary>
        /// 发送人
        /// </summary>
        public Uto_User SendUser
        {
            get
            {
                return utopiaService.GetUserById(this.SendUserId);
            }
        }

        /// <summary>
        /// 产生该信息的项的实体（如，微博，商品，午餐等）
        /// </summary>
        public object BelongItem
        {
            get
            {
                object item = null;
                switch (this.Type)
                {
                    case "microblogAtUser":
                        item = utopiaService.GetMicroBlogById(this.BelongId);
                        break;
                    case "microblogComment":
                        item = utopiaService.GetMicroBlogById(this.BelongId);
                        break;
                    default:
                        break;
                }
                return item;
            }
        }

        #endregion

        public Uto_Message AsDbMessage()
        {
            return new Uto_Message
            {
                Type = this.Type,
                IsIgnore = this.IsIgnore,
                DateCreated = this.DateCreated,
                BelongId = this.BelongId,
                ChildrenBelongId=this.ChildrenBelongId,
                UserId = this.UserId,
                SendUserId = this.SendUserId
            };
        }
    }

    public static class Uto_MessageExtexse
    {
        public static Message AsMessage(this Uto_Message DbMessage)
        {
            return new Message
            {
                MessageId = DbMessage.MessageId,
                Type = DbMessage.Type,
                IsIgnore = DbMessage.IsIgnore,
                DateCreated = DbMessage.DateCreated,
                BelongId = DbMessage.BelongId,
                ChildrenBelongId=DbMessage.ChildrenBelongId,
                UserId = DbMessage.UserId,
                SendUserId = DbMessage.SendUserId
            };
        }
    }
}