#if false
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Text;
using NovelEx;

namespace NovelEx {
	//ゲーム全体に関する情報を保持する
//	[Serializable]
	public class SaveGlobalObject {
		//global変数を保持する。ゲームごとに変わらない変数 global.x みたいなやつ
		//ToDo_Future:globalの同期周りを自動化したい
		public ParamDictionary globalVar = new ParamDictionary() ;
	}
}
#endif