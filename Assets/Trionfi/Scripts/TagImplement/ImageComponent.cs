using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //オブジェクト生成廃棄はシステム管轄から外れる（可搬性）

    //  color
    //  visible

    public class ImageComponent : AbstractComponent {
        public ImageComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "layer"
            };
        }

        protected override void TagFunction() { }

        protected override IEnumerator TagSyncFunction()
        {
            string storage = tagParam.Identifier("storage");

            yield return TRResourceLoader.Instance.Load(storage, TRResourceType.Texture);

            if (!TRResourceLoader.Instance.request.isHttpError && !TRResourceLoader.Instance.request.isNetworkError)
            {
                Image _image;

                int id = -1;

                if (tagParam.IsValid(ref id, "layer"))
                    _image = Trionfi.Instance.layerInstance[id].instance;
                else
                    _image = Trionfi.Instance.layerInstance[0].instance;

                Texture2D _texture = DownloadHandlerTexture.GetContent(TRResourceLoader.Instance.request);
                Sprite _sprite = Sprite.Create(_texture, new Rect(0,0, _texture.width, _texture.height), Vector2.zero);
                _image.sprite = _sprite;
                _image.SetNativeSize();
            }
        }
    }
    
	public class ImagefreeComponent : AbstractComponent {
		public ImagefreeComponent() {
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
		}

		protected override void TagFunction() {
            Image _image;

            int id = -1;

            if (tagParam.IsValid(ref id, "layer"))
                _image = Trionfi.Instance.layerInstance[id].instance;
            else
                _image = Trionfi.Instance.layerInstance[0].instance;

            _image.sprite = null;
        }
    }

    public class LayoptComponent : AbstractComponent
    {
        public LayoptComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
        }

        protected override void TagFunction()
        {
            Image _image;

            int id = -1;

            if (tagParam.IsValid(ref id, "layer"))
                _image = Trionfi.Instance.layerInstance[id].instance;
            else
                _image = Trionfi.Instance.layerInstance[0].instance;

            //ToDo:
        }
    }

    //
    public class ImagetweenComponent : AbstractComponent
    {
        public ImagetweenComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
        }

        protected override void TagFunction()
        {
            Image _image;

            int id = -1;

            if (tagParam.IsValid(ref id, "layer"))
                _image = Trionfi.Instance.layerInstance[id].instance;
            else
                _image = Trionfi.Instance.layerInstance[0].instance;

            float time = tagParam.Float("time", 1.0f);
            Vector3 pos = new Vector3(tagParam.Float("pos_x"), tagParam.Float("pos_y"), tagParam.Float("pos_z"));
            Vector3 scale  = new Vector3(tagParam.Float("scale_x", 1.0f), tagParam.Float("scale_y", 1.0f), tagParam.Float("scale_z", 1.0f));
            Vector3 rotate = new Vector3(tagParam.Float("rot_x"), tagParam.Float("rot_y"), tagParam.Float("rot_z"));

            Sequence seq = DOTween.Sequence();
            seq.Append(_image.rectTransform.DOLocalMove(pos, time));
            seq.Join (_image.rectTransform.DOScale(scale, time));
            seq.Join(_image.rectTransform.DORotate(rotate, time));
            seq.Play();
        }
    }
}
