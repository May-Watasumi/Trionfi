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

            TRScriptParser tagParser = new TRScriptParser(text);

            arrayComponents = tagParser.BeginParse();

            //エラーがあるときはtrue
            return !ErrorLogger.ShowAll();
        }
	}
}
