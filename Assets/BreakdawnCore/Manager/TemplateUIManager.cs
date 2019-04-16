using Breakdawn.Expansion;
using Breakdawn.Factory;
using Breakdawn.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breakdawn.Manager
{
	public abstract class TemplateUIManager<T> : TemplateSingleton<T> where T : class
	{
		protected ISeriesFactory<string, GameObject> prefabPanels;
		protected string uiPrefabPath;
		protected GameObject canvas;
		protected Dictionary<string, GameObject> instancePanels;

		protected TemplateUIManager()
		{
			uiPrefabPath = SetUIPrefabPath();
			canvas = GameObject.Find("Canvas") ?? throw new Exception($"UI Manager异常:请在Hierarchy面板创建一个Canvas");
			prefabPanels = new ResourceFactory<GameObject>(uiPrefabPath);
			instancePanels = new Dictionary<string, GameObject>();
		}

		/// <summary>
		/// UI预制体在Resources文件夹下的路径
		/// </summary>
		protected abstract string SetUIPrefabPath();

		/// <summary>
		/// 加载UI面板
		/// </summary>
		/// <param name="name">UI面板预制体的名字</param>
		/// <param name="layer">设置层级,注意越后生成面板,渲染时越靠底层</param>
		/// <param name="isReset">是否重置</param>
		/// <returns>UI面板实例</returns>
		public GameObject LoadPanel(string name, int layer = 0, bool isReset = false)
		{
			var panelPrefab = prefabPanels.GetElement(name);
			var panel = GameObject.Instantiate(panelPrefab, canvas.transform);
			instancePanels.Add(name, panel);
			var panelRectTrans = panel.transform as RectTransform ?? throw new Exception($"UI Manager异常:预制体{panelPrefab}可能不是UI");
			panelRectTrans.SetSiblingIndex(layer);

			if (isReset)
			{
				panelRectTrans.offsetMin = Vector2.zero;
				panelRectTrans.offsetMax = Vector2.zero;
				panelRectTrans.anchoredPosition3D = Vector3.zero;
				panelRectTrans.anchorMin = Vector2.zero;
				panelRectTrans.anchorMax = Vector2.one;
			}

			return panel;
		}

		public GameObject HidePanel(string name)
		{
			if (instancePanels.TryGetValue(name, out var v))
			{
				return v.Hide();
			}
			throw new Exception($"UIManager异常:不存在{name}面板实例");
		}

		public void DestoryInstancePanel(string name)
		{
			if (instancePanels.TryGetValue(name, out var v))
			{
				GameObject.Destroy(v);
				instancePanels.Remove(name);
			}
			else
			{
				throw new Exception($"UIManager异常:不存在{name}面板实例");
			}
		}
	}
}
