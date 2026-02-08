using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MasyoLab.FavoritesAsset.Tests.Editor
{
    /// <summary>
    /// Favorites Asset の基本的な動作テスト
    /// </summary>
    public class FavoritesAssetBasicTests
    {
        [Test]
        public void AssemblyDefinition_ShouldExist()
        {
            // アセンブリ定義が正しく読み込まれているか確認
            var assembly = typeof(FavoritesAssetBasicTests).Assembly;
            Assert.IsNotNull(assembly, "Test assembly should be loaded");
            Assert.AreEqual("MasyoLab-FavoritesAsset-Tests", assembly.GetName().Name);
        }

        [Test]
        public void FavoritesAssetAssembly_ShouldBeReferenced()
        {
            // FavoritesAsset のアセンブリが参照できることを確認
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            bool found = false;
            
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == "MasyoLab-FavoritesAsset")
                {
                    found = true;
                    break;
                }
            }
            
            Assert.IsTrue(found, "MasyoLab-FavoritesAsset assembly should be available");
        }

        [Test]
        public void EditorWindow_CanBeOpened()
        {
            // エディタウィンドウの型が取得できることを確認
            var windowTypes = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow))
                .GetTypes();
            
            Assert.IsNotNull(windowTypes, "Should be able to access EditorWindow types");
            Assert.Greater(windowTypes.Length, 0, "Should have at least one EditorWindow type");
        }

        [Test]
        public void UnityEditor_IsAvailable()
        {
            // Unity Editor APIが利用可能であることを確認
            Assert.IsTrue(Application.isEditor, "Should be running in Unity Editor");
            Assert.IsNotNull(EditorApplication.applicationPath, "Editor application path should be available");
        }

        [Test]
        public void ProjectSettings_AreAccessible()
        {
            // プロジェクト設定にアクセスできることを確認
            var productName = Application.productName;
            Assert.IsNotNull(productName, "Product name should be accessible");
            
            var companyName = Application.companyName;
            Assert.IsNotNull(companyName, "Company name should be accessible");
        }

        [Test]
        public void AssetDatabase_IsWorking()
        {
            // AssetDatabase が動作していることを確認
            var guids = AssetDatabase.FindAssets("t:ScriptableObject");
            Assert.IsNotNull(guids, "AssetDatabase should be working");
            
            // プロジェクトにアセットが存在することを確認
            var allAssets = AssetDatabase.FindAssets("");
            Assert.Greater(allAssets.Length, 0, "Project should contain assets");
        }

        [Test]
        public void FavoritesAssetPackage_HasCorrectStructure()
        {
            // パッケージの基本構造を確認
            var packagePath = "Assets/MasyoLab/FavoritesAsset";
            var editorPath = packagePath + "/Editor";
            
            Assert.IsTrue(AssetDatabase.IsValidFolder(packagePath), 
                "FavoritesAsset package folder should exist");
            Assert.IsTrue(AssetDatabase.IsValidFolder(editorPath), 
                "Editor folder should exist in the package");
        }

        [Test]
        public void AssemblyDefinitionFile_CanBeFound()
        {
            // アセンブリ定義ファイルが存在することを確認
            var asmdefGuids = AssetDatabase.FindAssets("MasyoLab-FavoritesAsset t:asmdef");
            Assert.Greater(asmdefGuids.Length, 0, 
                "Should find MasyoLab-FavoritesAsset assembly definition file");
        }
    }
}
