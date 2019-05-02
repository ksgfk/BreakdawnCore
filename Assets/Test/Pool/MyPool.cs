using Breakdawn.Core;
using System.Collections;
using UnityEngine;

namespace Breakdawn.Test
{
	public class MyPool : MonoBehaviour
	{
		private EntrustFactory<GameObject> mfactory;
		private IObjectPool<GameObject> mPool;

		private SeriesFactory<string, GameObject> sFactory;
		private SeriesPool<string, GameObject> sPool;

		private void Start()
		{
			//mfactory = new EntrustFactory<GameObject>(() =>
			//{
			//	var res = Resources.Load<GameObject>("Cube (1)");
			//	var go = GameObject.Instantiate(res);
			//	go.transform.parent = transform;
			//	go.SetActive(false);
			//	return go;
			//});
			//mPool = new EntrustPool<GameObject>(mfactory, 10, (go) => go.SetActive(false));


			sFactory = new SeriesFactory<string, GameObject>()
				.Add("Cube1", () =>
				{
					var res = Resources.Load<GameObject>("Cube (1)");
					var go = GameObject.Instantiate(res);
					go.transform.parent = transform;
					go.SetActive(false);
					return go;
				})
				.Add("Sphere", () =>
				{
					var res = Resources.Load<GameObject>("Sphere");
					var go = GameObject.Instantiate(res);
					go.transform.parent = transform;
					go.SetActive(false);
					return go;
				});
			sPool = new SeriesPool<string, GameObject>(sFactory, 10)
				.SetResetMethod("Cube1", (go) => go.SetActive(false))
				.SetResetMethod("Sphere", (go) => go.SetActive(false));

			//var i = mPool.Get().Show();

			var a = sPool.Get("Cube1").Show();
			//this.InvokeCoroutine(() => mPool.Recycling(i), 5);
			this.InvokeCoroutine(() => sPool.Recycling("Cube1", a), 5);
		}
	}
}
