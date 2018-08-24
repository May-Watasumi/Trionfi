using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter.Markup;
using UnityEngine;

namespace LetterWriter.Unity.Components
{
    public class LetterWriterExtensibilityProvider : MonoBehaviour
    {
        public virtual LetterWriterMarkupParser CreateMarkupParser()
        {
            return new LetterWriterMarkupParser();
        }

        public virtual TextFormatter CreateTextFormatter(Font font, int fontSize, Color color)
        {
            return new UnityTextFormatter(font, fontSize, color);
        }
    }
}
