#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Breakdawn.Unity
{
    public class ExportAssetPackage : EditorWindow
    {
        [SerializeField] private List<string> exportPaths = new List<string>();
        private SerializedObject _serializedObject;
        private SerializedProperty _assetLstProperty;
        private string _targetPath = "";
        private string _packageName = "";
        private string _suffix = "";

        [MenuItem("Breakdawn/导出Asset Package %e")]
        private static void MenuClicker()
        {
            var rect = new Rect(50, 50, 400, 400);
            GetWindowWithRect<ExportAssetPackage>(rect, false, "导出资源包");
        }

        private void OnEnable()
        {
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
#endif