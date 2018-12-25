using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.U2D;
#endif
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
//                "storage",
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
            int id = tagParam["layer", 0];

            RawImage _image;
            _image = Trionfi.Instance.layerInstance[id].instance;

            string actor = tagParam["actor", string.Empty];
            Trionfi.Instance.layerInstance[id].actor = actor;

            Vector2 pos = _image.gameObject.GetComponent<RectTransform>().anchoredPosition;

            string layerPos = "";

            bool updatePos = false;

            if (tagParam.ContainsKey("pos"))
            {
                layerPos = tagParam["pos"].Literal();
                pos.x = TRSystemConfig.Instance.layerPos[layerPos] * TRSystemConfig.Instance.screenSize.x / 2.0f;
                updatePos = true;
            }

            int offsetY = 0;

            if (tagParam.ContainsKey("yoff"))
            {
                offsetY = tagParam["yoff"].Int();
                pos.y += offsetY;
                updatePos = true;
            }

            string storage = tagParam["storage", string.Empty];
            _image.texture = null;

            int mtime = tagParam["time", 0];
            float time = (float)mtime / 1000.0f;

            Color tempColor = TRSystemConfig.Instance.imageDefaultColor;//Color.white;
            Color destColor = TRSystemConfig.Instance.imageDefaultColor;//Color.white;

            if (time > 0.0f)
                tempColor = new Color(destColor.r, destColor.g, destColor.b, 0.0f);
            else if (tagParam.ContainsKey("color"))
                TRUtility.GetColorName(ref tempColor, tagParam["color"].Literal());

            _image.color = tempColor;

#if UNITY_2017_1_OR_NEWER
            if (tagParam.ContainsKey("atlas"))
            {
                SpriteAtlas atlas = Resources.Load<SpriteAtlas>(tagParam["atlas"].Literal());
                Sprite sprite = atlas.GetSprite(tagParam["storage", string.Empty]);
                _image.texture = sprite.texture;
            }
            else
            {
#endif
                if (!string.IsNullOrEmpty(storage))
                {
                    yield return TRResourceLoader.Instance.LoadTexture(storage);

                    //                    while (TRResourceLoader.Instance.isLoading)
                    //                        yield return new WaitForSeconds(1.0f);

                    if (TRResourceLoader.Instance.defaultTextureLoader.instance != null)
                    {
                        Trionfi.Instance.layerInstance[id].path = storage;
                        _image.texture = TRResourceLoader.Instance.defaultTextureLoader.instance;
                    }
                }
            }

            if (updatePos)
                _image.gameObject.GetComponent<RectTransform>().anchoredPosition = pos;

            if (!string.IsNullOrEmpty(storage))
            {

                if (tagParam.ContainsKey("width") || tagParam.ContainsKey("height"))
                {
                    Vector2 size = TRSystemConfig.Instance.screenSize;
                    size.x = tagParam["width", TRSystemConfig.Instance.screenSize.x];
                    size.y = tagParam["height", TRSystemConfig.Instance.screenSize.y];
                    _image.GetComponent<RectTransform>().sizeDelta = size;
                }
                else if (tagParam["snap", false])
                {
                    _image.GetComponent<RectTransform>().sizeDelta = TRSystemConfig.Instance.screenSize;
                }
                else
                    _image.SetNativeSize();
            }
            else
                _image.GetComponent<RectTransform>().sizeDelta = TRSystemConfig.Instance.screenSize;

            _image.enabled = true;

            if (time > 0)
            {
                Tweener _tweener = DOTween.ToAlpha(
                                () => _image.color,
                                color => _image.color = color,
                                1.0f,
                                time
                               );
                yield return new WaitWhile(_tweener.IsPlaying);
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

            int id = tagParam["layer", 0];

            _image = Trionfi.Instance.layerInstance[id].instance;
            _image.enabled = false;
            _image.texture = null;
        }
    }

    public class LaytextComponent : AbstractComponent
    {
        public LaytextComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
//                "string"
            };
#endif
        }

        protected override void TagFunction()
        {
            RawImage _image;

            string text = tagParam["string", string.Empty];
            Trionfi.Instance.layerText.text = text;
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

            int id = tagParam["layer", 0];
            _image = Trionfi.Instance.layerInstance[id].instance;

            //ToDo:
        }
    }

    //
    public class ImagecolorComponent : AbstractComponent
    {
        public ImagecolorComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer",
                "color"
            };
#endif
        }

        protected override void TagFunction()
        {
            RawImage _image;

            int id = tagParam["layer", 0];
            _image = Trionfi.Instance.layerInstance[id].instance;

            Color color = Color.white;

            TRUtility.GetColorName(ref color, tagParam["color"].Literal());

            _image.color = color;
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

            int id = tagParam["layer", 0];
            _image = Trionfi.Instance.layerInstance[id].instance;

            float time = tagParam["time", 1.0f];
            Vector3 pos = new Vector3(tagParam["pos_x", 0.0f], tagParam["pos_y", 0.0f], tagParam["pos_z", 0.0f]);
            Vector3 scale  = new Vector3 (tagParam["scale_x", 1.0f], tagParam["scale_y", 1.0f], tagParam["scale_z", 1.0f]);
            Vector3 rotate = new Vector3(tagParam["rot_x", 0.0f], tagParam["rot_y", 0.0f], tagParam["rot_z", 1.0f]);

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
            isSync = tagParam["sync", true];

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
            int timeMsec = tagParam["time", (int)(TRSystemConfig.Instance.defaultEffectTime * 1000.0f)];
            float time = timeMsec / 1000.0f;

            if (!tagParam.ContainsKey("rule"))
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
                string ruleTexture = string.Empty;
                ruleTexture = tagParam["rule", string.Empty];

                //ルール画像はResourcesで固定
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

            int waitTime = tagParam["time", 0];

            if (waitTime > 0)
                yield return new WaitForSeconds((float)waitTime / 1000.0f);

            yield return null;
        }
    }

    public class ShakeComponent : AbstractComponent
    {
        bool isSync = true;

        public ShakeComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer",
            };
#endif
        }

        protected override void TagFunction()
        {

        }

        public override IEnumerator TagSyncFunction()
        {
            if (isSync)
                yield return (ShakeFunction());
        }

        IEnumerator ShakeFunction()
        {
            RectTransform _rect = null;

            int id = -1;
            string name = string.Empty;

            int strength = tagParam["strength", 5];
            int vibratio = tagParam["vibrato", 20];

            if ( tagParam["layer", string.Empty] == "message")
                _rect = Trionfi.Instance.messageWindow.gameObject.GetComponent<RectTransform>();
            else
                _rect = Trionfi.Instance.layerInstance[tagParam["layer", 0]].instance.gameObject.GetComponent<RectTransform>();

            int timeMsec = tagParam["time", (int)(TRSystemConfig.Instance.defaultEffectTime * 1000.0f)];
            float time = timeMsec / 1000.0f;

            Tweener _tween = _rect.GetComponent<RectTransform>().DOShakePosition(time, strength, vibratio, 90.0f, false, false);

            yield return new WaitWhile(_tween.IsPlaying);
        }
    }
}
