using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Breakdawn.Unity.Editor
{
    [CreateAssetMenu(fileName = "ExportABConfig", menuName = "CreateExportABConfig", order = 0)]
    public class ExportAssetBundleConfig : ScriptableObject
    {
        public string exportPath = Application.streamingAssetsPath;
        public List<string> allPrefabPaths = new List<string>();
        public List<ABFileDirName> allABFileDir = new List<ABFileDirName>();
    }

    [Serializable]
    public struct ABFileDirName
    { 
        public string name;
        public string path;
    }
}