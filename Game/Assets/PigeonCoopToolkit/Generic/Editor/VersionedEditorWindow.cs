using UnityEditor;
using UnityEngine;

namespace PigeonCoopToolkit.Generic.Editor
{
    public abstract class VersionedEditorWindow : EditorWindow
    {
        public abstract VersionInformation VersionInformation();

        public void ShowAbout(Texture2D logo, string userGuidePath)
        {
            AboutUtillityWindow win = GetWindow<AboutUtillityWindow>(true, "About",true);
            win.Init(logo, VersionInformation(), userGuidePath);
        }
    }
}
