using System.Collections;
using System.Collections.Generic;
using System;
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
    }
}
