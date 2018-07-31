using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
        public string scriptID = "";

        public TRTagList arrayComponents = new TRTagList();

        public int currentComponentIndex = -1;

		public bool CompileScriptString(string text)
        {
            ErrorLogger.Clear();

            TRScriptParser tagParser = new TRScriptParser(text);

            arrayComponents = tagParser.BeginParse();

            return ErrorLogger.ShowAll();
        }

        public bool CompileScriptFile(string storage)
        {
            TextAsset script_text = TRResourceLoader.Instance.LoadObject(storage, TRAssetType.TextAsset) as TextAsset;

            scriptID = storage;

            return CompileScriptString(script_text.text);
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
