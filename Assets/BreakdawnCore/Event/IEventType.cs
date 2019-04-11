namespace Breakdawn.Event
{
	public interface IEventType<K, V>
	{
		void AddEventType(K eventKey, V value);

		V GetEventType(K eventKey);
	}
}
