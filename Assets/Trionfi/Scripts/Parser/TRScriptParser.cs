using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Trionfi;

namespace Trionfi
{
    enum TRParserError
    {
        EOF,
        UnmatchType
    }

    class TRParserExecption : System.Exception
    {
        public TRParserError error;
        public TRParserExecption(TRParserError _error) { _error = error; }
    }

    public class TRParserBase
    {
        protected const string nameSpace = "Trionfi";
        protected const string operatorString = "!\"#$%&'()=-^~\\|@`{}*:;+<,>.?/";

        public static System.Globalization.TextInfo tf = new System.Globalization.CultureInfo("ja-JP"/*"en-US"*/, false).TextInfo;

        //使用側がリセットする
        public static int lineCount = 0;

        protected int currentPos = 0;
        protected int startPos = 0;
        protected int endPos = 0;

        protected char[] charArray;

        public TRParserBase(string statement)
        {
            charArray = statement.ToCharArray();
        }

        protected bool SkipSpace()
        {
            while(currentPos < charArray.Length)
            {
                if (charArray[currentPos] == '\r' || charArray[currentPos] == '\n')
                    lineCount++;

                if (!IsSpace(charArray[currentPos]))
                    return false;

                ++currentPos;
            }

            return true;
        }

        protected bool IsEOF()
        {
            return !(currentPos < charArray.Length);
        }

        protected bool SkipSpaceWithoutReturn()
        {
            while (currentPos < charArray.Length)
            {
                if(charArray[currentPos] != ' ' || charArray[currentPos] != '\t')
                    return false;

                ++currentPos;
            }

            return true;
        }

        protected string GetString(char terminator)
        {
            string buffer = "";

            while (charArray[currentPos] != terminator && !IsEOF())
            {
                if (charArray[currentPos] == '\r' || charArray[currentPos] == '\n')
                    lineCount++;

                buffer += charArray[currentPos++];
            }

            return buffer;
        }

        protected string ReadLine()
        {
            string buffer = "";

            startPos = currentPos;

            while (!IsEOF() && charArray[currentPos] != '\r' && charArray[currentPos] != '\n')
                buffer += charArray[currentPos++];

            endPos = currentPos;

            return buffer.TrimEnd();
        }

        protected static bool IsSpace(char character)
        {
            return character == '\r' || character == '\n' || character == ' ' || character == '\t';
        }

        protected static bool IsNumber(char character)
        {
            return character >= '0' && character <= '9';
        }

        protected static bool IsAlphabet(char character)
        {
            return (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z');
        }

        protected static bool IsPartOfVariable(char character)
        {
            return IsAlphabet(character) || IsNumber(character) || character == '_';
        }
    }

    public class TRTagParser : TRParserBase
    {
        public TRTagParser(string statement) : base(statement) { }

        protected string tagName;
        protected TRVariableDictionary paramList = new TRVariableDictionary();

        //you must check statement is not empty. 
        public bool GetFirstToken()
        {
            paramList.Clear();

            tagName = "";

            if(SkipSpace())
                throw new TRParserExecption(TRParserError.UnmatchType);

            if (!IsAlphabet(charArray[currentPos]))
                throw new TRParserExecption(TRParserError.UnmatchType);

            while(!IsEOF() && IsPartOfVariable(charArray[currentPos]))
                tagName += charArray[currentPos++];

            if (IsEOF() || IsSpace(charArray[currentPos]))
                endPos = currentPos;
            else
                throw new TRParserExecption(TRParserError.UnmatchType);

            return true;
        }

        protected bool GetParamToken()
        {
            //SkiSpaceは直前で実行している前提
            string leftParam = "";
            string rightParam = "";

            if (SkipSpace())
                return false;
            //left side is identifier ?
            else if (!IsAlphabet(charArray[currentPos]))
                throw new TRParserExecption(TRParserError.UnmatchType);

            while (IsPartOfVariable(charArray[currentPos]))
                leftParam += charArray[currentPos++];

            //splitter is '=' ?
            if (SkipSpace() || currentPos >= charArray.Length || charArray[currentPos] != '=')
                throw new TRParserExecption(TRParserError.UnmatchType);

            ++currentPos;

            if (SkipSpace())
                throw new TRParserExecption(TRParserError.UnmatchType);

            //literal
            if (charArray[currentPos] == '\"')
            {
                bool isEnd = false;
                do
                {
                    if (++currentPos >= charArray.Length)
                        throw new TRParserExecption(TRParserError.UnmatchType);

                    isEnd = charArray[currentPos] == '\"' && charArray[currentPos - 1] != '\\';

                    if(!isEnd)
                        rightParam += charArray[currentPos];

                } while (!isEnd);

                paramList[leftParam] = new TRVariable(rightParam);

                currentPos++;
                return true;
            }
            else
            {
                while(currentPos < charArray.Length && !IsSpace(charArray[currentPos]))
                {
                    rightParam += charArray[currentPos++];
                }

                if (string.IsNullOrEmpty(rightParam))
                    throw new TRParserExecption(TRParserError.UnmatchType);

                int _isInt;
                double _isFloat;

                if (rightParam[0] == '0' && rightParam.Length > 2 && (rightParam[1] == 'x' || rightParam[1] == 'X') &&
                    int.TryParse(rightParam, System.Globalization.NumberStyles.AllowHexSpecifier, null, out _isInt)
                   )
                    paramList[leftParam] = new TRVariable(rightParam, TRDataType.Hex);
                else if (int.TryParse(rightParam, out _isInt))
                    paramList[leftParam] = new TRVariable(rightParam, TRDataType.Int);
                else if (double.TryParse(rightParam, out _isFloat))
                    paramList[leftParam] = new TRVariable(rightParam, TRDataType.Float);
                else
                    paramList[leftParam] = new TRVariable(rightParam, TRDataType.String);

                return true;
            }
        }

        public AbstractComponent Parse()
        {
            try
            {
                if (GetFirstToken())
                {
                    do
                    {
                        if (SkipSpace())
                            break;
                    } while (GetParamToken());

                    string className = nameSpace + "." + tf.ToTitleCase(tagName) + "Component";

                    AbstractComponent _component = null;

                    // リフレクションで動的型付け
                    Type masterType = Type.GetType(className);

                    if (masterType != null)
                        _component = (AbstractComponent)Activator.CreateInstance(masterType);
                    else
                    {
                        _component = new UnknownComponent();
                        paramList["name"] = new TRVariable(tagName);
                    }

                    _component.tagParam = paramList;
#if UNITY_EDITOR && TR_DEBUG
                    _component.Validate();
#endif
                    return _component;
                }
                else
                {
                    throw new TRParserExecption(TRParserError.UnmatchType);
                }

            }
            catch (TRParserExecption error)
            {
                if (currentPos >= charArray.Length)
                    ErrorLogger.Log("Statement is aborted");
                else
                    ErrorLogger.Log("Unmatched word");
            }

            return null;
        }
    }

    public class TRTagList : List<AbstractComponent>
    {
        public Dictionary<string, int> labelPos = new Dictionary<string, int>();
    }

    public class TRScriptParser : TRParserBase
    {
        //名前仕切り文字
        const string nameSplitter = "【】";

        public TRScriptParser(string statement) : base(statement) { }

        public TRTagList BeginParse()
        {
            TRTagList result = new TRTagList();

            AbstractComponent _tagComponent = null;

            string textBuffer = "";

            while (currentPos < charArray.Length)
            {
                bool isText = false;

                SkipSpaceWithoutReturn();

                if (charArray[currentPos] == '\r' || charArray[currentPos] == '\n')
                {
                    lineCount++;
                }
                else if (charArray[currentPos] == '#')
                {
                    ReadLine();
                }
                //lineobject
                else if (charArray[currentPos] == '@')
                {
                    string statement = ReadLine();

                    TRTagParser tagParser = new TRTagParser(statement);
                    _tagComponent = tagParser.Parse();

                    if (_tagComponent != null)
                        result.Add(_tagComponent);
                }
                //comment
                else if ((charArray[currentPos] == '/' && charArray[currentPos + 1] == '/') || charArray[currentPos] == ';')
                {
                    ReadLine();
                }
                //comments
                else if (charArray[currentPos] == '/' && charArray[currentPos + 1] == '*')
                {
                    while(!(charArray[currentPos] == '*' && charArray[currentPos + 1] == '/'))
                        currentPos++;
                }
                //label
                else if (charArray[currentPos] == '*')
                {
                    string labelBuffer = ReadLine();
                    result.labelPos[labelBuffer] = result.Count-1;
                }
                //tag
                else if (charArray[currentPos] == '[')
                {
                    currentPos++;
                    string _tagParam = GetString(']');

                    //非ASCIIで始まるときはアクタータグと認識する
                    if (_tagParam[0] < 0 || _tagParam[0] > 127)
                    {
                        _tagComponent = new ActorComponent();
                        _tagComponent.tagParam = new TRVariableDictionary();
                        _tagComponent.tagParam["param"] = new TRVariable(_tagParam);
                    }
                    else
                    {
                        TRTagParser tagParser = new TRTagParser(_tagParam);
                        _tagComponent = tagParser.Parse();
                    }

                    if (_tagComponent != null)
                        result.Add(_tagComponent);
                }
                //text
                else
                {
                    string _temp = ReadLine();
                    if (_temp[0] == nameSplitter[0] && _temp[_temp.Length - 1] == nameSplitter[1])
                    {
                        string _value = _temp.Remove(0, 1);
                        _temp = _value.Remove(_value.Length - 1, 1);

                        _tagComponent = new NameComponent();
                        _tagComponent.tagParam = new TRVariableDictionary();
                        _tagComponent.tagParam["val"] = new TRVariable(_temp);
                        result.Add(_tagComponent);
                    }
                    else
                    {
                        isText = true;
                        textBuffer += _temp + "\n";
                    }
                }

                if (!isText && !string.IsNullOrEmpty(textBuffer))
                {
                    _tagComponent = new MessageComponent();
                    _tagComponent.tagParam = new TRVariableDictionary();
                    _tagComponent.tagParam["val"] = new TRVariable(textBuffer);
                    result.Add(_tagComponent);
                    textBuffer = "";
                }

                currentPos++;
            }

            if(!string.IsNullOrEmpty(textBuffer))
            {
                _tagComponent = new MessageComponent();
                _tagComponent.tagParam = new TRVariableDictionary();
                _tagComponent.tagParam["val"] = new TRVariable(textBuffer);
                result.Add(_tagComponent);
            }

            return result;
        }
    }
}

#if false
        [Conditional("UNITY_EDITOR"), Conditional("TRIONFI_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        void Dump(List<LineObject> list, string path)
        {
            System.IO.StreamWriter stream = new System.IO.StreamWriter(path, false, Encoding.GetEncoding("utf-8"));
            foreach (LineObject line in list)
            {
                stream.WriteLine(line.line_num.ToString() + ":" + line.line);
            }
            stream.Close();
        }

        private string errorMessage = "";
        private string warningMessage = "";
        public bool onRegistMacro = false;

        [SerializeField]
        public bool ignoreCR = true;

        [SerializeField]
        public string actorMarker = "【】";

        [SerializeField]
        public string actorTag = "talk_name";

        struct LineObject
        {
            public int line_num;
            public string line;
            public LineObject(int _line_num, string _line)
            {
                line_num = _line_num;
                line = _line;
            }
        }

        //予約語
        private List<string> RevervedWords = new List<string>
        {
            "#SetignoreCR",
            "#ResetignoreCR",
            "#include",
            "#define",
        };

        public bool ParsePreproseccor(string lineText)
        {
            switch (lineText) {
                case "#SetignoreCR":
                    ignoreCR = true;
                    break;
                case "#ResetignoreCR":
                    ignoreCR = false;
                    break;
                default:
                    return false;
            }
            return true;
        }

        //パースしたタグクラスのListを返す
        public List<AbstractComponent> Parse(string script_text)
        {
            List<AbstractComponent> components = new List<AbstractComponent>();
            string[] lines = script_text.Split('\n');

            List<LineObject> line_objects = new List<LineObject>();

            bool isCommentNow = false;

            //lines の前に、一文字ずつ解析してすべての要素を分解する必要がある
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                //EX:
                if (line == "\r" || line == "\n" || line == "\r\n" || line == "") {
                    line_objects.Add(new LineObject(i + 1, "\r"));
                    continue;
                }

                line = line.Replace("\r", "").Replace("\n", "");

                //Debug.Log (line);

                if (line == "")
                    continue;

                string firstChar = line[0].ToString();

                //コメント開始
                if (line.IndexOf("/*") != -1)
                    isCommentNow = true;

                if (line.IndexOf("*/") != -1) {
                    isCommentNow = false;
                    continue;
                }

                if (isCommentNow == true)
                    continue;

                // ;で始まってたらコメントなので無視する
                if (firstChar == ";")
                    continue;

                line = line.Replace("|", "\r\n");

                //ラベルを表します
                if (line.IndexOf("*/") == -1 && firstChar == "*")
                    line = "[label name='" + line.Replace("*", "").Trim() + "' ]";

                //１行の命令なので、残りの文字列をまとめて、タグ作成に回す
                //１行のタグ命令にして渡す
                if (firstChar == "@")
                    line = "[" + line.Replace("@", "") + "]";

                if (firstChar == "#") {
                    line_objects.Add(new LineObject(i + 1, line));
                    continue;
                }

                bool flag_now_tag = false;
                StringBuilder tag_line = new StringBuilder();

                for (int k = 0; k < line.Length; k++) {
                    string c = line[k].ToString();

                    if (c == "[" && flag_now_tag == true) {
                        line_objects.Add(new LineObject(i + 1, tag_line.ToString()));
                        flag_now_tag = false;
                        tag_line = new StringBuilder();
                    }

                    tag_line.Append(c);

                    //最後の一文字の場合
                    if (k == line.Length - 1) {
                        line_objects.Add(new LineObject(i + 1, tag_line.ToString()));
                        continue;
                    }

                    flag_now_tag = true;

                    if (c == "]") {
                        flag_now_tag = false;
                        line_objects.Add(new LineObject(i + 1, tag_line.ToString()));
                        tag_line = new StringBuilder();
                    }
                }
            }


            bool isText = false;

            foreach (LineObject lo in line_objects) {
                string line = lo.line;
                int line_num = lo.line_num;

                string firstChar = line[0].ToString();

                //プリプロセッサ的なアレ。
                //ToDo:名前タグを【】にする
                if (!string.IsNullOrEmpty(actorMarker)) {
                    if (firstChar == actorMarker.Substring(0, 1)) {
                        if (actorMarker.Length <= 1) {
                            line = "[" + actorTag/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
                            AbstractComponent cmp = MakeTag(line, line_num);
                            components.Add(cmp);
                        }
                        else if (line[line.Length - 1] == actorMarker[1]) {
                            line = line.Replace(actorMarker[1].ToString(), "");
                            line = "[" + actorTag/*  talk_name */ + " val='" + line.Replace(firstChar, "") + "' ]";
                            AbstractComponent cmp = MakeTag(line, line_num);
                            components.Add(cmp);
                        }
                        else { /*error?*/}
                        continue;
                    }
                }

                if (firstChar == "#") {
                    if (RevervedWords.Contains(line)) {
                        ParsePreproseccor(line);
                    }
                    else {
                        line = "[" + actorTag/* talk_name */ + " val='" + line.Replace("#", "") + "' ]";
                        AbstractComponent cmp = MakeTag(line, line_num);
                        components.Add(cmp);
                    }
                    continue;
                }

                if (line == "\r") {
                    //ToDo:直前のRを消す
                    if (isText == true && ignoreCR)
                        components.Add(new PComponent());

                    isText = false;

                    continue;
                }

                //テキストの場合
                if (firstChar != "[" && firstChar != "@") {
                    line = "[message val=\"" + line + "\"]";
                    firstChar = "[";
                    isText = true;
                }
                else
                    isText = false;

                if (firstChar == "[" || firstChar == "@") {

                    AbstractComponent cmp = MakeTag(line, line_num);

                    //リストに追加
                    components.Add(cmp);
                }

                //ToDo:
                //				if(isText == true && ignoreCR)
                //					components.Add(new RComponent());

            }
            return components;
        }
    }
}

            // 実行中のアセンブリを取得
            Assembly assembly = Assembly.GetExecutingAssembly();

            // インスタンスを生成
            AbstractComponent cmp = (AbstractComponent)assembly.CreateInstance(
　　              className,    // 名前空間を含めたクラス名
                  false,        // 大文字小文字を無視するかどうか
                  BindingFlags.CreateInstance,      // インスタンスを生成
                  null,         // 通常はnullでOK
                  new object[] { tag },    // コンストラクタの引数
                  null,         // カルチャ設定（通常はnullでOK）
                  null          // ローカル実行の場合はnullでOK
                );
#endif
