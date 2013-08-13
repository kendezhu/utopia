using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
using System.Net.Mail;
using System.Web.Routing;
using System.Web.Mvc;
using Utopia.Service;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace Utopia
{
    public static class Utility
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }

        /// <summary>
        /// 加密Cookie
        /// </summary>
        public static string EncodeCookie(long userId, long randomNum)
        {
            long final = (userId * 100 + 100) * 5 - 2 + 3 + randomNum;
            byte[] finalByteArray = Encoding.GetEncoding("UTF-8").GetBytes(final.ToString() + ",床前明月光,疑似地上霜,举头望明月,低头思故乡。");
            string finalStr = Convert.ToBase64String(finalByteArray);
            return finalStr;
        }

        /// <summary>
        /// 解密Cookie
        /// </summary>
        public static string DecodeCookie(string str, long randomNum)
        {
            byte[] finalByteArray = Convert.FromBase64String(str);
            string finalStr = Encoding.GetEncoding("UTF-8").GetString(finalByteArray).Split(',')[0];
            long final = long.Parse(finalStr);
            final = ((final - randomNum - 3 + 2) / 5 - 100) / 100;
            return final.ToString();
        }

        /// <summary>
        /// 向某用户发激活邮件
        /// </summary>
        /// <param name="user"></param>
        public static void SendMail(Uto_User user)
        {
            ////发邮件
            try
            {
                //构建邮件对象
                MailMessage mail = new MailMessage();
                //设置发件人
                mail.From = new MailAddress("kendezhu126@126.com");
                //设置收件人
                mail.To.Add(new MailAddress(user.Email));
                mail.SubjectEncoding = mail.BodyEncoding = System.Text.Encoding.UTF8;//设置编码
                mail.Subject = "你已成功注册乌托邦，请点击链接激活帐号";
                mail.Body = "你好，" + user.Username + "，你已成功在乌托邦注册，你的密码是" + user.Password + "，请妥善保管，点击链接以便激活帐号：<a target='_blank' href=" + "'http://localhost" + UrlHelper.Instance().ChannelActivity() + "?userId=" + user.UserId + "&activityCode=" + user.ActivityCode + "'>我要激活</a>";
                //简单邮件传输协议
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 25;
                smtp.Host = "smtp.126.com";
                smtp.Credentials = new System.Net.NetworkCredential("kendezhu126", "13730942229");
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// 获取当前访问地址中的UserId，从而获取该用户
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static Uto_User UrlUser(this System.Web.Mvc.UrlHelper Url)
        {
            //获取当前访问地址中的UserId，从而获取该用户
            object resultValue = null;
            Url.RequestContext.RouteData.Values.TryGetValue("userId", out resultValue);
            if (resultValue == null)
            {
                try
                {
                    resultValue = HttpContext.Current.Request.QueryString["urlUserId"].ToString();
                }
                catch (Exception)
                {
                    resultValue = UserContext.CurrentUser.UserId;
                }
               
            }
            return new UtopiaService().GetUserById(long.Parse(resultValue.ToString()));
        }

        /// <summary>
        /// 友好的时间格式
        /// </summary>
        public static string FriendlyTime(this DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
                if (span.TotalDays > 30)
                {
                    return "1个月前(" + dt.ToShortDateString() + ")";
                }
                else
                    if (span.TotalDays > 14)
                    {
                        return "2周前(" + dt.ToShortDateString() + ")";
                    }
                    else
                        if (span.TotalDays > 7)
                        {
                            return "1周前(" + dt.ToShortDateString() + ")";
                        }
                        else
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format("{0}天前(" + dt.ToShortDateString() + ")", (int)Math.Floor(span.TotalDays));
                            }
                            else
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return "1秒前";
                                        }
        }

        /// <summary>
        /// 获取某路径下的所有文件名
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFileNameOfDirectory(string path)
        {
            List<string> fileNameList = new List<string>();
            string[] fileNames = Directory.GetFiles(path);
            foreach (var fileName in fileNames)
            {
                fileNameList.Add(GetFileName(fileName));
            }
            return fileNameList;
        }

        /// <summary>
        /// 获取文件名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            if (path.Contains("\\"))
            {
                string[] arr = path.Split('\\');
                return arr[arr.Length - 1];
            }
            else
            {
                string[] arr = path.Split('/');
                return arr[arr.Length - 1];
            }
        }

        /// <summary>
        /// 解析Body
        /// </summary>
        /// <returns></returns>
        public static string AnalyzeBody(string body)
        {
            //将[字母或数字]的表情字符串替换为图片
            MatchCollection emotionItems = Regex.Matches(body, @"\[[a-z0-9]*\]");
            foreach (Match item in emotionItems)
            {
                body = body.Replace(item.Value, "<img src='../../Content/Emotion/" + item.Value.TrimStart('[').TrimEnd(']') + ".gif'/>");
            }

            //将网址替换成链接
            MatchCollection urlItems = Regex.Matches(body, @"[a-zA-z]+://[^\s]*[a-zA-z0-9//]");
            foreach (Match item in urlItems)
            {
                body = body.Replace(item.Value, "<a target='_blank' href='" + item.Value + "'>" + item.Value + "</a>");
            }

            //将@用户替换成链接
            MatchCollection atUserItems = Regex.Matches(body, @"\@.*?\)");
            foreach (Match item in atUserItems)
            {
                string[] splitAtUsers = item.Value.Split(new char[] { '(', ')' });
                body = body.Replace(item.Value, "<a target='_blank' href='" + UrlHelper.Instance().ChannelHome() + "/" + splitAtUsers[1] + "'>" + splitAtUsers[0] + "</a>");
            }

            //将 转自**; 替换成链接
            MatchCollection repeatUserItems = Regex.Matches(body, @"转自.*?\;");
            foreach (Match item in repeatUserItems)
            {
                string[] splitAtUsers = item.Value.Split(new char[] { '(', ')' });
                body = body.Replace(item.Value, "转自<a target='_blank' href='" + UrlHelper.Instance().ChannelHome() + "/" + splitAtUsers[1] + "'>" + splitAtUsers[0].TrimStart('转','自') + "</a>&nbsp;");
            }

            return body;
        }

        /// <summary>
        /// 解析出其中@用户的userId
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static List<string> atUserIds(string body)
        {
            List<string> userIds = new List<string>();
            MatchCollection atUserItems = Regex.Matches(body, @"\@.*?\)");
            foreach (Match item in atUserItems)
            {
                string[] splitAtUsers = item.Value.Split(new char[] { '(', ')' });
                userIds.Add(splitAtUsers[1]);
            }
            return userIds;
        }

        #region 保存图片

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="AttachmentType">附件类型</param>
        public static void SavePic(HttpPostedFileBase file, string attachmentType, out string attachmentUrl, out long attachmentId)
        {
            string extenseName = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            if (!CheckExtensionName(extenseName))
            {
                attachmentUrl = "0";
                attachmentId = 0;
                return;
            }

            UtopiaService utopiaService = new UtopiaService();

            Uto_Attachment attachment = new Uto_Attachment();
            attachment.UserId = UserContext.CurrentUser.UserId;
            attachment.BelongId = 0;
            attachment.Type = attachmentType;
            attachment.Url = string.Empty;

            //创建附件记录
            utopiaService.AddAttachment(attachment);

            //每100张图片在一个目录
            long i = 100;
            while (true)
            {
                if (attachment.AttachmentId < i)
                    break;
                else
                    i = i + 100;
            }

            string path = HttpContext.Current.Server.MapPath("~/Content/Attachment/" + attachmentType + "/" + i.ToString() + "/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //获取上传图片的宽与高
            System.Drawing.Image img = System.Drawing.Image.FromStream(file.InputStream);
            attachment.Width = img.Width;
            attachment.Height = img.Height;
            //更新附件URL
            attachment.Url = "../../Content/Attachment/" + attachmentType + "/" + i.ToString() + "/" + attachment.AttachmentId.ToString() + extenseName;
            utopiaService.UpdateAttachment(attachment);

            //设置 attachmentUrl attachmentId
            attachmentUrl = attachment.Url;
            attachmentId = attachment.AttachmentId;

            file.SaveAs(path + attachment.AttachmentId.ToString() + extenseName);
        }

        public static bool CheckExtensionName(string extenseName)
        {
            if (extenseName == ".jpg" || extenseName == ".JPG" || extenseName == ".jpeg" || extenseName == ".JPEG" || extenseName == ".gif" || extenseName == ".GIF" || extenseName == ".png" || extenseName == ".PNG" || extenseName == ".gif" || extenseName == ".GIF")
            {
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 截字符串
        /// </summary>
        public static string GetStr(string s, int l)
        {
            string temp = s;
            if (Regex.Replace(temp, "[\u4e00-\u9fa5]", "zz", RegexOptions.IgnoreCase).Length <= l)
            {
                return temp;
            }
            for (int i = temp.Length; i >= 0; i--)
            {
                temp = temp.Substring(0, i);
                if (Regex.Replace(temp, "[\u4e00-\u9fa5]", "zz", RegexOptions.IgnoreCase).Length <= l - 3)
                {
                    return temp + "...";
                }
            }
            return "...";
        }
    }
}