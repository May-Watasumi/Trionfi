using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterWriter;
using UnityEngine;

namespace LetterWriter.Unity
{
    public class UnityGlyph : Glyph
    {
        private UIVertex[] _baseVertices = new[] { UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert, UIVertex.simpleVert, };

        public CharacterInfo CharacterInfo { get; private set; }

        public Color? Color { get; private set; }
        public UnityTextDecoration TextDecoration { get; private set; }

        public UnityGlyph(char character, CharacterInfo characterInfo, Color? color, int height, UnityTextDecoration textDecoration) : base(character, characterInfo.advance, height)
        {
            this.CharacterInfo = characterInfo;
            this.Color = color;
            this.TextDecoration = textDecoration;
            this.UpdateBaseVertices();
        }

        public void FillBaseVertices(UIVertex[] targetVertices)
        {
            targetVertices[0] = this._baseVertices[0];
            targetVertices[1] = this._baseVertices[1];
            targetVertices[2] = this._baseVertices[2];
            targetVertices[3] = this._baseVertices[3];
        }

        private void UpdateBaseVertices()
        {
            // MEMO: なぜか高さはuGUIと微妙に違うサイズのが出てくる…。TextGeneratorが誤差であれになってるのではという気もしなくなくもなくもない。

            var characterInfo = this.CharacterInfo;
            var minX = characterInfo.minX;
            var minY = characterInfo.minY;
            var maxX = characterInfo.maxX;
            var maxY = characterInfo.maxY;

            // 左下
            this._baseVertices[0].uv0 = characterInfo.uvBottomLeft;
            this._baseVertices[0].position = new Vector3(minX, minY);

            // 右下
            this._baseVertices[1].uv0 = characterInfo.uvBottomRight;
            this._baseVertices[1].position = new Vector3(maxX, minY);

            // 右上
            this._baseVertices[2].uv0 = characterInfo.uvTopRight;
            this._baseVertices[2].position = new Vector3(maxX, maxY);

            // 左上
            this._baseVertices[3].uv0 = characterInfo.uvTopLeft;
            this._baseVertices[3].position = new Vector3(minX, maxY);

            if (this.Color.HasValue)
            {
                this._baseVertices[0].color = this.Color.Value;
                this._baseVertices[1].color = this.Color.Value;
                this._baseVertices[2].color = this.Color.Value;
                this._baseVertices[3].color = this.Color.Value;
            }
        }
    }
}
