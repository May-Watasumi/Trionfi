using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx
{
	[Serializable]
	public class Scenario
    {
		public string name;
		public List<AbstractComponent> arrayComponent;
		public Dictionary<string,int> dicLabel = new Dictionary<string,int>();

		public Scenario() { }

		public Scenario (string scenario_name, List<AbstractComponent> list) {
			name = scenario_name;
			arrayComponent = list;
		}

        public Scenario( string scriptText )
        {
            name = "__EXTERNAL__";
            arrayComponent = NovelParser.Instance.parseScript(scriptText);
        }

		public void addLabel(string label_name,int index)
        {
			this.dicLabel [label_name] = index;
		}

		public int getIndex(string label_name)
        {
			if(label_name == "")
				return -1;

			if(!this.dicLabel.ContainsKey (label_name))
				ErrorLogger.stopError(name + "にラベル「" + label_name + "」が見つかりません。");	

			return this.dicLabel[label_name];
		}
	}
}
