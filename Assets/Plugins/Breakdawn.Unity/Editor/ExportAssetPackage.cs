using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Breakdawn.Unity.Editor
{
    [Serializable]
    public class ExportAssetPackageConfig
    {
        [XmlArray("exportPaths")] public List<string> exportPaths;
        [XmlAttribute("targetPath")] public string targetPath;
        [XmlAttribute("packageName")] public string packageName;
        [XmlAttribute("suffix")] public string suffix;
    }

    public class ExportAssetPackage : EditorWindow
    {
        [SerializeField] private List<string> exportPaths;
        private SerializedObject _serializedObject;
        private SerializedProperty _assetLstProperty;
        private string _targetPath = "";
        private string _packageName = "";
        private string _suffix = "";
        private ExportAssetPackageConfig _assetPackageConfig;

        [MenuItem("Breakdawn/导出Asset Package %e")]
        private static void MenuClicker()
        {
            var rect = new Rect(50, 50, 400, 400);
            GetWindowWithRect<ExportAssetPackage>(rect, false, "导出资源包");
        }

        private void OnEnable()
        {
            try
            {
                var stream = new FileStream($"{Application.dataPath}/Resources/ExportAssetPackageConfig.xml",
                    FileMode.Open, FileAccess.Read, FileShare.Read);
                var reader = new StreamReader(stream, Encoding.UTF8);
                var xml = new XmlSerializer(typeof(ExportAssetPackageConfig));
                _assetPackageConfig = xml.Deserialize(reader) as ExportAssetPackageConfig;
                stream.Close();
                reader.Close();
            }
            catch (FileNotFoundException)
            {
                _assetPackageConfig = new ExportAssetPackageConfig();
            }

            if (_assetPackageConfig != null)
            {
                exportPaths = _assetPackageConfig.exportPaths;
                _targetPath = _assetPackageConfig.targetPath;
                _packageName = _assetPackageConfig.packageName;
                _suffix = _assetPackageConfig.suffix;
            }
            else
            {
                exportPaths = new List<string>();
            }

            _serializedObject = new SerializedObject(this);
            _assetLstProperty = _serializedObject.FindProperty("exportPaths");
        }

        private void OnGUI()
        {
            _serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_assetLstProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }

            _packageName = EditorGUILayout.TextField("资源包名:", _packageName);
            _suffix = EditorGUILayout.TextField("后缀:", _suffix);
            _targetPath = EditorGUILayout.TextField("导出路径:", _targetPath);
            if (GUILayout.Button("导出路径", GUILayout.ExpandWidth(true)))
            {
                _targetPath = EditorUtility.OpenFolderPanel("导出路径", _targetPath, "");
            }

            if (!GUILayout.Button("导出", GUILayout.Height(50)))
            {
                return;
            }

            Export();
            Application.OpenURL(_targetPath);
        }

        private void OnDestroy()
        {
            if (_assetPackageConfig == null)
            {
                _assetPackageConfig = new ExportAssetPackageConfig();
            }

            _assetPackageConfig.suffix = _suffix;
            _assetPackageConfig.exportPaths = exportPaths;
            _assetPackageConfig.packageName = _packageName;
            _assetPackageConfig.targetPath = _targetPath;

            var stream = new FileStream($"{Application.dataPath}/Resources/ExportAssetPackageConfig.xml",
                FileMode.Create, FileAccess.Write, FileShare.Write);
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var xml = new XmlSerializer(typeof(ExportAssetPackageConfig));
            xml.Serialize(writer, _assetPackageConfig);
            writer.Close();
            stream.Close();
        }

        private void Export()
        {
            var fileName = $"{_packageName}-{_suffix}.unitypackage";
            AssetDatabase.ExportPackage(exportPaths.ToArray(), fileName, ExportPackageOptions.Recurse);
            MoveFile(_targetPath, fileName);
        }

        private static void MoveFile(string target, string fileName)
        {
            var rootPath = $"{Path.Combine(Application.dataPath, "../")}";
            var expFile = new FileInfo($"{rootPath}{fileName}");
            if (!Directory.Exists($"{target}"))
            {
                Directory.CreateDirectory($"{target}");
            }

            expFile.MoveTo($"{target}/{fileName}");
        }
    }
}