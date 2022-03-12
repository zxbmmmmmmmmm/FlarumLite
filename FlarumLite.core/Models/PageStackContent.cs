using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.core.Models
{
    public class PageStackContent
    {
        public PageStackContent()
        {
        }
        public PageStackContent(object pageContent, object pageParameter = null)
        {
            this.PageContent = pageContent;
            this.PageParameter = pageParameter;
        }

        public object PageContent { get; set; }
        public object PageParameter { get; set; }
    }
}
