namespace CsvReadWrite
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// CSV形式のストリームを書き込む CsvWriter を実装します。
    /// </summary>
    public class CsvWriter : IDisposable
    {
        /// <summary>
        /// CSVファイルに書き込むストリーム
        /// </summary>
        private StreamWriter stream = null;

        /// <summary>
        /// ファイル名を指定して、 <see cref="CsvWriter">CsvWriter</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path">書き込む完全なファイルパス。</param>
        public CsvWriter(string path) :
            this(path, Encoding.Default)
        {
        }

        /// <summary>
        /// ファイル名、文字エンコーディングを指定して、 <see cref="CsvWriter">CsvWriter</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path">書き込む完全なファイルパス。</param>
        /// <param name="encoding">使用する文字エンコーディング。</param>
        public CsvWriter(string path, Encoding encoding)
        {
            var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            this.stream = new StreamWriter(stream, encoding);
        }

        /// <summary>
        /// 使用する文字エンコーディングを取得します。
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this.stream.Encoding;
            }
        }

        /// <summary>
        /// 現在のストリームで利用される改行文字列を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get
            {
                return this.stream.NewLine;
            }

            set
            {
                this.stream.NewLine = value;
            }
        }

        /// <summary>
        /// 現在のストリームオブジェクトと基になるストリームをとじます。
        /// </summary>
        public void Close()
        {
            if (this.stream == null)
            {
                return;
            }

            this.stream.Close();
        }

        /// <summary>
        /// CsvWriter で利用されているすべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            if (this.stream == null)
            {
                return;
            }

            this.stream.Close();
            this.stream.Dispose();
            this.stream = null;
        }

        /// <summary>
        /// 現在のライターで使用したすべてのバッファーをクリアし、バッファー内のすべてのデータをストリームに書き込みます。
        /// </summary>
        public void Flush()
        {
            this.stream.Flush();
        }

        /// <summary>
        /// 現在のライターで使用したすべてのバッファーを非同期的にクリアし、ストリームへ書き込みます。
        /// </summary>
        /// <returns>非同期のフラッシュ操作を表すタスク。</returns>
        public Task FlushAsync()
        {
            return this.stream.FlushAsync();
        }

        /// <summary>
        /// ストリームに文字を書き込みます。
        /// </summary>
        /// <typeparam name="T">リストの型。</typeparam>
        /// <param name="data">CSVデータ。</param>
        public void Write<T>(List<List<T>> data)
        {
            foreach (var row in data)
            {
                this.WriteRow<T>(row);
            }
        }

        /// <summary>
        /// ストリームに文字を非同期的に書き込みます。
        /// </summary>
        /// <typeparam name="T">リストの型。</typeparam>
        /// <param name="data">CSVデータ。</param>
        /// <returns>非同期の書き込み操作を表すタスク。</returns>
        public Task WriteAsync<T>(List<List<T>> data)
        {
            return Task.Factory.StartNew(() =>
            {
                this.Write<T>(data);
            });
        }

        /// <summary>
        /// ストリームに１レコード分の文字列を書き込みます。
        /// </summary>
        /// <typeparam name="T">リストの型。</typeparam>
        /// <param name="row">CSVの１レコード。</param>
        public void WriteRow<T>(List<T> row)
        {
            var sb = new StringBuilder();

            foreach(var cell in row)
            {
                var value = cell.ToString();

                if (value.Contains(this.NewLine) ||
                    value.Contains(",") ||
                    value.Contains("\""))
                {
                    value = value.Replace("\"", "\"\"");
                    sb.Append("\"");
                    sb.Append(value);
                    sb.Append("\"");
                }
                else
                {
                    sb.Append(value);
                }

                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            
            this.stream.WriteLine(sb.ToString());
        }

        /// <summary>
        /// ストリームに１レコード分の文字列を非同期的に書き込みます。
        /// </summary>
        /// <typeparam name="T">リストの型。</typeparam>
        /// <param name="row">CSVの１レコード。</param>
        /// <returns>非同期の書き込み操作を表すタスク。</returns>
        public Task WriteRowAsync<T>(List<T> row)
        {
            return Task.Factory.StartNew(() =>
            {
                this.WriteRow<T>(row);
            });
        }
    }
}