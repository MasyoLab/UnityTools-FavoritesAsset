# CI クイックスタートガイド

PRを作成した際に、複数のUnity LTSバージョンで自動テストを実行するCIが設定されました。

## 必要な設定（初回のみ）

CIを機能させるには、GitHubリポジトリにUnityのライセンス情報を設定する必要があります。

### 1. Unity License の取得

**最も簡単な方法** - GitHub Actionsを使用:

1. このリポジトリの `Actions` タブを開く
2. 左のサイドバーから `Acquire Activation File` ワークフローを選択（まだ作成していない場合は以下を参照）
3. `Run workflow` をクリックして実行
4. 完了後、Artifacts から `.alf` ファイルをダウンロード
5. https://license.unity3d.com/manual にアクセスして `.alf` ファイルをアップロード
6. `.ulf` ファイルをダウンロード

### 2. GitHub Secrets の設定

1. リポジトリの `Settings` > `Secrets and variables` > `Actions` を開く
2. `New repository secret` をクリックして以下を追加:

   - **UNITY_LICENSE**
     - 取得した `.ulf` ファイルの内容をすべてコピー＆ペースト
   
   - **UNITY_EMAIL**
     - あなたのUnity IDのメールアドレス
   
   - **UNITY_PASSWORD**
     - あなたのUnity IDのパスワード

### 3. 完了！

これで、PRを作成すると以下のバージョンで自動テストが実行されます：

- Unity 6000.0 LTS
- Unity 2022.3 LTS
- Unity 2021.3 LTS
- Unity 2020.3 LTS
- Unity 2019.4 LTS

## Activation File を取得するワークフローの作成（Option）

`.github/workflows/activation.yml` を作成して以下を記述:

```yaml
name: Acquire Activation File
on:
  workflow_dispatch: {}
jobs:
  activation:
    name: Request manual activation file
    runs-on: ubuntu-latest
    steps:
      - name: Request manual activation file
        id: getManualLicenseFile
        uses: game-ci/unity-request-activation-file@v2
        with:
          unityVersion: 2021.3.45f1
      
      - name: Upload activation file
        uses: actions/upload-artifact@v4
        with:
          name: Manual Activation File
          path: ${{ steps.getManualLicenseFile.outputs.filePath }}
```

## CIの動作確認

1. 新しいブランチを作成
2. 何か変更を加える（例: README.mdを編集）
3. PRを作成
4. `Checks` タブで各Unityバージョンのテスト結果を確認
5. すべてのテストがパスすれば ✅ 完了！

## トラブルシューティング

### "Invalid Unity license" エラー

- `UNITY_LICENSE` シークレットの内容が完全にコピーされているか確認
- `.ulf` ファイルの先頭・末尾の空白・改行も含めてすべてコピーすること

### テストが実行されない

- `.github/workflows/unity-test.yml` ファイルがmasterブランチにコミットされているか確認
- PRのターゲットブランチがmasterまたはdevelopになっているか確認

### 詳しい情報

[CI_SETUP.md](.github/CI_SETUP.md) に詳細なドキュメントがあります。

---

**関連ファイル:**
- [.github/workflows/unity-test.yml](../workflows/unity-test.yml) - CIの設定ファイル
- [CI_SETUP.md](CI_SETUP.md) - 詳細なセットアップガイド
- [Tests/Editor/](../../Assets/MasyoLab/FavoritesAsset/Tests/Editor/) - テストコード
