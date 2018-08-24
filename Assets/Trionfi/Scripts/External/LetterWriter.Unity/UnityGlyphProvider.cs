using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Unity.Components;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityGlyphProvider : GlyphProvider<UnityTextModifierScope>
    {
        private Dictionary<char, UnityGlyph> _recentGlyphCache = new Dictionary<char, UnityGlyph>();
        private TextGenerator _cachedTextGenerator;

        public Font Font { get; set; }

        public UnityGlyphProvider(Font font)
        {
            this.Font = font;
            this._cachedTextGenerator = new TextGenerator();
        }

        protected override void GetGlyphsFromStringCore(UnityTextModifierScope textModifierScope, string value, IList<IGlyph> buffer)
        {
            var fontSize = textModifierScope.FontSize ?? 24;
            var fontStyle = textModifierScope.FontStyle ?? FontStyle.Normal;
            var color = textModifierScope.Color;
            var textDecoration = textModifierScope.TextDecoration ?? UnityTextDecoration.None;

            if (!this._cachedTextGenerator.Populate(value + "…M", new TextGenerationSettings() { font = this.Font, fontSize = fontSize, fontStyle = fontStyle }))
            {
                throw new Exception("TextGenerator.Populate failed");
            }

            this.Font.RequestCharactersInTexture(value, fontSize, fontStyle);

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (Char.IsControl(c)) continue;

                CharacterInfo characterInfo;
                if (!this.Font.GetCharacterInfo(c, out characterInfo, fontSize, fontStyle))
                {
                    throw new Exception("this.Font.GetCharacterInfo failed: " + c);
                }

                UnityGlyph glyph;
                if (this._recentGlyphCache.TryGetValue(c, out glyph))
                {
                    if ((glyph.Color.HasValue != color.HasValue) ||
                        (glyph.Color.HasValue && color.HasValue && ((Vector4)color.Value != (Vector4)color.Value)) ||
                        glyph.Height != fontSize ||
                        glyph.AdvanceWidth != characterInfo.advance ||
                        glyph.TextDecoration != textDecoration)
                    {
                        glyph = null;
                    }
                }

                if (glyph == null)
                {
                    glyph = new UnityGlyph(c, characterInfo, color, fontSize, textDecoration);
                    this._recentGlyphCache[c] = glyph;
                }

                buffer.Add((IGlyph)glyph);
            }
        }
    }

}
