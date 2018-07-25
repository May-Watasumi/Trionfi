using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
        public Dictionary<string, int> labelInfo = new Dictionary<string, int>();

        public List<AbstractComponent> arrayComponents = new List<AbstractComponent>();

        public int currentComponentIndex = -1;      //-1は未初期化、0からスタート

		public bool CompileScriptString(string text)
        {
            ErrorLogger.Clear();

            TRScriptParser tagParser = new TRScriptParser(text);

            arrayComponents = tagParser.BeginParse();

            //ToDo:
            /*
            int _index = 0;
            foreach (AbstractComponent _component in arrayComponents)
            {
                if (_component.tagName == "label")
                    AddLabel(_component.tagParam["name"], _index);

                _index++;
            }
            */

            return ErrorLogger.ShowAll();
        }

        public bool CompileScriptFile(string storage)
        {
            //				string fullpath = /*useStoragePath ? StorageManager.Instance.PATH_SD_SCENARIO :*/ "";
            TextAsset script_text = TRResourceLoader.Instance.LoadObject(storage, TRAssetType.TextAsset) as TextAsset;

            return CompileScriptString(script_text.text);
        }

        public void AddLabel(string label_name, int index)
        {
            labelInfo[label_name] = index;
        }

        public int GetLabelPosition(string label_name)
        {
            if (string.IsNullOrEmpty(label_name) || !labelInfo.ContainsKey(label_name))
            {
                ErrorLogger.StopError("にラベル「" + label_name + "」が見つかりません。");
                return -1;
            }

            return labelInfo[label_name];
        }

        //0=デフォルト1=componentのフラグが立ってない-1シナリオ最後に
        public IEnumerator Run(int index = 0)
        {
            currentComponentIndex = index;

            if (currentComponentIndex < arrayComponents.Count)
            {
                AbstractComponent _tagComponent = arrayComponents[currentComponentIndex];

                _tagComponent.Before();

#if UNITY_EDITOR || DEVELOPMENT_BUILD || TRIONFI_DEBUG
                if(TRSystemConfig.Instance.showTag)
                {
                    string _params = "";

                    foreach(KeyValuePair<string, KeyValuePair<string, TRDataType>> key in _tagComponent.tagParam)
                    {
                        _params += " " + key.Key + "= " + key.Value.Key;
                    }
                    ErrorLogger.Log("[" + _tagComponent.tagName + _params +" ]");
                }
#endif
                _tagComponent.Execute();

                _tagComponent.After();
                yield return _tagComponent.TagAsyncWait();

                //ToDo:flag
                currentComponentIndex++;
            }
            yield return null;			
		}
	}
}
