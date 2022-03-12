using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.Helpers
{
    class WidthFit
    {
        /// <summary>
        /// 获取自适应列表项宽度
        /// </summary>
        /// <param name="width">当前窗口宽度</param>
        /// <param name="max">列表项最大宽度</param>
        /// <param name="min">列表项最小宽度</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static double GetWidth(double width, int max, int min, int offset = 8)
        {
            if (offset < 0 || offset > 12)
            {
                offset = 8;
            }
            double w = 1;
            int column = 1;
            int maxcolumn = (int)width / min;
            double i2 = width / min;
            for (int i = 1; i <= maxcolumn; i++)
            {
                if (Math.Abs(i - i2) < 1)
                {
                    column = (int)Math.Truncate(i2) == 0 ? 1 : (int)Math.Truncate(i2);
                }
            }
            w = width / column;
            w -= offset * column;
            return w;
        }
    }
}
