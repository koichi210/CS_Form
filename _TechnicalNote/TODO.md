# 開発TODO（CS_Form / CS_Wpf）

## 確認待ち

- [ ] **WebCamera をカメラ付きPCで動作確認する**
  - ライブ表示（Start/Stop）と Snapshot（PNG保存）を実装済み。開発PCにカメラが無いため、
    確認できているのは「カメラが見つからない」ときの動作まで
  - 確認したいこと: 映像が映る / Stop で止まる / 表示中にウィンドウを閉じられる /
    途中でカメラを抜いてもメッセージが出て止まる / Snapshot で PNG が保存できる
  - 場所: `WebCamera/WebCamera/Form1.cs`（OpenCvSharp 使用、64bit で動作）
