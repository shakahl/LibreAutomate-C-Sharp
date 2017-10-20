using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

using JObject = System.Int64; //64-bit in 32-bit process too, unless it is on 32-bit OS which we don't support

namespace Catkeys
{
	public partial class Acc
	{
		static unsafe class _Java
		{
			/// <summary>
			/// Loads Java Access Bridge (JAB) dll and functions. Allows to call the functions.
			/// Need this because 64-bit and 32-bit versions of the dll have different names, and [DllImport] does not support it.
			/// </summary>
			unsafe class _JApi
			{
				IntPtr _hmodule;

				~_JApi()
				{
					IsLoaded = false;
					if(_hmodule != default) {
						Api.FreeLibrary(_hmodule);
						_hmodule = default;
					}
				}

				public bool IsLoaded { get; private set; }

				public void Load()
				{
					if(IsLoaded) return;
					if(!Ver.Is64BitOS) return; //don't support 32-bit OS. Then JObject etc is 32-bit. Too much work for the dying platform.
					var dllName = Ver.Is64BitProcess ? "WindowsAccessBridge-64.dll" : "WindowsAccessBridge-32.dll";
					var hm = Api.LoadLibrary(dllName);
					if(hm == default) {
						//Acc CONSIDER: warning
						return;
					}
					_hmodule = hm;
					try {
						_GetApi(nameof(Windows_run), out Windows_run);
						_GetApi(nameof(releaseJavaObject), out releaseJavaObject);
						_GetApi(nameof(isSameObject), out isSameObject);
						_GetApi(nameof(getAccessibleContextFromHWND), out getAccessibleContextFromHWND);
						_GetApi(nameof(getHWNDFromAccessibleContext), out getHWNDFromAccessibleContext);
						_GetApi(nameof(getAccessibleContextAt), out getAccessibleContextAt);
						_GetApi(nameof(getAccessibleContextWithFocus), out getAccessibleContextWithFocus);
						_GetApi(nameof(getAccessibleContextInfo), out getAccessibleContextInfo);
						_GetApi(nameof(getAccessibleChildFromContext), out getAccessibleChildFromContext);
						_GetApi(nameof(getAccessibleParentFromContext), out getAccessibleParentFromContext);
						_GetApi(nameof(getAccessibleActions), out getAccessibleActions);
						_GetApi(nameof(doAccessibleActions), out doAccessibleActions);
						_GetApi(nameof(getAccessibleTextInfo), out getAccessibleTextInfo);
						_GetApi(nameof(getAccessibleTextRange), out getAccessibleTextRange);
						_GetApi(nameof(getCurrentAccessibleValueFromContext), out getCurrentAccessibleValueFromContext);
						_GetApi(nameof(addAccessibleSelectionFromContext), out addAccessibleSelectionFromContext);
						_GetApi(nameof(clearAccessibleSelectionFromContext), out clearAccessibleSelectionFromContext);
						_GetApi(nameof(removeAccessibleSelectionFromContext), out removeAccessibleSelectionFromContext);
						_GetApi(nameof(setTextContents), out setTextContents);
						_GetApi(nameof(getTopLevelObject), out getTopLevelObject);
						_GetApi(nameof(getVirtualAccessibleName), out getVirtualAccessibleName);
						_GetApi(nameof(requestFocus), out requestFocus);
					}
					catch(ArgumentNullException) {
						Output.Warning("Invalid " + dllName);
						return;
					}
					IsLoaded = true;

					void _GetApi<T>(string name, out T del) where T : class
					{
						del = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(Api.GetProcAddress(hm, name), typeof(T)));
					}
				}

				#region delegates, structs

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate void Windows_runT();

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate void releaseJavaObjectT(int vmID, JObject jo);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool isSameObjectT(int vmID, JObject jo1, JObject jo2);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleContextFromHWNDT(Wnd w, out int vmID, out JObject jo);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate Wnd getHWNDFromAccessibleContextT(int vmID, JObject jo);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleContextAtT(int vmID, JObject acParent, int x, int y, out JObject jo);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleContextInfoT(int vmID, JObject jo, AccessibleContextInfo* info);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate JObject getAccessibleChildFromContextT(int vmID, JObject jo, int i);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate JObject getAccessibleParentFromContextT(int vmID, JObject jo);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleActionsT(int vmID, JObject jo, AccessibleActions* actions);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool doAccessibleActionsT(int vmID, JObject jo, AccessibleActionsToDo* actionsToDo, out int failure);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleTextInfoT(int vmID, JObject jo, out AccessibleTextInfo textInfo, int x, int y);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getAccessibleTextRangeT(int vmID, JObject jo, int start, int end, char* text, int len);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool getCurrentAccessibleValueFromContextT(int vmID, JObject jo, char* text, int len);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				internal delegate void addAccessibleSelectionFromContextT(int vmID, JObject jo, int i);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool setTextContentsT(int vmID, JObject jo, BSTR text);

				[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
				[return: MarshalAs(UnmanagedType.Bool)]
				internal delegate bool requestFocusT(int vmID, JObject jo);

				internal Windows_runT Windows_run;
				internal releaseJavaObjectT releaseJavaObject;
				internal isSameObjectT isSameObject;
				internal getAccessibleContextFromHWNDT getAccessibleContextFromHWND;
				internal getHWNDFromAccessibleContextT getHWNDFromAccessibleContext;
				internal getAccessibleContextAtT getAccessibleContextAt;
				internal getAccessibleContextFromHWNDT getAccessibleContextWithFocus;
				internal getAccessibleContextInfoT getAccessibleContextInfo;
				internal getAccessibleChildFromContextT getAccessibleChildFromContext;
				internal getAccessibleParentFromContextT getAccessibleParentFromContext;
				internal getAccessibleActionsT getAccessibleActions;
				internal doAccessibleActionsT doAccessibleActions;
				internal getAccessibleTextInfoT getAccessibleTextInfo;
				internal getAccessibleTextRangeT getAccessibleTextRange;
				internal getCurrentAccessibleValueFromContextT getCurrentAccessibleValueFromContext;
				internal addAccessibleSelectionFromContextT addAccessibleSelectionFromContext;
				internal releaseJavaObjectT clearAccessibleSelectionFromContext;
				internal addAccessibleSelectionFromContextT removeAccessibleSelectionFromContext;
				internal setTextContentsT setTextContents;
				internal getAccessibleParentFromContextT getTopLevelObject;
				internal getCurrentAccessibleValueFromContextT getVirtualAccessibleName;
				internal requestFocusT requestFocus;

				internal const int MAX_STRING_SIZE = 1024;
				internal const int SHORT_STRING_SIZE = 256;

#pragma warning disable CS0649
				internal struct AccessibleContextInfo
				{
					public fixed char name[MAX_STRING_SIZE];
					public fixed char description[MAX_STRING_SIZE];
					public fixed char role[SHORT_STRING_SIZE]; //localized
					public fixed char role_en_US[SHORT_STRING_SIZE];
					public fixed char states[SHORT_STRING_SIZE]; //comma separated, localized
					public fixed char states_en_US[SHORT_STRING_SIZE]; //comma separated
					public int indexInParent, childrenCount;
					public int x, y, width, height;
					public int accessibleComponent, accessibleAction, accessibleSelection, accessibleText; //BOOLs for various additional Java Accessibility interfaces that are implemented
					public uint accessibleInterfaces; //bitfield containing additional interface flags. Bit 0 is for value.
				}

				internal struct AccessibleTextInfo
				{
					public int charCount, caretIndex, indexAtPoint;
				}

				//internal struct AccessibleActionInfo //not useful because C# does not allow to get address
				//{
				//	public fixed char name[SHORT_STRING_SIZE]; //256
				//}

				internal const int MAX_ACTION_INFO = 256;
				internal struct AccessibleActions
				{
					public int actionsCount;
					public fixed char actionInfo[MAX_ACTION_INFO * SHORT_STRING_SIZE]; //AccessibleActionInfo actionInfo[MAX_ACTION_INFO], but C# does not allow to make it fixed
				}

				internal const int MAX_ACTIONS_TO_DO = 32;
				internal struct AccessibleActionsToDo
				{
					public int actionsCount;
					public fixed char actions[MAX_ACTIONS_TO_DO * SHORT_STRING_SIZE]; //AccessibleActionInfo actionInfo[MAX_ACTION_INFO], but C# does not allow to make it fixed
				}
#pragma warning restore CS0649

				#endregion
			}

			static _JApi _api = new _JApi();

			static bool _InitJab()
			{
				if(s_inited == 0) {
					lock(_api) {
						if(s_inited == 0) {
							s_inited = -1;
							_api.Load();
							if(_api.IsLoaded) {
								var t = new Thread(o => _JabThread(o)) { IsBackground = true };
								//t.SetApartmentState(ApartmentState.STA);
								using(var ev = new ManualResetEvent(false)) {
									Time.LibSleepPrecision.LibTempSet1(1);
									t.Start(ev);
									ev.WaitOne();
								}
							}
						}
					}

				}
				return s_inited > 0;
			}
			static int s_inited;

			static void _JabThread(object oEvent)
			{
				//How JAB works:
				//initializeAccessBridge loads JAB dll and functions, and calls Windows_run.
				//Windows_run creates a hidden dialog "Access Bridge status" and posts "AccessBridge-FromJava-Hello" messages to all top-level windows.
				//Java JAB-enabled processes also have hidden "Access Bridge status" dialogs. They run in a separate thread.
				//These dialogs, when received our hello message, post back the message to our dialog. wParam is poster dialog hwnd.
				//Only when our dialog receives these messages, we can get accessible context of that Java windows.
				//Therefore we must have a message loop etc. We use this thread for it.
				//Alternatively, each thread could call Windows_run and DoEvents etc, but then we would have multiple dialogs. Also would need DoEvents before each GetAccessibleContextFromHWND etc, or it would fail with Java windows created later.
				//Note: JAB events work only in thread that called Windows_run. Currently not using events. Not tested much. When tested with JavaFerret, most events either didn't work or used to stop working etc.

				uint m1 = Api.RegisterWindowMessage("AccessBridge-FromWindows-Hello"); //received by our hidden "Access Bridge status" dialog once after initializeAccessBridge
				uint m2 = Api.RegisterWindowMessage("AccessBridge-FromJava-Hello"); //received by the dialog from each JAB-enabled Java window
				Api.ChangeWindowMessageFilter(m1, 1); Api.ChangeWindowMessageFilter(m2, 1);

				_api.Windows_run(); //the slowest part, ~10 ms when warm CPU, else 10-33 ms

				int i, nmsg;
				for(i = 0; i < 10; i++) {
					for(nmsg = 0; Api.PeekMessage(out var msg, default, 0, 0, Api.PM_REMOVE); nmsg++) Api.DispatchMessage(ref msg);
					if(nmsg == 0 && i > 0) break; //non-Java windows that use JAB (probably as client) don't post back the message
					1.ms();
				}
				Debug.Assert(i == 1); //just to test reliability in stress conditions. Never was not 1.

				s_inited = 1;
				(oEvent as ManualResetEvent).Set();

				while(Api.GetMessage(out var msg, default, 0, 0) > 0) Api.DispatchMessage(ref msg);
				s_inited = 0;
			}

			static bool _NoJab { get => s_inited < 0; }

			/// <summary>
			/// Returns true if w process has a JAB hidden dialog ("Access Bridge status", "#32770").
			/// It can be a Java window or other window that uses JAB (eg JavaMonkey; but not a window of this process).
			/// </summary>
			/// <remarks>
			/// speed: faster than AccessibleObjectFromWindow/AccessibleObjectFromPoint.
			/// Call once for a top-level window, don't call for controls.
			/// </remarks>
			static bool _IsJavaWindow(Wnd w)
			{
				if(w.IsChildWindow) return false;

				var f = Wnd.Lib.WinFlags.Get(w);
				if(f.Has_(Wnd.Lib.WFlags.JavaYes)) return true;
				if(f.Has_(Wnd.Lib.WFlags.JavaNo)) return false;

				bool yes = false; int pid = 0;
				for(Wnd wJAB = default; !(wJAB = Wnd.FindFast("Access Bridge status", "#32770", wJAB)).Is0;) {
					if(pid == 0 && (pid = w.ProcessId) == 0) break;
					if(wJAB.ProcessId != pid) continue;
					yes = pid != Api.GetCurrentProcessId();
					break;
				}
				Wnd.Lib.WinFlags.Set(w, f | (yes ? Wnd.Lib.WFlags.JavaYes : Wnd.Lib.WFlags.JavaNo));
				return yes;
			}

			static JObject _JObjectFromWindow(Wnd w, out int vmID, bool getFocused = false)
			{
				vmID = 0;
				if(!_IsJavaWindow(w)) return 0;
				if(!_InitJab()) return 0;
				_JApi.getAccessibleContextFromHWNDT f = getFocused ? _api.getAccessibleContextWithFocus : _api.getAccessibleContextFromHWND;
				if(!f(w, out vmID, out var jo)) return 0;
				return jo;
			}

			public static Acc AccFromWindow(Wnd w, bool getFocused = false)
			{
				if(_NoJab) return null;
				var jo = _JObjectFromWindow(w, out var vmID, getFocused); if(jo == 0) return null;
				return _JObjectToAcc(jo, vmID);
			}

			/// <summary>
			/// Gets Java accessible object from point.
			/// </summary>
			/// <param name="p">Point in screen coordinates.</param>
			/// <param name="w">Window containing the point. If default(Wnd), calls Api.WindowFromPoint.</param>
			public static Acc AccFromPoint(Point p, Wnd w = default)
			{
				if(_NoJab) return null;
				if(w.Is0) w = Api.WindowFromPoint(p);
				var ap = _JObjectFromWindow(w, out var vmID); if(ap == 0) return null;
				bool ok = _api.getAccessibleContextAt(vmID, ap, p.X, p.Y, out var jo);
				if(ok && jo == 0) { //JAB bug: the API returns 0 object if the window never received a mouse message
									//Debug_.Print("workaround");
					w.Post(Api.WM_MOUSEMOVE);
					for(int i = 0; i < 10; i++) {
						w.LibMinimalSleepNoCheckThread();
						ok = _api.getAccessibleContextAt(vmID, ap, p.X, p.Y, out jo) && jo != 0;
						if(ok) break;
					}
				}
				_api.releaseJavaObject(vmID, ap);
				if(!ok) return null;
				return _JObjectToAcc(jo, vmID);
			}

			static IAccessible _JObjectToIAccessible(JObject jo, int vmID)
			{
				var ja = new JAccessible(jo, vmID);
				var a = Marshal.GetComInterfaceForObject(ja, typeof(IJAccessible));
				return new IAccessible(a);
			}

			static Acc _JObjectToAcc(JObject jo, int vmID)
			{
				return new Acc(_JObjectToIAccessible(jo, vmID));
			}

			[ComImport, Guid("618736e0-3c3d-11cf-810c-00aa00389b71")]
			interface IJAccessible
			{
				//note, the interface and all parameters must exactly match the Acc.IAccessible._Vtbl delegates.

				[PreserveSig] int get_accParent(out IAccessible ppdispParent);
				[PreserveSig] int get_accChildCount(out int pcountChildren);
				[PreserveSig] int get_accChild(VARIANT varChild, out IAccessible ppdispChild);
				[PreserveSig] int get_accName(VARIANT varChild, out BSTR pszName);
				[PreserveSig] int get_accValue(VARIANT varChild, out BSTR pszValue);
				[PreserveSig] int get_accDescription(VARIANT varChild, out BSTR pszDescription);
				[PreserveSig] int get_accRole(VARIANT varChild, out VARIANT pvarRole);
				[PreserveSig] int get_accState(VARIANT varChild, out VARIANT pvarState);
				[PreserveSig] int get_accHelp(VARIANT varChild, out BSTR pszHelp);
				[PreserveSig] int get_accHelpTopic(IntPtr pszHelpFile, VARIANT varChild, IntPtr pidTopic);
				[PreserveSig] int get_accKeyboardShortcut(VARIANT varChild, out BSTR pszKeyboardShortcut);
				[PreserveSig] int get_accFocus(out VARIANT pvarChild);
				[PreserveSig] int get_accSelection(out VARIANT pvarChildren);
				[PreserveSig] int get_accDefaultAction(VARIANT varChild, out BSTR pszDefaultAction);
				[PreserveSig] int accSelect(AccSELFLAG flagsSelect, VARIANT varChild);
				[PreserveSig] int accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VARIANT varChild);
				[PreserveSig] int accNavigate(AccNAVDIR navDir, VARIANT varStart, out VARIANT pvarEndUpAt);
				[PreserveSig] int accHitTest(int xLeft, int yTop, out VARIANT pvarChild);
				[PreserveSig] int accDoDefaultAction(VARIANT varChild);
				[PreserveSig] int put_accName(VARIANT varChild, BSTR szName);
				[PreserveSig] int put_accValue(VARIANT varChild, BSTR szValue);
			}

			[ClassInterface(ClassInterfaceType.None)]
			class JAccessible :IJAccessible
			{
				JObject _jo;
				int _vmID;

				public JAccessible(JObject jo, int vmID)
				{
					//Print("JAccessible");
					_jo = jo; _vmID = vmID;
				}

				~JAccessible()
				{
					//Print("~JAccessible");
					_api.releaseJavaObject(_vmID, _jo);
					//SHOULDDO: dispose when refcount 0, not in finalizer.
					//	But there is no legal/safe way to know when refcount 0.
					//	idea: hook Release: replace VTBL[2] with our hook function pointer.
				}

				int IJAccessible.get_accParent(out IAccessible ppdispParent)
				{
					//Debug_.PrintFunc();
					ppdispParent = default;
					JObject jo;

					//Workaround JAB bug: if eg dialog has owner window, GetAccessibleParentFromContext(dialog) gets a half-valid object of the owner window.
					//Instead we'll get standard WINDOW object. Need it for WindowFromAccessibleObject.
					//Makes slower 2-3 times, but still not too slow.
					//tested: getObjectDepth cannot be used. It then returns 1, not 0. Same speed.
					jo = _api.getTopLevelObject(_vmID, _jo);
					if(jo != 0) //gets correct object, not of owner window
					{
						if(_api.isSameObject(_vmID, _jo, jo)) {
							_api.releaseJavaObject(_vmID, jo);
							Wnd w = _api.getHWNDFromAccessibleContext(_vmID, _jo);
							if(!w.Is0 && 0 == Api.AccessibleObjectFromWindow(w, AccOBJID.WINDOW, ref Api.IID_IAccessible, out ppdispParent)) return 0;
							return 1;
						}
						_api.releaseJavaObject(_vmID, jo);
					}

					if((jo = _api.getAccessibleParentFromContext(_vmID, _jo)) == 0) return 1;
					ppdispParent = _JObjectToIAccessible(jo, _vmID);
					return 0;
				}

				int IJAccessible.get_accChildCount(out int pcountChildren)
				{
					//Debug_.PrintFunc();
					pcountChildren = 0;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pcountChildren = k.childrenCount;
					return 0;
				}

				int IJAccessible.get_accChild(VARIANT varChild, out IAccessible ppdispChild)
				{
					//Debug_.PrintFunc();
					ppdispChild = default;
					int i = varChild.value - 1;
					if(varChild.vt != Api.VARENUM.VT_I4 || i < 0) return Api.E_INVALIDARG;
					var jo = _api.getAccessibleChildFromContext(_vmID, _jo, i);
					if(jo == 0) {
						if(_GetObjectInfo(out var k) && i >= k.childrenCount) return Api.E_INVALIDARG;
						return Api.E_FAIL;
					}
					ppdispChild = _JObjectToIAccessible(jo, _vmID);
					return 0;
				}

#if true
				int IJAccessible.get_accName(VARIANT varChild, out BSTR pszName)
				{
					//Debug_.PrintFunc();
					pszName = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					var m = stackalloc char[_JApi.MAX_STRING_SIZE];
					if(!_api.getVirtualAccessibleName(_vmID, _jo, m, _JApi.MAX_STRING_SIZE)) return Api.E_FAIL;
					pszName = BSTR.CopyFrom(m);
					return 0;

					//getVirtualAccessibleName is better than getAccessibleContextInfo (GACI).
					//It gets names of toolbar buttons. With GACI, name empty, we can use description.
					//It gets name of related control - adjacent label etc.
					//Much faster than GACI.
					//Makes searching ~10% slower when GACI is cached, but not when role used (not called if role does not match).
				}
#else
				int IJAccessible.get_accName(VARIANT varChild, out BSTR pszName)
				{
					//Debug_.PrintFunc();
					pszName = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pszName = (BSTR)k.name;
					return 0;
				}
#endif

				int IJAccessible.get_accValue(VARIANT varChild, out BSTR pszValue)
				{
					//Debug_.PrintFunc();
					pszValue = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					if(k.accessibleValue) {
						var m = stackalloc char[_JApi.MAX_STRING_SIZE];
						if(_api.getCurrentAccessibleValueFromContext(_vmID, _jo, m, 1024)) //speed: 20 times faster than getAccessibleContextInfo
							pszValue = BSTR.CopyFrom(m);
					} else if(k.accessibleText && _api.getAccessibleTextInfo(_vmID, _jo, out var ati, 0, 0)) { //slow
						pszValue = BSTR.Alloc(ati.charCount);
						if(ati.charCount != 0 && !_api.getAccessibleTextRange(_vmID, _jo, 0, ati.charCount - 1, pszValue.Ptr, ati.charCount))
							pszValue.Dispose();
					}
					return pszValue.Is0 ? 1 : 0;

					//tested: most objects don't have value/text. Value usually is numeric, eg "1" for checked checkbox. Text objects don't have value, therefore we get text instead.
					//AccessibleContextInfo::accessibleInterfaces flags, from AccessibleBridgePackages.h (cAccessibleValueInterface etc):
					//	1 value, 2 actions, 4 component, 8 selection, 0x10 table, 0x20 text, 0x40 hypertext.
				}

				int IJAccessible.get_accDescription(VARIANT varChild, out BSTR pszDescription)
				{
					//Debug_.PrintFunc();
					pszDescription = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pszDescription = (BSTR)k.description;
					return 0;
				}

				int IJAccessible.get_accRole(VARIANT varChild, out VARIANT pvarRole)
				{
					//Debug_.PrintFunc();
					pvarRole = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pvarRole = k.role;
					return 0;
				}

				int IJAccessible.get_accState(VARIANT varChild, out VARIANT pvarState)
				{
					//Debug_.PrintFunc();
					pvarState = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pvarState = (int)k.state;
					return 0;
				}

				int IJAccessible.get_accHelp(VARIANT varChild, out BSTR pszHelp)
				{
					pszHelp = (BSTR)"IJAccessible"; //this is how tools can recognize our Java objects. Don't change.
					return 0;
				}

				int IJAccessible.get_accHelpTopic(IntPtr pszHelpFile, VARIANT varChild, IntPtr pidTopic)
				{
					return Api.E_NOTIMPL;
				}

				int IJAccessible.get_accKeyboardShortcut(VARIANT varChild, out BSTR pszKeyboardShortcut)
				{
					pszKeyboardShortcut = default;
					return Api.E_NOTIMPL;
				}

				int IJAccessible.get_accFocus(out VARIANT pvarChild)
				{
					pvarChild = default;
					return Api.E_NOTIMPL;
					//Difficult to implement so that would work correctly, ie return direct child even if focused is another descendant.
					//getAccessibleContextWithFocus is unstable. More often does not work than works.
					//Rarely used.
				}

				int IJAccessible.get_accSelection(out VARIANT pvarChildren)
				{
					pvarChildren = default;
					return Api.E_NOTIMPL;
					//Rarely used.
				}

				int IJAccessible.get_accDefaultAction(VARIANT varChild, out BSTR pszDefaultAction)
				{
					//Debug_.PrintFunc();
					pszDefaultAction = default;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					return _get_accDefaultAction(out pszDefaultAction, varChild.value == -1000_000);
				}

				int _get_accDefaultAction(out BSTR b, bool getList)
				{
					b = default;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					if(!k.accessibleAction) return 1;
					var actions = (_JApi.AccessibleActions*)Util.NativeHeap.Alloc(sizeof(_JApi.AccessibleActions)); //131076 bytes
					try {
						int hr = 1, n;
						if(_api.getAccessibleActions(_vmID, _jo, actions) && (n = actions->actionsCount) > 0 && n <= _JApi.MAX_ACTION_INFO) {
							hr = 0;
							char* a0 = actions->actionInfo;
							if(!getList) b = BSTR.CopyFrom(a0);
							else {
								const int siz = _JApi.SHORT_STRING_SIZE;
								int len = 0;
								var a = a0;
								for(int i = 0; i < n; i++, a += siz) len += Util.LibCharPtr.Length(a, siz) + 2; //2 for ", "
								b = BSTR.Alloc(len - 2);
								var p = b.Ptr;
								a = a0;
								for(int i = 0; i < n; i++, a += siz) {
									len = Util.LibCharPtr.Length(a, siz);
									Api.memcpy(p, a, len * 2);
									if(i < n - 1) { p += len; *p = ','; p++; *p = ' '; p++; }
								}
							}
						}
						return hr;
					}
					finally { Util.NativeHeap.Free(actions); }
				}

				int IJAccessible.accSelect(AccSELFLAG flagsSelect, VARIANT varChild)
				{
					//Debug_.PrintFunc();
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(flagsSelect.HasAny_(AccSELFLAG.TAKESELECTION | AccSELFLAG.ADDSELECTION | AccSELFLAG.REMOVESELECTION)) {
						if(!_GetObjectInfo(out var k)) return Api.E_FAIL; int i = k.indexInParent;
						var ap = _api.getAccessibleParentFromContext(_vmID, _jo); if(ap == 0) return Api.E_FAIL;
						try {
							_ReleaseObjectInfoCache(); //the action changes object state
							if(flagsSelect.Has_(AccSELFLAG.TAKESELECTION)) _api.clearAccessibleSelectionFromContext(_vmID, ap);
							if(flagsSelect.HasAny_(AccSELFLAG.TAKESELECTION | AccSELFLAG.ADDSELECTION)) _api.addAccessibleSelectionFromContext(_vmID, ap, i);
							if(flagsSelect.Has_(AccSELFLAG.REMOVESELECTION)) _api.removeAccessibleSelectionFromContext(_vmID, ap, i);
						}
						finally { _api.releaseJavaObject(_vmID, ap); }
					}
					if(flagsSelect.Has_(AccSELFLAG.TAKEFOCUS)) {
						_ReleaseObjectInfoCache(); //the action changes object state
						if(!_api.requestFocus(_vmID, _jo)) return Api.E_FAIL;
					}
					return 0;
				}

				int IJAccessible.accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, VARIANT varChild)
				{
					//Debug_.PrintFunc();
					pxLeft = pyTop = pcxWidth = pcyHeight = 0;
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_GetObjectInfo(out var k)) return Api.E_FAIL;
					pxLeft = k.x; pyTop = k.y; pcxWidth = k.width; pcyHeight = k.height;
					return 0;
				}

				int IJAccessible.accNavigate(AccNAVDIR navDir, VARIANT varStart, out VARIANT pvarEndUpAt)
				{
					//PrintList("accNavigate", navDir, varStart.vt, varStart.value);
					pvarEndUpAt = default;

					//WindowFromAccessibleObject (WFAO) at first calls this with an undocumented navDir 10.
					//	tested: accNavigate(10) for a standard Windows control returns VARIANT(VT_I4, hwnd).
					//	If we return window handle, WFAO does not call get_accParent. Else also calls it for each ancestor.
					if(navDir == (AccNAVDIR)10 && varStart.vt == Api.VARENUM.VT_I4) {
						Wnd w = _GetHWND();
						if(!w.Is0) {
							pvarEndUpAt = (int)(LPARAM)w;
							return 0;
						}
					}

					if(navDir < AccNAVDIR.UP || navDir > AccNAVDIR.LASTCHILD) return Api.E_INVALIDARG;
					if(_InvalidVarChildParam(ref varStart)) return Api.E_INVALIDARG;
					JObject ac = 0;

					if(navDir >= AccNAVDIR.NEXT && navDir <= AccNAVDIR.LASTCHILD) {
						int iChild = -1; JObject ap = 0; _JObjectInfo k = null;
						if(navDir != AccNAVDIR.FIRSTCHILD && !_GetObjectInfo(out k)) return 1;
						switch(navDir) {
						case AccNAVDIR.FIRSTCHILD: iChild = 0; ap = _jo; break; //GetAccessibleChildFromContext will fail if this does not have children
						case AccNAVDIR.LASTCHILD: iChild = k.childrenCount - 1; ap = _jo; break;
						case AccNAVDIR.NEXT: iChild = k.indexInParent + 1; break; //GetAccessibleChildFromContext will fail if this is the last
						case AccNAVDIR.PREVIOUS: iChild = k.indexInParent - 1; break;
						}
						if(iChild >= 0) {
							if(ap == 0) ap = _api.getAccessibleParentFromContext(_vmID, _jo);
							if(ap != 0) { ac = _api.getAccessibleChildFromContext(_vmID, ap, iChild); if(ap != _jo) _api.releaseJavaObject(_vmID, ap); }
						}
					}

					if(ac == 0) return 1;
					pvarEndUpAt.value = (IntPtr)_JObjectToIAccessible(ac, _vmID);
					pvarEndUpAt.vt = Api.VARENUM.VT_DISPATCH;
					return 0;
				}

				int IJAccessible.accHitTest(int xLeft, int yTop, out VARIANT pvarChild)
				{
					pvarChild = default;
					return Api.E_NOTIMPL;
					//Rarely used.
				}

				int IJAccessible.accDoDefaultAction(VARIANT varChild)
				{
					BSTR name = varChild.vt == Api.VARENUM.VT_BSTR ? varChild.ValueBstr : default;
					int len = name.Length; if(len >= _JApi.SHORT_STRING_SIZE) len = 0;
					if(len == 0) {
						if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
						int hr = _get_accDefaultAction(out name, false);
						if(hr != 0) return hr == 1 ? Api.DISP_E_MEMBERNOTFOUND : Api.E_FAIL;
						len = name.Length;
					}
					var atd = (_JApi.AccessibleActionsToDo*)Util.NativeHeap.Alloc(sizeof(_JApi.AccessibleActionsToDo));
					try {
						_ReleaseObjectInfoCache(); //the action may change object props
						atd->actionsCount = 1;
						Api.memcpy(atd->actions, name.Ptr, (len + 1) * 2);
						if(!_api.doAccessibleActions(_vmID, _jo, atd, out var failure)) {
							//Debug_.Print(failure); //0-based index of the first action that failed or is unknown, or -1 if not action-related
							return Api.E_FAIL;
						}
					}
					finally { Util.NativeHeap.Free(atd); }
					return 0;
				}

				int IJAccessible.put_accName(VARIANT varChild, BSTR szName)
				{
					return Api.E_NOTIMPL;
					//Rarely used, deprecated.
				}

				int IJAccessible.put_accValue(VARIANT varChild, BSTR szValue)
				{
					if(_InvalidVarChildParam(ref varChild)) return Api.E_INVALIDARG;
					if(!_api.setTextContents(_vmID, _jo, szValue)) return 1;
					_ReleaseObjectInfoCache();
					return 0;
				}

				#region get object info

				//_api.getAccessibleContextInfo gets multiple properties (name, state, etc) that are then used by multiple JAccessible functions.
				//It is slow etc. We call it once and store the retrieved/converted properties in a thread-static variable t_joInfo. Not in JAccessible because big.
				//t_joInfo is used for all JAccessible instances, but owned by a single JAccessible instance at a time.

				[ThreadStatic] static WeakReference<_JObjectInfo> t_joInfo;

				_JObjectInfo _GetObjectInfoCache()
				{
					var wr = t_joInfo ?? (t_joInfo = new WeakReference<_JObjectInfo>(null));
					if(!wr.TryGetTarget(out var r)) wr.SetTarget(r = new _JObjectInfo());
					return r;
				}

				bool _IsObjectInfoCached()
				{
					var k = _GetObjectInfoCache();
					return k.owner == this && Time.Milliseconds - k.time <= 10;
				}

				void _ReleaseObjectInfoCache()
				{
					var k = _GetObjectInfoCache();
					if(k.owner == this) k.owner = null;
				}

				bool _GetObjectInfo(out _JObjectInfo k)
				{
					k = _GetObjectInfoCache();
					if(k.owner != this || Time.Milliseconds - k.time > 10) {
						var c = new _JApi.AccessibleContextInfo(); //struct, 6188 bytes
						if(!_api.getAccessibleContextInfo(_vmID, _jo, &c)) return false;
						k.Init(&c);
						k.time = Time.Milliseconds;
						k.owner = this;
					}
					return true;
				}

				internal class _JObjectInfo
				{
					public void Init(_JApi.AccessibleContextInfo* c)
					{
						name = Util.StringCache.LibAdd(c->name, _JApi.MAX_STRING_SIZE, true);
						description = Util.StringCache.LibAdd(c->description, _JApi.MAX_STRING_SIZE, true);
						role = Util.StringCache.LibAdd(c->role_en_US, _JApi.SHORT_STRING_SIZE, true);
						indexInParent = c->indexInParent; childrenCount = c->childrenCount;
						x = c->x; y = c->y; width = c->width; height = c->height;
						accessibleComponent = c->accessibleComponent != 0; accessibleAction = c->accessibleAction != 0; accessibleSelection = c->accessibleSelection != 0; accessibleText = c->accessibleText != 0;
						accessibleValue = (c->accessibleInterfaces & 1) != 0;

						state = AccSTATE.READONLY | AccSTATE.DISABLED | AccSTATE.INVISIBLE;
						for(char* s = c->states_en_US; *s != 0;) {
							var se = s; while(*se != 0 && *se != ',') se++;
							switch(Util.StringCache.LibAdd(s, (int)(se - s))) {
							case "busy": state |= AccSTATE.BUSY; break;
							case "checked": state |= AccSTATE.CHECKED; break;
							case "collapsed": state |= AccSTATE.COLLAPSED; break;
							case "editable": state &= ~AccSTATE.READONLY; break;
							case "enabled": state &= ~AccSTATE.DISABLED; break;
							case "expanded": state |= AccSTATE.EXPANDED; break;
							case "focusable": state |= AccSTATE.FOCUSABLE; break;
							case "focused": state |= AccSTATE.FOCUSED; break;
							case "indeterminate": state |= AccSTATE.INDETERMINATE; break;
							case "modal": state |= AccSTATE.HASPOPUP; break;
							case "multiselectable": state |= AccSTATE.MULTISELECTABLE; break;
							case "pressed": state |= AccSTATE.PRESSED; break;
							case "resizable": state |= AccSTATE.SIZEABLE; break;
							case "selectable": state |= AccSTATE.SELECTABLE; break;
							case "selected": state |= AccSTATE.SELECTED; break;
							case "showing": state &= ~AccSTATE.INVISIBLE; break;
							case "visible": if(width > 0 && height > 0) state &= ~AccSTATE.INVISIBLE; break;
							}
							if(*se == 0) break; s = se + 1;
						}
					}

					public string name, description, role;
					public int indexInParent, childrenCount;
					public int x, y, width, height;
					public bool accessibleComponent, accessibleAction, accessibleSelection, accessibleText, accessibleValue;
					public AccSTATE state;
					public long time;
					public JAccessible owner;
				}

				#endregion

				bool _InvalidVarChildParam(ref VARIANT v)
				{
					if(v.vt == 0) return false; //forgive
					return v.vt != Api.VARENUM.VT_I4 || (v.value != 0 && v.value != -1000_000); //-1000_000 is used to get all actions
				}

				Wnd _GetHWND()
				{
					var joTL = _api.getTopLevelObject(_vmID, _jo);
					if(joTL != 0) {
						Wnd w = _api.getHWNDFromAccessibleContext(_vmID, joTL);
						_api.releaseJavaObject(_vmID, joTL);
						return w;
					}
					return default;
				}
			}
		}

#if false
		static void _TestJava()
		{
			var w = Wnd.Find("Java Control Panel").OrThrow();
			//var w = Wnd.Find("Network Settings").OrThrow();
			//var w = Wnd.Find("Temporary Files Settings").OrThrow();
			//var w = Wnd.Find("Catkeys -*").OrThrow();
			//w.Activate();
			//g1:
			using(var aw = _Java.AccFromWindow(w, false)) {
				//var s = aw.Name;
				//Print(s);
				Print(aw);
				Print("----");
				//using(var a2 = aw.Find("push button").OrThrow()) {
				//using(var a2 = aw.Find("push button", "Network*").OrThrow()) {
				//using(var a2 = aw.Find("page tab list").OrThrow()) {
				using(var a2 = aw.Find("page tab", "Java").OrThrow()) {
					//using(var a2 = aw.Find("text", "Proxy server address text field").OrThrow()) {
					//using(var a2 = aw.Find("text", "Proxy server address text field").OrThrow()) {
					Print(a2);
					//Print(a2.JavaActions);
					//for(var ap = a2.Navigate("pa"); ap != null; ap = ap.Navigate("pa", true)) {
					//	Print(ap);
					//}
					//Perf.First();
					//a2.DoDefaultAction();
					//a2.DoJavaAction("click");
					//a2.DoJavaAction();
					//Perf.NW();
					//Print(a2.WndContainer);
					//a2.Focus();
					//a2.Select(AccSELFLAG.TAKESELECTION);
					//a2.Value = "10000";
					//Print(a2.Navigate("pr", true));

					//500.ms();
					//var w3 = Wnd.Find("Network Settings").OrThrow();
					////Print(w3);
					//using(var aw3 = _Java.AccFromWindow(w3, false)) {
					//	using(var a3 = aw3.Find("push button", "Cancel").OrThrow()) {
					//		Print(a3);
					//		a3.DoJavaAction();
					//	}
					//}
				}
				Print("----");
			}
			//if(TaskDialog.ShowYesNo("Retry?")) goto g1;

			//2.s();
			//Perf.First();
			//for(int i = 0; i < 5; i++) {
			//	Acc._Java.JAccessibleFromWindow(w);
			//	//Acc._Java.JAccessibleFromPoint(Mouse.XY);
			//	Perf.Next();
			//}
			//Perf.Write();
		}
		//static void _TestJava()
		//{
		//	var w = Wnd.Find("Java Control Panel").OrThrow();
		//	//var w = Wnd.Find("Catkeys -*").OrThrow();
		//	var ja = _Java.JAccessibleFromWindow(w, false);
		//	//var a = ja as Api.IAccessible;

		//	var a = Marshal.GetIUnknownForObject(ja);
		//	Print(a);
		//	var a2 = new IAccessible(a);
		//	//PrintHex(a2.get_accParent(out var p));
		//	a2.put_accValue(0, "ddd");
		//	//var c = new Acc(a2);
		//	//c.Navigate("pa");
		//	//c.Dispose();
		//	a2.Dispose();
		//	//Print(Marshal.Release(a));

		//	//var h = GCHandle.Alloc(ja, GCHandleType.Pinned);
		//	//var a = new IAccessible(h.AddrOfPinnedObject());

		//	//PrintHex(a.get_accParent(out var p));

		//	//2.s();
		//	//Perf.First();
		//	//for(int i = 0; i < 5; i++) {
		//	//	Acc._Java.JAccessibleFromWindow(w);
		//	//	//Acc._Java.JAccessibleFromPoint(Mouse.XY);
		//	//	Perf.Next();
		//	//}
		//	//Perf.Write();
		//}

		static void _TestJavaIntegration()
		{
			var w = Wnd.Find("Java Control Panel").OrThrow();
			//var w = Wnd.Find("Network Settings").OrThrow();
			//var w = Wnd.Find("Temporary Files Settings").OrThrow();
			//var w = Wnd.Find("Catkeys -*").OrThrow();
			//var w = Wnd.Find("Quick*").OrThrow();
			w.Activate();
			//var r = w.ClientRectInScreen;
			////using(var aw = Acc.FromXY(w, 100, 10)) {
			//	using(var aw = Acc.FromXY(r.left + 100, r.top + 10)) {
			//	Print(aw);
			//}
			//using(var aw = _Java.AccFromWindow(w, false)) {
			////using(var aw = FromWindow(w)) {
			//	Print(aw);
			//	Print(aw.ChildFromXY(100, 40));
			//}

			//using(var a=Acc.Find(w, "java:push button", "netwo*")) {
			using(var a=Acc.Focused()) {
			//using(var a=Acc.Find(w, "java:root pane/layered pane/panel/page tab list/page tab/panel/filler/label", "Net*")) {
				Print(a);
			}
		}

		public static void TestJava()
		{
			_TestJavaIntegration();
			//_TestJava();
			//return;
			//GC.Collect();
			//GC.WaitForPendingFinalizers();
			Print("END");
		}
#endif
	}
}
