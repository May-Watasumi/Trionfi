using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if !TR_PARSEONLY
using UnityEngine;
#endif

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
#if !TR_PARSEONLY
        [SerializeField]
#endif
        public TRTagList arrayComponents = new TRTagList();

#if TR_PARSEONLY || UNITY_EDITOR
        public string textIdentifiedScript;
        public string textDataCSV;
#endif
        //        [SerializeField]
        public List<bool> isJMessageReadFlags;// = new List<bool>();

        public TRMultiLanguageText textData;

        public void ReadTextData(string text)
		{
            textData = TREnviromentCSVLoader.LoadTextData(text);
        }

        public void SerializeBinary(string binFile)
		{
            //            string ggg0 = JsonUtility.ToJson(arrayComponents[0]);
            //            string ggg1 = JsonUtility.ToJson(arrayComponents[1]);
            //            string ggg2 = JsonUtility.ToJson(arrayComponents[2]);
            //            string ggg3 = JsonUtility.ToJson(arrayComponents[3]);
            //            string ggg = JsonUtility.ToJson(arrayComponents.tagList);
            //            string json = JsonConvert.SerializeObject(arrayComponents.tagList);
            //            AbstractComponent fgff =  JsonUtility.FromJson<AbstractComponent>(ggg3);

            //            return JsonConvert.SerializeObject(arrayComponents);  //JsonUtility.ToJson(arrayComponents);

            using (FileStream fs = new FileStream(binFile, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(fs, arrayComponents);
            }

        }

        public void DeserializeBinary(string binFile)
        {
            arrayComponents = new TRTagList();

            using (FileStream fs = new FileStream(binFile, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                arrayComponents = (TRTagList)bf.Deserialize(fs);
            }
        }

        public bool CompileScriptString(string text)
        {
            ErrorLogger.Clear();

            //改行コードをパースしやすくするために1byte化しておく。
            string _returnFixText = text.Replace("\r\n", "\n");

            TRScriptParser tagParser = new TRScriptParser(_returnFixText);

            arrayComponents = tagParser.BeginParse(string.Empty);

#if TR_PARSEONLY || UNITY_EDITOR
            textIdentifiedScript = tagParser.textIdentifiedScript;
            textDataCSV = tagParser.textDataCSV;
#endif
            //エラーがあるときはtrue
            return !ErrorLogger.ShowAll();
        }
        
        public string GetReadFlagJsonData()
        {
#if !TR_PARSEONLY || UNITY_EDITOR
            return Newtonsoft.Json.JsonConvert.SerializeObject(isJMessageReadFlags);
#else
            return string.Empty;
#endif
        }

        public void SetReadFlagJsonData(string jsonString)
        {
#if !TR_PARSEONLY || UNITY_EDITOR
            isJMessageReadFlags = Newtonsoft.Json.JsonConvert.DeserializeObject<List<bool>>(jsonString);
#endif
        }

        [Conditional("UNITY_EDITOR"), Conditional("TR_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        public void GetTagInfo()
        {
            for (int a = 0; a < arrayComponents.Count; a++)
            {
                AbstractComponent _tagComponent = arrayComponents[a];
                ErrorLogger.Log("tag:" + _tagComponent.tagName);

                foreach (KeyValuePair<string, Jace.Operations.VariableCalcurator> pair in _tagComponent.tagParam)
                {
                    ErrorLogger.Log(pair.Key + "=" + pair.Value.paramString);
                }
            }
        }
    }
}
