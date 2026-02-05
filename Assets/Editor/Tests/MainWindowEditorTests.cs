using System.Linq;
#if UNITY_EDITOR
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MasyoLab.Editor.FavoritesAsset.Tests
{
    public class MainWindowEditorTests
    {
        [SetUp]
        public void SetUp()
        {
            // Close existing windows to start clean
            var existing = Resources.FindObjectsOfTypeAll<MainWindow>();
            foreach (var w in existing)
            {
                (w as EditorWindow)?.Close();
            }
        }

        [Test]
        public void OpenMainWindow_DoesNotThrow_AndWindowExists()
        {
            // Execute the menu item that opens the window. Uses CONST.MENU_ITEM
            Assert.DoesNotThrow(() => EditorApplication.ExecuteMenuItem(CONST.MENU_ITEM));

            var win = Resources.FindObjectsOfTypeAll<MainWindow>().FirstOrDefault();
            Assert.IsNotNull(win, "MainWindow was not opened by menu item");
        }
    }
}
#endif
