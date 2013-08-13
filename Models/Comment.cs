using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utopia.Service;

namespace Utopia
{
    public static class Uto_CommentExtensions
    {
        public static UtopiaService utopiaService = new UtopiaService();

        /// <summary>
        /// 获取评论人
        /// </summary>
        public static Uto_User SendUser(this Uto_Comment comment)
        {
            return utopiaService.GetUserById(comment.SendUserId);
        }
    }
}