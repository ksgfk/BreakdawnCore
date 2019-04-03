using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Breakdawn
{
	public class AutoExportAssetPackage
	{
		[MenuItem("Breakdawn/导出Asset Package %e")]
		private static void DoExport()
		{
			string[] assetPathName = {"Assets/BreakdawnCore"};
			var rootPath = $"{Path.Combine(Application.dataPath, "../")}";
			var fileName = $"BreakdawnCore{DateTime.Now:yyyyMMdd_HHmmss}.unitypackage";

			AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);

			var expFile = new FileInfo($"{rootPath}{fileName}");
			if (!Directory.Exists($"{rootPath}/Build"))
			{
				Directory.CreateDirectory($"{rootPath}/Build");
			}
			expFile.MoveTo($"{rootPath}/Build/{fileName}");

			Application.OpenURL($"file://{rootPath}/Build");
		}
	}
}
