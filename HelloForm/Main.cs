using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// アプリケーションを動かすメインクラス
class AplicationMain
{
	public static void Main(){
		// キャラクター用のフォームを生成
		CharaForm form = new CharaForm();

		// フォームを動かす
		Application.Run(form);
	}
}