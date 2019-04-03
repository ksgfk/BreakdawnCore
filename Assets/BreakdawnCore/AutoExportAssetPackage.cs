using System;
using System.IO;
#if UNITY_EDITOR
using UnityEngine;
#endif
using UnityEditor;


namespace Breakdawn
{
	public class AutoExportAssetPackage
	{
		public const string ASSET_PATH_NAME = "Assets/BreakdawnCore";
		public static readonly string ROOT_PATH = $"{Path.Combine(Application.dataPath, "../")}";

#if UNITY_EDITOR
		[MenuItem("Breakdawn/导出Asset Package %e")]
		private static void LableDoExport()
		{
			Export();
		}
#endif
		/// <summary>
		/// 导出资源包,默认Assets/BreakdawnCore路径
		/// </summary>
		public static void Export()
		{
			var fileName = $"BreakdawnCore{DateTime.Now:yyyyMMdd_HHmmss}.unitypackage";
			AssetDatabase.ExportPackage(ASSET_PATH_NAME, fileName, ExportPackageOptions.Recurse);
			MoveFile(ROOT_PATH, fileName);
			Application.OpenURL($"file://{ROOT_PATH}/Build");
		}
		/// <summary>
		/// 导出资源包
		/// </summary>
		/// <param name="path">所有资源的路径</param>
		public static void Export(params string[] path)
		{
			var fileName = $"BreakdawnCore{DateTime.Now:yyyyMMdd_HHmmss}.unitypackage";
			AssetDatabase.ExportPackage(path, fileName, ExportPackageOptions.Recurse);
			MoveFile(ROOT_PATH, fileName);
			Application.OpenURL($"file://{ROOT_PATH}/Build");
		}

		private static void MoveFile(string rootPath, string fileName)
		{
			var expFile = new FileInfo($"{rootPath}{fileName}");
			if (!Directory.Exists($"{rootPath}/Build"))
			{
				Directory.CreateDirectory($"{rootPath}/Build");
			}
			expFile.MoveTo($"{rootPath}/Build/{fileName}");
		}
	}
}
