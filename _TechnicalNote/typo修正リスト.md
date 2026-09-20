# typo修正リスト

CS_Form配下のtypoを一括修正したときの記録（2026-09-20）。

## 修正済み（コード側）

識別子（コントロール名・メソッド名・変数名・クラス名・ファイル名）はすべて修正済み。

| 誤 | 正 | 箇所数 | 主な対象 |
|:--|:--|--:|:--|
| Refrence | Reference | 88 | FileArranger |
| Serch | Search | 93 | TrimFileData / StaticAnalysisViewer / FFEdit |
| TimeStump | TimeStamp | 54 | FFEdit / Cheetos |
| Runk | Rank | 22 | StaticAnalysisViewer |
| Operatoin | Operation | 21 | FFEdit |
| Prifix | Prefix | 18 | Cheetos |
| Seprator | Separator | 17 | FileArranger |
| Marge | Merge | 17 | Cheetos |
| butotn | button | 13 | FFEdit |
| REVENT | REVERT | 6 | PerforceWrapper |
| Patition | Partition | 3 | FileArranger |
| UpdteListBox | UpdateListBox | 3 | FileArranger |

ファイル名の修正:

| 誤 | 正 |
|:--|:--|
| Cheetos/CapureWindow.cs | Cheetos/CaptureWindow.cs |
| FFEdit/TimeStump.cs | FFEdit/TimeStamp.cs |
| FFEdit.Tests/TimeStumpTests.cs | FFEdit.Tests/TimeStampTests.cs |

## ⚠️ 未修正（設定ファイルのキー名）

設定ファイル（XML）のキーは、SaveRestore.cs で文字列リテラルとして書かれており、
**これを変えると既存の設定ファイルの値が読めなくなる**（初期値に戻る）ため、
今回は意図的にtypoのまま残した。

```csharp
// 例: TrimFileData/SaveRestore.cs
RegistCtrl("Common", "textBox_SerchCommonWord", Parent.textBox_SearchCommonWord);
//                    ↑キー名(typoのまま)         ↑コントロール名(修正済み)
```

残っているキー名は以下の7つ。

| プロジェクト | 設定キー（現状） | 直したい名前 |
|:--|:--|:--|
| Cheetos | cw_TextBox_SaveFilePrifix | cw_TextBox_SaveFilePrefix |
| Cheetos | cw_checkBox_AddTimeStump | cw_checkBox_AddTimeStamp |
| FileArranger | pf_textBox_RefrenceFile | pf_textBox_ReferenceFile |
| FileArranger | pf_textBox_TargetSeprator | pf_textBox_TargetSeparator |
| TrimFileData | textBox_SerchCommonWord | textBox_SearchCommonWord |
| TrimFileData | textBox_SerchWordList | textBox_SearchWordList |
| TrimFileData | textBox_SerchResultList | textBox_SearchResultList |

### いつ直すか

設定ファイルをXMLからJSONへ移行するときに、**旧キー→新キーの読み替え**を
1箇所に用意して一緒に解消するのがよい。
移行処理で「旧キーで書かれた値を新キーとして読み込む」ようにすれば、
ユーザーの設定を失わずにキー名を正せる。
