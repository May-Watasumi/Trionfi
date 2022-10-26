#if !TR_PARSEONLY
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Networking;
using SpriteDicing;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

#else
using TRTask = System.Threading.Tasks.Task;
using TRTaskString = System.Threading.Tasks.Task<string>;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //  color
    //  visible

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
            hasSync = true;
#if !TR_PARSEONLY
            TRLayerID id = (TRLayerID)tagParam["layer", 0];

            Image _image;
            _image = Trionfi.Instance.layerInstance[id].instance;

            //話者セット
            string actor = tagParam["actor", string.Empty];
            Trionfi.Instance.layerInstance[id].actor = actor;

            Vector2 pos = _image.gameObject.GetComponent<RectTransform>().anchoredPosition;

            string layerPos = "";

            bool updatePos = false;

            if (tagParam.ContainsKey("pos"))
            {
                layerPos = tagParam["pos"].Literal();
                pos.x = TRSystemConfig.Instance.layerPos.GetPos(layerPos) * TRSystemConfig.Instance.screenSize.x / 2.0f;
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
            _image.sprite = null;
            _image.useSpriteMesh = false;

            int mtime = tagParam["time", 0];
            float time = (float)mtime / 1000.0f;

            Color tempColor = TRSystemConfig.Instance.imageDefaultColor;//Color.white;
            Color destColor = TRSystemConfig.Instance.imageDefaultColor;//Color.white;

            if (time > 0.0f)
                tempColor = new Color(destColor.r, destColor.g, destColor.b, 0.0f);
            else if (tagParam.ContainsKey("color"))
                TRUtility.GetColorName(ref tempColor, tagParam["color"].Literal());

            _image.color = tempColor;

            if (tagParam.ContainsKey("atlas"))
            {
                SpriteAtlas atlas = Resources.Load<SpriteAtlas>(tagParam["atlas"].Literal());
                Sprite sprite = atlas.GetSprite(tagParam["storage", string.Empty]);
                _image.sprite = sprite;
            }
            else if (tagParam.ContainsKey("dicedatlas"))
            {
                DicedSpriteAtlas atlas = Resources.Load<DicedSpriteAtlas>(tagParam["dicedatlas"].Literal());
                Sprite sprite = atlas.GetSprite(tagParam["storage", string.Empty]);
                _image.sprite = sprite;
                _image.useSpriteMesh = true;
            }
            /*
            else if (tagParam.ContainsKey("renderbuf"))
            {
                Trionfi.Instance.layerInstance[(TRLayerID)id].instance.texture = Trionfi.Instance.subRenderBuffer[0];
                Trionfi.Instance.layerInstance[(TRLayerID)id].tagParam = tagParam;
                Trionfi.Instance.layerInstance[(TRLayerID)id].resourceType = GetResourceType();
            }
            */
            else
            {
                TRResourceType type = GetResourceType();
                _image.sprite = await Trionfi.Instance.LoadImage(tagParam, type);
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

                await UniTask.WaitWhile(() => _tweener.IsPlaying());
            }

            return string.Empty;
        }
#endif
	}

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Image _image;

            TRLayerID id = (TRLayerID)tagParam["layer", 0];

            _image = Trionfi.Instance.layerInstance[id].instance;
            _image.enabled = false;
            _image.sprite = null;
#endif
            return string.Empty;
		}
    }

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Image _image;

            TRLayerID id = (TRLayerID)tagParam["layer", 0];
            _image = Trionfi.Instance.layerInstance[id].instance;

            //ToDo:
#endif
            return string.Empty;
		}
    }

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Image _image;

            TRLayerID id = (TRLayerID)tagParam["layer", 0];
            _image = Trionfi.Instance.layerInstance[id].instance;

            Color color = Color.white;

            TRUtility.GetColorName(ref color, tagParam["color"].Literal());

            _image.color = color;
#endif
            return string.Empty;
		}
    }

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Image _image;

            TRLayerID id = (TRLayerID)tagParam["layer", 0];
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
#endif
            return string.Empty;
		}
    }

    [Serializable]
    public class ImageeffectComponent : AbstractComponent
    {
        public ImageeffectComponent()
        {


#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialMoreOneParams = new List<string>
            {
                "layer",
                "type"
            };
#endif
        }

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Material mat = null;

            if (tagParam.ContainsKey("type"))
            {
                switch (tagParam["type"].Literal().ToLower())
                {
                    case "sepia":
                        mat = Resources.Load<Material>("Materials/Sepia");
                        break;
                    case "mosaic":
                        mat = Resources.Load<Material>("Materials/Mosaic");
                        break;
                    case "mono":
                        mat = Resources.Load<Material>("Materials/MonoTone");
                        break;
                    case "blur":
                        mat = Resources.Load<Material>("Materials/Blur");
                        break;
                }
            }

            if (tagParam.ContainsKey("layer"))
            {
                Image _image;
                TRLayerID id = (TRLayerID)tagParam["layer", 0];
                _image = Trionfi.Instance.layerInstance[id].instance;
                _image.material = mat;
            }
            else
			{
                PostEffect.Instance.effect = mat;
			}
#endif
            return string.Empty;
        }
    }

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Trionfi.Instance.rawImage.color = Color.white;
            Trionfi.Instance.targetCamera.targetTexture = Trionfi.Instance.captureBuffer;
            Trionfi.Instance.targetCamera.Render();
            /*
            Texture2D tex = new Texture2D(Trionfi.Instance.targetCamera.targetTexture.width, Trionfi.Instance.targetCamera.targetTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = Trionfi.Instance.targetCamera.targetTexture;
            tex.ReadPixels(new Rect(0, 0, Trionfi.Instance.targetCamera.targetTexture.width, Trionfi.Instance.targetCamera.targetTexture.height), 0, 0);
            tex.Apply();

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            UnityEngine.Object.Destroy(tex);

            //Write to a file in the project folder
            File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
            */


            await UniTask.WaitForEndOfFrame(Trionfi.Instance);

            Trionfi.Instance.targetCamera.targetTexture = null;
            Trionfi.Instance.rawImage.gameObject.SetActive(true);
#endif
            return string.Empty;
        }
	}

    [Serializable]
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

        protected override async TRTaskString TagFunction()
        {
            //default is "sync" = true.
            isSync = tagParam["sync", true];

            if(!isSync)
                FadeFunction().Forget();

            if (isSync)
                await (FadeFunction());

            return string.Empty;
        }

        async TRTask FadeFunction()
        {
#if !TR_PARSEONLY
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

                await UniTask.WaitWhile(() => wait);
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

                while (timeCount < time && !TRGameConfig.configData.effectSkip)
                {
                    maskFader.Range = timeCount / time;
                    await UniTask.WaitForEndOfFrame(Trionfi.Instance);
                    timeCount += Time.deltaTime;
                }

                maskFader.Range = 1.0f;

            }

            int waitTime = tagParam["time", 0];

            if (waitTime > 0)
                await UniTask.Delay(timeMsec);
#endif
        }
    }

    [Serializable]
    public class QuakeComponent : AbstractComponent
    {
        bool isSync = true;

        public QuakeComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "layer",
            };
#endif
        }

#if !TR_PARSEONLY
        protected override async TRTaskString TagFunction()
        {
            if (isSync)
                await (ShakeFunction());
            else
                ShakeFunction().Forget();

            return string.Empty;
        }

        async TRTask ShakeFunction()
        {
            RectTransform _rect = null;

//            int id = -1;
            string name = string.Empty;

            int strength = tagParam["strength", 5];
            int vibratio = tagParam["vibrato", 20];

            if ( tagParam["layer", string.Empty] == "message")
                _rect = Trionfi.Instance.messageWindow.gameObject.GetComponent<RectTransform>();
            else
                _rect = Trionfi.Instance.layerInstance[(TRLayerID)tagParam["layer", 0]].instance.gameObject.GetComponent<RectTransform>();

            int timeMsec = tagParam["time", (int)(TRSystemConfig.Instance.defaultEffectTime * 1000.0f)];
            float time = timeMsec / 1000.0f;

            Tweener _tween = _rect.GetComponent<RectTransform>().DOShakePosition(time, strength, vibratio, 90.0f, false, false);

            await UniTask.WaitWhile(() => _tween.IsPlaying());
        }
#endif
	}
}
