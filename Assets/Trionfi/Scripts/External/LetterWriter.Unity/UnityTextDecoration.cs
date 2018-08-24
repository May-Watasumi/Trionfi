using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LetterWriter.Unity
{
    [Flags]
    public enum UnityTextDecoration
    {
        /// <summary>
        /// 装飾なし
        /// </summary>
        None = 0,

        /// <summary>
        /// 下線
        /// </summary>
        Underline = 1 << 1,

        /// <summary>
        /// 打消し線
        /// </summary>
        LineThrough = 1 << 2,

        /// <summary>
        /// 上線
        /// </summary>
        Overline = 1 << 3,
    }
}
