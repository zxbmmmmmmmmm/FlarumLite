using Html2Markdown;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlarumLite.Helpers
{
    public class CSStoMarkdown
    {
        public static string HTMLtoMarkdown(string text)
        {
            if(text != null)
            {
                text = text.FixHtml();//去除scripts
            }
            else
            {
                return text;
            }
            try
            {
                text = text.Replace("<div>", "");
                text = text.Replace("</div>", "");
                text = text.Replace("<sub>", "");
                text = text.Replace("</sub>", "");
                Converter converter = new Converter();
                var converted = converter.Convert(text);
                return converted;
            }
            catch
            {
               // text = RemoveBetween(text, "\" title=\"\" alt=\"", "\">");
                text = RemoveBetween(text, "alt=\"", "\">");
                text = RemoveBetween(text, "title=\"", "\">");

                Regex h1 = new Regex(@"<h1.*?>", RegexOptions.IgnoreCase);
                Regex h2 = new Regex(@"<h2.*?>", RegexOptions.IgnoreCase);
                Regex h3 = new Regex(@"<h3.*?>", RegexOptions.IgnoreCase);
                Regex h4 = new Regex(@"<h4.*?>\n", RegexOptions.IgnoreCase);
                Regex div = new Regex(@"<div.*?>", RegexOptions.IgnoreCase);
                Regex p = new Regex(@"<p.*?>", RegexOptions.IgnoreCase);
                Regex ul = new Regex(@"<ul.*?>", RegexOptions.IgnoreCase);
                Regex li = new Regex(@"<li.*?>", RegexOptions.IgnoreCase);
                Regex span = new Regex(@"<span.*?>", RegexOptions.IgnoreCase);

                text = text.Replace("</h1>", "");
                text = text.Replace("</h2>", "");
                text = text.Replace("</h3>", "");
                text = text.Replace("</h4>", "");
                text = text.Replace("</div>", "");
                text = text.Replace("<p>", "");
                text = text.Replace("</p>", "");
                text = text.Replace("</ul>", "");
                text = text.Replace("</li>", "");
                text = text.Replace("</span>", "**");
                text = text.Replace("</strong>", "**");

                text = h1.Replace(text, "#");
                text = h2.Replace(text, "##");
                text = h3.Replace(text, "###");
                text = h4.Replace(text, "####");
                text = text.Replace("<blockquote class=\"uncited\">", "> ");
                text = text.Replace("</blockquote>", "");


                text = text.Replace("<br>", "  \n");//换行
                text = text.Replace("<br/>", "  \n");
                text = text.Replace("<br />", "  \n");
                text = text.Replace("<hr>", "\n *** \n");//分割线
                text = text.Replace("<del>", "~~");//删除线
                text = text.Replace("</del>", "~~");
                text = text.Replace("<em>", "_");//斜体
                text = text.Replace("</em>", "_");
                text = text.Replace("<em>", "_");//斜体
                text = text.Replace("</em>", "_");
                text = text.Replace("<code>", "`");//代码
                text = text.Replace("</code>", "`");
                text = text.Replace("<img src=\"", "![](");//图片
                text = text.Replace("\" title=\"\" alt=\"\">", ")");
                text = text.Replace("\" alt=\"\">", ")");
                text = text.Replace("\" title=\"\">", ")");



                text = text.Replace("<input data-task-id=\"61476adea62b3\" type=\"checkbox\" disabled>", "- [ ] ");//任务列表-未完成
                text = text.Replace("<input data-task-id=\"61476adea62ff\" type=\"checkbox\" checked disabled>", "- [x] ");//任务列表-已完成

                text = text.Replace("<script async=\"\" crossorigin=\"anonymous\" data-hljs-style=\"github\" integrity=\"sha384 - oTqfbnKDrROJYNQZI1U//Vr36HEjwJafOewSUYYyb5OXhv0r2qRQcjAP3yXa4HCg\" onload=\"hljsLoader.highlightBlocks(this.parentNode)\" src=\"https://cdn.jsdelivr.net/gh/s9e/hljs-loader@1.0.24/loader.min.js\"></script></pre>", "");//插件


                text = div.Replace(text, "");
                text = p.Replace(text, "");
                text = ul.Replace(text, "");
                text = li.Replace(text, " - ");
                text = span.Replace(text, "**");
                text = text.Replace("<strong>", "**");


                for (int i = 0; i < 20; i++) { text = text.Replace("(" + i.ToString() + ") ", " 1. "); }

                return text;
            }
        }
        public static string RemoveBetween(string sourceString, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            return regex.Replace(sourceString, startTag + endTag);
        }
    }
}
