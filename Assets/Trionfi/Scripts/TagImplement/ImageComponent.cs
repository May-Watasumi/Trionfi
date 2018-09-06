using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //  color
    //  visible

    public class ImageComponent : AbstractComponent {
        public ImageComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "storage",
                "layer"
            };
#endif
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

        public override IEnumerator TagSyncFunction()
        {
            string storage = tagParam.Identifier("storage");

            TRResourceLoader.Instance.Load(storage, TRResourceType.Texture);

            while (TRResourceLoader.Instance.isLoading)
                yield return new WaitForSeconds(1.0f);

            if (TRResourceLoader.Instance.isSuceeded)
            {
                int id = tagParam.Int("layer");

                RawImage _image;
                _image = Trionfi.Instance.layerInstance[id].instance;

                Vector2 pos = _image.gameObject.GetComponent<RectTransform>().anchoredPosition;

                string layerPos = "";
                
                if (tagParam.IsValid(ref layerPos, "pos") && TRSystemConfig.Instance.layerPos.ContainsKey(layerPos))
                    pos.x = TRSystemConfig.Instance.layerPos[layerPos];

                if(id != 0)
                    pos.y = (TRResourceLoader.Instance.texture.height - TRSystemConfig.Instance.screenSize.y) / 2.0f;

                int offsetY = 0;

                if (tagParam.IsValid(ref offsetY, "yoff"))
                    pos.y += offsetY;

                Trionfi.Instance.layerInstance[id].path = storage;

                _image.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;
                _image.texture = TRResourceLoader.Instance.texture;
                _image.SetNativeSize();

                _image.enabled = true;
            }
        }
    }
    
	public class ImagefreeComponent : AbstractComponent {
		public ImagefreeComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
#endif
        }

		protected override void TagFunction() {
            RawImage _image;

            int id = -1;

            if (tagParam.IsValid(ref id, "layer"))
                _image = Trionfi.Instance.layerInstance[id].instance;
            else
                _image = Trionfi.Instance.layerInstance[0].instance;

            _image.enabled = false;
            _image.texture = null;
        }
    }

    public class LayoptComponent : AbstractComponent
    {
        public LayoptComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
#endif
        }

        protected override void TagFunction()
        {
            RawImage _image;

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
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer"
            };
#endif
        }

        protected override void TagFunction()
        {
            RawImage _image;

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

    public class SnapshotComponent : AbstractComponent
    {
        public SnapshotComponent()
        {
#if UNITY_EDITOR && TR_DEBUG

            //必須項目
            essentialParams = new List<string>
            {
            };
#endif
        }

        protected override void TagFunction()
        {
            Trionfi.Instance.rawImage.color = Color.white;
            Trionfi.Instance.targetCamera.targetTexture = Trionfi.Instance.captureBuffer;
            Trionfi.Instance.targetCamera.Render();
        }

        public override IEnumerator TagSyncFunction()
        {
            yield return new WaitForEndOfFrame();
            Trionfi.Instance.targetCamera.targetTexture = null;
            Trionfi.Instance.rawImage.gameObject.SetActive(true);
        }
    }


    public class TransComponent : AbstractComponent
    {
        bool isSync = true;

        public TransComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
            };
#endif
        }

        protected override void TagFunction()
        {
            //default is "sync" = true.
            if(!tagParam.IsValid(ref isSync, "sync"))
                isSync = true;

            if(!isSync)
                Trionfi.Instance.StartCoroutine(FadeFunction());

        }

        public override IEnumerator TagSyncFunction()
        {
            if (isSync)
                yield return (FadeFunction());
        }

        IEnumerator FadeFunction()
        {
            float time = tagParam.Float("time", TRSystemConfig.Instance.defaultEffectTime);

            string ruleTexture = "";
            if (!tagParam.IsValid(ref ruleTexture, "rule"))
            {
                bool wait = false;

                Color _color = Trionfi.Instance.rawImage.color;
                Trionfi.Instance.rawImage.color = new Color(_color.r, _color.g, _color.b, 1.0f);

                Trionfi.Instance.rawImage.material = null;
                Trionfi.Instance.rawImage.gameObject.GetComponent<MaskFader>().enabled = false;

                DOTween.ToAlpha(
                                () => Trionfi.Instance.rawImage.color,
                                color => Trionfi.Instance.rawImage.color = color,
                                0.0f,
                                time
                            ).OnKill(() =>
                            {
                                Trionfi.Instance.rawImage.gameObject.SetActive(false);
                                wait = true;
                            });

                yield return new WaitUntil( () => wait);
            }
            else
            {
                Texture _rule = Resources.Load<Texture>(ruleTexture);

                MaskFader maskFader = Trionfi.Instance.rawImage.gameObject.GetComponent<MaskFader>();
                maskFader.Range = 0.0f;
                maskFader.maskTexture = _rule;
                maskFader.enabled = true;

                float timeCount = 0.0f;

                while(timeCount < time)
                {
                    maskFader.Range = timeCount / time;
                    yield return new WaitForEndOfFrame();
                    timeCount += Time.deltaTime;
                }

                maskFader.Range = 1.0f;

            }

            int waitTime = 0;

            if (tagParam.IsValid(ref waitTime, "wait"))
                yield return new WaitForSeconds((float)waitTime / 1000.0f);

            yield return null;
        }
    }
}
