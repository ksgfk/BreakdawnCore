#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Breakdawn
{
	public class AutoExportAssetPackage
	{
		public const string AssetPathName = "Assets/BreakdawnCore";
		public static readonly string RootPath = $"{Path.Combine(Application.dataPath, "../")}";

		[MenuItem("Breakdawn/导出Asset Package %e")]
		private static void LableDoExport()
		{
			Export();
		}
		/// <summary>
		/// 导出资源包,默认Assets/BreakdawnCore路径
		/// </summary>
		public static void Export()
		{
			var fileName = $"BreakdawnCore{DateTime.Now:yyyyMMdd_HHmmss}.unitypackage";
			AssetDatabase.ExportPackage(AssetPathName, fileName, ExportPackageOptions.Recurse);
			MoveFile(RootPath, fileName);
			Application.OpenURL($"file://{RootPath}/Build");
		}
		/// <summary>
		/// 导出资源包
		/// </summary>
		/// <param name="path">所有资源的路径</param>
		public static void Export(params string[] path)
		{
			var fileName = $"BreakdawnCore{DateTime.Now:yyyyMMdd_HHmmss}.unitypackage";
			AssetDatabase.ExportPackage(path, fileName, ExportPackageOptions.Recurse);
			MoveFile(RootPath, fileName);
			Application.OpenURL($"file://{RootPath}/Build");
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
#endif