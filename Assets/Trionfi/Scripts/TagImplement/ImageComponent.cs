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

            if (TRResourceLoader.Instance.isSuceeded)
            {
                int id = tagParam.Int("layer");

                Image _image;
                _image = Trionfi.Instance.layerInstance[id].instance;

                Trionfi.Instance.layerInstance[id].path = storage;

                Texture2D _texture = TRResourceLoader.Instance.texture;
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

    public class LockComponent : AbstractComponent
    {
        public LockComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
            };
        }

        protected override void TagFunction()
        {
            Trionfi.Instance.rawImage.color = Color.white;
            Trionfi.Instance.rawImage.gameObject.SetActive(true);
            Trionfi.Instance.targetCamera.targetTexture = Trionfi.Instance.captureBuffer;
            Trionfi.Instance.targetCamera.Render();
            Trionfi.Instance.targetCamera.targetTexture = null;
        }
    }


    public class TransComponent : AbstractComponent
    {
        public TransComponent()
        {
            //必須項目
            essentialParams = new List<string>
            {
            };
        }

        protected override void TagFunction()
        {
            float time = tagParam.Float("time", TRSystemConfig.Instance.defaultEffectTime);

            DOTween.ToAlpha(
                            () => Trionfi.Instance.rawImage.color,
                            color => Trionfi.Instance.rawImage.color = color,
                            0.0f,
                            time
                        ).OnComplete(() =>  Trionfi.Instance.rawImage.gameObject.SetActive(false));
        }
    }
}
