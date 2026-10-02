using System;
using System.Windows.Forms;

//// ノート
//★ToyingData
//	・重複行の削除
//	・全角を半角に変換
//		・数値
//		・英大字
//		・英小字
//		・スペース

//★ToyingFile
//	・指定フォルダ内のファイルから、指定の文字を削除
//	　・大文字小文字を区別
//	　・行を削除

//★TrimFileData
//	・ファイルを指定
//    指定されたファイル内から、指定の文字がある行をリストアップ

//★TrimHtmlData
//	・URLを指定。
//	　各URLの中から、指定された文字が含まれる行をリストアップ
//		・最初にヒットしたワードだけ抽出
//		・大文字小文字を区別

/// <summary>
/// ////////////////////////////////////////////////////
/// </summary>
//　・入力
//    　・フォルダ指定し、サブディレクトリ内のファイルを対象
//　　　・ファイルフルパスを指定
//    　・文字列をテキストコントロールに貼り付け
//    　・URLをテキストコントロールに列挙
//　・出力
//    　・同一ファイル
//　　　・別ファイル（？）
//    　・文字列
//　・機能
//　	・全角を半角に変換
//	　	　・数値
//		　・英大字
//		　・英小字
//		　・スペース

//      ・完全一致行を削除
//　　　・指定文字が含まれる行に対して操作
//    　　・行を削除
//    　　・文字だけを削除
//　　　　・行をリストアップ
//　　　　・オプション
//　    　　・大文字小文字を区別
//    　　　・最初にヒットしたワードだけを対象

namespace FiddleAroundString
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitializePlaceholders();
        }

        // 入力欄が空の時に薄く表示する入力例([[_Common/TextBoxEx.cs]]のPlaceholderText)。
        // Multilineの欄(検索ワード・検索結果・変更前/変更後データ・取得元/取得値の一覧、
        // Windowsの仕様で表示されない)は対象外
        private void InitializePlaceholders()
        {
            textBox_ReferencePath.PlaceholderText = @"例: C:\Work\reference.txt";
            textBox_SearchCommonWord.PlaceholderText = "例: TODO";

            textBox_SearchWord.PlaceholderText = "例: <title>";
            textBox_DelimiterWord.PlaceholderText = "例: </title>";
            textBox_TrimLineNum.PlaceholderText = "例: 1";
        }

        private void button_Execute_Click(object sender, EventArgs e)
        {

        }

        private void textBox_SearchWord_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox_DestList_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox_SourceList_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
