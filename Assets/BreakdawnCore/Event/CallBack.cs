namespace Breakdawn.Event
{
	public delegate void CallBack();
	public delegate void CallBack<in T>(T args);
	public delegate void CallBack<in T, in TA>(T arg, TA arg1);
	public delegate void CallBack<in T, in TA, in TB>(T arg, TA arg1, TB arg2);
	public delegate void CallBack<in T, in TA, in TB, in TC>(T arg, TA arg1, TB arg2, TC arg3);
	public delegate void CallBack<in T, in TA, in TB, in TC, in TD>(T arg, TA arg1, TB arg2, TC arg3, TD arg4);
}
