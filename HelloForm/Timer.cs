using System;
using System.Windows.Forms;

namespace Timers{
	//================================================
	// FPSを利用するタイマークラス
	// 厳密なFPSを定義させたい。
	// C#では分解能が1/18程度なのでC++を利用すべきか
	// 参照：http://dixq.net/rp/43.html
	//================================================
	class FPSTimer : Timer
	{
		// FPS
		private readonly int FRAME = 60;

		// コンストラクタ
		public FPSTimer()
		{
			// 間隔を1フレームにセット
			this.Interval = 1000 / FRAME;
		}
	}

	// キャラクターステータス更新用タイマー
	class StatusTimer : Timer
	{
		// 間隔(ミリ秒)
		private readonly int INTERVAL = 3000;	// 3分

		// コンストラクタ
		public StatusTimer()
		{
			// 間隔をセット
			this.Interval = INTERVAL;
		}
	}
}

