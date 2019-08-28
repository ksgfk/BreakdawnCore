using Breakdawn.Unity;
using UnityEngine;

namespace Breakdawn.Test
{
    public class GameStart : MonoBehaviour
    {
        private GameObject _attack;

        private void Awake()
        {
            AssetBundleManager.Instance.LoadConfig("Assets/StreamingAssets");
        }

        private void Start()
        {
            _attack = AssetBundleManager.Instance.GetAssetDependAB("Attack.prefab").LoadAsset<GameObject>("Attack");
            _attack = AssetBundleManager.Instance.GetAssetDependAB("Attack.prefab").LoadAsset<GameObject>("Attack");
            var a = AssetBundleManager.Instance;
            Instantiate(_attack);
        }
    }
}