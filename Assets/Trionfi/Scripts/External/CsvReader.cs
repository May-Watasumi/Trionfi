namespace CsvReadWrite
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// CSV形式のストリームを読み込む CsvReader を実装します。
    /// </summary>
    public class CsvReader : IDisposable
    {
        /// <summary>
        /// 現在読み込んでいるフィールドがダブルクォートで囲まれたフィールドかどうか
        /// </summary>
        private bool isQuotedField = false;

        /// <summary>
        /// CSVを読み込むストリーム
        /// </summary>
        private StreamReader stream = null;

        /// <summary>
        /// ファイル名を指定して、 <see cref="CsvReader">CsvReader</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path">読み込まれる完全なファイルパス。</param>
        public CsvReader(string path) :
            this(path, Encoding.Default)
        {
        }

        /// <summary>
        /// ファイル名、文字エンコーディングを指定して、 <see cref="CsvReader">CsvReader</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="path">読み込まれる完全なファイルパス。</param>
        /// <param name="encoding">使用する文字エンコーディング。</param>
        public CsvReader(string path, Encoding encoding)
        {
            var file = new FileStream(path, FileMode.Open, FileAccess.Read);
            this.stream = new StreamReader(file, encoding);
        }

        /// <summary>
        /// ストリームを指定して、 <see cref="CsvReader">CsvReader</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="stream">読み込まれるストリーム。</param>
        public CsvReader(Stream stream)
        {
            this.stream = new StreamReader(stream);
        }

        /// <summary>
        /// 文字列データを指定して、 <see cref="CsvReader">CsvReader</see> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">文字列データ。</param>
        public CsvReader(StringBuilder data)
        {
            var buffer = Encoding.Unicode.GetBytes(data.ToString());
            var memory = new MemoryStream(buffer);
            this.stream = new StreamReader(memory);
        }

        /// <summary>
        /// CsvReader オブジェクトと、その基になるストリームを閉じ、
        /// リーダーに関連付けられたすべてのシステムリソースを解放します。
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
        /// この CsvReader オブジェクトによって使用されているすべてのリソースを解放します。
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
        /// 現在のストリームから 1 レコード分の文字を読み取り、そのデータを文字配列として返します。
        /// </summary>
        /// <returns>入力ストリームからの次のレコード。入力ストリームの末尾に到達した場合は null。</returns>
        public List<string> ReadRow()
        {
            var file = this.stream;
            var line = string.Empty;
            var record = new List<string>();
            var field = new StringBuilder();

            while ((line = file.ReadLine()) != null)
            {
                for (var i = 0; i < line.Length; i++)
                {
                    var item = line[i];

                    if (item == ',' && !this.isQuotedField)
                    {
                        record.Add(field.ToString());
                        field.Clear();
                    }
                    else if (item == '"')
                    {
                        if (!this.isQuotedField)
                        {
                            if (field.Length == 0)
                            {
                                this.isQuotedField = true;
                                continue;
                            }
                        }
                        else
                        {
                            if (i + 1 >= line.Length)
                            {
                                this.isQuotedField = false;
                                continue;
                            }
                        }

                        var peek = line[i + 1];

                        if (peek == '"')
                        {
                            field.Append('"');
                            i += 1;
                        }
                        else if (peek == ',' && this.isQuotedField)
                        {
                            this.isQuotedField = false;
                            i += 1;
                            record.Add(field.ToString());
                            field.Clear();
                        }
                    }
                    else
                    {
                        field.Append(item);
                    }
                }

                if (this.isQuotedField)
                {
                    field.Append(Environment.NewLine);
                }
                else
                {
                    record.Add(field.ToString());

                    return record;
                }
            }

            return null;
        }

        /// <summary>
        /// 現在のストリームから非同期的に 1 レコード分の文字を読み取り、そのデータを文字配列として返します。
        /// </summary>
        /// <returns>入力ストリームからの次のレコード。入力ストリームの末尾に到達した場合は null。</returns>
        public Task<List<string>> ReadRowAsync()
        {
            return Task.Factory.StartNew<List<string>>(() =>
            {
                return this.ReadRow();
            });
        }

        /// <summary>
        /// すべての文字の現在位置から末尾までを読み込みます。
        /// </summary>
        /// <returns>
        /// ストリームの現在位置から末尾までのストリームの残り部分。
        /// 現在の位置がストリームの末尾である場合は、空の配列が返されます。
        /// </returns>
        public List<List<string>> ReadToEnd()
        {
            var data = new List<List<string>>();
            var record = new List<string>();

            while ((record = this.ReadRow()) != null)
            {
                data.Add(record);
            }

            return data;
        }

        /// <summary>
        /// すべての文字の現在位置から末尾までを非同期的に読み込みます。
        /// </summary>
        /// <returns>
        /// ストリームの現在位置から末尾までのストリームの残り部分。
        /// 現在の位置がストリームの末尾である場合は、空の配列が返されます。
        /// </returns>
        public Task<List<List<string>>> ReadToEndAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return this.ReadToEnd();
            });
        }
    }
}