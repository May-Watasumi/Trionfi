using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{

/*	
--------------

[doc]
tag=playbgm
group=オーディオ関連
title=BGM再生

[desc]
BGMを再生します。
対応ファイル形式は .aif、.wav、.mp3、 .oggファイルです
 
[sample]

;BGMの再生
[playbgm wait=false time=1 storage="music"]

[param]

storage=再生する音楽ファイルを指定します
time=音が最大ボリュームになるまでにかかる秒数をしていします。つまり値をおおきくすると長い時間をかけて徐々に音楽が再生されます
vol=再生時のボリュームを指定します（0〜1.0）１が最大です。
wait=trueを指定することでtimeで指定した時間が完了するまで次の処理に移動しなくなります。
next=falseを指定すると次の処理に移動することなく、音楽を再生します。

[_doc]
--------------------
 */

	//音楽再生用のコンポーネント
	public class PlaybgmComponent : AbstractComponent {
		bool isWait;

		public PlaybgmComponent()
        {
			//必須項目
			arrayVitalParam = new List<string> {
				"storage" 
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "storage",""},
				{ "time","0"},
				{ "vol","1"}, //ボリューム 0〜1
				{ "wait","true"},
				{ "next","true"},
			};
		}

		public override void Start()
        {
			float time = float.Parse(paramDic["time"]);
			float volume = float.Parse(paramDic["vol"]);
			bool isWait =  paramDic["wait"] == "true" ? true : false;
			string storage = paramDic["storage"];
			string file = StorageManager.Instance.PATH_AUDIO_BGM + storage;
//			CompleteDelegate completeDelegate = this.complete;
			StatusManager.Instance.currentPlayBgm = storage;

            AudioObject audioObject = AudioManager.GetAudio(file, AudioType.Bgm);
	//		audioObject.time = time;
			audioObject.volume1 = volume;
			audioObject.completeDelegate = this.complete; //completeDelegate;
			audioObject.audioSource.loop = true;
			audioObject.isWait = false;

			if(StatusManager.Instance.onSkip)
				time = 0.0f;

			//アニメーション中にクリックして次に進めるかどうか。
			if (time > 0.0f && paramDic["wait"] != "false")
            { 
				nextOrder = false;
				StatusManager.Instance.Wait();
				audioObject.isWait = true;
			}

			audioObject.Play(time);
		}

		public void complete() {
			if(isWait) {
				StatusManager.Instance.NextOrder();
			}
/*
			nextOrder = false;
			if (paramDic ["wait"] == "true") {
				StatusManager.Instance.currentState = JokerState.NextOrder;
				//StatusManager.Instance.enableNextOrder = true;
				if(paramDic["next"] =="true")
					StatusManager.Instance.currentState = JokerState.NextOrder;
				//ToDo:
//				nextOrder = true;
//				this.gameManager.nextOrder();
			}
 */
		}
	}


	/*		
--------------

[doc]
tag=stopbgm
group=オーディオ関連
title=BGM停止

[desc]
BGMを停止します。
storageを指定することで複数再生中の場合特定の音源のみを停止することができます。
ファイルはbgmフォルダ以下に格納してください

[sample]

;BGMの停止
[stopbgm]

[param]

storage=再生する音楽ファイルを指定します
time=音が停止するまでにかかる秒数を指定します。徐々に音楽が停止していきます
wait=trueを指定することでtimeで指定した時間が完了するまで次の処理に移動しなくなります。

[_doc]
--------------------
 */

	//BGMのwait は　フェードインが終わるのを待つことができる。
	public class StopbgmComponent : AbstractComponent
	{
		private bool endComplete = false; //同時に複数止めた時にコールバックを一回しか実行させないため

		public StopbgmComponent() {
			//必須項目
			arrayVitalParam = new List<string> { 	};

			originalParamDic = new Dictionary<string,string>() {
				{ "storage",""},
				{ "time","1"},
				{ "wait","true"},
			};
		}

		public override void Start() {
			//storage が指定されている場合は、そのAudioのみ停止する
			StatusManager.Instance.Wait();
//			StatusManager.Instance.enableNextOrder = false;

			string storage = paramDic ["storage"];
			float time = float.Parse(paramDic ["time"]);
			//			string wait = paramDic ["wait"];

			CompleteDelegate completeDelegate = OnComplete;

			if (storage != "")
            {
				string file = StorageManager.Instance.PATH_AUDIO_BGM + storage;
				AudioManager.StopAudio(file, AudioType.Bgm, time, completeDelegate);
			}
            else
            {
				AudioManager.StopAudio("", AudioType.Bgm, time, completeDelegate);
			}

			//this.gameManager.scene.MessageSpeed = 0.02f;
			//this.gameManager.scene.coroutineShowMessage (message);
			StatusManager.Instance.currentPlayBgm = "";

			nextOrder = false;

            if (paramDic ["wait"] != "true")
            {
				StatusManager.Instance.NextOrder();

//				StatusManager.Instance.enableNextOrder = true;
//ToDo:
//				nextOrder = true;
//				this.gameManager.nextOrder();
			}
		}

		public void OnComplete()
        {
			if (paramDic ["wait"] == "true") {
				if (this.endComplete == false) {
					this.endComplete = true;
					StatusManager.Instance.NextOrder();
//					StatusManager.Instance.enableNextOrder = true;
					nextOrder = true;
//					this.gameManager.nextOrder();
				}
			}
		}

	}


	/*		
--------------

[doc]
tag=playse
group=オーディオ関連
title=効果音再生

[desc]
効果音を再生します。
対応ファイル形式は .aif、.wav、.mp3、 .oggファイルです
ファイルはsoundフォルダ以下に格納してください

[sample]

;効果音の再生
[playse storage="button"]


[param]

storage=再生する音楽ファイルを指定します
vol=効果音の音量を指定します(0〜1.0)の間で指定してください
wait=trueを指定することでtimeで指定した時間が完了するまで次の処理に移動しなくなります。
loop=trueを指定すると音楽を繰り返し再生します。

[_doc]
--------------------
 */


	//効果音は再生が停止するのを待つことができる。
	public class PlayseComponent : AbstractComponent {
		public PlayseComponent()
        {
			//必須項目
			arrayVitalParam = new List<string> {
				"storage" 
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "storage",""},
				{ "vol","1"},
				{ "wait","true"},
				{ "loop","false"},
			};
		}

		public override void Start() {
			StatusManager.Instance.Wait();
//			StatusManager.Instance.enableNextOrder = false;

			string storage = paramDic ["storage"];
			string file = StorageManager.Instance.PATH_AUDIO_SE + storage;

			//			string wait = paramDic ["wait"];

			CompleteDelegate completeDelegate = this.complete;
			AudioObject audioObject = AudioManager.GetAudio(file, AudioType.Sound);
//			audioObject.time = 0;
			audioObject.volume1  = float.Parse(paramDic["vol"]);
			audioObject.completeDelegate = completeDelegate;
			audioObject.audioSource.loop = bool.Parse(paramDic["loop"]);
			audioObject.Play();

			//this.gameManager.scene.MessageSpeed = 0.02f;
			//this.gameManager.scene.coroutineShowMessage (message);

			nextOrder = false;

			if (paramDic["wait"] != "true")
			{
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//ToDo:
//				nextOrder = true;
//				this.gameManager.nextOrder();
			}
		}

		public void complete() {
			if (paramDic ["wait"] == "true") {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//ToDo:
//				nextOrder = true;
//					this.gameManager.nextOrder();
			}
		}
	}

	/*		
--------------

[doc]
tag=stopse
group=オーディオ関連
title=効果音停止

[desc]
効果音を停止します。

[sample]

;効果音の再生
[stopse]

[param]

storage=再生する音楽ファイルを指定します
time=指定した秒数をかけて効果音が停止します
loop=wait 効果音が停止するまで待ちます

[_doc]
--------------------
 */

	public class StopseComponent:AbstractComponent
    {
		private bool endComplete = false; //同時に複数止めた時にコールバックを一回しか実行させないため

		public StopseComponent()
		{
			//必須項目
			arrayVitalParam = new List<string> { };

			originalParamDic = new Dictionary<string,string>() {
				{ "storage",""},
				{ "time","0"},
				{ "wait","true"},
			};
		}

		public override void Start() {
			//storage が指定されている場合は、そのAudioのみ停止する
//			StatusManager.Instance.enableNextOrder = false;
			StatusManager.Instance.Wait();

			string storage = paramDic ["storage"];
			float time = float.Parse(paramDic ["time"]);
			//			string wait = paramDic ["wait"];

			CompleteDelegate completeDelegate = this.complete;

			if (storage != "") {
				string file = StorageManager.Instance.PATH_AUDIO_SE + storage;
				AudioManager.StopAudio(file, AudioType.Sound, time, completeDelegate);
			} else {
				AudioManager.StopAudio("", AudioType.Sound, time, completeDelegate);
			}

			nextOrder = false;

			if (paramDic["wait"] != "true")
			{
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				this.gameManager.nextOrder();
			}
		}

		public void complete() {
			if (paramDic ["wait"] == "true") {

				if (this.endComplete == false) {
					this.endComplete = true;
					StatusManager.Instance.NextOrder();
//					StatusManager.Instance.enableNextOrder = true;
//					nextOrder = true;
//					this.gameManager.nextOrder();
				}
			}
		}
	}
}

