using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Breakdawn.Unity.Editor
{
    /// <summary>
    /// 读取Assets/Resources下的ExportABConfig.asset文件来导出AB包
    /// 生成两份配置表，XML版在 工程根目录/Logs/AssetConfig.xml，二进制版在Assets/StreamingAssets/AssetConfig.config
    /// 二进制配置表不包含资源路径
    /// </summary>
    public class ExportAssetBundle
    {
        private static string _exportPath;

        /// <summary>
        /// key:资源文件夹名，value:资源文件夹路径
        /// </summary>
        private readonly Dictionary<string, string> _allFileDir;

        /// <summary>
        /// 资源文件夹路径
        /// </summary>
        private readonly List<string> _allFileAB;

        /// <summary>
        /// key:预制体name，value:预制体依赖文件路径
        /// </summary>
        private readonly Dictionary<string, List<string>> _allPrefabsDir = new Dictionary<string, List<string>>();

        /// <summary>
        /// 有效路径
        /// </summary>
        private readonly List<string> _validPaths = new List<string>();

        [MenuItem("Breakdawn/构建Asset Bundle")]
        public static void Build()
        {
            var abConfig =
                AssetDatabase.LoadAssetAtPath<ExportAssetBundleConfig>("Assets/Resources/ExportABConfig.asset"); //加载配置
            _exportPath = abConfig.exportPath;
            var eab = new ExportAssetBundle(abConfig);
            var allPrefabsGuid = AssetDatabase.FindAssets("t:Prefab", abConfig.allPrefabPaths.ToArray()); //获取所有Prefab
            for (var i = 0; i < allPrefabsGuid.Length; i++)
            {
                var prefabGuid = allPrefabsGuid[i];
                var path = AssetDatabase.GUIDToAssetPath(prefabGuid); //获取Prefab的GUID
                EditorUtility.DisplayProgressBar("查找Prefabs", $"Prefab:{path}", i * 1F / allPrefabsGuid.Length);
                eab._validPaths.Add(path);
                if (eab.ContainAllFileAB(path))
                {
                    continue;
                }

                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (eab._allPrefabsDir.ContainsKey(go.name))
                {
                    Debug.LogError($"存在相同名字的Prefab:{go.name}");
                    return;
                }

                var allDepends = AssetDatabase.GetDependencies(path); //获取Prefab依赖
                var allDependsPath = new List<string>();
                foreach (var dependPath in from depend in allDepends
                    where !eab.ContainAllFileAB(depend) && !depend.EndsWith(".cs")
                    select depend) //获取Prefab依赖文件的路径
                {
                    eab._allFileAB.Add(dependPath);
                    allDependsPath.Add(dependPath);
                }

                eab._allPrefabsDir.Add(go.name, allDependsPath);
            }

            foreach (var name in eab._allFileDir.Keys) //设置AB包名
            {
                SetABName(name, eab._allFileDir[name]);
            }

            foreach (var name in eab._allPrefabsDir.Keys) //设置AB包名
            {
                SetABName(name, eab._allPrefabsDir[name]);
            }

            try
            {
                eab.BuildAssetBundles();
            }
            finally
            {
                Array.ForEach(AssetDatabase.GetAllAssetBundleNames(),
                    name => AssetDatabase.RemoveAssetBundleName(name, true));
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        private ExportAssetBundle(ExportAssetBundleConfig abConfig)
        {
            _allFileDir = new Dictionary<string, string>(abConfig.allABFileDir.Count);
            _allFileAB = new List<string>(abConfig.allABFileDir.Count);
            foreach (var config in abConfig.allABFileDir)
            {
                _allFileDir.Add(config.name, config.path);
                _allFileAB.Add(config.path);
                _validPaths.Add(config.path);
            }
        }

        /// <summary>
        /// 是否已经获取到了文件
        /// </summary>
        /// <param name="path">文件路径</param>
        private bool ContainAllFileAB(string path)
        {
            return _allFileAB.Any(fileAB =>
                fileAB == path || (path.Contains(fileAB) && path.Replace(fileAB, string.Empty)[0] == '/'));
        }

        private static void SetABName(string name, string path)
        {
            var assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                Debug.LogError($"不存在文件路径:{path}");
                return;
            }

            assetImporter.assetBundleName = name;
        }

        private static void SetABName(string name, IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                SetABName(name, path);
            }
        }

        private void BuildAssetBundles()
        {
            var allABs = AssetDatabase.GetAllAssetBundleNames();
            var resPaths = new Dictionary<string, string>(); //key:路径，value:包名
            foreach (var ab in allABs)
            {
                var allPaths = AssetDatabase.GetAssetPathsFromAssetBundle(ab);
                foreach (var path in allPaths)
                {
                    if (path.EndsWith(".cs"))
                    {
                        continue;
                    }

                    Debug.Log($"包 {ab} 下包含资源 {path}");
                    if (IsValidPath(path))
                    {
                        resPaths.Add(path, ab);
                    }
                }
            }

            RemoveNoUseAB();

            var manifest = BuildPipeline.BuildAssetBundles(_exportPath,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
            WriteABsInfo(resPaths);
        }

        private static void RemoveNoUseAB()
        {
            var allABs = AssetDatabase.GetAllAssetBundleNames();
            var direction = new DirectoryInfo(_exportPath);
            var fileInfos = direction.GetFiles("*", SearchOption.AllDirectories);
            foreach (var info in fileInfos)
            {
                if (ContainABName(info.Name, allABs) || info.Name.EndsWith(".meta"))
                {
                    continue;
                }

                Debug.Log($"该AB包可能已被删除或改名:{info.Name}");
                if (File.Exists(info.FullName))
                {
                    File.Delete(info.FullName);
                }
            }
        }

        /// <summary>
        /// 检查AB包名是否重复
        /// </summary>
        /// <param name="name">检查的AB包</param>
        /// <param name="abs">所有AB包</param>
        /// <returns>是否重复</returns>
        private static bool ContainABName(string name, IEnumerable<string> abs)
        {
            return abs.Any(str => str == name);
        }

        private static void WriteABsInfo(Dictionary<string, string> resPath)
        {
            var config = new AssetConfig
            {
                assetList = new List<AssetInfo>()
            };
            //var crc32 = new CRC32();
            foreach (var path in resPath.Keys)
            {
                var resDepend = AssetDatabase.GetDependencies(path);
                var dependList = new List<string>();
                foreach (var depend in resDepend)
                {
                    if (depend == path || path.EndsWith(".cs"))
                    {
                        continue;
                    }

                    if (!resPath.TryGetValue(depend, out var name))
                    {
                        continue;
                    }

                    if (name == resPath[path])
                    {
                        continue;
                    }

                    if (!dependList.Contains(name))
                    {
                        dependList.Add(name);
                    }
                }

                var abBase = new AssetInfo(resPath[path], path, dependList);
                config.assetList.Add(abBase);
            }

            var xmlPath = $"{Application.dataPath}/../Logs/AssetConfig.xml";
            var fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.Write, FileShare.Write);
            var writer = new StreamWriter(fileStream, Encoding.UTF8);
            var serializer = new XmlSerializer(typeof(AssetConfig));
            serializer.Serialize(writer, config);
            writer.Close();
            fileStream.Close();

            var bytePath = $"{_exportPath}/AssetConfig.config";
            var fs = new FileStream(bytePath, FileMode.Create, FileAccess.Write, FileShare.Write);
            var binary = new BinaryFormatter();
            binary.Serialize(fs, config);
            fs.Close();
        }

        private bool IsValidPath(string path)
        {
            return _validPaths.Any(path.Contains);
        }
    }
}