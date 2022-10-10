using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;
//using Xamarin.Essentials;
using Trionfi;


namespace Mira
{
	enum TextType
	{
		UTF8,
		SJIS
	}
	
	public partial class Form1 : Form
	{
//		private string[] fileNameArray;

		private ScriptGenerator scriptGen = new ScriptGenerator();

		private PDFInfo pdfInfo = new PDFInfo();

		private string[] textEncoder = new string[2]
		{
			"utf-8",
			"Shift_JIS"
		};

		private string[] nameSpliter = new string[5]
		{
			"【】",
			"〔〕",
			"〈〉",
			"《》",
			"［］"
		};

		public Form1()
		{
			InitializeComponent();
		}

		const string baseKey = @"SOFTWARE\Trionfi\Mira\";

		void LoadParams()
		{
			var rKey = Registry.CurrentUser.OpenSubKey(baseKey);
			if (rKey != null)
			{
				decimal temp = 50;

				if ((rKey.GetValue("Margin_L") == null) || !decimal.TryParse((string)rKey.GetValue("Margin_L"), out temp))
					numericUpDown1.Value = 50;
				else
					numericUpDown1.Value = temp;

				temp = 50;

				if ((rKey.GetValue("Margin_V") == null) || !decimal.TryParse((string)rKey.GetValue("Margin_V"), out temp))
					numericUpDown2.Value = 50;
				else
					numericUpDown2.Value = temp;

				comboBox1.SelectedIndex = rKey.GetValue("TextEncording") != null ? (int)rKey.GetValue("TextEncording") : 0;
				comboBox2.SelectedIndex = rKey.GetValue("NameSplitter") != null ? (int)rKey.GetValue("NameSplitter") : 0;
				textBox1.Text = rKey.GetValue("Title") != null ? (string)rKey.GetValue("Title") : "Title";
				textBox2.Text = rKey.GetValue("Author") != null ? (string)rKey.GetValue("Author") : "Author";
				textBox3.Text = rKey.GetValue("OutputPath") != null ? (string)rKey.GetValue("OutputPath") : Application.ExecutablePath;
				label12.Text = rKey.GetValue("ActorCSVPath") != null ? (string)rKey.GetValue("ActorCSVPath") : string.Empty;

				if(!string.IsNullOrEmpty(label12.Text))
				{
					LoadActorCSV(label12.Text);
				}

				object[,] dataGridViewObjectsArray = new object[dataGridView1.Rows.Count, dataGridView1.Columns.Count];

				/*
				if (rKey.GetValue("SelectedFiles") != null)
				{
					string[] allFileName = (string[])rKey.GetValue("SelectedFiles");

					foreach (string file in allFileName)
					{
						dataGridView2.Rows.Add(true, file, dataGridView2.ColumnCount.ToString("f2");
					}
				}
				*/
				rKey.Close();
			}
			else
			{
				comboBox1.SelectedIndex = 0;
				comboBox2.SelectedIndex = 0;
			}
		}

		void SaveParams()
		{
			var rKey = Registry.CurrentUser.OpenSubKey(baseKey, true);
			if (rKey == null)
				rKey = Registry.CurrentUser.CreateSubKey(baseKey);

			if (rKey != null)
			{
				rKey.SetValue("Margin_L", numericUpDown1.Value);
				rKey.SetValue("Margin_V", numericUpDown2.Value);
				rKey.SetValue("TextEncording", comboBox1.SelectedIndex);
				rKey.SetValue("NameSplitter", comboBox2.SelectedIndex);
				rKey.SetValue("Title", textBox1.Text);
				rKey.SetValue("Author", textBox2.Text);
				rKey.SetValue("OutputPath", textBox3.Text);
				rKey.SetValue("ActorCSVPath", label12.Text);

				/*
				string[] allFileName = new string[checkedListBox1.Items.Count];
				int count = 0;

				foreach (string file in checkedListBox1.Items)
				{
					allFileName[count++] = file;
				}

				rKey.SetValue("SelectedFiles", allFileName);
				*/
				rKey.Close();
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadParams();
		}

		private void Form1_Closed(object sender, FormClosedEventArgs e)
		{
			SaveParams();
		}

		//フォント選択
		private void button1_Click(object sender, EventArgs e)
		{
			//FontDialogクラスのインスタンスを作成
			FontDialog fd = new FontDialog();

			//初期のフォントを設定
			fd.Font = label1.Font;
			//初期の色を設定
//			fd.Color = TextBox1.ForeColor;
			//ユーザーが選択できるポイントサイズの最大値を設定する
			fd.MaxSize = (int)fd.Font.Size;
			fd.MinSize = (int)fd.Font.Size;
			//存在しないフォントやスタイルをユーザーが選択すると
			//エラーメッセージを表示する
			fd.FontMustExist = true;
//			//横書きフォントだけを表示する
			fd.AllowVerticalFonts = true;
//			//色を選択できるようにする
			fd.ShowColor = false;
//			//取り消し線、下線、テキストの色などのオプションを指定可能にする
			//デフォルトがTrueのため必要はない
			fd.ShowEffects = false;
			//固定ピッチフォント以外も表示する
			//デフォルトがFalseのため必要はない
			fd.FixedPitchOnly = false;
			//ベクタ フォントを選択できるようにする
			//デフォルトがTrueのため必要はない
			fd.AllowVectorFonts = true;

			//ダイアログを表示する
			if (fd.ShowDialog() != DialogResult.Cancel)
			{
				//TextBox1のフォントと色を変える
				label1.Font = fd.Font;
			}
		}

		//出力先フォルダ
		private void button2_Click(object sender, EventArgs e)
		{
			//FolderBrowserDialogクラスのインスタンスを作成
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			//上部に表示する説明テキストを指定する
			fbd.Description = "フォルダ選択";
			//ルートフォルダを指定する
			//デフォルトでDesktop
			fbd.RootFolder = Environment.SpecialFolder.Desktop;
			//最初に選択するフォルダを指定する
			//RootFolder以下にあるフォルダである必要がある
			fbd.SelectedPath = @"C:\Windows";
			//ユーザーが新しいフォルダを作成できるようにする
			//デフォルトでTrue
			fbd.ShowNewFolderButton = true;

			//ダイアログを表示する
			if (fbd.ShowDialog(this) == DialogResult.OK)
			{
				//選択されたフォルダを表示する
				textBox3.Text = fbd.SelectedPath;
			}
		}

		private void LoadActorCSV(string path)
		{
			Dictionary<string, TRActorInfo> result = scriptGen.ReadActorCSV(path, comboBox1.SelectedItem.ToString());
			if (result != null)
			{
				label12.Text = path;

				foreach (KeyValuePair<string, TRActorInfo> val in result)
				{
					dataGridView1.Rows.Add(true, val.Value.GetActorName(LocalizeID.JAPAN), val.Value.prefix, val.Value.hasVoice);
				}
			}
		}


		//CSVファイル
		private void button3_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = Application.ExecutablePath;
			dialog.Filter = "CSVファイル(*.csv)|*.csv";
			dialog.Title = "ボイス定義ファイルを開く";

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				LoadActorCSV(dialog.FileName);
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = Application.ExecutablePath;
			dialog.Filter = "プレーンテキスト(*.txt)|*.txt";
			dialog.Title = "シナリオファイルを開く";
			dialog.Multiselect = true;
			/*
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.Description = "フォルダ選択";
			fbd.RootFolder = Environment.SpecialFolder.Desktop;
			fbd.SelectedPath = @"C:\Windows";
			fbd.ShowNewFolderButton = false;

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				label13.Text = fbd.SelectedPath;
				string[] files = System.IO.Directory.GetFiles(fbd.SelectedPath, "*.txt", System.IO.SearchOption.AllDirectories);

				foreach (string file in files)
				{
					dataGridView2.Rows.Add(true, file, dataGridView2.ColumnCount.ToString("f2"));
				}
			}
			*/
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				foreach (string file in dialog.FileNames)
				{
					dataGridView2.Rows.Add(true, file, dataGridView2.ColumnCount.ToString("f2"));
				}
			}
		}

		private void buttonExecute_Click(object sender, EventArgs e)
		{
			PDFInfo pdfInfo = new PDFInfo();
			pdfInfo.font = label1.Font;
			pdfInfo.marginV = Decimal.ToInt32(numericUpDown1.Value);
			pdfInfo.marginH = Decimal.ToInt32(numericUpDown2.Value);
			pdfInfo.Title = textBox1.Text;
			pdfInfo.Author = textBox2.Text;

			for (int a = 0; a < dataGridView2.RowCount; a++)
			{
				if(dataGridView2.Rows[a].Cells[1].Value != null)
					scriptGen.Output(dataGridView2.Rows[a].Cells[1].Value.ToString(), textBox3.Text, pdfInfo, comboBox1.SelectedItem.ToString());
			}
		}

		private void listBox1_DragEnter(object sender, DragEventArgs e)
		{
			//ドラッグされているデータがstring型か調べ、
			//そうであればドロップ効果をMoveにする
			//			if (e.Data.GetDataPresent(typeof(string)))
			e.Effect = DragDropEffects.Copy;
			//			else
			//				//string型でなければ受け入れない
			//				e.Effect = DragDropEffects.None;
		}

		private void listBox1_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileNameArray = (string[])e.Data.GetData(DataFormats.FileDrop, false);

			foreach (string file in fileNameArray)
			{
				dataGridView2.Rows.Add(true, file, dataGridView2.ColumnCount.ToString("f2"));
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			dataGridView2.Rows.Clear();
		}
	}
}
