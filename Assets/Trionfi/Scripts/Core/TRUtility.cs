using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Trionfi
{
    public class TRUtility
    {
        private static readonly Dictionary<string, Color> _colorTable = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
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

        public static bool GetColorName(ref Color color, string name)
        {
            color = _colorTable.ContainsKey(name) ? _colorTable[name] : _colorTable["black"];

            return _colorTable.ContainsKey(name);
        }

		public static class ColorExtensions
		{
			public static Color32 FromHex(string hex)
			{
				hex = hex.Replace("#", "");
				byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
				byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
				byte a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
				return new Color32(r, g, b, a);
			}
		}


	}
}

public class CoroutineHelper<TResult, TDetail>
{
    public CoroutineHelper(ProcessFunc i_function)
    {
        m_function = i_function;
    }

    public delegate void OnFinishAction(TResult i_result, TDetail i_detail);
    public delegate IEnumerator ProcessFunc(OnFinishAction i_onFinished);

    private ProcessFunc m_function;

    public TResult Result
    {
        get;
        private set;
    }

    public TDetail Detail
    {
        get;
        private set;
    }

    public IEnumerator Update()
    {
        if (m_function == null)
        {
            yield break;
        }

        OnFinishAction onFinished = (i_result, i_detail) =>
        {
            Result = i_result;
            Detail = i_detail;
        };

        yield return m_function(onFinished);
    }
}

    /// <summary>
    /// コルーチンに関する汎用クラス
    /// </summary>
    public static class CoroutineCommon
    {
        private static readonly MonoBehaviour mMonoBehaviour;

        /// <summary>
        /// コルーチンを管理するゲームオブジェクトを生成するコンストラクタ
        /// </summary>
        static CoroutineCommon()
        {
            var gameObject = new GameObject("CoroutineCommon");
            GameObject.DontDestroyOnLoad(gameObject);
            mMonoBehaviour = gameObject.AddComponent<MonoBehaviour>();
        }

        /// <summary>
        /// 1 フレーム待機してから Action デリゲートを呼び出します
        /// </summary>
        public static void CallWaitForOneFrame(Action act)
        {
            mMonoBehaviour.StartCoroutine(DoCallWaitForOneFrame(act));
        }

        /// <summary>
        /// 指定された秒数待機してから Action デリゲートを呼び出します
        /// </summary>
        public static void CallWaitForSeconds(float seconds, Action act)
        {
            mMonoBehaviour.StartCoroutine(DoCallWaitForSeconds(seconds, act));
        }

        /// <summary>
        /// 1 フレーム待機してから Action デリゲートを呼び出します
        /// </summary>
        private static IEnumerator DoCallWaitForOneFrame(Action act)
        {
            yield return 0;
            act();
        }

        /// <summary>
        /// 指定された秒数待機してから Action デリゲートを呼び出します
        /// </summary>
        private static IEnumerator DoCallWaitForSeconds(float seconds, Action act)
        {
            yield return new WaitForSeconds(seconds);
            act();
        }
    }
