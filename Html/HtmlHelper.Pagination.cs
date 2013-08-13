using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Mvc;

namespace Utopia
{
    public static class HtmlHelperPaginationExtensions
    {
        /// <summary>
        /// 分页控件
        /// </summary>
        public static MvcHtmlString Pagination(this HtmlHelper htmlHelper, string actionName, string controllerName, int totalRecord, int pageSize, string size = "pagination-large", string align = "pagination-centered", string ajaxTarget = "",List<string> paramNames=null,List<object> paramValues=null)
        {
            //actionName跟controllerName
            htmlHelper.ViewData["actionName"] = actionName;
            htmlHelper.ViewData["controllerName"] = controllerName;
            //大小跟对齐
            htmlHelper.ViewData["paginationLarge"] = size;
            htmlHelper.ViewData["paginationCentered"] = align;
            //是否ajax分页（替换目标ID）
            htmlHelper.ViewData["ajaxTarget"] = ajaxTarget;
            //总记录数、每页记录数、有几页
            htmlHelper.ViewData["totalRecord"] = totalRecord;
            htmlHelper.ViewData["pageSize"] = pageSize;
            int pageCount = totalRecord % pageSize == 0 ? totalRecord / pageSize : totalRecord / pageSize + 1;
            htmlHelper.ViewData["pageCount"] = pageCount;
            //其他参数
            Dictionary<string,object> paramDic=new Dictionary<string,object>();
            for (int i = 0; i < paramNames.Count; i++)
			{
                paramDic.Add(paramNames.ElementAt<string>(i),paramValues.ElementAt(i));
			}
            htmlHelper.ViewData["param-Name-Value"] = paramDic;

            return htmlHelper.DisplayForModel("Pagination");
        }
    }
}