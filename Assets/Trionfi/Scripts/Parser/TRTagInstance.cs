using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
        public int currentComponentIndex = -1;

        public TRTagList arrayComponents = new TRTagList();

		public bool CompileScriptString(string text)
        {
            ErrorLogger.Clear();

            //改行コードをパースしやすくするために1byte化しておく。
            string _returnFixText = text.Replace("\r\n", "\n");

            TRScriptParser tagParser = new TRScriptParser(_returnFixText);

            arrayComponents = tagParser.BeginParse();

            //エラーがあるときはtrue
            return !ErrorLogger.ShowAll();
        }

        public bool Jump(string label)
        {
            if (!arrayComponents.labelPos.ContainsKey(label))
                return false;

            currentComponentIndex = arrayComponents.labelPos[label];
            return true;
        }
	}
}
