# CI / GitHub Actions - Unity EditMode テストの準備

このリポジトリに追加されたワークフロー `unity-window-tests.yml` は、EditMode テストを複数の Unity バージョンで実行します。CI を回すために必要な準備を下記にまとめます。

必須（GitHub のリポジトリシークレット）
- `UNITY_LICENSE` (推奨)
  - Unity のライセンスファイル（`Unity_v202x.xxxxx.ulf` の内容）をそのまま文字列で登録します。
  - hosted runner で Unity をライセンス認証する必要がある場合に使用します。

オプション
- `GITHUB_TOKEN` は Actions が自動で提供します。追加のシークレットは使用する Unity 配布方法や game-ci 設定に応じて必要になります。

ローカルでのテスト実行方法（Unity がマシンにインストール済みの場合）

- EditMode テストをコマンドラインで実行する例:

```bash
"/path/to/Unity" -projectPath . -runTests -testMode EditMode -testResults ./TestResults/results.xml
```

- Unity Hub を使う場合は該当バージョンをインストールして Unity を起動し、Test Runner から `EditMode` テストを実行できます。

ワークフローのカスタマイズ
- 現在のワークフローは例として以下のパッチバージョンを使っています。必要に応じて `.github/workflows/unity-window-tests.yml` の `matrix.unityVersion` を変更してください:
  - `6000.3f1`
  - `2022.3.30f1`
  - `2021.3.32f1`
  - `2020.3.48f1`
  - `2019.4.40f1`

トラブルシュート
- テスト結果やログはワークフローのアーティファクト `editmode-test-results-<version>` に保存されます。失敗時はまずその XML とログを確認してください。
- ライセンスやインストール失敗はランナーのログに出ます。必要なら `UNITY_LICENSE` を追加してください。

その他サポートが必要なら教えてください。CI の詳細設定（キャッシュ、並列実行数、ログ保存の拡張など）を追加します。