using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Breakdawn.Unity.Editor
{
    [CreateAssetMenu(fileName = "ExportABConfig", menuName = "CreateExportABConfig", order = 0)]
    public class ExportAssetBundleConfig : ScriptableObject
    {
        public List<string> allPrefabPaths = new List<string>();
        public List<ABFileDirName> allABFileDirName = new List<ABFileDirName>();
    }

    [Serializable]
    public struct ABFileDirName
    {
        public string Name;
        public string Path;
    }
}