using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Chara
{
	// キャラクターの動きを管理するクラス
	class CharaMove
	{
		// 動かすキャラクタのポインタ
		private Character moveChara;

		// 移動速度
		public double vx, vy;
		// バウンド数カウンター
		public int boundCounter;
		// 動ける範囲
		private Size scopeFloor;

		// 乱数生成用のシード
		private int rndSeed;
		// 乱数
		private Random random;

		// バウンド回数
		private readonly int BOUND = 5;

		// 重力加速度
		private readonly double GRAVITY = 0.3;

		// 摩擦係数
		private readonly double FRICTION = 0.6;

		// 移動速度閾値
		private readonly double THRESHOLD = 2.0;

		// 一度に歩く距離
		private readonly int DISTANCE = 5;
		// 歩いて進む最大の距離
		private readonly int WALK_DISTANCE_MAX = 100;

		// 移動先の目標地点
		private Point target;
		// 目標地点に着いたと判定する距離
		private readonly double ARRIVAL_DISTANCE = 10;
		// 目的地に到達しているかどうかのフラグ
		private bool arrivalFlag;

		// コンストラクタ
		public CharaMove(Character chara, Size scope)
		{
			// キャラクタのポインタの設定
			moveChara = chara;

			// 速度の初期化
			vx = 0;
			vy = 0;

			// バウンド数カウンターの初期化
			boundCounter = 0;

			// 到着フラグの初期化
			this.arrivalFlag = true;

			// 動ける範囲の設定
			scopeFloor = scope;

			// 乱数のシードの設定
			this.rndSeed = Environment.TickCount;
			// 乱数の初期化
			this.random = new Random(rndSeed++);
		}

		// 停止
		public void stop()
		{
			vx = 0;
			vy = 0;
			// 乱数の初期化
			random = new Random(rndSeed++);
			int randNum = random.Next(100);

			// 10%の確率で状態を歩くへ
			if (randNum <= 10)
			{
				// 歩く状態へ
				moveChara.setState(ActionStatus.WALK);
			}
		}

		// 歩く時の処理
		// image:描画イメージ
		public void walk(CharaImage image)
		{
			// 描画サイズの取得
			Size imageSize = image.drawSize;
			if (arrivalFlag)
			{
				// 乱数の初期化
				random = new Random(rndSeed++);
				// 移動できる範囲から画像の大きさを引いた値を最大値とする乱数の発生
				// 左はじの座標を中心に移動させるので、表示画像の大きさ分を引く
				int randNum = random.Next(scopeFloor.Width - imageSize.Width);
				// 出た移動値が少なすぎたらもう一度
				if (Math.Abs(randNum - moveChara.position.X) < this.ARRIVAL_DISTANCE)
				{
					randNum = random.Next(scopeFloor.Width - imageSize.Width);
				}

				// 移動先の設定
				target = new Point(randNum, moveChara.position.Y);
				this.arrivalFlag = false;
			}
			//Console.WriteLine("target{0}", target);

			// 移動先に到着したら停止
			if (Math.Abs(target.X - moveChara.position.X) < ARRIVAL_DISTANCE)
			{
				arrivalFlag = true;
				moveChara.setState(ActionStatus.STOP);
				return;
			}

			if (target.X - moveChara.position.X > 0)
			{
				// 左を向いていたら
				if (!image.IsRight)
				{
					// 画像を反転させる
					image.reverse();
				}
				rightWalk();
			}
			else
			{
				// 右を向いていたら
				if (image.IsRight)
				{
					// 画像を反転させる
					image.reverse();
				}
				leftWalk();
			}
		}

		// 右向きに歩く
		private void rightWalk()
		{
			vx = DISTANCE;
			moveX();
		}
		// 左向きに歩く
		private void leftWalk()
		{
			vx = -DISTANCE;
			moveX();
		}

		// ドラッグされている時の処理
		public void drag()
		{

		}

		// 床まで自由落下
		public void freeFall()
		{
			// 速度に重力加速度を追加
			vy += GRAVITY;
			moveY();
			moveX();
			// 5回バウンドしたら止まる
			if (boundCounter > BOUND)
			{
				if (Math.Abs(vy) < THRESHOLD)
				{
					moveChara.setState(ActionStatus.STOP);
				}
			}
		}

		// vx,vy方向に動く
		public void move()
		{
			moveX();
			moveY();
		}

		// Y軸方向にvyの量移動
		private void moveY()
		{
			// 移動
			moveChara.position = new Point(moveChara.position.X, (int)(moveChara.position.Y + this.vy));
		}

		// X軸方向にvxの量移動
		private void moveX()
		{
			// 移動
			moveChara.position = new Point((int)(moveChara.position.X + this.vx), this.moveChara.position.Y);
		}

		// 当たり判定
		// movingFloor:動かせる範囲のサイズ
		// imageSize:表示される画像のサイズ
		// leftDiff:表示される画像サイズと実際の画像サイズの左側のずれの大きさ
		// topDiff:同上
		public void collisionDetection(Size movingFloor,Size imageSize,int leftDiff,int topDiff)
		{
			scopeFloor = movingFloor;
			// 底の座標(左上のY座標+描画部のY座標+描画部の高さ)
			int bottom = moveChara.position.Y + topDiff + imageSize.Height;
			// 右の座標(左上のX座標+描画部のX座標+描画部の幅)
			int right = moveChara.position.X + leftDiff + imageSize.Width;
			// 左の座標(左上のX座標+描画部のX座標)
			int left = leftDiff + moveChara.position.X;
			// 下にはみ出たら
			if (bottom + this.vy >= movingFloor.Height)
			{
				// 反射
				vy *= -FRICTION;
				// バウンドカウンターの加算
				boundCounter++;
				// 摩擦による減速
				vx *= FRICTION;
				// 位置の補正
				moveChara.position.Y = movingFloor.Height - topDiff - imageSize.Height;
			}
			// 右にはみ出たら
			if (right + this.vx >= movingFloor.Width)
			{
				// 反射
				vx *= -FRICTION;
				// 位置の補正
				moveChara.position.X = movingFloor.Width -leftDiff - imageSize.Width;
			}
			// 左にはみ出たら
			if (left + this.vx <= 0)
			{
				// 反射
				vx *= -FRICTION;
				// 位置の補正
				moveChara.position.X = -leftDiff;
			}

		}
	}
}
