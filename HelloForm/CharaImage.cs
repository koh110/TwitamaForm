using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chara
{
	// キャラクターの動きの画像を扱うクラス
	class CharaActionImage
	{
		// キャラクターの種類毎の画像
		private CharaBreedImage currentBreed;	// 現在の種族
		private CharaBreedImage normalImage = new CharaBreedImage("normal");
		private CharaBreedImage meetImage = new CharaBreedImage("meet");
		private CharaBreedImage vegImage = new CharaBreedImage("vegetable");
		
		// 現在表示されている画像
		private CharaImage current;
		public CharaImage currentImage { get { return current; } }

		// コンストラクタ
		public CharaActionImage(CharacterStatus characterStatus)
		{
			// イメージの初期化
			setBreedImage(characterStatus);

			// currentの初期値
			current = currentBreed.StopImage;
		}

		// キャラクターの種類に応じたイメージのセット
		public void setBreedImage(CharacterStatus characterStatus)
		{
			// 現在のキャラクターの種類をセット
			switch (characterStatus.Breed)
			{
				case "normal":
					currentBreed = normalImage;
					break;
				case "meet":
					currentBreed = meetImage;
					break;
				case "vegetable":
					currentBreed = vegImage;
					break;
				default:
					currentBreed = normalImage;
					break;
			}
		}

		// キャラクターのアクションステータスに応じた画像を取得
		public CharaImage getImage(CharacterStatus characterStatus, ActionStatus actionStatus)
		{
			setBreedImage(characterStatus);

			switch(actionStatus){
				case ActionStatus.STOP:
					current = currentBreed.StopImage;
					break;
				case ActionStatus.WALK:
					current = currentBreed.WalkImage;
					break;
				default:
					current = currentBreed.StopImage;
					break;
			}
			return current;
		}
	}

	// キャラクターの種類毎のイメージクラス
	class CharaBreedImage
	{
		// カレントディレクトリ
		private readonly String currentDir;

		// キャラクターの種類
		private String breed;	// 現在の種類に応じたパスを入れる変数
		private readonly String NORMAL = @"\normal";	// 通常のキャラクター
		private readonly String MEET = @"\meet";	// 肉食のキャラクター
		private readonly String VEGETABLE = @"\vegetable";	// 草食のキャラクター

		// キャラクター画像のパス
		private readonly String stop = @"\stop\stop01.png";
		private readonly String walk = @"\walk\walk01.png";

		// キャラクターの画像管理インスタンス
		// 現在使われているキャラクターのアクションイメージ
		private CharaImage stopImage;
		public CharaImage StopImage { get { return stopImage; } }
		private CharaImage walkImage;
		public CharaImage WalkImage { get { return walkImage; } }

		// コンストラクタ
		// breed:生成する種族
		public CharaBreedImage(String breed)
		{
			// カレントディレクトリの取得
			currentDir = System.IO.Directory.GetCurrentDirectory() + @"\pictures";

			// 種族を設定
			switch (breed)
			{
				case "normal":
					breed = NORMAL;
					break;
				case "meet":
					breed = MEET;
					break;
				case "vegetable":
					breed = VEGETABLE;
					break;
				default:
					breed = NORMAL;
					break;
			}

			// それぞれのイメージの初期化
			stopImage = new CharaImage(currentDir + breed + stop);
			walkImage = new CharaImage(currentDir + breed + walk);
		}
	}

	// キャラクターの画像を扱うクラス
	class CharaImage
	{
		// 表示する画像
		private Bitmap bmp;
		public Bitmap pic { get { return bmp; } }

		// 右を向いているかのフラグ
		private bool isRight;
		public bool IsRight { get { return isRight; } }

		// 透明色
		private Color tColor;
		public Color transColor { get { return tColor; } }

		// 画像サイズ
		private Size bmpSize;
		public Size BMPSize { get { return bmpSize; } }

		// 実際に描画されている領域の最大サイズ
		private Size drawedSize;
		public Size drawSize { get { return drawedSize; } }

		// 現在地から描画位置のずれ
		private int topDiff;
		public int TopDifference { get { return topDiff; } }
		private int leftDiff;
		public int LeftDifference { get { return leftDiff; } }

		// コンストラクタ
		public CharaImage(String bmpAddress)
		{
			// 画像の読み込み
			loadPic(bmpAddress);

			// 透明色の読み込み
			this.tColor = bmp.GetPixel(0, 0);
			// 描画サイズの設定
			setDrawedSize();
			//画像を透明にする
			this.bmp.MakeTransparent(tColor);
			// 画像サイズ
			this.bmpSize = bmp.Size;

			// 右向き状態をセット
			this.isRight = true;
		}

		// 現在表示されている画像の大きさを返すメソッド
		public Size getImageSize(){
			return bmpSize;
		}

		// 画像を反転させる
		public void reverse()
		{
			// 水平方向に180度回転
			bmp.RotateFlip(RotateFlipType.Rotate180FlipY);

			// 向きの入れ替え
			if (isRight)
			{
				isRight = false;
			}
			else
			{
				isRight = true;
			}
		}

		// 画像の読み込みメソッド
		private void loadPic(String bmpAddress)
		{
			// 画像の読み込み
			try
			{
				// 画像の読み込み
				this.bmp = new Bitmap(bmpAddress);
			}
			catch (Exception exception)
			{
				MessageBox.Show("画像の読み込みに失敗しました\n\n"+exception, "error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);

				// プログラムの終了
				Environment.Exit(0);
			}
		}

		// 描画されている範囲のサイズの設定メソッド
		private void setDrawedSize()
		{
			Color tmp = tColor;
			int top = 0, left = 0, right = 0, bottom = 0;
			// 一番上の座標を求める
			for (int i = 0; i < bmp.Size.Height; i++)
			{
				for (int j = 0; j < bmp.Size.Width; j++)
				{
					tmp = bmp.GetPixel(j, i);
					if (!tColor.Equals(tmp))
					{
						top = i;
						goto TOPEND;
					}
				}
			}
		TOPEND: ;
			// 左端の座標を求める
			for (int i = 0; i < bmp.Size.Width; i++)
			{
				for (int j = 0; j < bmp.Size.Height; j++)
				{
					tmp = bmp.GetPixel(i, j);
					if (!tColor.Equals(tmp))
					{
						left = i;
						goto LEFTEND;
					}
				}
			}
		LEFTEND: ;
			// 右端の座標を求める
			for (int i = bmp.Size.Width - 1; i >= 0; i--)
			{
				for (int j = 0; j < bmp.Size.Height; j++)
				{
					tmp = bmp.GetPixel(i, j);
					if (!tColor.Equals(tmp))
					{
						right = i;
						goto RIGHTEND;
					}
				}
			}
		RIGHTEND: ;
			// 底の座標を求める
			for (int i = bmp.Size.Height - 1; i >= 0; i--)
			{
				for (int j = 0; j < bmp.Size.Width; j++)
				{
					tmp = bmp.GetPixel(j, i);
					if (!tColor.Equals(tmp))
					{
						bottom = i;
						goto BOTTOMEND;
					}
				}
			}
		BOTTOMEND: ;

			// サイズのセット
			this.drawedSize.Height = bottom - top;
			this.drawedSize.Width = right - left;
			this.topDiff = top;
			this.leftDiff = left;
		}
	}
}
