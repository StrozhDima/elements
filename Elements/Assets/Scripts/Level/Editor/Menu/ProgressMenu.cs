using UnityEditor;
using UnityEngine;

namespace Elements.Level.Editor
{
    public static class ProgressMenu
    {
        private const string MenuName = "Elements";

        [MenuItem(MenuName + "/Clear Progress")]
        private static void ClearProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log($"[{MenuName}] Progress cleared.");
        }
    }
}