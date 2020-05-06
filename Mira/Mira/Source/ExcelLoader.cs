using System.Collections;
using System.Runtime.InteropServices;

namespace ExcelLoader
{
	class ExcelSheetReader
	{
		/// <summary>
		/// エクセルファイルの指定したシートを2次元配列に読み込む.
		/// </summary>
		/// <param name="filePath">エクセルファイルのパス</param>
		/// <param name="sheetIndex">シートの番号 (1, 2, 3, ...)</param>
		/// <param name="startRow">最初の行 (>= 1)</param>
		/// <param name="startColmn">最初の列 (>= 1)</param>
		/// <param name="lastRow">最後の行</param>
		/// <param name="lastColmn">最後の列</param>
		/// <returns>シート情報を格納した2次元文字配列. ただしファイル読み込みに失敗したときには null.</returns>
		public ArrayList Read(string filePath, int sheetIndex,
							  int startRow, int startColmn,
							  int lastRow, int lastColmn)
		{
			// ワークブックを開く
			if (!Open(filePath)) { return null; }

			Microsoft.Office.Interop.Excel.Worksheet sheet = mWorkBook.Sheets[sheetIndex];
			sheet.Select();

			var arrOut = new ArrayList();

			for (int r = startRow; r <= lastRow; r++)
			{
				// 一行読み込む
				var row = new ArrayList();
				for (int c = startColmn; c <= lastColmn; c++)
				{
					var cell = sheet.Cells[r, c];

					if (cell == null || cell.Value == null) { row.Add(""); }

					row.Add(cell.Value);
				}

				arrOut.Add(row);
			}

			// ワークシートを閉じる
			Marshal.ReleaseComObject(sheet);
			sheet = null;

			// ワークブックとエクセルのプロセスを閉じる
			Close();

			return arrOut;
		}


		/// <summary>
		/// 指定されたパスのエクセルワークブックを開く
		/// </summary>
		/// <param name="filePath">エクセルワークブックのパス(相対パスでも絶対パスでもOK)</param>
		/// <returns>エクセルワークブックのオープンに成功したら true. それ以外 false.</returns>
		protected bool Open(string filePath)
		{
			if (!System.IO.File.Exists(filePath))
			{
				return false;
			}

			try
			{
				mApp = new Microsoft.Office.Interop.Excel.Application();
				mApp.Visible = false;

				// filePath が相対パスのとき例外が発生するので fullPath に変換
				string fullPath = System.IO.Path.GetFullPath(filePath);
				mWorkBook = mApp.Workbooks.Open(fullPath);
			}
			catch
			{
				Close();
				return false;
			}

			return true;
		}

		/// <summary>
		/// 開いているワークブックとエクセルのプロセスを閉じる.
		/// </summary>
		protected void Close()
		{
			if (mWorkBook != null)
			{
				mWorkBook.Close();
				Marshal.ReleaseComObject(mWorkBook);
				mWorkBook = null;
			}

			if (mApp != null)
			{
				mApp.Quit();
				Marshal.ReleaseComObject(mApp);
				mApp = null;
			}
		}

		~ExcelSheetReader()
		{
			Close();
		}

		protected Microsoft.Office.Interop.Excel.Application mApp = null;
		protected Microsoft.Office.Interop.Excel.Workbook mWorkBook = null;
	}
/*
	class Program
	{
		static void Main(string[] args)
		{
			var reader = new ExcelSheetReader();

			// 'sample.xlsx' の1番シートの 1A から 6C までを読む
			var sheet = reader.Read(@"sample.xlsx", 1, 1, 1, 6, 3);

			// コンソールに出力
			foreach (ArrayList row in sheet)
			{
				string s = "";
				foreach (var cell in row) { s += cell + ", "; }
				System.Console.WriteLine(s);
			}
		}
	}
*/
}