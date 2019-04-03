using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn
{
	public class ReflactToInstance : MonoBehaviour
	{
		private Text t;

		private void Awake()
		{
			t = GameObject.Find("Canvas/Text").GetComponent<Text>();
			//Init();
		}

		public void Init()
		{
			var classes = Assembly.GetExecutingAssembly().GetTypes();
			var types = from need in classes where need.IsDefined(typeof(TestAttribute)) select need;
			foreach (var type in types)
			{
				var attr = type.GetCustomAttribute<TestAttribute>();
				var instance = Activator.CreateInstance(type, attr.Id);

				Debug.Log(instance.ToString());
				t.text += instance.ToString();
			}
		}
	}
}
