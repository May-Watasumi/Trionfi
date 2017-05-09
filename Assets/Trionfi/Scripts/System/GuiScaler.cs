using System;
using UnityEngine;
using Novel;

public class GuiScaler
{

	private float fixWidth = 1280f; 
	private float fixHeight = 720f; 

	private float corTall = 1.0f;
	private float corWide = 1.0f;

	private int fixFontSize ;
	private float pixX;
	private float pixY;
	private GUIText guiText;

	public float ratio = 1;

	public GuiScaler(GUIText guiText){
/*
		//画面サイズの設定状況によって、縦横比を変える
//		JokerSetting jokerSetting = NovelSingleton.GameManager.jokerSetting;

		//縦長の場合入れ替える
		if (jokerSetting.aspect.x < jokerSetting.aspect.y) {
			float tmp = this.fixWidth;
			this.fixWidth = this.fixHeight;
			this.fixHeight = tmp;
		}

		this.guiText = guiText;

		fixFontSize = this.guiText.fontSize;
		pixX = this.guiText.pixelOffset.x;
		pixY = this.guiText.pixelOffset.y;
*/
	}

	public void fontResizeButton(){

		//現在のscreen Size取得
		float scWidth  = Screen.width;
		float scHeight = Screen.height;
		//float winAspect = scWidth / scHeight;

		//初期の画面サイズと現在の画面サイズの比率を計算     
		float wdRatio = 100.0f / (fixWidth / scWidth);
		float heRatio = 100.0f / (fixHeight / scHeight);

		//縦横時の判別
		float  ratio ;//倍率
		if(scWidth < scHeight){
			//tallの場合は横で比率を合わせる
			ratio = wdRatio * corTall;
		}else{
			//wideの場合は縦で比率を合わせる
			ratio = heRatio * corWide;
		}

		//リサイズするフォントサイズ
		float reFontSize  = fixFontSize *  (ratio / 100);
		//リサイズフォント位置
		float rePixOffsetX = pixX *  (ratio / 100);
		float rePixOffsetY = pixY *  (ratio / 100);

		this.ratio = ratio;

		//フォントサイズ変更
		guiText.fontSize = (int)reFontSize;
		Vector2 v = new Vector2 (rePixOffsetX,rePixOffsetY);
		guiText.pixelOffset = v;

	}


	public void fontResize (){

		if (this.guiText == null)
			return;

		//現在のscreen Size取得
		float scWidth  = Screen.width;
		float scHeight = Screen.height;
		//float winAspect = scWidth / scHeight;

		//初期の画面サイズと現在の画面サイズの比率を計算     
		float wdRatio = 100.0f / (fixWidth / scWidth);
		float heRatio = 100.0f / (fixHeight / scHeight);

		//縦横時の判別
		float  ratio ;//倍率
		if(scWidth < scHeight){
			//tallの場合は横で比率を合わせる
			ratio = wdRatio * corTall;
		}else{
			//wideの場合は縦で比率を合わせる
			ratio = heRatio * corWide;
		}

		//リサイズするフォントサイズ
		float reFontSize  = fixFontSize *  (ratio / 100);
		//リサイズフォント位置
		float rePixOffsetX = pixX *  (ratio / 100);
		float rePixOffsetY = pixY *  (ratio / 100);

		this.ratio = ratio;

		//フォントサイズ変更
		guiText.fontSize = (int)reFontSize;
		Vector2 v = new Vector2 (rePixOffsetX,rePixOffsetY);
		guiText.pixelOffset = v;

	}

}
