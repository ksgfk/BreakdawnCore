using PENet;
using UnityEngine;

namespace Breakdawn.Core
{
	/// <summary>
	/// 客户端会话
	/// </summary>
	/// <typeparam name="T">消息协议</typeparam>
	public class ClientSession<T, K> : PESession<K> where T : class where K : PEMessage
	{
		internal TemplateNetworkManager<T, K> manager;

		protected override void OnConnected()
		{
			Debug.Log("已连接到服务器");
		}

		protected override void OnReciveMsg(K msg)
		{
			Debug.Log($"收到数据包:[cmd:{msg.cmd},err:{msg.err},seq:{msg.seq}]");
			manager.Messages.Enqueue(msg);
		}

		protected override void OnDisConnected()
		{
			Debug.Log("已断开连接");
		}
	}
}
