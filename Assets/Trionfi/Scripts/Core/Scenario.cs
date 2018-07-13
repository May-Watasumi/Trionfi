using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
	[Serializable]
	public class TagInstance
    {
		public string name;
		public List<AbstractComponent> arrayComponent;
		public Dictionary<string,int> dicLabel = new Dictionary<string,int>();

		public TagInstance() { }

		public TagInstance(string scenario_name, List<AbstractComponent> list) {
			name = scenario_name;
			arrayComponent = list;
		}
        /*
        public TagInstance( string scriptText )
        {
            name = "__EXTERNAL__";
            arrayComponent = TRScriptParser.Instance.parseScript(scriptText);
        }
        */
		public void AddLabel(string label_name,int index)
        {
			this.dicLabel [label_name] = index;
		}

		public int GetLabelPosition(string label_name)
        {
			if(label_name == "")
				return -1;

			if(!this.dicLabel.ContainsKey (label_name))
				ErrorLogger.stopError(name + "にラベル「" + label_name + "」が見つかりません。");	

			return this.dicLabel[label_name];
		}
	}
}
