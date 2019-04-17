using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Breakdawn.Core
{
	public abstract class TemplateUIManager<T> : TemplateSingleton<T> where T : class
	{
		protected ISeriesFactory<string, GameObject> prefabPanels;
		protected Dictionary<string, GameObject> instancePanels;
		protected GameObject canvas;
		protected CanvasScaler canvasScaler;

		protected TemplateUIManager(ISeriesFactory<string, GameObject> panelsFactory)
		{
			canvas = GameObject.Find("Canvas") ?? throw new Exception($"UI Manager异常:请在Hierarchy面板创建一个Canvas");
			canvasScaler = canvas.GetComponent<CanvasScaler>();
			prefabPanels = panelsFactory;
			instancePanels = new Dictionary<string, GameObject>();
		}
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
			if (instancePanels.ContainsKey(name))
			{
				throw new Exception($"UI Manager异常:预制体{panelPrefab}已经被实例化了");
			}
			else
			{
				instancePanels.Add(name, panel);
			}
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

		public void SetCanvasResolution(float width, float height, float matchWidthOrHeight)
		{
			canvasScaler.referenceResolution = new Vector2(width, height);
			canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
		}
	}
}
