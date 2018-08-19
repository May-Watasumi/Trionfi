using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Text;
using System;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class HyphenationJpn : UIBehaviour, IMeshModifier
{
	// http://answers.unity3d.com/questions/424874/showing-a-textarea-field-for-a-string-variable-in.html
	[TextArea(3,10), SerializeField]
	private string text;

	private RectTransform _RectTransform {
		get{
			if( _rectTransform == null )
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}
	private RectTransform _rectTransform;

	private Text _Text {
		get{
			if( _text == null )
				_text = GetComponent<Text>();
			return _text;
		}
	}
	private Text _text;


    [SerializeField]
    GameObject[] rubyList = new GameObject[3];

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		UpdateText(text);
	}

#if UNITY_EDITOR
    public new void OnValidate()
    {
        base.OnValidate();

        var graphics = base.GetComponent<Graphic>();
        if (graphics != null)
        {
            graphics.SetVerticesDirty();
        }

        UpdateText(text);
    }
#endif

    void UpdateText(string str)
	{
        for (int i = 0; i < 3; i++)
        {
            rubyList[i].GetComponent<Text>().text = "";
        }

        ruby_alot = 0;

        //本文からルビ文字部分をカットして、ルビ文字データを作る。
        string _tempStr = cut_ruby(str);

        //ルビ文字の準備　その１
        for (int i = 0; i < ruby_alot; i++)
        {
            ruby_txt[i] = rubyList[i];
            ruby_txt[i].GetComponent<Text>().text = ruby_d[i].txt;
            ruby_txt[i].GetComponent<Text>().font = _Text.font;
            ruby_txt[i].GetComponent<Text>().color = _Text.color;
            ruby_txt[i].GetComponent<Text>().fontSize = (int)(_Text.fontSize * 0.5f);
        }

        _Text.text = GetFormatedText(_Text, _tempStr);
	}
	
	public void GetText(string str)
	{
		text = str;
		UpdateText(text);
	}

	float GetSpaceWidth(Text textComp)
	{
		float tmp0 = GetTextWidth(textComp, "m m");
		float tmp1 = GetTextWidth(textComp, "mm");
		return (tmp0 - tmp1);
	}

	float GetTextWidth(Text textComp, string message)
	{
		if( _text.supportRichText )
			message = Regex.Replace(message, RITCH_TEXT_REPLACE, string.Empty);

		textComp.text = message;
		return textComp.preferredWidth;
	}

	string GetFormatedText(Text textComp, string msg)
	{
		if(string.IsNullOrEmpty(msg)){
			return string.Empty;
		}
		
		float rectWidth = _RectTransform.rect.width;
		float spaceCharacterWidth = GetSpaceWidth(textComp);

		// override
		textComp.horizontalOverflow = HorizontalWrapMode.Overflow;

		// work
		StringBuilder lineBuilder = new StringBuilder();

		float lineWidth = 0;
		foreach( var originalLine in GetWordList(msg))
		{
			lineWidth += GetTextWidth(textComp, originalLine);

			if( originalLine == Environment.NewLine )
				lineWidth = 0;
			else {
				if( originalLine == " " )
					lineWidth += spaceCharacterWidth;

				if( lineWidth > rectWidth ) {
					lineBuilder.Append( Environment.NewLine );
					lineWidth = GetTextWidth(textComp, originalLine);
				}
			}
			lineBuilder.Append( originalLine );
		}

		return lineBuilder.ToString();
	}

	private List<string> GetWordList(string tmpText)
	{
		List<string> words = new List<string>();
		StringBuilder line = new StringBuilder();
		char emptyChar = new char();

		for(int characterCount = 0; characterCount < tmpText.Length; characterCount++)
		{
			char currentCharacter = tmpText[characterCount];
			char nextCharacter = (characterCount < tmpText.Length-1) ? tmpText[characterCount+1] : emptyChar;
			char preCharacter = (characterCount > 0) ? preCharacter = tmpText[characterCount-1] : emptyChar;

			line.Append( currentCharacter );

			if( ((IsLatin(currentCharacter) && IsLatin(preCharacter) ) && (IsLatin(currentCharacter) && !IsLatin(preCharacter))) ||
			    (!IsLatin(currentCharacter) && CHECK_HYP_BACK(preCharacter)) ||
			    (!IsLatin(nextCharacter) && !CHECK_HYP_FRONT(nextCharacter) && !CHECK_HYP_BACK(currentCharacter))||
			    (characterCount == tmpText.Length - 1)){
				words.Add(line.ToString());
				line = new StringBuilder();
				continue;
			}
		}
		return words;
	}

	// helper
	public float textWidth {
		set{
			_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
		}
		get{
			return _RectTransform.rect.width;
		}
	}
	public int fontSize
	{
		set{
			_Text.fontSize = value;
		}
		get{
			return _Text.fontSize;
		}
	}

	// static
	private readonly static string RITCH_TEXT_REPLACE = 
		"(\\<color=.*\\>|</color>|" +
		"\\<size=.n\\>|</size>|"+
		"<b>|</b>|"+
		"<i>|</i>)";

	// 禁則処理 http://ja.wikipedia.org/wiki/%E7%A6%81%E5%89%87%E5%87%A6%E7%90%86
	// 行頭禁則文字
	private readonly static char[] HYP_FRONT = 
		(",)]｝、。）〕〉》」』】〙〗〟’”｠" +// 終わり括弧類 簡易版
		 "ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ" +//行頭禁則和字 
		 "‐゠–〜ー" +//ハイフン類
		 "?!！？‼⁇⁈⁉" +//区切り約物
		 "・:;" +//中点類
		 "。.").ToCharArray();//句点類

	private readonly static char[] HYP_BACK = 
		 "(（[｛〔〈《「『【〘〖〝‘“｟".ToCharArray();//始め括弧類

	private readonly static char[] HYP_LATIN = 
		("abcdefghijklmnopqrstuvwxyz" +
	     "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + 
	     "0123456789" + 
	     "<>=/().,").ToCharArray();

	private static bool CHECK_HYP_FRONT(char str)
	{
		return Array.Exists<char>(HYP_FRONT, item => item == str);
	}

	private static bool CHECK_HYP_BACK(char str)
	{
		return Array.Exists<char>(HYP_BACK, item => item == str);
	}

	private static bool IsLatin(char s)
	{
		return Array.Exists<char>(HYP_LATIN, item => item == s);
	}

    //-------------------------------
    //
    //各ルビ文字のデータ
    //
    //-------------------------------
    public struct ruby_data
    {
        public int num1;//本文の何文字目から何文字目にルビを降るか？
        public int num2;//本文の何文字目から何文字目にルビを降るか？
        public string txt;//ルビ文字の内容

    };

        //-------------------------------
        //本文のテキストの文字数は200文字まで。
        //ルビを振る箇所は10箇所まで。
        //リミッターは付けてないので注意。
        //-------------------------------

        GameObject[] ruby_txt = new GameObject[10];//ルビ文字表示用のテキストオブジェクト
        ruby_data[] ruby_d = new ruby_data[10];//ルビ情報
        Vector2[] mojipos = new Vector2[200];//各文字の座標データ

        int ruby_alot;//ルビ箇所の総数

        public new void OnEnable()
        {
        }

        public new void OnDisable()
        {

        }

        public new void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < ruby_alot; i++)
            {
                Destroy(ruby_txt[i]);
            }
        }

        public void ModifyMesh(Mesh mesh) { }
        public void ModifyMesh(VertexHelper verts)
        {
            if (!this.IsActive())
            {
                return;
            }

            List<UIVertex> vertexList = new List<UIVertex>();
            verts.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            verts.Clear();
            verts.AddUIVertexTriangleStream(vertexList);

        }

        void ModifyVertices(List<UIVertex> vertexList)
        {

            // １文字が６頂点で構成されてる
            // mojipos[] に各文字の中心座標を格納している。
            int cnt = 0;//本文の文字数

            for (int i = 0; i < vertexList.Count; i += 6)
            {
                mojipos[cnt] = new Vector2((vertexList[i].position.x + vertexList[i + 2].position.x) * 0.5f, (vertexList[i].position.y + vertexList[i + 2].position.y) * 0.5f);
                cnt++;
            }

            //ルビ文字の準備　その２
            //表示位置を決める。（ルビ文字の表示位置は ruby_d [i].num1 番目の文字とruby_d [i].num2 番目の文字の中間地点としている）

            float dy = _Text.fontSize * 0.8f;//どれだけ上にルビを表示するか。(フォントサイズの大きさx0.8)分上にずらして表示。

            for (int i = 0; i < ruby_alot; i++)
            {
                ruby_txt[i].transform.localPosition =
                    new Vector3((mojipos[ruby_d[i].num1].x + mojipos[ruby_d[i].num2].x) * 0.5f, (mojipos[ruby_d[i].num1].y + mojipos[ruby_d[i].num2].y) * 0.5f + dy, 0);
            }

        }

    //-----------------------
    //
    //ルビ記号を探して、ルビデータと本文を分離する。
    //
    //-----------------------
    public string cut_ruby(string txt1)
    {
        int n1 = txt1.IndexOf('[');//記号が何文字目にあるか？
        int n2 = txt1.IndexOf(':');//記号が何文字目にあるか？
        int n3 = txt1.IndexOf(']');//記号が何文字目にあるか？

        //記号が発見できなかったら何もしない。
        if (n1 == -1 || n2 == -1 || n3 == -1)
        {
            return txt1;//そのまま本文を返す。
        }

        string sub1 = txt1.Substring(0, n1);  // 先頭から'['までの部分
        string sub2 = txt1.Substring(n1 + 1, n2 - n1 - 1);//  '['〜':'の部分
        string sub3 = txt1.Substring(n2 + 1, n3 - n2 - 1);//  ':'〜']'の部分 (ルビ文字の部分)
        string sub4 = txt1.Substring(n3 + 1);   //  ']'以降の部分


        ruby_d[ruby_alot].num1 = n1;//何文字目にルビを降るか？
        ruby_d[ruby_alot].num2 = n2 - 2;//何文字目にルビを降るか？
        ruby_d[ruby_alot].txt = sub3;
        ruby_alot++;

        return cut_ruby(sub1 + sub2 + sub4);//再帰的に処理する。
    }
}
