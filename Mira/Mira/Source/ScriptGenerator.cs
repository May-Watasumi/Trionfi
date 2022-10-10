using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

using HPdf;
using TinyCsvParser;
using TinyCsvParser.Mapping;

using Trionfi;

namespace Mira
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class PDFInfo
	{
		public System.Drawing.Font font;
		public float marginV;
		public float marginH;
		public string Title;
		public string Author;
	};

	public class ScriptGenerator
	{
		public string highLightName = "";
		public float referenceFontSize = 16.0f;
		public float highLightFontSize = 19.0f;
		public float textBeginRatio = 0.20f;
		public float borderPosY = 0.0f;

		public int textMaxLength;
		public float lineHeight;

		const float _INCH_ = 25.4f;
		const float A4_width_point = 297.0f * 72.0f / _INCH_;   //830くらい
		const float A4_height_point = 210.0f * 72.0f / _INCH_;      //597くらい

		HPdfPage hPage = null;
		HPdfPoint textPos;
		PDFInfo pdfInfo;

		public ScriptGenerator()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		Dictionary<string, TRActorInfo> actorInfo = new Dictionary<string, TRActorInfo>();

		public Dictionary<string, TRActorInfo> ReadActorCSV(string filePath, string encode)
		{
			StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(encode));// "Shift_JIS"));
			string text = sr.ReadToEnd();

			return TREnviromentCSVLoader.LoadActorInfo(LocalizeID.JAPAN, text);
		}

		public void AddPage(HPdfDoc _hPdf)
		{
			hPage = _hPdf.AddPage();
			hPage.SetSize(HPdfPageSizes.HPDF_PAGE_SIZE_A4, HPdfPageDirection.HPDF_PAGE_LANDSCAPE);
			textPos = hPage.GetCurrentTextPos();
			textPos.x = A4_width_point - pdfInfo.marginH;
			textPos.y = A4_height_point - pdfInfo.marginV;

			float borderHeight = (A4_height_point - pdfInfo.marginV * 2) * textBeginRatio;
			borderPosY = A4_height_point - borderHeight - pdfInfo.marginV;

			hPage.SetLineWidth(1.0f);
			hPage.MoveTo(pdfInfo.marginH, borderPosY);
			hPage.LineTo(A4_width_point - pdfInfo.marginH, borderPosY);
			hPage.Stroke();
		}

		public void Output(string filePath, string outputPath, PDFInfo _pdfInfo, string encode)
		{
			TRTagInstance tagInstance = new TRTagInstance();
			StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(encode));// "Shift_JIS"));
			string text = sr.ReadToEnd();
			tagInstance.CompileScriptString(text);
			sr.Close();
			/*
					string csvFileName = textBox3.Text + "\\" + Path.GetFileNameWithoutExtension(file) + ".csv";

					StreamWriter sw = new StreamWriter(csvFileName, false, Encoding.GetEncoding("Shift_JIS"));

					string message;
					string name = string.Empty;
			*/


			//		sw.Close();
			pdfInfo = _pdfInfo;

			HPdfDoc hPdf = new HPdfDoc();
			hPdf.SetInfoAttr(HPdfInfoType.HPDF_INFO_AUTHOR, pdfInfo.Author);
			hPdf.SetInfoAttr(HPdfInfoType.HPDF_INFO_TITLE, pdfInfo.Title);
			hPdf.UseJPFonts();
			uint sd =  hPdf.UseJPEncodings();
			uint dsd =  hPdf.UseUTFEFonts();

			//		hPdf.SetPermission(HPdfDoc.HPDF_ENABLE_PRINT);

			HPdfFont hFont1 = hPdf.GetFont("MS-Gothic,Bold", "90ms-RKSJ-V");
			HPdfFont hFont2 = hPdf.GetFont("MS-Mincho", "90ms-RKSJ-V");
			HPdfFont hFont3 = hPdf.GetFont("MS-Gothic", "90ms-RKSJ-H");

			//string ddd = hPdf.LoadTTFontFromFile("C:\\Windows\\Fonts\\msgothic.ttc", true);

			AddPage(hPdf);
			//		HPdfPoint textPos = hPage.GetCurrentTextPos();
			//		try
			//		{

			string nameText = string.Empty;
			string voiceID = string.Empty;
			foreach (AbstractComponent tag in tagInstance.arrayComponents)
			{
				if (tag.tagName == "Audio")
				{
					if (tag.tagParam["buf"].Literal() == ((int)TRAudioID.VOICE1).ToString())
						voiceID = tag.tagParam["storage"].Literal();
				}
				if (tag.tagName == "Name")
				{
					nameText = tag.tagParam["val"].Literal(string.Empty);
				}
				if (tag.tagName == "Message")
				{
					string _text = tag.tagParam["val"].Literal(string.Empty);

					float fontSize = referenceFontSize;

					float maxLineLength = borderPosY / (referenceFontSize+1) - 1;
					int lineCount = 0;

//					uint writtenLength = 0;
//					hFont2.MeasureText(_text, (uint)_text.Length, borderPosY, referenceFontSize, 0, 0, HPdfDoc.HPDF_TRUE, ref maxLineLength);

					lineCount = (int)(_text.Length / maxLineLength);
					
					if(!string.IsNullOrEmpty(voiceID))
						lineCount++;

					if (_text.Length % (int)maxLineLength != 0)
						lineCount++;

					if ( textPos.x - referenceFontSize / 2.0f * lineCount * 1.35f < pdfInfo.marginH)
						AddPage(hPdf);

					if (!string.IsNullOrEmpty(voiceID))
					{
						hPage.SetFontAndSize(hFont3, referenceFontSize/1.65f);

						// Get the radian value
						float fRad = 270.0f * 3.141592f / 180.0f;

						hPage.BeginText();
						hPage.SetTextMatrix((float)Math.Cos(fRad), (float)Math.Sin(fRad), -(float)Math.Sin(fRad), (float)Math.Cos(fRad),textPos.x, textPos.y );
						//hPage.TextOut(textPos.x, textPos.y, voiceID);
						hPage.ShowText(voiceID);			
						hPage.EndText();

						textPos.x -= fontSize * 1.35f;

						voiceID = string.Empty;
					}
					if (nameText == highLightName)
					{
						fontSize = highLightFontSize;
						hPage.SetFontAndSize(hFont1, highLightFontSize);
						float textWidth = hPage.TextWidth(nameText);
						hPage.Rectangle(textPos.x - fontSize / 2 + 0.3f, textPos.y - fontSize / 2 + 0.3f, fontSize + 0.6f, textWidth + 0.6f);
						hPage.Stroke();
					}
//					else
					{
						hPage.SetFontAndSize(hFont2, referenceFontSize);
						//float textWidth = hPage.TextWidth(nameText);
					}

					hPage.BeginText();

					if (!String.IsNullOrEmpty(nameText))
						hPage.TextOut(textPos.x, textPos.y, nameText);

					if (!String.IsNullOrEmpty(_text))
					{
						//						hPage.TextRect( textPos.x - fontSize * 1.4f * lineCount + 400, borderPosY - 2, textPos.x+400, A4_height_point - pdfInfo.marginV, _text, HPdfTextAlignment.HPDF_TALIGN_RIGHT, ref writtenLength);
						//						hPage.SetTextLeading(fontSize * 1.35f);
						//						hPage.MoveTextPos(textPos.x, borderPosY - 2.0f);
						//						hPage.ShowText(_text);
						int len = 0;
						while (len < _text.Length)
						{
							string tempStr = string.Empty;
							int bufLen = 0;
							while (bufLen++ < maxLineLength && _text[len] != 0x0D && _text[len] != 0x0A)
							{
								if(_text[len] == '<')
									tempStr += '〈';
								else if(_text[len] == '>')
									tempStr += '〉';
								else if (_text[len] == '=')
									tempStr += '＝';
								else
									tempStr += _text[len];
	
								len++;
							}

							if (_text[len] == 0x0D || _text[len] == 0x0A)
								len++;
							hPage.TextOut(textPos.x, borderPosY - 2.0f, tempStr);

							textPos.x -= fontSize * 1.35f;
						}
#if FALSE

						for (int a = 0; a < _text.Length / 20; a++)
						{
							string tempStr = _text.Substring(a * 20, 20);

//							hPage.TextOut(textPos.x, borderPosY - 2.0f, tempStr);

							textPos.x -= fontSize * 1.35f;
						}
						if (_text.Length % 20 != 0)
						{
							string tempStr = _text.Substring(_text.Length / 20 * 20, _text.Length % 20);
							hPage.TextOut(textPos.x, borderPosY - 2.0f, tempStr);
							textPos.x -= fontSize * 1.35f;
						}
#endif
					}

					textPos.x -= fontSize * 1.35f;
					//textPos.y = pdfInfo.marginV;

					hPage.EndText();
				}
			}

			hPdf.SaveToFile(outputPath + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".pdf");
			hPdf.FreeDocAll();

			//		}
#if false
		catch (IOException ex)
		{
			string f = ex.Message;
			//			MessageBox.Show(ex.Message, "IOException");
		}
		finally
		{
			//ドキュメントを閉じる
//			doc.Close();
		}
#endif
		}
	}
}
