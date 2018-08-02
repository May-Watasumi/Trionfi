using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //オブジェクト生成廃棄はシステム管轄から外れる（可搬性）

    public class LayerComponent : AbstractComponent {
		public LayerComponent() {

			//必須項目
			essentialParams = new List<string> {
				"storage",
                "id"
			};
/*
			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "x",""},
				{ "y",""},
				{ "z",""},
				{ "scale_x",""},
				{ "scale_y",""},
				{ "scale_z",""},
				{ "scale",""},
			};
*/
		}

		protected override void TagFunction()
        {
        }

        public override IEnumerator TagAsyncWait()
        {
            string storage = tagParam.Identifier("storage");

            yield return TRResourceLoader.Instance.Load(storage, TRResourceType.Texture);

            if (!TRResourceLoader.Instance.request.isHttpError && !TRResourceLoader.Instance.request.isNetworkError)
            {
                Image _image;

                int ch = -1;

                if (tagParam.IsValid(ref ch, "id"))
                    _image = Trionfi.Instance.GetLayer(TRAssetType.Character, ch);
                else
                    _image = Trionfi.Instance.GetLayer(TRAssetType.Character);

                Texture2D _texture = DownloadHandlerTexture.GetContent(TRResourceLoader.Instance.request);
                Sprite _sprite = Sprite.Create(_texture, new Rect(0,0, _texture.width, _texture.height), Vector2.zero);
                _image.sprite = _sprite;
                _image.SetNativeSize();
            }
        }
    }
    
	public class LayerfreeComponent : AbstractComponent {
		public LayerfreeComponent() {
			//必須項目
			essentialParams = new List<string>
            {
                "id"
            };	//"name"
		}

		protected override void TagFunction() {
            Image _dest;

            int ch = -1;
            if (tagParam.IsValid(ref ch, "id"))
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character);

            _dest.sprite = null;
        }
    }

    public class LayoptComponent : AbstractComponent
    {
        public LayoptComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "id"
            };  //"name"
        }

        protected override void TagFunction()
        {
            Image _dest;

            int ch = -1;
            if (tagParam.IsValid(ref ch, "id"))
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character);

            //ToDo:
        }
    }

    public class LaytransComponent : AbstractComponent
    {
        public LaytransComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "id"
            };
        }

        protected override void TagFunction()
        {
            Image _dest;

            int ch = -1;
            if (tagParam.IsValid(ref ch, "id"))
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character, ch);
            else
                _dest = Trionfi.Instance.GetLayer(TRAssetType.Character);

            float time = tagParam.Float("time", 1.0f);
            Vector3 pos = new Vector3(tagParam.Float("pos_x"), tagParam.Float("pos_y"), tagParam.Float("pos_z"));
            Vector3 scale  = new Vector3(tagParam.Float("scale_x", 1.0f), tagParam.Float("scale_y", 1.0f), tagParam.Float("scale_z", 1.0f));
            Vector3 rotate = new Vector3(tagParam.Float("rot_x"), tagParam.Float("rot_y"), tagParam.Float("rot_z"));

            Sequence seq = DOTween.Sequence();
            seq.Append(_dest.rectTransform.DOLocalMove(pos, time));
            seq.Join (_dest.rectTransform.DOScale(scale, time));
            seq.Join(_dest.rectTransform.DORotate(rotate, time));
            seq.Play();
        }
    }

}
