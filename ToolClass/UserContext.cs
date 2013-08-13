using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utopia.Service;

namespace Utopia
{
    public class UserContext
    {
        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        public static Uto_User CurrentUser
        {
            get
            {
                HttpContext context = HttpContext.Current;
                HttpCookie cookie = context.Request.Cookies["authCookie"];
                if (cookie != null)
                {
                    UtopiaService utopiaService = new UtopiaService();
                    int randomNum = int.Parse(cookie["authAdd"]);
                    long userId = long.Parse(Utility.DecodeCookie(cookie["authCookie"], randomNum));
                    if (utopiaService.GetUserById(userId)==null)
                    {
                        return null;
                    }
                    return utopiaService.GetUserById(userId);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 记录当前用户到Cookie
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public static void AddUserToCookie(string email,string password,bool rememberme)
        {
            UtopiaService utopiaService=new UtopiaService();
            Uto_User user=utopiaService.GetUserByEmail(email);

            HttpCookie cookie=new HttpCookie("authCookie");
            int randomNum = new Random().Next(100);
            cookie.Values["authAdd"] = randomNum.ToString();
            cookie.Values["authCookie"] = Utility.EncodeCookie(user.UserId,randomNum);

            if (rememberme)
                cookie.Expires = DateTime.Now.AddDays(7);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 清楚当前用户的Cookie
        /// </summary>
        public static void ClearUserFromCookie()
        {
            HttpCookie cookie =  HttpContext.Current.Request.Cookies["authCookie"];
            cookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
    }
}