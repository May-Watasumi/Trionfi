using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Markup;
using LetterWriter.Unity.Components;
using UnityEngine;

namespace LetterWriter.Unity.Markup
{
    public class UnityMarkupParser : LetterWriter.Markup.LetterWriterMarkupParser
    {
        private Dictionary<string, Color> _colorTable = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            {"red", new Color(1f, 0.0f, 0.0f, 1f)},
            {"green", new Color(0.0f, 1f, 0.0f, 1f)},
            {"blue", new Color(0.0f, 0.0f, 1f, 1f)},
            {"white", new Color(1f, 1f, 1f, 1f)},
            {"black", new Color(0.0f, 0.0f, 0.0f, 1f)},
            {"yellow", new Color(1f, 0.9215686f, 0.01568628f, 1f)},
            {"cyan", new Color(0.0f, 1f, 1f, 1f)},
            {"magenta", new Color(1f, 0.0f, 1f, 1f)},
            {"gray", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"grey", new Color(0.5f, 0.5f, 0.5f, 1f)},
            {"clear", new Color(0.0f, 0.0f, 0.0f, 0.0f)},
        };

        private bool TryConvertToColor(string value, out Color result)
        {
            if (value.StartsWith("#") && value.Length == 4 || value.Length == 9 || value.Length == 7)
            {
                // Web-color code (#rrggbb, #rgb, #rrggbbaa)

                if (value.Length == 4)
                {
                    // #abc style -> #aabbcc
                    value = String.Format("#{0}{0}{1}{1}{2}{2}", value.Substring(1, 1), value.Substring(2, 1),
                        value.Substring(3, 1));
                }

                if (value.Length == 9)
                {
                    // #rrggbbaa
                    var color = UInt32.Parse(value.Substring(1), System.Globalization.NumberStyles.HexNumber);
                    var r = (((color >> 24) & 0xff)/255.0f);
                    var g = (((color >> 16) & 0xff)/255.0f);
                    var b = (((color >> 8) & 0xff)/255.0f);
                    var a = (((color >> 0) & 0xff)/255.0f);

                    result = new Color(r, g, b, a);
                    return true;
                }
                else
                {
                    // #rrggbb
                    var color = UInt32.Parse(value.Substring(1), System.Globalization.NumberStyles.HexNumber);
                    var r = (((color >> 16) & 0xff)/255.0f);
                    var g = (((color >> 8) & 0xff)/255.0f);
                    var b = (((color >> 0) & 0xff)/255.0f);

                    result =  new Color(r, g, b);
                    return true;
                }
            }
            else if (this._colorTable.ContainsKey(value))
            {
                result = this._colorTable[value];
                return true;
            }

            result = Color.clear;

            return false;
        }

        protected override IEnumerable<TextRun> VisitMarkupElement(Element element, string tagNameUpper)
        {
            Color c;

            switch (tagNameUpper)
            {
                case "RUBY":
                    Color? color = null;
                    float? scale = null;

                    if (element.Attributes.ContainsKey("color"))
                    {
                        if (this.TryConvertToColor(element.Attributes["Color"], out c))
                        {
                            color = c;
                        }
                    }
                    if (element.Attributes.ContainsKey("scale"))
                    {
                        var tmpSize = 0f;
                        if (Single.TryParse(element.Attributes["Scale"], out tmpSize))
                        {
                            scale = tmpSize;
                        }
                    }

                    yield return new UnityTextModifier() { RubyColor = color, RubyFontScale = scale };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                case "COLOR":
                    var value = element.GetAttribute("Value");
                    if (this.TryConvertToColor(value, out c))
                    {
                        yield return new UnityTextModifier() { Color = c };
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                        yield return TextEndOfSegment.Default;
                    }
                    else
                    {
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    }
                    break;

                case "SIZE":
                    var size = 0;
                    if (Int32.TryParse(element.GetAttribute("Value"), out size))
                    {
                        yield return new UnityTextModifier() { FontSize = size };
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                        yield return TextEndOfSegment.Default;
                    }
                    else
                    {
                        foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    }
                    break;

                case "B":
                    yield return new UnityTextModifier() { FontStyle = FontStyle.Bold };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                case "S":
                    yield return new UnityTextModifier() { TextDecoration = UnityTextDecoration.LineThrough };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                case "U":
                    yield return new UnityTextModifier() { TextDecoration = UnityTextDecoration.Underline };
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    yield return TextEndOfSegment.Default;
                    break;

                default:
                    foreach (var x in base.VisitMarkupElement(element, tagNameUpper)) yield return x;
                    break;
            }
        }
    }

}
