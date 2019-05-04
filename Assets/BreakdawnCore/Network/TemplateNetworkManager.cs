using PENet;
using System.Collections.Concurrent;
using UnityEngine;

namespace Breakdawn.Core
{
	public abstract class TemplateNetworkManager<T, K> : TemplateSingleton<T> where T : class where K : PEMessage
	{
		protected PESocket<ClientSession<T, K>, K> socket;
		protected string ip;
		protected int port;

		public ConcurrentQueue<K> Messages { get; private set; }

		protected TemplateNetworkManager(string ip, int port)
		{
			this.ip = ip;
			this.port = port;

			socket = new PESocket<ClientSession<T, K>, K>();
			Messages = new ConcurrentQueue<K>();
			socket.Session.manager = this;
			socket.StartAsClient(ip, port);

			socket.SetLog(true, (message, level) =>
			{
				var l = (LogLevel)level;
				switch (l)
				{
					case LogLevel.None:
						Debug.Log(message);
						break;
					case LogLevel.Warn:
						Debug.LogWarning(message);
						break;
					case LogLevel.Error:
						Debug.LogError(message);
						break;
					case LogLevel.Info:
						Debug.Log(message);
						break;
					default:
						break;
				}
			});
		}

		/// <summary>
		/// 在主管理员的Update里调用
		/// </summary>
		public virtual void OnUpdate()
		{
			if (!Messages.IsEmpty)
			{
				if (Messages.TryDequeue(out var msg))
				{
					ProcessMessage(msg);
				}
				else
				{
					Debug.LogWarning("可能有多个线程竞争消息队列");
				}
			}
		}

		protected abstract void ProcessMessage(K msg);

		public void SendNetMessage(K msg)
		{
			if (socket.Session != null)
			{
				socket.Session.SendMsg(msg);
			}
			else
			{
				Debug.LogWarning("未连接到服务器,请重试");
				socket = new PESocket<ClientSession<T, K>, K>();
				socket.StartAsClient(ip, port);
			}
		}
	}
}
