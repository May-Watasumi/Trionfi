using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityTextModifierScope : TextModifierScope<UnityTextModifier>, IRubyTextModifierScope
    {
        protected new UnityTextModifierScope Parent { get { return (UnityTextModifierScope)base.Parent; } }

        public Color? RubyColor
        {
            get { return this.TextModifier.RubyColor ?? ((this.Parent != null) ? this.Parent.RubyColor : null); }
            set { this.TextModifier.RubyColor = value; }
        }
        public FontStyle? RubyFontStyle
        {
            get { return this.TextModifier.RubyFontStyle ?? ((this.Parent != null) ? this.Parent.RubyFontStyle : null); }
            set { this.TextModifier.RubyFontStyle = value; }
        }
        public float? RubyFontScale
        {
            get { return this.TextModifier.RubyFontScale ?? ((this.Parent != null) ? this.Parent.RubyFontScale : null); }
            set { this.TextModifier.RubyFontScale = value; }
        }
        public int? FontSize
        {
            get { return this.TextModifier.FontSize ?? ((this.Parent != null) ? this.Parent.FontSize : null); }
            set { this.TextModifier.FontSize = value; }
        }
        public FontStyle? FontStyle
        {
            get { return this.TextModifier.FontStyle ?? ((this.Parent != null) ? this.Parent.FontStyle : null); }
            set { this.TextModifier.FontStyle = value; }
        }
        public Color? Color
        {
            get { return this.TextModifier.Color ?? ((this.Parent != null) ? this.Parent.Color : null); }
            set { this.TextModifier.Color = value; }
        }
        public UnityTextDecoration? TextDecoration
        {
            get
            {
                if (this.Parent != null)
                {
                    return (this.TextModifier.TextDecoration ?? UnityTextDecoration.None) | this.Parent.TextDecoration;
                }
                else
                {
                    return this.TextModifier.TextDecoration;
                }
            }
            set { this.TextModifier.TextDecoration = value; }
        }
        public UnityTextModifierScope(TextModifierScope<UnityTextModifier> parent, UnityTextModifier textModifier) : base(parent, textModifier)
        {
        }

        public override void Apply(UnityTextModifier textModifier)
        {
            base.Apply(textModifier);

            this.RubyColor = textModifier.RubyColor;
            this.RubyFontScale = textModifier.RubyFontScale;
            this.RubyFontStyle = textModifier.RubyFontStyle;
            this.FontSize = textModifier.FontSize;
            this.FontStyle = textModifier.FontStyle;
            this.Color = textModifier.Color;
            this.TextDecoration = textModifier.TextDecoration;
        }

        public TextModifierScope RubyScope
        {
            get
            {
                return new UnityTextModifierScope(this, new UnityTextModifier() { Color = this.RubyColor, FontSize = (int)((this.FontSize ?? 1) * (this.RubyFontScale ?? 1)), FontStyle = this.RubyFontStyle });
            }
        }
    }

}
