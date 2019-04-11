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
using System.Linq;
using System.Reflection.Emit;
using Microsoft.Win32.SafeHandles;
using System.Resources;
using System.Globalization;
using System.Security.Principal;

using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Miscellaneous classes and functions, used in this library as utility/helper.
	/// Some classes are public, and can be used not only in this library.
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	/// <summary>
	/// Gets native module handle, or path from handle.
	/// </summary>
	public static class ModuleHandle_
	{
		/// <summary>
		/// Gets native module handle of type's assembly.
		/// </summary>
		public static IntPtr OfType(Type t)
		{
			return t == null ? default : Marshal.GetHINSTANCE(t.Module);

			//Tested these to get caller's module without Type parameter:
			//This is dirty/dangerous and 50 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(new StackFrame(1).GetMethod().DeclaringType.Module);
			//This is dirty/dangerous, does not support multi-module assemblies and 12 times slower: [MethodImpl(MethodImplOptions.NoInlining)] ... return Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().GetLoadedModules()[0]);
			//This is dirty/dangerous/untested and 12 times slower: [MethodImpl(MethodImplOptions.AggressiveInlining)] ... return Marshal.GetHINSTANCE(MethodBase.GetCurrentMethod().DeclaringType.Module);
		}

		/// <summary>
		/// Gets native module handle of an assembly.
		/// Returns default(IntPtr) if <paramref name="asm"/> is null or if the assembly is in-memory (loaded from byte[]) or dynamic.
		/// </summary>
		public static IntPtr OfAssembly(Assembly asm)
		{
			if(asm == null || asm.IsDynamic || asm.Location.Length == 0) return default; //Location.Length == 0 if in-memory; it would throw if dynamic.
			var h = Marshal.GetHINSTANCE(asm.ManifestModule);
			if(h == (IntPtr)(-1)) h = default; //MSDN lies that it returns -1 for in-memory. It returns some invalid value.
			return h;
		}

		/// <summary>
		/// Gets native module handle of the entry assembly of this appdomain.
		/// Returns default(IntPtr) if the assembly is in-memory (loaded from byte[]) or dynamic.
		/// </summary>
		public static IntPtr OfAppDomainEntryAssembly()
		{
			return OfAssembly(Assembly_.EntryAssembly);
		}

		/// <summary>
		/// Gets native module handle of Au.dll.
		/// </summary>
		public static IntPtr OfAuDll()
		{
			return Marshal.GetHINSTANCE(typeof(ModuleHandle_).Module);
		}

		/// <summary>
		/// Gets native module handle of the program file of this process.
		/// </summary>
		public static IntPtr OfProcessExe()
		{
			return Api.GetModuleHandle(null);
		}

		//rejected. For script appdomains we use in-memory assemblies. They don't have a module handle (Marshal.GetHINSTANCE returns an invalid value). Use OfProcessExe.
		///// <summary>
		///// Gets native module handle of the assembly containing native icon that can be displayed as icon of this app.
		///// Some API functions need it when you use <msdn>IDI_APPLICATION</msdn>.
		///// If the entry assembly of this appdomain is dll with icon, gets dll handle; else gets exe handle.
		///// Returns default(IntPtr) if there are no native icons.
		///// </summary>
		//public static IntPtr OfAppIcon()
		//{
		//	if(s_hmodAppIcon == default) {
		//		IntPtr h = OfAppDomainEntryAssembly();
		//		if(h == default || default == Api.FindResource(h, Api.IDI_APPLICATION, 14)) { //RT_GROUP_ICON
		//			h = OfProcessExe();
		//			if(default == Api.FindResource(h, Api.IDI_APPLICATION, 14)) h = (IntPtr)1;
		//		}
		//		s_hmodAppIcon = h;
		//	}
		//	return s_hmodAppIcon == (IntPtr)1 ? default : s_hmodAppIcon;
		//}
		//static IntPtr s_hmodAppIcon;

		/// <summary>
		/// Gets full path of dll or exe file from its native handle.
		/// Returns null if fails. Supports <see cref="WinError.Code"/>.
		/// Calls API <msdn>GetModuleFileName</msdn>.
		/// </summary>
		public static string GetFilePath(IntPtr hModule)
		{
			for(int na = 300; ; na *= 2) {
				var b = Buffers.LibChar(ref na);
				int n = Api.GetModuleFileName(default, b, na);
				if(n < na) return n == 0 ? null : b.ToString(n);
			}
		}
	}

	/// <summary>
	/// Assembly functions.
	/// </summary>
	public static class Assembly_
	{
		/// <summary>
		/// Gets the entry assembly of this appdomain.
		/// Normally instead can be used <see cref="Assembly.GetEntryAssembly"/>, but it returns null if appdomain launched through <see cref="AppDomain.DoCallBack"/>.
		/// </summary>
		public static Assembly EntryAssembly {
			get {
				if(_appdomainAssembly == null) {
					var asm = Assembly.GetEntryAssembly(); //fails if this domain launched through DoCallBack
					if(asm == null) asm = AppDomain.CurrentDomain.GetAssemblies()[1]; //[0] is mscorlib, 1 should be our assembly
					_appdomainAssembly = asm;
				}
				return _appdomainAssembly;
			}
		}
		static Assembly _appdomainAssembly;

		//not used. Don't add Au to GAC, because then appdomains start very slowly, don't know why.
		///// <summary>
		///// Returns true if Au.dll is installed in the global assembly cache.
		///// </summary>
		//internal static bool LibIsAuInGAC => typeof(Assembly_).Assembly.GlobalAssemblyCache;

		/// <summary>
		/// Returns true if Au.dll is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process or appdomain.
		/// </summary>
		internal static bool LibIsAuNgened => (s_auNgened ?? (s_auNgened = IsNgened(typeof(Assembly_).Assembly))).GetValueOrDefault();
		static bool? s_auNgened;
		//tested: Module.GetPEKind always gets ILOnly.

		/// <summary>
		/// Returns true if assembly asm is compiled to native code using ngen.exe.
		/// It means - no JIT-compiling delay when its functions are called first time in process or appdomain.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		public static bool IsNgened(Assembly asm)
		{
			var s = asm.CodeBase;
			//if(asm.GlobalAssemblyCache) return s.Contains("/GAC_MSIL/"); //faster and maybe more reliable, but works only with GAC assemblies
			s = s.Substring(s.LastIndexOf('/') + 1);
			s = s.Insert(s.LastIndexOf('.') + 1, "ni.");
			return default != Api.GetModuleHandle(s);
		}

		//much slower first time when ngened. Also it is undocumented that GetModuleFileName returns 0 if non-ngened (LOAD_LIBRARY_AS_DATAFILE?).
		//[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		//public static unsafe bool IsAssemblyNgened2(Assembly asm)
		//{
		//	var module =asm.GetLoadedModules()[0];
		//	var h = Marshal.GetHINSTANCE(module); //slow first time, especially when ngened
		//	var b = stackalloc char[4];
		//	return 0 != Api.GetModuleFileName(h, b, 4);
		//}

		/// <summary>
		/// Returns flags for loaded assemblies: 1 System.Windows.Forms, 2 WindowsBase (WPF).
		/// </summary>
		internal static int LibIsLoadedFormsWpf()
		{
			if(s_isLoadedFormsWpf == 0) {
				lock("zjm5R47f7UOmgyHUVZaf1w") {
					if(s_isLoadedFormsWpf == 0) {
						var ad = AppDomain.CurrentDomain;
						var a = ad.GetAssemblies();
						foreach(var v in a) {
							_FlagFromName(v);
							if(s_isLoadedFormsWpf == 3) return 3;
						}
						ad.AssemblyLoad += (_, x) => _FlagFromName(x.LoadedAssembly);
						s_isLoadedFormsWpf |= 0x100;
					}
				}
			}

			return s_isLoadedFormsWpf & 3;

			void _FlagFromName(Assembly a)
			{
				string s = a.FullName; //fast, cached. GetName can be slow because not cached.
				if(0 == (s_isLoadedFormsWpf & 1) && s.StartsWith_("System.Windows.Forms,")) s_isLoadedFormsWpf |= 1;
				else if(0 == (s_isLoadedFormsWpf & 2) && s.StartsWith_("WindowsBase,")) s_isLoadedFormsWpf |= 2;
			}
		}
		static volatile int s_isLoadedFormsWpf;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void LibEnsureLoaded(bool systemCore, bool formsDrawing = false)
		{
			if(systemCore) _ = typeof(System.Linq.Enumerable).Assembly; //System.Core, System
			if(formsDrawing) _ = typeof(System.Windows.Forms.Control).Assembly; //System.Windows.Forms, System.Drawing
		}
	}

	/// <summary>
	/// JIT-compiles methods.
	/// </summary>
	public static class Jit
	{
		const BindingFlags c_bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		/// <summary>
		/// JIT-compiles method.
		/// Uses <see cref="RuntimeHelpers.PrepareMethod"/>.
		/// </summary>
		/// <param name="type">Type containing the method.</param>
		/// <param name="method">Method name.</param>
		/// <exception cref="ArgumentException">Method does not exist.</exception>
		/// <exception cref="AmbiguousMatchException">Multiple overloads exist.</exception>
		public static void Compile(Type type, string method)
		{
			var m = type.GetMethod(method, c_bindingFlags);
			if(m == null) throw new ArgumentException($"Method {type.Name}.{method} does not exist.");
			RuntimeHelpers.PrepareMethod(m.MethodHandle);
			//tested: maybe MethodHandle.GetFunctionPointer can be used to detect whether the method is jited and assembly ngened.
			//	Call GetFunctionPointer before and after PrepareMethod. If was not jited, the second call returns a different value.
			//	Undocumented, therefore unreliable.
		}

		//rejected. Don't JIT-compile overloads.
		///// <summary>
		///// JIT-compiles a method overload.
		///// Uses <see cref="RuntimeHelpers.PrepareMethod"/>.
		///// </summary>
		///// <param name="type">Type containing the method.</param>
		///// <param name="method">Method name.</param>
		///// <param name="paramTypes">Types of parameters of this overload.</param>
		///// <exception cref="ArgumentException">Method does not exist.</exception>
		///// <exception cref="AmbiguousMatchException">Multiple overloads exist that match <paramref name="paramTypes"/>.</exception>
		//public static void Compile(Type type, string method, params Type[] paramTypes)
		//{
		//	var m = type.GetMethod(method, c_bindingFlags, null, paramTypes, null);
		//	if(m == null) throw new ArgumentException($"Method {type.Name}.{method} does not exist.");
		//	RuntimeHelpers.PrepareMethod(m.MethodHandle);
		//	//tested: MethodHandle.GetFunctionPointer cannot be used to detect whether the method is jited.
		//	//	Tried to find a faster way to detect whether the assembly is ngened.
		//}

		/// <summary>
		/// JIT-compiles multiple methods of same type.
		/// Uses <see cref="RuntimeHelpers.PrepareMethod"/>.
		/// </summary>
		/// <param name="type">Type containing the methods.</param>
		/// <param name="methods">Method names.</param>
		/// <exception cref="ArgumentException">Method does not exist.</exception>
		public static void Compile(Type type, params string[] methods)
		{
			foreach(var v in methods) Compile(type, v);
		}
	}

	//currently not used
	///// <summary>
	///// Extends <see cref="Marshal"/>.
	///// </summary>
	//public static class Marshal_
	//{
	//	//currently not used
	//	///// <summary>
	//	///// Increments the reference count of COM object's RCW (managed runtime callable wrapper).
	//	///// </summary>
	//	///// <param name="o">Managed COM object (RCW).</param>
	//	///// <remarks>
	//	///// This function is the opposite of <see cref="Marshal.ReleaseComObject"/>, which decrements the RCW reference count.
	//	///// Call this function when cloning a variable of a type that wraps a managed COM object and calls Marshal.ReleaseComObject when disposing. Without it, after disposing one of the variables, cannot call methods etc of other variable because the RCW then is invalid.
	//	///// This function does not increment the reference count of the native COM object.
	//	///// </remarks>
	//	//public static void AddRefComObject<T>(T o) where T: class
	//	//{
	//	//	//ugly, but .NET does not have a better method for it.

	//	//	var u = Marshal.GetIUnknownForObject(o); //gets native COM pointer and calls its AddRef
	//	//	var o2 = Marshal.GetObjectForIUnknown(u); //increments RCW ref count. Calls QueryInterface and Release of the native COM object.
	//	//											  //var o2 = Marshal.GetTypedObjectForIUnknown(u, typeof(T)); //works too, but MSDN info is unclear. In both cases ToString says it's System.__ComObject.
	//	//	Marshal.Release(u); //because GetIUnknownForObject called AddRef
	//	//	Debug.Assert(ReferenceEquals(o, o2));
	//	//}

	//	//returns new RCW
	//	//internal static object GetUniqueComObject<T>(T o) where T: class
	//	//{
	//	//	var u = Marshal.GetIUnknownForObject(o);
	//	//	var o2 = Marshal.GetUniqueObjectForIUnknown(u); //many QI etc
	//	//	Marshal.Release(u);
	//	//	Debug.Assert(!ReferenceEquals(o, o2));
	//	//	return u2;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// Gets another COM interface through <msdn>IUnknown.QueryInterface</msdn>.
	//	///// Returns false if the COM object (iunkFrom) does not support the requested interface or if fails.
	//	///// </summary>
	//	///// <typeparam name="T">IntPtr or an IntPtr-based type. Must have size of IntPtr (exception if not).</typeparam>
	//	///// <param name="iunkFrom">COM object as IUnknown.</param>
	//	///// <param name="iTo">Receives the requested COM interface pointer.</param>
	//	///// <param name="riid">Interface GUID.</param>
	//	//internal static unsafe bool QueryInterface<T>(IntPtr iunkFrom, out T iTo, Guid riid) where T : unmanaged
	//	//{
	//	//	if(sizeof(T) != IntPtr.Size) throw new ArgumentException();
	//	//	iTo = default;
	//	//	if(0 != Marshal.QueryInterface(iunkFrom, ref riid, out IntPtr ip) || ip == default) return false;
	//	//	var v=*(T*)&ip; iTo=v;
	//	//	return true;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// Gets another COM interface through <msdn>IServiceProvider.QueryService</msdn>.
	//	///// Returns false if the COM object (iunkFrom) does not support IServiceProvider or the requested interface or if fails.
	//	///// </summary>
	//	///// <typeparam name="T">IntPtr or an IntPtr-based type. Must have size of IntPtr (exception if not).</typeparam>
	//	///// <param name="iunkFrom">COM object as IUnknown.</param>
	//	///// <param name="iTo">Receives the requested COM interface pointer.</param>
	//	///// <param name="guidService">Service GUID. If it is the same as riid, you can use other overload.</param>
	//	///// <param name="riid">Interface GUID.</param>
	//	//internal static unsafe bool QueryService<T>(IntPtr iunkFrom, out T iTo, in Guid guidService, in Guid riid) where T: unmanaged
	//	//{
	//	//	if(sizeof(T) != IntPtr.Size) throw new ArgumentException();
	//	//	if(iunkFrom==default) throw new ArgumentNullException();
	//	//	iTo = default;
	//	//	if(0 != Api.IUnknown_QueryService(iunkFrom, guidService, riid, out IntPtr ip) || ip==default) return false;
	//	//	var v=*(T*)&ip; iTo=v;
	//	//	return true;
	//	//}

	//	//currently not used
	//	///// <summary>
	//	///// This overload calls <see cref="QueryService{T}(IntPtr, out T, in Guid, in Guid)"/> with guidService = riid.
	//	///// </summary>
	//	//internal static unsafe bool QueryService<T>(IntPtr iunkFrom, out T iTo, in Guid riid) where T : struct
	//	//{
	//	//	return QueryService(iunkFrom, out iTo, riid, riid);
	//	//}
	//}

	/// <summary>
	/// Functions to work with managed resources.
	/// </summary>
	public static class Resources_
	{
		/// <summary>
		/// Gets an Image or other object from managed resources of appdomain's entry assembly.
		/// Returns null if not found.
		/// </summary>
		/// <param name="name">Resource name, like "example", not like "Project.Properties.Resources.example".</param>
		/// <remarks>
		/// Uses <see cref="ResourceManager.GetObject(string, CultureInfo)"/>.
		/// The Image is not cached. Will need to Dispose.
		/// </remarks>
		public static object GetAppResource(string name)
		{
			try {
				var rm = LibGetAppResourceManager(out var culture);
				return rm?.GetObject(name, culture);
			}
			catch { return null; }

			//info: why need culture? Because much much faster if culture is set to invariant.
		}

		/// <summary>
		/// Full name of embedded resources file, like "Project.Properties.Resources.resources".
		/// Set this property once before calling <see cref="GetAppResource"/>.
		/// If not set, it will look for resource where name ends with ".Resources.resources". If there is no such resources, uses the first.
		/// To see embedded resource file names you can use this code: <c>Print(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames()</c>.
		/// </summary>
		public static string AppResourcesName {
			get => s_appResourcesName;
			set { if(value != s_appResourcesName) { s_appResourcesName = value; _appResourceManager = null; } }
		}
		static string s_appResourcesName;

		/// <summary>
		/// Gets ResourceManager of appdomain's entry assembly.
		/// Returns null if fails or if the assembly does not have resources.
		/// Note: if the assembly contains multiple embedded .resource files, may need to set <see cref="AppResourcesName"/> before.
		/// </summary>
		internal static ResourceManager LibGetAppResourceManager(out CultureInfo culture)
		{
			if(_appResourceManager == null) {
				culture = null;
				var asm = Assembly_.EntryAssembly; if(asm == null) return null;
				var a = asm.GetManifestResourceNames(); if(a == null || a.Length == 0) return null; //no resources
				string s;
				if(s_appResourcesName != null) {
					s = a.FirstOrDefault(k => k == s_appResourcesName);
				} else {
					if(a.Length == 1 && a[0].EndsWith_(".resources")) s = a[0];
					else {
						s = a.FirstOrDefault(k => k.EndsWith_(".Resources.resources")); //eg "Project.Properties.Resources.resources". Skip those like "Form1.resources".
						if(s == null) s = a.FirstOrDefault(k => k.EndsWith_(".resources"));
					}
				}
				if(s == null) return null;
				s = s.RemoveEnd_(10); //remove ".resources"
				var t = asm.GetType(s);
				if(t != null) {
					var fl = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static; //need NonPublic because default access is internal
					if(t.GetProperty("ResourceManager", fl)?.GetValue(null) is ResourceManager rm) {
						_appResourceCulture = t.GetProperty("Culture")?.GetValue(null) as CultureInfo;
						_appResourceManager = rm;
					}
				}
				if(_appResourceManager == null) {
					//Debug_.Print("failed to get ResourceManager property"); //eg Au-compiled scripts don't have the type
					_appResourceManager = new ResourceManager(s, asm);
				}
			}
			culture = _appResourceCulture;
			return _appResourceManager;
		}
		static ResourceManager _appResourceManager;
		static CultureInfo _appResourceCulture;

		//rejected: now in generic methods we can use sizeof(T) if with 'unmanaged' constraint. Or Unsafe.SizeOf<T>().
		///// <summary>
		///// Do not call. Use class TypeSize, which caches the type size.
		///// This is used by TypeSize, not in it, because it is a generic type...
		///// </summary>
		///// <param name="t"></param>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		//internal static int LibGetTypeSize(Type t)
		//{
		//	var dm = new DynamicMethod("SizeOfType", typeof(int), Type.EmptyTypes);
		//	ILGenerator il = dm.GetILGenerator();
		//	il.Emit(OpCodes.Sizeof, t);
		//	il.Emit(OpCodes.Ret);
		//	return (int)dm.Invoke(null, null);
		//	//Print(dm.MethodImplementationFlags);
		//}
	}

	///// <summary>
	///// Gets managed run-time size of type T. Works with any type.
	///// Unlike sizeof, can be used in generic classes too.
	///// Unlike Marshal.SizeOf, gets managed type size (eg 1 for bool), not native type size (eg 4 for bool).
	///// Example: <c>Print(Au.Util.TypeSize&lt;T&gt;.Size);</c>.
	///// </summary>
	///// <typeparam name="T"></typeparam>
	//public static class TypeSize<T>
	//{
	//	/// <summary>
	//	/// Gets T type size.
	//	/// </summary>
	//	public readonly static int Size;
	//	static TypeSize() { Size = Misc.LibGetTypeSize(typeof(T)); }

	//	//speed: quite fast, especially when ngened. When using this generic class, LibGetTypeSize is called once for each type.
	//}

	/// <summary>
	/// Functions for high-DPI screen support.
	/// </summary>
	/// <remarks>
	/// High DPI is when in Windows Settings is set display text size other than 100%.
	/// Currently this class and this library don't support multiple screens that have different DPI. The Windows OS supports it since version 8.1.
	/// </remarks>
	public static class Dpi
	{
		/// <summary>
		/// Gets DPI of the primary screen.
		/// </summary>
		/// <remarks>
		/// On newer Windows versions, users can change DPI without logoff-logon. This function gets the setting that was after logon.
		/// </remarks>
		public static int BaseDPI {
			get {
				if(_baseDPI == 0) {
					using(var dcs = new LibScreenDC(0)) _baseDPI = Api.GetDeviceCaps(dcs, 90); //LOGPIXELSY
				}
				return _baseDPI;
			}
		}
		static int _baseDPI;

		/// <summary>
		/// Gets small icon size that depends on DPI of the primary screen.
		/// Width and Height are <see cref="BaseDPI"/>/6, which is 16 if DPI is 96 (100%).
		/// </summary>
		public static SIZE SmallIconSize { get { var t = BaseDPI / 6; return new SIZE(t, t); } }

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns stretched i.
		/// Else returns i.
		/// </summary>
		/// <param name="i"></param>
		public static int ScaleInt(int i)
		{
			long dpi = BaseDPI;
			if(dpi > 96) i = (int)(i * dpi / 96);
			return i;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96, returns scaled (stretched) z.
		/// Else returns z.
		/// Note: for images use <see cref="ImageSize"/>.
		/// </summary>
		/// <param name="z"></param>
		public static SIZE ScaleSize(SIZE z)
		{
			int dpi = BaseDPI;
			if(dpi > 96) {
				z.width = (int)((long)z.width * dpi / 96);
				z.height = (int)((long)z.height * dpi / 96);
			}
			return z;
		}

		/// <summary>
		/// If <see cref="BaseDPI"/> is more than 96 and image resolution is different, returns scaled (stretched) image.Size.
		/// Else returns image.Size.
		/// </summary>
		/// <param name="image"></param>
		public static SIZE ImageSize(System.Drawing.Image image)
		{
			if(image == null) return default;
			SIZE r = image.Size;
			int dpi = BaseDPI;
			if(dpi > 96) {
				r.width = (int)((long)r.width * dpi / (int)Math.Round(image.HorizontalResolution));
				r.height = (int)((long)r.height * dpi / (int)Math.Round(image.VerticalResolution));
			}
			return r;
		}

		//TEST: Win10 API GetDpiForWindow, GetSystemDpiForProcess, GetSystemMetricsForDpi.
		//	Win8.1 LogicalToPhysicalPointForPerMonitorDPI, PhysicalToLogicalPointForPerMonitorDPI.
	}

	/// <summary>
	/// Wraps a waitable timer handle. Allows to create, open, set and wait.
	/// More info: API <msdn>CreateWaitableTimer</msdn>.
	/// Note: will need to dispose.
	/// </summary>
	public class WaitableTimer : WaitHandle
	{
		WaitableTimer() { }

		/// <summary>
		/// Calls API <msdn>CreateWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="manualReset"></param>
		/// <param name="timerName">Timer name. If a timer with this name already exists, opens it if possible. If null, creates unnamed timer.</param>
		/// <exception cref="AuException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		public static WaitableTimer Create(bool manualReset = false, string timerName = null)
		{
			var h = Api.CreateWaitableTimer(Api.SECURITY_ATTRIBUTES.ForLowIL, manualReset, timerName);
			if(h.IsInvalid) {
				var ex = new AuException(0, "*create timer");
				h.SetHandleAsInvalid();
				throw ex;
			}
			return new WaitableTimer() { SafeWaitHandle = h };
		}

		/// <summary>
		/// Calls API <msdn>OpenWaitableTimer</msdn> and creates a WaitableTimer object that wraps the timer handle.
		/// </summary>
		/// <param name="timerName">Timer name. Fails if it does not exist; to open-or-create use <see cref="Create"/>.</param>
		/// <param name="access">.See <msdn>Synchronization Object Security and Access Rights</msdn>. The default value TIMER_MODIFY_STATE|SYNCHRONIZE allows to set and wait.</param>
		/// <exception cref="AuException">Failed. For example, a non-timer kernel object with this name already exists.</exception>
		/// <param name="inheritHandle"></param>
		/// <param name="noException">If fails, return null, don't throw exception. Supports <see cref="WinError.Code"/>.</param>
		/// <exception cref="AuException">Failed. For example, the timer does not exist.</exception>
		public static WaitableTimer Open(string timerName, uint access = Api.TIMER_MODIFY_STATE | Api.SYNCHRONIZE, bool inheritHandle = false, bool noException = false)
		{
			var h = Api.OpenWaitableTimer(access, inheritHandle, timerName);
			if(h.IsInvalid) {
				var e = WinError.Code;
				h.SetHandleAsInvalid();
				if(noException) {
					WinError.Code = e;
					return null;
				}
				throw new AuException(e, "*open timer");
			}
			return new WaitableTimer() { SafeWaitHandle = h };
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="WinError.Code"/>.
		/// </summary>
		/// <param name="dueTime">
		/// The time after which the state of the timer is to be set to signaled. It is relative time (from now).
		/// If positive, in milliseconds. If negative, in 100 nanosecond intervals (microseconds*10), see <msdn>FILETIME</msdn>.
		/// Also can be 0, to set minimal time.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		/// <exception cref="OverflowException">dueTime*10000 is greater than long.MaxValue.</exception>
		public bool Set(long dueTime, int period = 0)
		{
			if(dueTime > 0) dueTime = -checked(dueTime * 10000);
			return Api.SetWaitableTimer(this.SafeWaitHandle, ref dueTime, period, default, default, false);
		}

		/// <summary>
		/// Calls API <msdn>SetWaitableTimer</msdn>.
		/// Returns false if fails. Supports <see cref="WinError.Code"/>.
		/// </summary>
		/// <param name="dueTime">The UTC date/time at which the state of the timer is to be set to signaled.</param>
		/// <param name="period">The period of the timer, in milliseconds. If 0, the timer is signaled once. If greater than 0, the timer is periodic.</param>
		public bool SetAbsolute(DateTime dueTime, int period = 0)
		{
			var t = dueTime.ToFileTimeUtc();
			return Api.SetWaitableTimer(this.SafeWaitHandle, ref t, period, default, default, false);
		}
	}

	/// <summary>
	/// Static functions to open a help topic etc.
	/// </summary>
	public static class Help
	{
		/// <summary>
		/// Opens file "Au Help.chm" and a help topic in it.
		/// The file must be in <see cref="Folders.ThisApp"/>.
		/// </summary>
		/// <param name="topic">Topic file name, like "M_Au_Acc_Find" or "0248143b-a0dd-4fa1-84f9-76831db6714a".</param>
		public static void AuHelp(string topic)
		{
			var s = Folders.ThisAppBS + "Help/Au Help.chm::/html/" + topic + ".htm";
			Api.HtmlHelp(Api.GetDesktopWindow(), s, 0, 0); //HH_DISPLAY_TOPIC
		}

	}

	/// <summary>
	/// Manages a kernel handle.
	/// Must be disposed.
	/// Has static functions to open process handle.
	/// </summary>
	internal struct LibKernelHandle : IDisposable
	{
		IntPtr _h;

		///
		public IntPtr Handle => _h;

		public static implicit operator IntPtr(LibKernelHandle p) { return p._h; }

		///
		public bool Is0 => _h == default;

		/// <summary>
		/// Attaches a kernel handle to this new variable.
		/// No exception when handle is invalid.
		/// </summary>
		/// <param name="handle"></param>
		public LibKernelHandle(IntPtr handle) { _h = handle; }

		///
		public void Dispose()
		{
			if(_h != default && _h != (IntPtr)(-1)) Api.CloseHandle(_h);
			_h = default;
		}

		/// <summary>
		/// Opens process handle.
		/// Calls API OpenProcess.
		/// Returns default if fails. Supports <see cref="WinError.Code"/>.
		/// </summary>
		/// <param name="processId">Process id.</param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibKernelHandle OpenProcess(int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			if(processId == 0) { WinError.Code = Api.ERROR_INVALID_PARAMETER; return default; }
			_OpenProcess(out var h, processId, desiredAccess);
			return new LibKernelHandle(h);
		}

		/// <summary>
		/// Opens window's process handle.
		/// This overload is more powerful: if API OpenProcess fails, it tries API GetProcessHandleFromHwnd, which can open higher integrity level processes, but only if current process is uiAccess and desiredAccess includes only PROCESS_DUP_HANDLE, PROCESS_VM_OPERATION, PROCESS_VM_READ, PROCESS_VM_WRITE, SYNCHRONIZE.
		/// Returns default if fails. Supports <see cref="WinError.Code"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <param name="desiredAccess">Desired access (Api.PROCESS_), as documented in MSDN -> OpenProcess.</param>
		public static LibKernelHandle OpenProcess(Wnd w, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION)
		{
			int pid = w.ProcessId; if(pid == 0) return default;
			_OpenProcess(out var h, pid, desiredAccess, w);
			return new LibKernelHandle(h);
		}

		static bool _OpenProcess(out IntPtr R, int processId, uint desiredAccess = Api.PROCESS_QUERY_LIMITED_INFORMATION, Wnd processWindow = default)
		{
			R = Api.OpenProcess(desiredAccess, false, processId);
			if(R != default) return true;
			if(processWindow.Is0) return false;
			if(0 != (desiredAccess & ~(Api.PROCESS_DUP_HANDLE | Api.PROCESS_VM_OPERATION | Api.PROCESS_VM_READ | Api.PROCESS_VM_WRITE | Api.SYNCHRONIZE))) return false;
			int e = WinError.Code;
			if(Uac.OfThisProcess.IsUIAccess) R = Api.GetProcessHandleFromHwnd(processWindow);
			if(R != default) return true;
			Api.SetLastError(e);
			return false;
		}
	}

	/// <summary>
	/// Kernel handle that is derived from WaitHandle.
	/// When don't need to wait, use <see cref="LibKernelHandle"/>, it's more lightweight and has more creation methods.
	/// </summary>
	internal class LibKernelWaitHandle : WaitHandle
	{
		public LibKernelWaitHandle(IntPtr nativeHandle, bool ownsHandle)
		{
			base.SafeWaitHandle = new SafeWaitHandle(nativeHandle, ownsHandle);
		}

		/// <summary>
		/// Opens process handle.
		/// Returns null if failed.
		/// </summary>
		/// <param name="pid"></param>
		/// <param name="desiredAccess"></param>
		public static LibKernelWaitHandle FromProcessId(int pid, uint desiredAccess)
		{
			LibKernelWaitHandle wh = null;
			try { wh = new LibKernelWaitHandle(LibKernelHandle.OpenProcess(pid, desiredAccess), true); }
			catch(Exception ex) { Debug_.Print(ex); }
			return wh;
		}
	}

	/// <summary>
	/// Security-related functions, such as enabling privileges.
	/// </summary>
	public static class Security_
	{
		/// <summary>
		/// Enables or disables a privilege for this process.
		/// Returns false if fails. Supports <see cref="WinError.Code"/>.
		/// </summary>
		/// <param name="privilegeName"></param>
		/// <param name="enable"></param>
		public static bool SetPrivilege(string privilegeName, bool enable)
		{
			bool ok = false;
			var p = new Api.TOKEN_PRIVILEGES { PrivilegeCount = 1, Privileges = new Api.LUID_AND_ATTRIBUTES { Attributes = enable ? 2u : 0 } }; //SE_PRIVILEGE_ENABLED
			if(Api.LookupPrivilegeValue(null, privilegeName, out p.Privileges.Luid)) {
				Api.OpenProcessToken(Api.GetCurrentProcess(), Api.TOKEN_ADJUST_PRIVILEGES, out IntPtr hToken);
				Api.AdjustTokenPrivileges(hToken, false, p, 0, null, default);
				ok = 0 == WinError.Code;
				Api.CloseHandle(hToken);
			}
			return ok;
		}
	}

	/// <summary>
	/// Calls API <msdn>AttachThreadInput</msdn> to attach/detach thread input.
	/// Constructor attaches thread input of this thread to that of the specified thread. <b>Dispose</b> detaches.
	/// </summary>
	internal struct LibAttachThreadInput : IDisposable
	{
		int _tidThis, _tidAttach;

		/// <summary>
		/// Attaches thread input of this thread to that of the specified thread.
		/// </summary>
		public LibAttachThreadInput(int idThreadAttachTo, out bool succeeded)
		{
			_tidThis = Api.GetCurrentThreadId();
			succeeded = Api.AttachThreadInput(_tidThis, idThreadAttachTo, true);
			_tidAttach = succeeded ? idThreadAttachTo : 0;
		}

		/// <summary>
		/// Detaches thread input.
		/// </summary>
		public void Dispose()
		{
			if(_tidAttach != 0) {
				Api.AttachThreadInput(_tidThis, _tidAttach, false);
				_tidAttach = 0;
			}
		}

		/// <summary>
		/// Returns true if AttachThreadInput succeeded and this variable is not disposed.
		/// </summary>
		public bool IsAttached => _tidAttach != 0;
	}

	//rejected: does not work with Key/Text/Paste. And too dirty. Somehow similar code worked in QM2.
	///// <summary>
	///// Switches to another thread and sleeps for the remainder of this time slice of the thread scheduler.
	///// Unlike Sleep(0) etc, works on multi-processor computers too.
	///// </summary>
	///// <remarks>
	///// Causes this thread to yield execution to another thread that is ready to run on ANY logical processor.
	///// If the remainder of this time slice is less than 400 mcs, retries to sleep during the next time slice.
	///// If there are no other ready threads, does not sleep. Then the speed is about 100 mcs.
	///// </remarks>
	//internal struct LibThreadSwitcher :IDisposable
	//{
	//	ulong _processAffinity, _threadAffinity;

	//	public void Dispose()
	//	{
	//		if(_threadAffinity != 0) {
	//			//Print(_threadAffinity);
	//			Api.SetThreadAffinityMask(Api.GetCurrentThread(), _threadAffinity);
	//		}
	//	}

	//	static uint s_nProc = Api.GetMaximumProcessorCount(Api.ALL_PROCESSOR_GROUPS);

	//	/// <summary>
	//	/// Switches to another thread and sleeps for the remainder of this time slice of the thread scheduler.
	//	/// Returns false if fails. Fails if there are more than 64 logical processors.
	//	/// </summary>
	//	public bool Switch()
	//	{
	//		const int c_nTry = 2;
	//		uint nProc = s_nProc;
	//		long t0 = Time.PerfMicroseconds;
	//		bool switched, retry = false;
	//		g2:
	//		switched = false;
	//		if(nProc == 1) {
	//			for(int i = 0; i < c_nTry; i++) {
	//				if(switched=Api.SwitchToThread()) break;
	//			}
	//		} else {
	//			if(nProc == 0 || nProc > 64) return false;
	//			if(_processAffinity == 0) {
	//				if(!Api.GetProcessAffinityMask(Api.GetCurrentProcess(), out var amProcess, out var amSystem)) return false;
	//				ulong processAffinity = amProcess, systemAffinity = amSystem;
	//				//Print((int)nProc, processAffinity);
	//				for(int i = 0; i < nProc; i++) {
	//					var bit = 1UL << i;
	//					if((bit & processAffinity) == 0 && (bit & systemAffinity) != 0) return false; //are all bits 1?
	//				}
	//				_processAffinity = processAffinity;
	//			}

	//			var ht = Api.GetCurrentThread();
	//			for(int i = 0; i < c_nTry; i++) {
	//				for(int j = 0; j < nProc; j++) {
	//					var bit = 1UL << j;
	//					if((bit & _processAffinity) == 0) continue; //is this bit in system affinity?
	//					var tam = Api.SetThreadAffinityMask(ht, bit);
	//					if(tam == 0) return false;
	//					if(_threadAffinity == 0) _threadAffinity = tam;
	//					if(switched=Api.SwitchToThread()) {
	//						//Print(i, j);
	//						goto g1;
	//					}
	//				}
	//			}
	//		}
	//		g1:
	//		if(switched) {
	//			long t1 = Time.PerfMicroseconds;
	//			if(t1 - t0 < 400) {
	//				//Print("-->", t1 - t0, retry);
	//				if(!retry) { retry = true; goto g2; }
	//				Time.Sleep(1);
	//			}
	//		}
	//		return true;
	//	}

	//[DllImport("kernel32.dll")]
	//internal static extern bool SwitchToThread();

	//internal const ushort ALL_PROCESSOR_GROUPS = 0xFFFF;

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern uint GetMaximumProcessorCount(ushort GroupNumber);

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern bool GetProcessAffinityMask(IntPtr hProcess, out LPARAM lpProcessAffinityMask, out LPARAM lpSystemAffinityMask);

	//[DllImport("kernel32.dll", SetLastError = true)]
	//internal static extern LPARAM SetThreadAffinityMask(IntPtr hThread, LPARAM dwThreadAffinityMask);
	//}

	internal static class LibTaskScheduler
	{
		static string _SidCurrentUser => WindowsIdentity.GetCurrent().User.ToString();
		static string _SddlCurrentUserReadExecute => "D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;GRGX;;;" + _SidCurrentUser + ")";
		static string c_sddlEveryoneReadExecute = "D:AI(A;;FA;;;SY)(A;;FA;;;BA)(A;;GRGX;;;WD)";

		static Api.ITaskFolder _GetOrCreateFolder(Api.ITaskService ts, string taskFolder, out bool createdNew)
		{
			Api.ITaskFolder tf;
			try { tf = ts.GetFolder(taskFolder); createdNew = false; }
			catch(FileNotFoundException) { tf = ts.GetFolder(null).CreateFolder(taskFolder, c_sddlEveryoneReadExecute); createdNew = true; }
			return tf;
		}

		/// <summary>
		/// Creates or updates a trigerless task that executes a program as system, admin or user.
		/// This process must be admin.
		/// You can use <see cref="RunTask"/> to run the task.
		/// </summary>
		/// <param name="taskFolder">
		/// <inheritdoc cref="RunTask"/>
		/// This function creates the folder (and ancestors) if does not exist.
		/// </param>
		/// <param name="taskName"><inheritdoc cref="RunTask"/></param>
		/// <param name="programFile">Full path of an exe file. This function does not normalize it.</param>
		/// <param name="IL">Can be System, High or Medium. If System, runs in SYSTEM account. Else in creator's account.</param>
		/// <param name="args">Command line arguments. Can contain literal substrings $(Arg0), $(Arg1), ..., $(Arg32) that will be replaced by <see cref="RunTask"/>.</param>
		/// <exception cref="UnauthorizedAccessException">Probably because this process is not admin.</exception>
		/// <exception cref="Exception"></exception>
		public static void CreateTaskToRunProgramOnDemand(string taskFolder, string taskName, UacIL IL, string programFile, string args = null)
		{
			var userId = IL == UacIL.System ? "<UserId>S-1-5-18</UserId>" : null;
			var runLevel = IL == UacIL.High ? "<RunLevel>HighestAvailable</RunLevel>" : null;
			var xml =
$@"<?xml version='1.0' encoding='UTF-16'?>
<Task version='1.3' xmlns='http://schemas.microsoft.com/windows/2004/02/mit/task'>

<RegistrationInfo>
<Author>Au</Author>
</RegistrationInfo>

<Principals>
<Principal id='Author'>
{userId}
{runLevel}
</Principal>
</Principals>

<Settings>
<MultipleInstancesPolicy>Parallel</MultipleInstancesPolicy>
<DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
<StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
<ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
<Priority>5</Priority>
</Settings>

<Actions Context='Author'>
<Exec>
<Command>{programFile}</Command>
<Arguments>{args}</Arguments>
</Exec>
</Actions>

</Task>";
			var ts = new Api.TaskScheduler() as Api.ITaskService;
			ts.Connect();
			var tf = _GetOrCreateFolder(ts, taskFolder, out bool createdNew);
			if(!createdNew) tf.DeleteTask(taskName, 0); //we use DeleteTask/TASK_CREATE, because TASK_CREATE_OR_UPDATE does not update task file's security
			var logonType = IL == UacIL.System ? Api.TASK_LOGON_TYPE.TASK_LOGON_SERVICE_ACCOUNT : Api.TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN;
			var sddl = IL == UacIL.System ? c_sddlEveryoneReadExecute : _SddlCurrentUserReadExecute;
			tf.RegisterTask(taskName, xml, Api.TASK_CREATION.TASK_CREATE, null, null, logonType, sddl);

			//note: cannot create a task that runs only in current interactive session, regardless of user.
			//	Tried INTERACTIVE: userId "S-1-5-4", logonType TASK_LOGON_GROUP. But then runs in all logged in sessions.
		}

		/// <summary>
		/// Runs a task. Does not wait.
		/// Returns process id.
		/// </summary>
		/// <param name="taskFolder">Can be like @"\Folder" or "Folder" or @"\" or "" or null.</param>
		/// <param name="taskName">Can be like "Name" or @"\Folder\Name" or @"Folder\Name".</param>
		/// <param name="joinArgs">Join args into single arg for $(Arg0).</param>
		/// <param name="args">Replacement values for substrings $(Arg0), $(Arg1), ..., $(Arg32) in 'create task' args. See <msdn>IRegisteredTask.Run</msdn>.</param>
		/// <exception cref="Exception">Failed. Probably the task does not exist.</exception>
		public static int RunTask(string taskFolder, string taskName, bool joinArgs, params string[] args)
		{
			object a; if(Empty(args)) a = null; else if(joinArgs) a = StringMisc.CommandLineFromArray(args); else a = args;
			var ts = new Api.TaskScheduler() as Api.ITaskService;
			ts.Connect();
			var rt = ts.GetFolder(taskFolder).GetTask(taskName).Run(a);
			rt.get_EnginePID(out int pid);
			return pid;
		}

		/// <summary>
		/// Returns true if the task exists.
		/// </summary>
		/// <param name="taskFolder"><inheritdoc cref="RunTask"/></param>
		/// <param name="taskName"><inheritdoc cref="RunTask"/></param>
		/// <exception cref="Exception">Failed.</exception>
		public static bool TaskExists(string taskFolder, string taskName)
		{
			var ts = new Api.TaskScheduler() as Api.ITaskService;
			ts.Connect();
			try { ts.GetFolder(taskFolder).GetTask(taskName); }
			catch(FileNotFoundException) { return false; }
			return true;
		}
	}
}
