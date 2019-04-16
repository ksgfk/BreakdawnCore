using Breakdawn.Factory;
using Breakdawn.Singleton;
using UnityEngine;

namespace Breakdawn.Manager
{
	public abstract class TemplateUIManager<T> : TemplateSingleton<T> where T : class
	{
		protected ResourcesLoadFactory<GameObject> panels;
		protected string uiPrefabPath;
		protected GameObject canvas;

		protected TemplateUIManager()
		{
			uiPrefabPath = SetUIPrefabPath();
			canvas = GameObject.Find("Canvas");
			panels = new ResourcesLoadFactory<GameObject>(uiPrefabPath);
		}

		/// <summary>
		/// UI预制体在Resources文件夹下的路径
		/// </summary>
		protected abstract string SetUIPrefabPath();

		/// <summary>
		/// 加载UI面板
		/// </summary>
		/// <param name="name">UI面板预制体的名字</param>
		/// <param name="isReset">是否重置</param>
		/// <returns>UI面板实例</returns>
		public GameObject LoadPanel(string name, bool isReset = false)
		{
			var panelPrefab = panels.Get(name);
			var panel = GameObject.Instantiate(panelPrefab, canvas.transform);
			if (isReset)
			{
				var panelRectTrans = panel.transform as RectTransform;
				panelRectTrans.offsetMin = Vector2.zero;
				panelRectTrans.offsetMax = Vector2.zero;
				panelRectTrans.anchoredPosition3D = Vector3.zero;
				panelRectTrans.anchorMin = Vector2.zero;
				panelRectTrans.anchorMax = Vector2.one;
			}

			return panel;
		}
	}
}
