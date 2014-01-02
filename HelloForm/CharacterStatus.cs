using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;
using System.Diagnostics;

namespace Chara
{
	// CSVファイルからキャラクターの状態を生成するクラス
	class CharacterStatus
	{
		// 読み込むファイル
		private readonly String file = "data/characterStatus.csv";

		// 名前
		private String name;
		// 種族
		private String breed;
		public String Breed { get { return breed; } }
		// HP
		private int hp;
		// 死亡フラグ
		private bool isDead;
		// 肉を食べた数
		private int meet;
		// 野菜を食べた数
		private int vegetable;

		// コンストラクタ
		public CharacterStatus()
		{
			// キャラクタの状態を生成する
			update();
			Console.Write(this);
		}

		// キャラクタの状態をCSVから生成する
		public void update()
		{
			// ステータスをサーバから取得する
			downloadStatus();

			// csvデータから要素を読み込む
			List<String[]> importDataList = CSVReaderWriter.read(file);

			// 読み込んだすべての要素をキャラクターの状態に振り分ける
			foreach (String[] line in importDataList)
			{
				switch (line[0])
				{
					case "name":
						name = line[1];
						break;
					case "breed":
						breed = line[1];
						break;
					case "hp":
						hp = int.Parse(line[1]);
						break;
					case "dead":
						isDead = System.Convert.ToBoolean(line[1]);
						break;
					case "meet":
						meet = int.Parse(line[1]);
						break;
					case "vegetable":
						vegetable = int.Parse(line[1]);
						break;
					default:
						break;
				}
			}
		}

		// サーバ上に置かれているキャラクタのステータスをCSVに落としてくる
		private void downloadStatus()
		{
			// サーバにアクセスして、結果をCSVに落とす
			ServerAccess.access(file);

			/*
			// サーバからステータスを取得してくるプログラムの名前
			String program = @"GAEaccess.exe";

			// プログラムを起動するプロセスのインスタンス
			Process process = new Process();
			// 起動するファイル名をプロセスに設定
			process.StartInfo.FileName = program;
			
			// プロセス開始
			process.Start();
			*/
		}

		public override string ToString()
		{
			return "name:" + name +
				"\nbreed:" + breed +
				"\nhp:" + hp +
				"\nisdead:" + isDead +
				"\nmeet:" + meet +
				"\nvegetable:" + vegetable;
		}
	}
}
