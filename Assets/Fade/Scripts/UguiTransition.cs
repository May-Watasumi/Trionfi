// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utage
{
	/// <summary>
	/// Graphicにトランジションをかける
	/// </summary>
	public class UguiTransition : MonoBehaviour, IMaterialModifier, IMeshModifier
	{
		public Graphic Target
		{
			get
			{
				if (target == null)
				{
					Target = this.GetComponent<Graphic>();
				}
				return target;
			}
			set
			{
				target = value;
				DefaultMaterial = target.material;
				Target.SetMaterialDirty();
			}
		}

        Graphic target;

        public Texture RuleTexture
		{
			get
			{
				return ruleTexture;
			}
			set
			{
				if (ruleTexture == value)
					return;

				ruleTexture = value;
				Target.SetMaterialDirty();
			}
		}

        [SerializeField]
		Texture ruleTexture;

		[SerializeField, Range(0, 1.0f)]
		float strengh = 0;
		public float Strengh
		{
			get
			{
				return strengh;
			}
			set
			{
				if (Mathf.Approximately(strengh, value))
					return;

				strengh = value;
				Target.SetMaterialDirty();
			}
		}

		[SerializeField, Range(0.001f, 1.0f)]
		float vague = 0.2f;
		public float Vague
		{
			get
			{
				return vague;
			}
			set
			{
				if (Mathf.Approximately(vague, value))
					return;

				vague = value;
				Target.SetMaterialDirty();
			}
		}

		public bool IsPremultipliedAlpha { get; set; }
		Material DefaultMaterial { get; set; }

#if UNITY_EDITOR
		void OnValidate()
        {
			DefaultMaterial = Target.material;
			Target.SetMaterialDirty();
        }
#endif

		void Awake()
		{
			Target.SetAllDirty();
		}

		public void ModifyMesh(Mesh mesh)
		{
			using (var vh = new VertexHelper(mesh))
			{
				ModifyMesh(vh);
				vh.FillMesh(mesh);
			}
		}

		public void ModifyMesh(VertexHelper vh)
		{
			if (!enabled)
				return;

			Rect r = Target.rectTransform.rect;
			Vector2 pivot = Target.rectTransform.pivot;
			float w = r.width;
			float h = r.height;
			UIVertex vert = new UIVertex();
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vert, i);
				vert.uv1 = new Vector2(vert.position.x/w + pivot.x, vert.position.y/ h + pivot.y);
				vh.SetUIVertex(vert, i);
			}
		}

        public Material GetModifiedMaterial(Material baseMaterial)
        {
			baseMaterial.SetFloat("_Strength", Strengh);
			baseMaterial.SetFloat("_Vague", Vague);
			baseMaterial.SetTexture("_RuleTex", RuleTexture);

			if (IsPremultipliedAlpha)
			{
				baseMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				baseMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);

				baseMaterial.EnableKeyword("PREMULTIPLIED_ALPHA");
			}
			else
			{
				baseMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				baseMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				baseMaterial.DisableKeyword("PREMULTIPLIED_ALPHA");
			}

			return baseMaterial;
        }

		//ルール画像つきのフェードイン
		public void RuleFadeIn(Texture texture, float vague, bool isPremultipliedAlpha,  float time, Action onComplete)
		{
			RuleTexture = texture;
			Vague = vague;
			IsPremultipliedAlpha = isPremultipliedAlpha;

			RuleFadeIn(time, onComplete);
		}

		//ルール画像つきのフェードイン
		public void RuleFadeIn(float time, Action onComplete)
		{
            /*
			Target.material = new Material(ShaderManager.RuleFade);
			Timer timer = this.gameObject.AddComponent<Timer>();
			timer.StartTimer(
				time,
				x => Strengh = x.Time01Inverse,
				x =>
				{
					Target.material = DefaultMaterial;
					Destroy(timer);
					onComplete();
				});
            */
		}

		//ルール画像つきのフェードアウト
		public void RuleFadeOut(Texture texture, float vague, bool isPremultipliedAlpha, float time, Action onComplete)
		{
			RuleTexture = texture;
			Vague = vague;
			IsPremultipliedAlpha = isPremultipliedAlpha;

			RuleFadeOut(time, onComplete);
		}

		//ルール画像つきのフェードイン
		public void RuleFadeOut(float time, Action onComplete)
		{
            /*
			Target.material = new Material(ShaderManager.RuleFade);
			Timer timer = this.gameObject.AddComponent<Timer>();
			timer.StartTimer(
				time,
				x => Strengh = x.Time01,
				x =>
				{
					Target.material = DefaultMaterial;
					Destroy(timer);
					onComplete();
				});
           */
		}
	}
}