using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Timers;

class CharaForm : Form
{
	// フォームのイベントタイマー
	FPSTimer timer;

	// 表示するキャラクター
	Chara.Character character;

	// マウスのクリック位置を記憶
	private Point mouseDownPoint;

	// マウスクリック、クリック終了時の時間
	private DateTime onMouseDownTime,onMouseUpTime;

	// マウスの移動軌跡を保持する
	private Point oldPoint;

	// スクリーンサイズ(タスクバーを除く)
	private Size screenSize;

	// コンストラクタ
	public CharaForm(){
		// フォーム名
		this.Text = "ついたま";
		// 背景色
		this.BackColor = SystemColors.Window;
		// フォームサイズ
		this.Size = new Size(500,300);
		// スクロールバーの使用
		this.AutoScroll = true;
		// 透明な背景色のサポートを有効にする
		this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

		// 初期化
		init();
	}

	// 初期化
	private void init()
	{
		// 初期位置の初期化
		oldPoint = new Point(this.DesktopLocation.X,this.DesktopLocation.Y);

		// キャラクターの生成
		character = new Chara.Character(new Point(this.DesktopLocation.X, this.DesktopLocation.Y),screenSize);

		// スクリーンサイズと地平線の設定
		setSize();

		// コンテキストメニューの追加
		addContextMenu();

		// フォームの画像等の設定
		setForm();

		// タイマーの設定
		timer = new FPSTimer();
		// タイマーのイベントハンドラの設定
		timer.Tick += new EventHandler(timerEvent);
		// タイマーのスタート
		timer.Enabled = true;
	}

	// タイマーのイベントハンドラ
	private void timerEvent(object sender, EventArgs e)
	{
		// 行動
		character.action();
		// 当たり判定
		character.collisionDetection(screenSize);
		

		// 画像の更新
		setForm();

		// キャラクターの位置とフォームの位置あわせ
		this.DesktopLocation = character.position;
	}

	// スクリーンサイズの取得(タスクバーをのぞく)
	private void setSize()
	{
		// スクリーンのサイズを取得してセットする
		screenSize = new Size(Screen.GetWorkingArea(this).Width,
			Screen.GetWorkingArea(this).Height);
	}

	// コンテキストメニューの追加
	private void addContextMenu()
	{
		// コンテキストメニュー
		ContextMenuStrip cms = new ContextMenuStrip();
		// 追加されるメニュー
		ToolStripMenuItem exit = new ToolStripMenuItem("終了");
		// メニューの追加
		cms.Items.AddRange(
			new ToolStripMenuItem[]{exit}
			);
		// メニューのクリックイベントハンドラの追加
		exit.Click += new EventHandler(menu_exitClick);

		// フォームにコンテキストメニューの追加
		this.ContextMenuStrip = cms;
	}

	// コンテキストメニューのexitのクリックイベントハンドラ
	protected void menu_exitClick(object sender,EventArgs e)
	{
		this.Close();
	}

	// フォームの移動
	public void dragMoveForm(Point pt)
	{
		// 移動前の座標を設定
		oldPoint = new Point(this.Left, this.Top);
		// フォームの移動
		this.Left += pt.X;
		this.Top += pt.Y;
		// キャラクタの移動
		character.position.X = this.Left;
		character.position.Y = this.Top;
		// キャラクタの速度の設定
		character.setVelocity((this.Left - oldPoint.X)/5,(this.Top - oldPoint.Y) /10);
	}

	// フォームの画像等の設定
	private void setForm()
	{
		// フォームサイズを画像のサイズに変更
		changeFormBmpSize();

		// フォームの透明化
		transparence();
	}

	// 画像の大きさに画面サイズを変更
	private void changeFormBmpSize()
	{
		// 画像サイズ
		Size bmpSize = new Size(character.image.getImageSize().Width, character.image.getImageSize().Height);
		// ウィンドウサイズの変更
		//this.SetBounds(this.Left, this.Top, bmp.Width, bmp.Height, BoundsSpecified.Size);
		this.ClientSize = bmpSize;
		// スクロールバーが出現する最小サイズ
		AutoScrollMinSize = bmpSize;
	}

	// フォームの透明化
	private void transparence()
	{
		// フォームの境界線をなくす
		this.FormBorderStyle = FormBorderStyle.None;
		//透明にする色
		Color transColor = character.image.transColor;

		//Console.WriteLine("{0}",transColor);
		//transColor = Color.White;
		//Console.WriteLine("{0}", transColor);

		//背景画像を指定する
		this.BackgroundImage = character.image.pic;
		//背景色を指定する
		this.BackColor = transColor;
		//透明を指定する
		this.TransparencyKey = transColor;
		
		//透明を指定する
		//this.TransparencyKey = Color.White;
	}

	// 描画時のイベントハンドラ
	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		// スクロールにあわせた位置の取得
		//Point pt = AutoScrollPosition;
		// 画像の表示
		//e.Graphics.DrawImage(character.pic, pt.X, pt.Y,bmp.Width,bmp.Height);
	}

	// マウスクリック時のイベントハンドラ
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			//位置を記憶する
			mouseDownPoint = new Point(e.X, e.Y);
			this.onMouseDownTime = DateTime.Now;
			// キャラクタの状態をドラッグ状態にする
			character.setState(Chara.ActionStatus.DRAG);
		}
	}

	// マウスクリック終了時のイベントハンドラ
	protected override void OnMouseUp(MouseEventArgs e)
	{
		// 左クリックだったら
		if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			// キャラクターのバウンド数の初期化
			character.resetBound();
			// キャラクタの状態を落下状態にする
			//character.setStateFall();
			character.setState(Chara.ActionStatus.FALL);
		}
		this.onMouseUpTime = DateTime.Now;
		base.OnMouseUp(e);
	}

	// マウス移動時のイベントハンドラ
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		// ドラッグ状態だったら
		if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			// 移動させる
			Point nowPt = new Point((e.X - mouseDownPoint.X), (e.Y - mouseDownPoint.Y));
			// フォームを移動させる
			dragMoveForm(nowPt);
		}
		
	}

	// 終了時のイベントハンドラ
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);
		/*if(MessageBox.Show("本当に終了してよろしいですか?", "確認"
			, MessageBoxButtons.YesNo, MessageBoxIcon.Question) 
			== DialogResult.No){
			e.Cancel = true;
		}*/
	}
	
	// フォームが移動した時のイベントハンドラ
	protected override void OnMove(EventArgs e)
	{
		base.OnMove(e);
		// スクリーンサイズの取得
		setSize();
	}

	/*
	// スクロールバーが動いたとき
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		const int WM_HSCROLL = 0x114;
		const int WM_VSCROLL = 0x115;

		Graphics g = CreateGraphics();
		Point pt;

		switch (m.Msg)
		{
			// スクロールバーが縦に動いた場合
			case WM_VSCROLL:
				this.Invalidate();
				pt = AutoScrollPosition;
				g.DrawImage(bmp, pt.X, pt.Y);
				break;
			case WM_HSCROLL:
				this.Invalidate();
				pt = AutoScrollPosition;
				g.DrawImage(bmp, pt.X, pt.Y);
				break;
			default:
				break;
		}
		g.Dispose();
	}
	*/
}
