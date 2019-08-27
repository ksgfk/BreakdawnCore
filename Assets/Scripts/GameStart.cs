using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private void Awake()
        {
            AssetBundleManager.Instance.LoadConfig("Assets/StreamingAssets");
        }
    }
}