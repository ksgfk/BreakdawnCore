using UnityEditor;

namespace Breakdawn.Unity.Editor
{
    public class ExportAssetBundle
    {
        [MenuItem("Breakdawn/构建Asset Bundle")]
        public static void Build()
        {
            var abConfig =
                AssetDatabase.LoadAssetAtPath<ExportAssetBundleConfig>("Assets/Resources/ExportABConfig.asset");
        }
    }
}