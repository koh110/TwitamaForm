using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Chara
{
	// キャラクターのアクション状態を示す
	enum ActionStatus { FALL, DRAG, WALK, STOP };

	// キャラクターを表すクラス
	class Character
	{
		// キャラクターステータスをアップデートするためのタイマー
		private Timers.StatusTimer timer;

		// 状態
		private CharacterStatus charaState;	// キャラクターのステータス
		public CharacterStatus CharacterState { get { return charaState; } }
		private ActionStatus actionState;	// キャラクターのウィンドウ上の現在の動き
		public ActionStatus ActionState { get { return actionState; } }

		// キャラクターの画像管理インスタンス
		private CharaActionImage imageGroup;
		public CharaImage image { get { return imageGroup.getImage(CharacterState,ActionState); } }

		// キャラクターの移動を管理するインスタンス
		private CharaMove charaMove;

		// アクションの制御カウンター(次の行動に移るまでの余白時間)
		private int actionCounter;
		// アクションカウンターの最大値
		private readonly int ACT_COUNTER_MAX = 60;

		// 現在地
		public Point position;
	

		// コンストラクタ
		public Character(Point nowPos, Size scope)
		{
			// キャラクターのステータスの初期化
			charaState = new CharacterStatus();

			// アクションステータスの初期化
			actionState = ActionStatus.FALL;

			// キャラクターイメージの初期化
			imageGroup = new CharaActionImage(charaState);

			// キャラクターの移動インスタンスの初期化
			charaMove = new CharaMove(this,scope);

			// 位置の設定
			this.position = nowPos;

			// アクションカウンターの初期化
			this.actionCounter = this.ACT_COUNTER_MAX;

			// タイマーの初期化
			timer = new Timers.StatusTimer();
			// タイマーのイベントハンドラの設定
			timer.Tick += new EventHandler(timerEvent);
			// タイマーのスタート
			timer.Enabled = true;
		}

		// キャラクターの状態のセット
		public void setState(ActionStatus setState)
		{
			actionState = setState;
			actionCounter = ACT_COUNTER_MAX;
		}

		// 速度のセット
		public void setVelocity(double vx, double vy)
		{
			charaMove.vx = vx;
			charaMove.vy = vy;
		}

		// バウンド数を初期化する
		public void resetBound()
		{
			charaMove.boundCounter = 0;
		}

		// キャラクターの行動
		public void action()
		{
			// アクションの制御カウンター
			actionCounter--;

			//Console.WriteLine("pos{0}", position);

			// ステータスの状態に応じてそれぞれのアクションを行う
			switch (actionState)
			{
				case ActionStatus.WALK:
					charaMove.walk(image);
					break;
				case ActionStatus.FALL:
					charaMove.freeFall();
					break;
				case ActionStatus.DRAG:
					charaMove.drag();
					break;
				case ActionStatus.STOP:
					if (actionCounter >= 0)
					{
						return;
					}
					charaMove.stop();
					break;
				default:
					if (actionCounter >= 0)
					{
						return;
					}
					charaMove.stop();
					break;
			}
		}

		// 文字列変換
		public string ToString()
		{
			return charaState.ToString();
		}


		// 当たり判定
		public void collisionDetection(Size movingFloor)
		{
			// 当たり判定を行う
			charaMove.collisionDetection(movingFloor, image.drawSize, image.LeftDifference, image.TopDifference);
		}


		// タイマーのイベントハンドラ
		private void timerEvent(object sender, EventArgs e)
		{
			// キャラクターステータスを更新する
			charaState.update();
		}
	}
}
