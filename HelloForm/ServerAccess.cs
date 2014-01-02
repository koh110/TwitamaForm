using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Chara
{
	// サーバにアクセスして値を取得するクラス
	class ServerAccess
	{
		// アクセスするページの基底URL
		private static readonly String baseUrl = "http://twitamaserver.appspot.com";

		// アクセスした結果を出力するファイル
		//private static readonly String fileName = "characterStatus.csv";

		// アクセスするメソッド
		// fileName 結果を出力するファイル名
		public static void access(String fileName)
		{
			// アクセスするページ
			String accessUrl = baseUrl + "/outgoingaccess";

			// 内容を保持するリスト
			List<String> list = new List<String>();

			// ウェブに開くアセスするためのインスタンス
			WebClient wCrient = new WebClient();
			// ウェブから読み込むためのストリーム
			Stream stream = wCrient.OpenRead(accessUrl);

			// UTF-8で読み込む
			StreamReader sReader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));

			// 読み込みできる文字がなくなるまで繰り返す
			while (sReader.Peek() >= 0)
			{
				// 1行ずつ読み込む
				String buffer = sReader.ReadLine();
				// 読み込んだものをリストに格納する
				list.Add(buffer);
			}

			// データ書き出し用にlistを配列に変換
			String[] array = (String[])list.ToArray();

			// データをファイルに書き出す
			Util.CSVReaderWriter.write(fileName, array);

			// クローズ
			sReader.Close();
			stream.Close();
		}
	}
}
