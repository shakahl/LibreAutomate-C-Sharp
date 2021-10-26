#if !true
namespace Au.More
{
	//tested: new SynchronizationContext() pumps messages in STA thread.
	//	Dispatches sent (all?) and some posted (eg WM_PAINT) messages, and don't know what does with others.

	/// <summary>
	/// A <b>SynchronizationContext</b> that does not pump messages etc even in STA thread.
	/// </summary>
	class NoPumpSynchronizationContext_ : SynchronizationContext
	{
		public NoPumpSynchronizationContext_() {
			base.SetWaitNotificationRequired();
		}

		public override unsafe int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout) {
#if !true //cannot be used in UI thread where we would want it to use. Deadlock.
			fixed (IntPtr* p = waitHandles)
				return Api.WaitForMultipleObjectsEx(waitHandles.Length, p, waitAll, millisecondsTimeout, false);
#elif !true //the same as default WPF sync context. Probably .NET uses the API.
			var hr = CoWaitForMultipleHandles(waitAll ? 1u : 0u, millisecondsTimeout, waitHandles.Length, waitHandles, out int i);
			if (hr == 0) return i;
			if (hr == RPC_S_CALLPENDING) return Api.WAIT_TIMEOUT;
			return Api.WAIT_FAILED;
			//throw new Win32Exception(hr);
#else //only sent messages and COM etc events, but no posted messages. The Delm code hangs anyway.
			long t1 = millisecondsTimeout == -1 ? 0 : (Environment.TickCount64 + (uint)millisecondsTimeout);
			fixed (IntPtr* p = waitHandles) {
				for (; ; ) {
					int r = Api.MsgWaitForMultipleObjectsEx(waitHandles.Length, p, millisecondsTimeout, Api.QS_SENDMESSAGE, waitAll ? Api.MWMO_WAITALL : 0);
					if (r != waitHandles.Length) return r;
					if (Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE | Api.PM_QS_SENDMESSAGE)) return Api.WAIT_TIMEOUT; //WM_QUIT
					if (millisecondsTimeout != -1) {
						long t2 = t1 - Environment.TickCount64; if (t2 <= 0) return Api.WAIT_TIMEOUT;
						millisecondsTimeout = (int)t2;
					}
				}
			}
#endif
			//what to return on timeout or when fails? Or should throw exception? It's not documented.
		}

		//[DllImport("ole32.dll", PreserveSig = true)]
		//internal static extern int CoWaitForMultipleHandles(uint dwFlags, int dwTimeout, int cHandles, [In] IntPtr[] pHandles, out int lpdwindex);

		//internal const int RPC_S_CALLPENDING = unchecked((int)0x80010115);

		/// <summary>
		/// Gets single auto-created <b>NoPumpSynchronizationContext_</b> of this thread.
		/// </summary>
		public static NoPumpSynchronizationContext_ Default => t_sc ??= new();

		[ThreadStatic] static NoPumpSynchronizationContext_ t_sc;

		/// <summary>
		/// Temporarily sets <see cref="NoPumpSynchronizationContext_"/> for this thread. Restores the old context when disposing.
		/// Example: <c>using var noPump = new NoPumpSynchronizationContext_.Scope();</c>
		/// </summary>
		public struct Scope : IDisposable
		{
			readonly SynchronizationContext _sc;

			public Scope() {
				_sc = SynchronizationContext.Current;
				SynchronizationContext.SetSynchronizationContext(NoPumpSynchronizationContext_.Default);
			}

			public void Dispose() { SynchronizationContext.SetSynchronizationContext(_sc); }
		}
	}
}
#endif
