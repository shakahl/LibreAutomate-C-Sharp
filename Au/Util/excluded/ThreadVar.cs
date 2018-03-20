using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;

using Au.Types;
using static Au.NoClass;

#if false //unfinished. Currently not used in lib.
namespace Au//.Util
{
	public static unsafe class Thread_
	{
		internal struct ProcessVariables
		{
			public int FlsIndex;
		}
		static ProcessVariables* _ProcVar => &Util.LibProcessMemory.Ptr->thread_;

		public static event EventHandler Exit
		{
			add
			{
				var v = _ProcVar;
				if(v->FlsIndex == 0) {
					lock("DFXSMJh8qUiZj077K1qBkg") {
						if(v->FlsIndex == 0) {
							var d = Util.AppDomain_.GetDefaultDomain(out bool isThisDomainDefault);
							if(isThisDomainDefault) _SetCallback(); else d.DoCallBack(_SetCallback);
						}
					}
				}
				if(v->FlsIndex == -1) throw new AuException();
				FlsSetValue(v->FlsIndex, (IntPtr)_domainHandle);

				//_Exit += value;

				//tested: cannot use [ThreadStatic] and ThreadLocal<T> to store event handlers.
				//	When the callback is called, they are cleared. probably CLR does it in its DllMain thread detach handler.

				//if(_exit == null) _exit = new List<EventHandler>();
				//Print("Exit", Api.GetCurrentThreadId(), Thread.GetDomainID()/*, _exit!=null*/);
				//_exit.Add(value);
				//_exit2.Value.Add(value);

				lock("DFXSMJh8qUiZj077K1qBkg") {
					//TODO: finish. Or reject.
				}
			}
			remove
			{
				lock("DFXSMJh8qUiZj077K1qBkg") {

				}
				//_Exit -= value;
				//_exit?.Remove(value);
				//_exit2.Value.Remove(value);
			}
		}
		//static event EventHandler _Exit;
		//static MulticastDelegate _exit;
		//[ThreadStatic] static List<EventHandler> _exit;
		//static ThreadLocal<List<EventHandler>> _exit2=new ThreadLocal<List<EventHandler>>(()=>new List<EventHandler>());
		static Dictionary<int, List<EventHandler>> _exit=new Dictionary<int, List<EventHandler>>();

		static GCHandle _domainHandle=GCHandle.Alloc(AppDomain.CurrentDomain);

		//always runs in default domain
		static void _SetCallback()
		{
			var v = _ProcVar;
			Debug.Assert(v->FlsIndex == 0 && _flsCallback==null);
			v->FlsIndex = FlsAlloc(_flsCallback = _FlsCallback); //tested: max 127, then fails
			if(v->FlsIndex == -1) return;
		}

		static PFLS_CALLBACK_FUNCTION _flsCallback;

		//always runs in default domain
		static void _FlsCallback(IntPtr param)
		{
			//Print("_FlsCallback", Api.GetCurrentThreadId());

			var domain = GCHandle.FromIntPtr(param).Target as AppDomain;
			if(domain.IsDefaultAppDomain()) _OnExit(); else domain.DoCallBack(_OnExit);
		}

		static void _OnExit()
		{
			Print("_OnExit", Api.GetCurrentThreadId(), Thread.GetDomainID()/*, _exit!=null, _exit2.IsValueCreated*/);

			//_Exit?.Invoke(null, null);

			//if(_exit == null) return;
			//for(int i=0; i< _exit.Count; i++) {
			//	_exit[i].Invoke(null, null);
			//}
		}

		internal delegate void PFLS_CALLBACK_FUNCTION(IntPtr lpFlsData);

		[DllImport("kernel32.dll")]
		internal static extern int FlsAlloc(PFLS_CALLBACK_FUNCTION lpCallback);

		[DllImport("kernel32.dll")]
		internal static extern bool FlsSetValue(int dwFlsIndex, IntPtr lpFlsData);

		[DllImport("kernel32.dll")]
		internal static extern bool FlsFree(int dwFlsIndex);
	}



	//[DebuggerStepThrough]
	public class ThreadVar<T> where T : class//, IDisposable
	{
		[ThreadStatic] static T _value;
		Func<T> _initFunc;


		public ThreadVar()
		{
		}

		public ThreadVar(Func<T> initFunc)
		{
			_initFunc = initFunc;
		}

		public bool IsValueCreated => _value != null;

		public T Value
		{
			get
			{
				if(_value == null) Value = _initFunc();
				return _value;
			}
			set
			{
				if(_value != null) throw new InvalidOperationException();
				_value = value;
				_LaterDispose();
			}
		}

		void _LaterDispose()
		{
			//todo: subscribe to Thread_.Exit.
		}
	}
}
#endif
