# 開発TODO（CS_Form / CS_Wpf）

## 未着手

- [ ] **WebCamera のプロジェクト実装**
  - お試しで作りかけのまま止まっている。カメラ映像の取り込み処理（BackgroundWorker の DoWork）はあるが、
    画面への描画処理（`backgroundWorker1_ProgressChanged` の中身）が丸ごとコメントアウトされていて、
    今は映像が表示されない
  - 何を作りたかったのか（撮影・録画・画像処理の試作など）を決めてから実装する
  - 場所: `WebCamera/WebCamera/Form1.cs`（OpenCvSharp 使用）
