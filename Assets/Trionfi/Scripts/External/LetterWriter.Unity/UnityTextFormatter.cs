using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Unity.Components;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityTextFormatter : LetterWriter.TextFormatter
    {
        public Font Font { get; private set; }
        public int FontSize { get; private set; }
        public Color Color { get; private set; }

        public UnityTextFormatter(Font font, int fontSize, Color color)
        {
            this.Font = font;
            this.FontSize = fontSize;
            this.Color = color;

            this.Initialize();
        }

        public override GlyphProvider CreateGlyphProvider()
        {
            return new UnityGlyphProvider(this.Font);
        }

        public override TextModifierScope CreateTextModifierScope(TextModifierScope parent, TextModifier textModifier)
        {
            return new UnityTextModifierScope((UnityTextModifierScope)parent, (UnityTextModifier)(textModifier ?? this.CreateDefaultTextModifier()));
        }

        public virtual TextModifier CreateDefaultTextModifier()
        {
            return new UnityTextModifier
            {
                FontSize = this.FontSize,
                FontStyle = FontStyle.Normal,
                Spacing = 0,
                RubyFontScale = 0.5f,
                Color = this.Color,
                TextDecoration = UnityTextDecoration.None,
            };
        }
    }
}
