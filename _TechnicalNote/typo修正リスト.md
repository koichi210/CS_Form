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

## ✅ 修正済み（設定ファイルのキー名、2026-09-21）

設定ファイルのキー（SaveRestore.cs の RegistCtrl 第2引数）も直した。
当初は「既存の設定ファイルが読めなくなる」ため見送っていたが、
`_Common/StandardTemplateClass.cs` の `OriginDB` に **旧キーの読み替え機能
（LegacyAttrValue）** を追加したことで、キー名を変えつつ旧ファイルも読めるようにした。

```csharp
// 例: TrimFileData/SaveRestore.cs
RegistCtrl("Common", "textBox_SearchCommonWord", Parent.textBox_SearchCommonWord,
    LegacyAttrValue: "textBox_SerchCommonWord");
//                    ↑今後使う新キー                              ↑旧キー(読み込み時のみ使う)
```

読み込み時、新キーで見つからなければ旧キーでも探す（XML・JSON両方に対応）。
保存は常に新キーで行われるため、**一度保存し直せば旧キーの読み替えは以後不要**になる。

| プロジェクト | 旧キー | 新キー |
|:--|:--|:--|
| Cheetos | cw_TextBox_SaveFilePrifix | cw_TextBox_SaveFilePrefix |
| Cheetos | cw_checkBox_AddTimeStump | cw_checkBox_AddTimeStamp |
| FileArranger | pf_textBox_RefrenceFile | pf_textBox_ReferenceFile |
| FileArranger | pf_textBox_TargetSeprator | pf_textBox_TargetSeparator |
| TrimFileData | textBox_SerchCommonWord | textBox_SearchCommonWord |
| TrimFileData | textBox_SerchWordList | textBox_SearchWordList |
| TrimFileData | textBox_SerchResultList | textBox_SearchResultList |

検証はテストで実施（`_Common/Tests/StandardTemplate.Tests/StcSaveRestoreTests.cs`）:
旧キーのXML/JSONを読み込めること、新旧両方あれば新キーが優先されること、
`LegacyAttrValue`を指定しなければ従来通り新キーのみ一致することを確認済み。
