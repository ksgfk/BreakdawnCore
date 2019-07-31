using Breakdawn.Core;
using UnityEngine;

namespace Breakdawn
{
    public class GameObjectPool : ObjectPool<GameObject>
    {
        public GameObjectPool(IFactory<GameObject> factory, int initCount = 0) : base(factory, initCount)
        {
        }
    }
}