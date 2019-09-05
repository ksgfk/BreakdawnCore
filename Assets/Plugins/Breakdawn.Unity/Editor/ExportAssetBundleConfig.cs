using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Unity.Editor
{
    [CreateAssetMenu(fileName = "ExportABConfig", menuName = "CreateExportABConfig", order = 0)]
    public class ExportAssetBundleConfig : ScriptableObject
    {
        public string exportPath = "Assets/StreamingAssets";
        public List<string> allPrefabPaths = new List<string>();
        public List<ABFileDirName> allABFileDir = new List<ABFileDirName>();
    }

    [Serializable]
    public struct ABFileDirName : IEquatable<ABFileDirName>
    {
        public string name;
        public string path;

        public bool Equals(ABFileDirName other)
        {
            return path + name == other.name + other.path;
        }
    }
}