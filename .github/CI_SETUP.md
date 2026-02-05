# CI/CD セットアップガイド

このプロジェクトでは、GitHub ActionsでUnityエディタ拡張の自動テストを実行しています。

## 対応Unityバージョン

以下のUnity LTSバージョンで自動テストが実行されます：

- Unity 6000.3 LTS (6000.3.7f1)
- Unity 6000.3 LTS (6000.3.0f1)
- Unity 6000.0 LTS (6000.0.67f1)
- Unity 6000.0 LTS (6000.0.59f2)
- Unity 2022.3 LTS (2022.3.62f3)
- Unity 2021.3 LTS (2021.3.45f2)
- Unity 2020.3 LTS (2020.3.49f1)
- Unity 2019.4 LTS (2019.4.41f2)

## GitHub Secretsの設定

CIを動作させるには、リポジトリに以下のシークレットを設定する必要があります。

### 必要なシークレット

1. `UNITY_LICENSE` - Unityのライセンスファイルの内容
2. `UNITY_EMAIL` - Unity IDのメールアドレス
3. `UNITY_PASSWORD` - Unity IDのパスワード

### Unityライセンスの取得方法

#### 方法1: Personal ライセンスの場合（推奨）

1. 以下のコマンドでライセンスファイルを取得します：

```bash
docker run -it --rm \
  -e "UNITY_EMAIL=your-email@example.com" \
  -e "UNITY_PASSWORD=your-password" \
  unityci/editor:ubuntu-2021.3.45f1-base-1 \
  bash -c 'unity-editor -quit -batchmode -nographics -logFile /dev/stdout -createManualActivationFile || true && cat *.alf'
```

2. 出力された `.alf` ファイルの内容をコピー
3. https://license.unity3d.com/manual にアクセスしてライセンスファイルを取得
4. 取得した `.ulf` ファイルの内容を `UNITY_LICENSE` シークレットに設定

#### 方法2: unity-request-activation-file アクションを使用

1. 一時的に以下のワークフローを作成して実行：

```yaml
name: Acquire activation file
on: workflow_dispatch: {}
jobs:
  activation:
    name: Request manual activation file
    runs-on: ubuntu-latest
    steps:
      - name: Request manual activation file
        uses: game-ci/unity-request-activation-file@v2
        with:
          unityVersion: 2021.3.45f1
      
      - name: Upload activation file
        uses: actions/upload-artifact@v4
        with:
          name: Manual Activation File
          path: ${{ steps.getManualLicenseFile.outputs.filePath }}
```

2. Artifactsからダウンロードした `.alf` ファイルを https://license.unity3d.com/manual でアクティベーション
3. 取得した `.ulf` ファイルの内容を `UNITY_LICENSE` シークレットに設定

### シークレットの設定手順

1. GitHubリポジトリの Settings > Secrets and variables > Actions に移動
2. "New repository secret" をクリック
3. 以下のシークレットを追加：
   - Name: `UNITY_LICENSE`, Secret: Unityライセンスファイル（.ulf）の内容
   - Name: `UNITY_EMAIL`, Secret: あなたのUnity IDのメールアドレス
   - Name: `UNITY_PASSWORD`, Secret: あなたのUnity IDのパスワード

## ワークフローの動作

### テストの実行

- Pull Requestが作成/更新されると自動的に実行されます
- 各Unityバージョンで EditMode テストを実行します
- コンパイルエラーがないことを確認します

### 対象ファイル

以下のファイルが変更された場合にCIが実行されます：

- `Assets/**`
- `Packages/**`
- `ProjectSettings/**`
- `.github/workflows/unity-test.yml`

## ローカルでのテスト

ローカルで同様のテストを実行する場合：

```bash
# Unity Editor をバッチモードで起動してテストを実行
"C:\Program Files\Unity\Hub\Editor\2021.3.45f1\Editor\Unity.exe" `
  -batchmode `
  -nographics `
  -silent-crashes `
  -logFile - `
  -projectPath . `
  -runTests `
  -testPlatform EditMode `
  -testResults results.xml
```

## トラブルシューティング

### ライセンスエラー

```
Invalid Unity license
```

- `UNITY_LICENSE` シークレットが正しく設定されているか確認
- ライセンスファイルの内容が完全にコピーされているか確認（改行も含む）

### バージョンエラー

```
Unity version not found
```

- Unityのバージョン番号が正しいか確認
- game-ciでサポートされているバージョンか確認: https://game.ci/docs/docker/versions

### キャッシュの問題

キャッシュが原因で問題が発生する場合は、GitHub Actions の Cache を削除してください：
Settings > Actions > Caches

## 参考リンク

- [GameCI Documentation](https://game.ci/docs)
- [unity-test-runner](https://game.ci/docs/github/test-runner)
- [unity-builder](https://game.ci/docs/github/builder)
- [Unity CI/CD Best Practices](https://docs.unity3d.com/Manual/UnityCloudBuild.html)
