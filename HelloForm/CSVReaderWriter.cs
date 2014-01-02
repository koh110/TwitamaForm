using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;

namespace Util
{
	// CSV形式ファイルをRead,Writeするクラス
	class CSVReaderWriter
	{
		// ファイル出力
		// writeFileName:出力先のファイル名
		// strlist:出力する行ごとの内容を保持した配列
		public static void write(String writeFileName,String[] strlist)
		{
			try
			{
				// 書き込み用エンコードをUTF-8に設定
				Encoding enc = Encoding.GetEncoding("UTF-8");
				// 行を全て書き込み
				File.WriteAllLines(writeFileName, strlist, enc);
			}
			catch (Exception e)
			{
				MessageBox.Show("" + e, "error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
			}
		}

		// ファイル入力
		// readFileName:入力先のファイル名
		// return:行ごとの情報を保持するリスト
		public static List<String[]> read(String readFileName)
		{
			// 読み込んできた行を保持するインスタンス
			List<String[]> readLines = new List<String[]>();
			try
			{
				// テキストを読み込むためのインスタンス
				TextFieldParser parser = new TextFieldParser(readFileName);
				// 分割される状態をセット
				parser.TextFieldType = FieldType.Delimited;
				// 区切り文字の設定
				parser.SetDelimiters(",");

				// ファイルの終端まで処理
				while (!parser.EndOfData)
				{
					String[] row = parser.ReadFields();	// 1行読み込み
					readLines.Add(row);	// 行を保持
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("" + e, "error",
				   MessageBoxButtons.OK,
				   MessageBoxIcon.Error);

			}

			return readLines;
		}
	}
}
