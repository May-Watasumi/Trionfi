using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityTextModifier : TextModifier
    {
        public Color? RubyColor { get; set; }
        public float? RubyFontScale { get; set; }
        public FontStyle? RubyFontStyle { get; set; }

        public int? FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public Color? Color { get; set; }
        public UnityTextDecoration? TextDecoration { get; set; }
    }
}
