//Classes and functions that were finished or almost finished, but rejected for some reason. Maybe still can be useful in the future.
//For example, when tried to make faster/better than existing .NET classes/functions, but the result was not fast/good enough.








//rejected: struct WildexStruct - struct version of Wildex class. Does not make faster, although creates less garbage.
/// <summary>
/// A slim version of <see cref="Wildex"/>.
/// </summary>
/// <remarks>
/// Has all the same capabilities as Wildex, but is just a struct of single pointer size.
/// The pointer is Object that can be of one of these types:
/// <see cref="Wildex"/> - if the assigned string was with wildcard expression options, like "**c text". To compare it with other strings, <see cref="Match"/> calls <see cref="Wildex.Match"/>.
/// <see cref="String"/> - if the assigned string was without wildcard expression options. To compare it with other strings, <see cref="Match"/> calls <see cref="String_.Like_(string, string, bool)"/> with <i>ignoreCase</i> true.
/// null - if was not assigned a non-null string.
/// </remarks>
public struct WildexStruct
{
	object _obj;

	/// <param name="wildcardExpression">
	/// <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">Wildcard expression</conceptualLink>.
	/// Cannot be null (throws exception).
	/// "" will match "".
	/// </param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException">Invalid "**options " or regular expression.</exception>
	public WildexStruct(string wildcardExpression)
	{
		var w = wildcardExpression;
		if(w == null) throw new ArgumentNullException();
		if(w.Length >= 3 && w[0] == '*' && w[1] == '*') _obj = new Wildex(w);
		else _obj = w;
	}

	/// <summary>
	/// Creates new WildexStruct from wildcard expression string.
	/// If the string is null, returns empty variable. Then it's <see cref="HasValue"/> property is false, <see cref="Value"/> property is null, and <see cref="Match"/> must not ne called.
	/// </summary>
	/// <param name="wildcardExpression">
	/// <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">Wildcard expression</conceptualLink>.
	/// </param>
	public static implicit operator WildexStruct(string wildcardExpression)
	{
		if(wildcardExpression == null) return default;
		return new WildexStruct(wildcardExpression);
	}

	/// <summary>
	/// Compares a string with the <conceptualLink target="0248143b-a0dd-4fa1-84f9-76831db6714a">wildcard expression</conceptualLink> used to create this variable.
	/// Returns true if they match.
	/// </summary>
	/// <param name="s">String. If null, returns false. If "", returns true if it was "" or "*" or a regular expression that matches "".</param>
	public bool Match(string s)
	{
		if(s == null) return false;
		if(_obj is string t) return s.Like_(t, true);
		if(_obj is Wildex x) return x.Match(s);
		throw new InvalidOperationException("Empty " + nameof(WildexStruct));
	}

	/// <summary>
	/// Returns true if assigned a non-null string.
	/// This variable contains either the string or <see cref="Wildex"/>.
	/// If this property is false, don't call <see cref="Match"/>, it will throw exception.
	/// </summary>
	public bool HasValue => _obj != null;

	/// <summary>
	/// Returns Wildex if the assigned string was with wildcard expression options, like "**c text".
	/// Returns String if the assigned string was without wildcard expression options.
	/// Returns null if was not assigned a non-null string.
	/// </summary>
	public object Value => _obj;

	///
	public override string ToString()
	{
		return _obj?.ToString();
	}
}






//rejected. Or instead use a struct containing member of 'object' type. Because now the calling code is huge and in most cases slower than with object.
/// <summary>
/// Used for method parameters that accept one of two types.
/// </summary>
/// <remarks>
/// Some methods have one or several parameters that must be one of several types. There are several ways of implementing such functions.
/// 1. Overloaded methods. However some programmers often don't like it, because in some cases need to create many overloads, then maintain their documentation. Also then method users have to spend much time to find the correct overload.
/// 2. Generic methods. However in most cases the compiler cannot protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense.
/// 3. Using the Object type for the parameter. However then the compiler does not protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense. Also slower, especially with value types (need boxing).
/// 4. Using this type for the parameter. It does not have the above problems.
/// 
/// When a parameter is of type Types&lt;T1, T2&gt;, method users can pass only values of type T1 or T2, else compiler error. They see the allowed types instantly in intellisense.
/// Also there are similar types that can be used to support more parameter types:
/// <see cref="Types{T1, T2, T3}"/>
/// <see cref="Types{T1, T2, T3, T4}"/>
/// 
/// To support null (for example for optional arguments), use nullable. See examples.
/// </remarks>
/// <example>
/// <code><![CDATA[
/// static void Example1(Types<string, IEnumerable<string>> x)
/// {
/// 	switch(x.type) {
/// 	case 1: Print("string", x.v1); break;
/// 	case 2: Print("IEnumerable<string>", x.v2); break;
/// 	default: throw new ArgumentException(); //0 if has default value, eg assigned default(Types<string, IEnumerable<string>>), unlikely
/// 	}
/// }
/// 
/// static void Example2(int param1, Types<int, double>? optionalParam = null)
/// {
/// 	var p = optionalParam.GetValueOrDefault();
/// 	switch(p.type) {
/// 	case 0: Print("null"); break;
/// 	case 1: Print("int", p.v1); break;
/// 	case 2: Print("double", p.v2); break;
/// 	}
/// }
/// 
/// static void TestExamples()
/// {
/// 	Example1("S");
/// 	Example1(new string[] { "a", "b" });
/// 	//Example1(5); //compiler error
/// 	//Example1(null); //compiler error
/// 	Example1((string)null);
/// 	//Example1(default(Types<string, IEnumerable<string>>)); //the function throws exception
/// 
/// 	Example2(0, 5);
/// 	Example2(0, 5.5);
/// 	//Example2(0, "S"); //compiler error
/// 	Example2(0, null);
/// 	Example2(0);
/// 	//Example2(0, default(Types<int, double>)); //the function throws exception
/// }
/// ]]></code>
/// </example>
[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
public struct Types<T1, T2>
{
	/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 0 if unassigned. </summary>
	public byte type;
	/// <summary> Value of type T1. Valid when type is 1. </summary>
	public T1 v1;
	/// <summary> Value of type T2. Valid when type is 2. </summary>
	public T2 v2;

	Types(T1 v) { type = 1; v1 = v; v2 = default; }
	Types(T2 v) { type = 2; v2 = v; v1 = default; }

	/// <summary> Assignment of a value of type T1. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)] //makes caller's native code much smaller, although slightly slower
	public static implicit operator Types<T1, T2>(T1 x) { return new Types<T1, T2>(x); }
	/// <summary> Assignment of a value of type T2. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2>(T2 x) { return new Types<T1, T2>(x); }

	//Prevent assigning null to a non-nullable Types variable when one of its types is a reference type, eg Types<string, int>.
	//Compiler allows to assign null to a struct only if exactly one operator is of a reference type.
	//This creates two problems: 1. Some functions allow null, some not. 2. Some type combinations allow null, some not.
	//Now compiler error if caller tries to pass null. If need to support null, let use nullable.
	//Need two operators because with single operator would allow to assign null when all value types, eg Types<double, int>.
	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2>(JustNull n) => default;
	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2>(JustNull2 n) => default;
}

/// <summary>
/// Used for method parameters that accept one of three types.
/// More info: <see cref="Types{T1, T2}"/>.
/// </summary>
[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
public struct Types<T1, T2, T3>
{
	/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 3 if T3 (v3 is valid), 0 if unassigned (unlikely). </summary>
	public byte type;
	/// <summary> Value of type T1. Valid when type is 1. </summary>
	public T1 v1;
	/// <summary> Value of type T2. Valid when type is 2. </summary>
	public T2 v2;
	/// <summary> Value of type T3. Valid when type is 3. </summary>
	public T3 v3;

	Types(T1 v) : this() { type = 1; v1 = v; }
	Types(T2 v) : this() { type = 2; v2 = v; }
	Types(T3 v) : this() { type = 3; v3 = v; }

	/// <summary> Assignment of a value of type T1. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3>(T1 x) { return new Types<T1, T2, T3>(x); }
	/// <summary> Assignment of a value of type T2. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3>(T2 x) { return new Types<T1, T2, T3>(x); }
	/// <summary> Assignment of a value of type T3. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3>(T3 x) { return new Types<T1, T2, T3>(x); }

	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2, T3>(JustNull n) => default;
	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2, T3>(JustNull2 n) => default;
}

/// <summary>
/// Used for method parameters that accept one of four types.
/// More info: <see cref="Types{T1, T2}"/>.
/// </summary>
[DebuggerStepThrough]
[StructLayout(LayoutKind.Auto)]
public struct Types<T1, T2, T3, T4>
{
	/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), and so on. 0 if unassigned (unlikely). </summary>
	public byte type;
	/// <summary> Value of type T1. Valid when type is 1. </summary>
	public T1 v1;
	/// <summary> Value of type T2. Valid when type is 2. </summary>
	public T2 v2;
	/// <summary> Value of type T3. Valid when type is 3. </summary>
	public T3 v3;
	/// <summary> Value of type T4. Valid when type is 4. </summary>
	public T4 v4;

	Types(T1 v) : this() { type = 1; v1 = v; }
	Types(T2 v) : this() { type = 2; v2 = v; }
	Types(T3 v) : this() { type = 3; v3 = v; }
	Types(T4 v) : this() { type = 4; v4 = v; }

	/// <summary> Assignment of a value of type T1. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3, T4>(T1 x) { return new Types<T1, T2, T3, T4>(x); }
	/// <summary> Assignment of a value of type T2. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3, T4>(T2 x) { return new Types<T1, T2, T3, T4>(x); }
	/// <summary> Assignment of a value of type T3. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3, T4>(T3 x) { return new Types<T1, T2, T3, T4>(x); }
	/// <summary> Assignment of a value of type T4. </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static implicit operator Types<T1, T2, T3, T4>(T4 x) { return new Types<T1, T2, T3, T4>(x); }

	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2, T3, T4>(JustNull n) => default;
	/// <summary>Infrastructure.</summary>
	public static implicit operator Types<T1, T2, T3, T4>(JustNull2 n) => default;
}

#if false
	//This varsion also works well.
	//Good: smaller calling code. Very fast for reference types.
	//Bad: need boxing for value types. It is quite fast (~50% slower), but in some cases can create much garbage.
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Auto)]
	public struct Types2<T1, T2>
	{
		/// <summary> The assigned value. </summary>
		public object Value;

		/// <summary> Assignment of a value of type T1. </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)] //makes caller's native code much smaller, but then speed same as class or worse
		public static implicit operator Types2<T1, T2>(T1 x) { return new Types2<T1, T2>() { Value = x }; }
		/// <summary> Assignment of a value of type T2. </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types2<T1, T2>(T2 x) { return new Types2<T1, T2>() { Value = x }; }

		//Prevent assigning null to a non-nullable Types variable when one of its types is a reference type, eg Types<string, int>.
		//Compiler allows to assign null to a struct only if exactly one operator is of a reference type.
		//This creates two problems: 1. Some functions allow null, some not. 2. Some type combinations allow null, some not.
		//Now compiler error if caller tries to pass null. If need to support null, let use nullable.
		//Need two operators because with single operator would allow to assign null when all value types, eg Types<double, int>.
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types2<T1, T2>(JustNull n) => default;
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types2<T1, T2>(JustNull2 n) => default;
	}
#endif





public static string TextToRTF(string s)
{
	if(Empty(s)) return s;
	var b = new StringBuilder(@"{\rtf1
			");
	for(int i = 0; i < s.Length; i++) {
		char c = s[i];
		if(c > 0x7f || c == 0) {
			b.Append(@"\u");
			b.Append((short)c);
			b.Append('?');
		} else if(c == '\r' || c == '\n') {
			if(c == '\n' && i > 0 && s[i - 1] == '\r') continue;
			b.AppendLine(@"\par");
		} else {
			if(c == '\\' || c == '{' || c == '}') b.Append('\\');
			b.Append(c);
		}
	}
	b.Append("}");
	return b.ToString();
}




///// <summary>
///// Registers and unregisters a window class.
///// Example: <c>static Wnd.Misc.MyWindowClass _myClass = Wnd.Misc.MyWindowClass.Register("MyClass", _MyWndProc); ... Wnd w = Wnd.Misc.CreateWindow(0, _myClass.Name, ...);</c>.
///// </summary>
//public class MyWindowClass
//{
//	private MyWindowClass() { } //disable '=new MyWindowClass()'

//	///
//	~MyWindowClass()
//	{
//		Unregister();
//	}

//	/// <summary>
//	/// Class atom.
//	/// </summary>
//	public ushort Atom { get; private set; }

//	/// <summary>
//	/// Actual class name that must be used to create windows.
//	/// It is not exactly the same as passed to Create() etc. It has a suffix containing current appdomain identifer.
//	/// </summary>
//	public string Name => _className;

//	/// <summary>
//	/// Base class extra memory size.
//	/// </summary>
//	public int BaseClassWndExtra { get; private set; }

//	/// <summary>
//	/// Base class window procedure to pass to API <msdn>CallWindowProc</msdn> in your window procedure.
//	/// </summary>
//	public Native.WNDPROC BaseClassWndProc { get; private set; }

//	IntPtr _hinst; //need for Unregister()
//	Native.WNDPROC _wndProc; //to keep reference to the caller's delegate to prevent garbage collection
//	string _className; //for warning text in Unregister()

//	/// <summary>
//	/// Registers new window class.
//	/// Returns new MyWindowClass variable.
//	/// Calls API <msdn>RegisterClassEx</msdn>.
//	/// </summary>
//	/// <param name="className">Class name.</param>
//	/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
//	/// <param name="wndExtra">Size of extra window memory which can be accessed with Wnd.SetWindowLong/GetWindowLong with >=0 offset.</param>
//	/// <param name="style">Class style.</param>
//	/// <param name="ex">
//	/// Can be used to specify API <msdn>WNDCLASSEX</msdn> fields not specified in parameters.
//	/// If not used, the function sets: hCursor = arrow; hbrBackground = COLOR_BTNFACE+1; others = 0/null/Zero.
//	/// </param>
//	/// <exception cref="Win32Exception">Failed, for example if the class already exists.</exception>
//	/// <remarks>
//	/// The actual class name is like "MyClass.2", where "MyClass" is className and "2" is current appdomain id. The <see cref="Name"/> property returns it.
//	/// If style does not have CS_GLOBALCLASS and ex is null or its hInstance field is not set, for hInstance uses exe module handle.
//	/// Not thread-safe.
//	/// </remarks>
//	public static MyWindowClass Register(string className, Native.WNDPROC wndProc, int wndExtra = 0, uint style = 0, WndClassEx ex = null)
//	{
//		//var x = (ex != null) ? new Api.WNDCLASSEX(ex) : new Api.WNDCLASSEX(true);
//		var x = new Api.WNDCLASSEX(true); //TODO

//		var r = new MyWindowClass();
//		r._Register(ref x, className, wndProc, wndExtra, style);
//		return r;

//		//hInstance=Api.GetModuleHandle(null); //tested: RegisterClassEx uses this if hInstance is Zero, even for app-local classes

//		//tested:
//		//For app-global classes, CreateWindowEx and GetClassInfo ignore their hInst argument (always succeed).
//		//For app-local classes, CreateWindowEx and GetClassInfo fail if their hInst argument does not match. However CreateWindowEx always succeeds if its hInst argument is Zero.
//	}

//	unsafe void _Register(ref Api.WNDCLASSEX x, string className, Native.WNDPROC wndProc, int wndExtra, uint style)
//	{
//		//Add appdomain id to the class name. It solves 2 problems:
//		//	1. Multiple domains cannot register exactly the same class name because they cannot use a common procedure.
//		//	2. In Release build compiler completely removes code 'static Wnd.Misc.MyWindowClass _myClass = Wnd.Misc.MyWindowClass.Register("MyClass", _MyWndProc);' if _MyWndProc is not referenced elsewhere. Then the class is not registered when we create window. Now programmers must use _myClass.Name with CreateWindowEx etc, and it prevents removing the code.
//		className = className + "." + AppDomain.CurrentDomain.Id;
//		//Print(className);

//		fixed (char* pCN = className) {
//			x.cbSize = Api.SizeOf(x);
//			x.lpszClassName = pCN;
//			x.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);
//			x.cbWndExtra = wndExtra;
//			x.style = style;
//			if(x.hInstance == Zero && (style & Api.CS_GLOBALCLASS) == 0) x.hInstance = Api.GetModuleHandle(null);

//			Atom = Api.RegisterClassEx(ref x);
//			if(Atom == 0) throw new Win32Exception();

//			_hinst = x.hInstance; //if was set in ex, will need this for Unregister()
//			_wndProc = wndProc; //keep the delegate from GC
//			_className = className;
//		}
//	}

//	/// <summary>
//	/// Registers new window class that extends an existing class.
//	/// Returns new MyWindowClass variable.
//	/// Calls API <msdn>GetClassInfoEx</msdn> and API <msdn>RegisterClassEx</msdn>.
//	/// </summary>
//	/// <param name="baseClassName">Existing class name.</param>
//	/// <param name="className">New class name.</param>
//	/// <param name="wndProc">Window procedure delegate. This variable will keep a reference to it to prevent garbage-collecting.</param>
//	/// <param name="wndExtra">Size of extra window memory not including extra memory of base class. Can be accessed with SetMyLong/GetMyLong.</param>
//	/// <param name="globalClass">If false, the function removes CS_GLOBALCLASS style.</param>
//	/// <param name="baseModuleHandle">If the base class is global (CS_GLOBALCLASS style), don't use this parameter, else pass the module handle of the exe or dll that registered the base class.</param>
//	/// <exception cref="Win32Exception">Failed, for example if the class already exists or class baseClassName does not exist.</exception>
//	/// <remarks>
//	/// The actual class name is like "MyClass.2", where "MyClass" is className and "2" is current appdomain identifer. The <see cref="Name"/> property returns it.
//	/// Not thread-safe.
//	/// </remarks>
//	public static MyWindowClass Superclass(string baseClassName, string className, Native.WNDPROC wndProc, int wndExtra = 0, bool globalClass = false, IntPtr baseModuleHandle = default)
//	{
//		var r = new MyWindowClass();
//		var x = new Api.WNDCLASSEX();
//		x.cbSize = Api.SizeOf(x);
//		if(0 == Api.GetClassInfoEx(baseModuleHandle, baseClassName, ref x)) throw new Win32Exception();

//		Api.GetDelegate(x.lpfnWndProc, out Native.WNDPROC wp);
//		int we = x.cbWndExtra;

//		r._Register(ref x, className, wndProc, x.cbWndExtra + wndExtra, globalClass ? x.style : x.style & ~Api.CS_GLOBALCLASS);

//		r.BaseClassWndProc = wp;
//		r.BaseClassWndExtra = we;
//		return r;
//	}

//	/// <summary>
//	/// Unregisters the window class if registered with Register() or Superclass().
//	/// Called implicitly when garbage-collecting this object.
//	/// </summary>
//	public void Unregister()
//	{
//		if(Atom != 0) {
//			bool ok = Api.UnregisterClass(Atom, _hinst);
//			if(!ok) Output.Warning($"Failed to unregister window class '{_className}'. {Native.GetErrorMessage()}.");
//			Atom = 0;
//			_hinst = Zero;
//			BaseClassWndProc = null;
//			BaseClassWndExtra = 0;
//		}
//	}

//	/// <summary>
//	/// Calls <see cref="SetWindowLong"/> and returns its return value.
//	/// </summary>
//	/// <param name="w">Window.</param>
//	/// <param name="value">Value.</param>
//	/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
//	/// <exception cref="WndException"/>
//	public LPARAM SetMyLong(Wnd w, LPARAM value, int offset = 0)
//	{
//		return w.SetWindowLong(BaseClassWndExtra + offset, value);
//	}

//	/// <summary>
//	/// Calls <see cref="GetWindowLong"/> and returns its return value.
//	/// </summary>
//	/// <param name="w">Window.</param>
//	/// <param name="offset">Offset in extra memory, not including the size of extra memory of base class.</param>
//	public LPARAM GetMyLong(Wnd w, int offset = 0)
//	{
//		return w.GetWindowLong(BaseClassWndExtra + offset);
//	}

//	/// <summary>
//	/// Rarely used API <msdn>WNDCLASSEX</msdn> fields. Used with some MyWindowClass functions.
//	/// </summary>
//	/// <tocexclude />
//	public class WndClassEx
//	{
//#pragma warning disable 1591 //XML doc
//		public int cbClsExtra;
//		public IntPtr hInstance;
//		public IntPtr hIcon;
//		public IntPtr hCursor;
//		public IntPtr hbrBackground;
//		public IntPtr hIconSm;
//#pragma warning restore 1591 //XML doc
//	}
//}






//rejected: not useful. If will need some day, move most of the code to cpp.
///// <summary>
///// Gets accessible object from point in window.
///// Returns null if the point is not in the window rectangle. Or if failed to get accessible object and noThrow is true.
///// </summary>
///// <param name="w">Window or control.</param>
///// <param name="x">X coordinate relative to the w client area.</param>
///// <param name="y">Y coordinate relative to the w client area.</param>
///// <param name="noThrow">Don't throw exception when fails to get accessible object from window. Then returns null.</param>
///// <exception cref="WndException">Invalid window.</exception>
///// <exception cref="AuException">Failed to get accessible object from window. For example, window of a higher UAC integrity level process.</exception>
//public static Acc FromXY(Wnd w, Coord x, Coord y, bool noThrow = false)
//{
//	var p = Coord.NormalizeInWindow(x, y, w);
//	if(!w.GetWindowAndClientRectInScreen(out RECT rw, out RECT rc)) {
//		if(noThrow) return null;
//		w.ThrowUseNative();
//	}
//	p.Offset(rc.left, rc.top);
//	if(!rw.Contains(p)) return null;
//	bool inClent = rc.Contains(p);
//	var a = FromWindow(w, inClent ? AccOBJID.CLIENT : AccOBJID.WINDOW, noThrow: noThrow);
//	if(a != null) {
//		if(!inClent) {
//		} else if(w.IsChildWindow) {
//		} else if(0 != _EnableInChrome(w, a._iacc, true)) {
//		} else if(0 == a._iacc.get_accChildCount(out int cc) && cc == 0) { //CLIENT of top-level window has 0 children
//			var ja = _Java.AccFromPoint(p, w);
//			if(ja != null) { a.Dispose(); return ja; }
//		}
//		if(0 == a._DescendantFromPoint(p, out var ac) && ac != a) {
//			Math_.Swap(ref a, ref ac);
//			ac.Dispose();
//		}
//	}
//	return a;
//}

///// <summary>
///// Gets a child or descendant object from point.
///// Returns self (this Acc variable, not a copy) if at that point is this object and not a child.
///// Returns null if that point is not within boundaries of this object. For non-rectangular objects can return null even if the point is in the bounding rectangle. Also returns null if fails. Supports <see cref="Native.GetError"/>.
///// </summary>
///// <param name="x">X coordinate.</param>
///// <param name="y">Y coordinate.</param>
///// <param name="screenCoord">x y are screen coordinates. If false (default), x y are relative to the bounding rectangle of this object.</param>
///// <param name="directChild">Get direct child. If false (default), gets the topmost descendant object, which often is not a direct child of this object.</param>
///// <remarks>
///// Uses API <msdn>IAccessible.accHitTest</msdn>.
///// Does not work with Java windows.
///// </remarks>
//public Acc ChildFromXY(Coord x, Coord y, bool screenCoord = false, bool directChild = false)
//{
//	Point p;
//	if(screenCoord) p = Coord.Normalize(x, y);
//	else if(GetRect(out var r)) {
//		p = Coord.NormalizeInRect(x, y, r);
//		if(!r.Contains(p)) return null;
//	} else return null;

//	_Hresult(_FuncId.child_object, _DescendantFromPoint(p, out var ac, directChild));
//	return ac;

//	//rejected: parameter disposeThis.
//	//never mind: does not work with Java windows.
//}

//int _DescendantFromPoint(Point p, out Acc ar, bool directChild = false)
//{
//	ar = null;
//	int hr = new _Acc(_iacc, _elem).DescendantFromPoint(p, out var ad, out bool isThis, directChild);
//	if(hr == 0) ar = isThis ? this : new Acc(ad.a, ad.elem);
//	return hr;
//}






#pragma code_seg(push, r1, ".remote_thread")
#pragma optimize("", off)
DWORD WINAPI _InjectDllThreadProc(LPVOID lpParameter)
{

	return 0;
}

bool _InjectDll(HWND w, out HWND& wa, DWORD tid, DWORD pid)
{
	//auto hh = SetWindowsHookExW(WH_CALLWNDPROC, HookCallWndProc, s_moduleHandle, tid); if(hh == 0) return false;
	//wa = (HWND)SendMessage(w, 0, c_injectWparam, 0);
	//UnhookWindowsHookEx(hh);
	//if(wa) return true;

	//problem: hook does not work with Windows.UI.Core.CoreWindow.
	//	SetWindowsHookEx succeeds, but the hook proc is never called, and the dll is not injected.
	//	But SendMessage works, eg WM_CLOSE closes the app.
	//	The CreateRemoteThread(LoadLibrary) method fails too. LoadLibrary returns 0.
	//	Maybe because our dll is not signed?


	bool ok = false;
	const DWORD desiredAccess = PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ;
	HANDLE hp = OpenProcess(desiredAccess, false, pid); if(!hp) return false;
	//SIZE_T memSize = (LPBYTE)_InjectDll - (LPBYTE)_InjectDllThreadProc, sizeWritten; //note: this will not work in Debug, because the function address is indirect
	//Print(memSize);
	//assert(memSize > 0 && memSize < 4096);
	SIZE_T memSize = 300, sizeWritten;
	WCHAR b[300]; GetModuleFileNameW(s_moduleHandle, b, 300);
	LPVOID mem = VirtualAllocEx(hp, null, max(memSize, 4096), MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
	if(mem) {
		Print("VirtualAllocEx ok");
		//if(WriteProcessMemory(hp, mem, _InjectDllThreadProc, memSize, &sizeWritten) && sizeWritten == memSize) {
		if(WriteProcessMemory(hp, mem, b, memSize, &sizeWritten) && sizeWritten == memSize) {
			Print("WriteProcessMemory ok");

			HANDLE ht = CreateRemoteThread(hp, null, 64 * 1024, (LPTHREAD_START_ROUTINE)LoadLibraryW, mem, 0, null);
			if(hp) {
				Print("CreateRemoteThread ok");

				//ok=true;
				WaitForSingleObject(ht, INFINITE);
				DWORD ec = 0; if(GetExitCodeThread(ht, &ec)) Printx((int)ec);

				CloseHandle(ht);
			}
		}
	}

	CloseHandle(hp);

	return ok;
}
#pragma optimize("", on)
#pragma code_seg(pop, r1)





/// <summary>
/// Manages an IAccessible/Wnd dictionary that is used by GetWnd and get_accParent for the Chrome bug workaround.
/// In Chrome versions 61-? is broken get_accParent in web pages.
///		It gets parent AO (except of DOCUMENT), but if we need container window of that AO, windowfromaccessibleobject (WFAO) fails.
///		It's probably because DOCUMENT's get_accParent is broken. It seems WFAO walks ancestors by calling get_accParent until finds WINDOW.
///		WFAO fails only for AO that are retrieved with get_accParent (or derived from it using navigation etc).
///			Probably because MSAA caches HWNDs. We can guess it from the speed difference.
///			WFAO usually is quite fast, but very slow if the AO was returned by get_accParent etc, because their HWNDs are not cached.
///	We use this dictionary not only with Chrome. It makes GetWnd much faster with these get_accParent-returned AOs.
///	This workaround is only for AO retrieved with get_accParent directly. If we then navigate to another object (next, child etc), WFAO fails for them. Never mind, too much work to track all.
///	WFAO does not fail for AO retrieved using navigation when the navigation start AO's HWND is cached.
/// Tried to get HWND from these interfaces, but Chrome does not give them: UIA.IElement, IOleWindow, IAccIdentity, IAccessible2.
///		IAccessible2.windowHandle works, but need to register IAccessible2Proxy.dll or inject into Chrome process. Too much work.
///		Besides windowHandle, IAccessible2 does not have anything useful that we could not get with other interfaces.
///	UI Automation works well. But cannot be used for this workaround (cannot get UIA.IElement from IAccessible).
/// </summary>
class _ChromeParentBugWorkaround
{
	//Acc CONSIDER: to use less memory, instead of Dictionary<LPARAM, Wnd> use Dictionary<Wnd, List<LPARAM>>

	Dictionary<LPARAM, Wnd> _d = new Dictionary<LPARAM, Wnd>();
	//long _time;
	//Acc FUTURE: sometimes remove disposed AO, eg once in 1 minute or when GC runs.
	//	For each dictionary key call AddRef/Release to get ref count. Remove if 1.

	public void Add(IntPtr iacc, Wnd w)
	{
		if(!w.IsAlive) { _RemoveWnd(w); return; }
		Marshal.AddRef(iacc);
		_d[iacc] = w;

		Debug_.PrintIf(_d.Count > 100, "big dictionary IAccessible/Wnd. Count: " + _d.Count.ToString());
	}

	public bool TryGet(IntPtr iacc, out Wnd w)
	{
		if(!_d.TryGetValue(iacc, out w)) return false;
		if(w.IsAlive) return true;
		_RemoveWnd(w);
		w = default;
		return false;
	}

	void _RemoveWnd(Wnd w)
	{
		var a = new Util.LibArrayBuilder<LPARAM>();
		foreach(var x in _d) if(x.Value == w) a.Add(x.Key);
		for(int i = a.Count - 1; i >= 0; i--) _d.Remove(a[i]);
	}

	~_ChromeParentBugWorkaround() //called when GC runs after this thread ended
	{
		foreach(var x in _d) Marshal.Release(x.Key);
	}
}

[ThreadStatic] static _ChromeParentBugWorkaround t_iaccWnd; //Acc FUTURE: try to make non-[ThreadStatic] if possible

public int get_accParent(out IAccessible iacc)
{
	iacc = default;

	//Chrome bug workaround: part 1.
#if DEBUG
				var t0 = Time.Microseconds;
				GetWnd(out Wnd w); //in browsers usually about 10 times faster (when uncached) than get_accParent. In many windows fast.
				var td = Time.Microseconds - t0;
				Debug_.PrintIf(td >= 1000, "slow GetWnd in Chrome bug workaround. Time: " + td.ToString());
#else
	GetWnd(out Wnd w);
#endif

	var hr = _F.get_accParent(_iptr, out var idisp);
	if(hr == 0) {
		hr = FromIDispatch(idisp, out iacc);

		//Chrome bug workaround: part 2.
		if(hr == 0 && !w.Is0 && !w.IsChildWindow && 0 == GetRole(0, out var role) && role != AccROLE.WINDOW) {
			//note: here we cannot call GetWnd to make sure that window of iacc == w, because it is very slow in any app, and in Chrome fails.
			//	If our parent window is top-level and our role is not WINDOW, logically window of iacc must be the same as ours.
			//	If out role is WINDOW, iacc window could be the root desktop.

			//PrintList("get_accParent: add", iacc._iptr);
			var d = t_iaccWnd;
			if(d == null) t_iaccWnd = d = new _ChromeParentBugWorkaround();
			d.Add(iacc, w);
		}
	}
	return hr;
}

public int GetWnd(out Wnd w)
{
	w = default;
	Debug.Assert(!Is0);

	//Chrome bug workaround: part 3.
	if(t_iaccWnd?.TryGet(_iptr, out w) ?? false) {
		//PrintList("GetWnd: cached", _iptr);
		return 0;
	}
	//PrintList("GetWnd: uncached", _iptr);

	int hr = _WindowFromAccessibleObject(this, out w);

	return hr;
}







delegate int CsFuncT(IntPtr x);

static CsFuncT _TestClrHost2 = TestClrHost2;
static int TestClrHost2(IntPtr x)
{
	//TaskDialog.Show(""+x);

	var w = Wnd.Find("* Google Chrome", "Chrome*").OrThrow();
	//var w = Wnd.Find("Keys").OrThrow();
	using(var a = Acc.Find(w, null, "Untitled")) {
		//TaskDialog.Show(a.ToString());
		if(a != null) {
			var r = LresultFromObject(ref Api.IID_IAccessible, 0, a._iacc);
			return r;
		}
	}

	return 0;
}
[DllImport("oleacc.dll")]
internal static extern LPARAM LresultFromObject(ref Guid riid, LPARAM wParam, IAccessible punk);

static int TestClrHost(string s)
{
	var p = (IntPtr*)s.ToInt64_();
	*p = Marshal.GetFunctionPointerForDelegate(_TestClrHost2);

	//Print(s);
	//TaskDialog.Show("CLR host", s);
	return 1;
}







///// <summary>
///// Gets or sets the cache request object that is used by functions of this class.
///// The object is [ThreadStatic].
///// </summary>
//public static UIA.ICacheRequest Cache
//{
//	get => t_cache;
//	set => t_cache = value;
//}
//[ThreadStatic] static UIA.ICacheRequest t_cache;

//static bool _FC(out UIA.IUIAutomation f, out UIA.ICacheRequest c)
//{
//	f = Factory;
//	c = t_cache;
//	return c != null;
//}

////public static UIA.IElement Root { get { var f = Factory; var c = Cache; return c == null ? f.GetRootElement() : f.GetRootElementBuildCache(c); } }
//public static UIA.IElement Root { get => _FC(out var f, out var c) ? f.GetRootElementBuildCache(c) : f.GetRootElement(); }






//rejected: either too unreliable or too slow. The problem is that children of some elements are not in element's rect.
//struct _DescendantFromPointFinder
//{
//	Point _p;
//	long _sizeFound;
//	public UIA.IElement eFound;
//	UIA.ICondition _cond;

//	//tested: with tree walker slower.
//	//tested: with rectangle cache slower.

//	public _DescendantFromPointFinder(Point pScreen, bool chromeWorkaround = false) : this()
//	{
//		_p = pScreen;
//		var f = Factory;
//		_cond = t_condRaw; //not t_condVisible, because offscreen elements can have visible children, for example offscreen TreeItem
//						   //if(chromeWorkaround) _cond = f.CreateAndCondition(_cond, f.CreateNotCondition(f.CreatePropertyConditionEx(UIA.PropertyId.ClassName, "Intermediate D3D Window", UIA.PropertyConditionFlags.IgnoreCase)));
//		if(chromeWorkaround) _cond = f.CreateNotCondition(f.CreatePropertyConditionEx(UIA.PropertyId.ClassName, "Intermediate D3D Window", UIA.PropertyConditionFlags.IgnoreCase));
//		//SHOULDDO: skip tooltip, dialog, floating pane...
//	}

//	public void Find(UIA.IElement eParent, int level = 0, RECT rParent=default)
//	{
//		if(level == 0) {
//			var r = eParent.BoundingRectangle;
//			if(!r.Contains(_p)) return;
//			eFound = eParent; _sizeFound = r.Width * r.Height;
//		}

//		var a = eParent.FindAll(UIA.TreeScope.Children, _cond);
//		for(int i = 0, n = a.Length; i < n; i++) {
//			var e = a.GetElement(i);
//			var r = e.BoundingRectangle;
//			if(!r.Contains(_p)) {
//				//children of some elements are not in element's rect...
//				goto g1; //will walk whole tree, too slow
//				//switch(e.ControlType) {
//				//case UIA.TypeId.TreeItem: //example: standard tree view controls, VS Solution Explorer
//				//case UIA.TypeId.TabItem: //example: VS Solution Explorer
//				//	goto g1;
//				//}
//				//continue;
//			}
//			var size = r.Width * r.Height;
//			if(size <= _sizeFound) {
//				if(size < _sizeFound || (eFound==eParent && r==rParent)) {
//					_sizeFound = size;
//					eFound = e;
//				}
//			}
//			g1:
//			//PrintList(level, e.LibToString_());
//			Find(e, level + 1, r);
//		}
//	}
//}

//public static UIA.IElement FromPoint(Wnd w, Point pScreen)
//{
//	return _DescendantFromPoint(Factory.ElementFromHandle(w), pScreen);
//}

//public static UIA.IElement FromPoint(UIA.IElement e, Point pScreen)
//{
//	return _DescendantFromPoint(e, pScreen);
//}

//static UIA.IElement _DescendantFromPoint(UIA.IElement eParent, Point pScreen, bool chromeWorkaround = false)
//{
//	var x = new _DescendantFromPointFinder(pScreen, chromeWorkaround);
//	x.Find(eParent);
//	return x.eFound;
//}








/// <summary>
/// Gets parent accessible object.
/// Uses <msdn>IAccessible.get_accParent</msdn>.
/// </summary>
/// <param name="disposeThis">Dispose this Acc variable (release the old COM object if need).</param>
/// <remarks>
/// Returns null if failed or there is no parent, for example if this is the root accessible object (<see cref="Wnd.Misc.WndRoot"/>). Supports <see cref="Native.GetError"/>.
/// </remarks>
public Acc Parent(bool disposeThis = false)
{
	if(Is0) throw new ObjectDisposedException(nameof(Acc));
	bool doNotRelease = false;
	try {
		IAccessible a = default;
		if(_elem != 0) {
			a = _iacc;
			if(disposeThis) doNotRelease = true; else a.AddRef();
		} else {
			var hr = _iacc.get_accParent(out a);
			if(hr != 0) {
				_Hresult(_FuncId.parent_object, hr);
				return null;
			}
		}
		return new Acc(a);
	}
	finally { if(disposeThis) _Dispose(doNotRelease: doNotRelease); }

	//rejected: option to replace fields of this Acc instead of creating new Acc.
	//	It can create problems. Eg if an EnumX callback does it, the EnumX may spin forever.

	//TODO: just call Navigate
}

//rejected: many objects don't implement get_accChild. Instead use Navigate, which calls AccessibleChildren. Also rarely used, unlike Parent.
///// <summary>
///// Gets a child accessible object.
///// Uses <msdn>IAccessible.get_accChild</msdn>.
///// </summary>
///// <param name="childIndex">1-based index of the child object.</param>
///// <param name="disposeThis">Dispose this Acc variable (release the old COM object if need).</param>
///// <remarks>
///// Returns null if fails, for example if childIndex is invalid. Supports <see cref="Native.GetError"/>.
///// </remarks>
//public Acc Child(int childIndex, bool disposeThis = false)
//{
//	if(Is0) throw new ObjectDisposedException(nameof(Acc));
//	bool doNotRelease = false;
//	try {
//		int hr;
//		if(_elem != 0) { hr = Api.E_INVALIDARG; goto ge; }
//		IAccessible a = default;
//		hr = _iacc.get_accChild(childIndex, out var idisp);
//		if(hr == 0) { //child IAccessible
//			hr = IAccessible.FromIDispatch(idisp, out a); if(hr != 0) goto ge;
//			childIndex = 0;
//		} else if(hr == 1) { //simple element of this IAccessible
//			a = _iacc;
//			if(disposeThis) doNotRelease = true; else a.AddRef();
//		} else goto ge;
//		return new Acc(a, childIndex);
//		ge:
//		_CheckHresult(_FuncId.child_object, hr);
//		return null;
//	}
//	finally { if(disposeThis) _Dispose(doNotRelease: doNotRelease); }
//}






static void _EnumChildren(IAccessible parent, bool allDescendants, Action<Args> f, Args args, int level)
{
#if true //slower
	using(var c = new _Children2(parent)) {
		while(c.Next(out var a, out var e)) {
			try {
				args.LibSetBeforeCallback(a, e, 0, level);
				f(args);
				if(args.stop) return;
				if(args.skipChildren) continue;
				if(!allDescendants) continue;
				if(e != 0) continue;
				_EnumChildren(a, true, f, args, level + 1);
				if(args.stop) return;
			}
			finally {
				if(e == 0) a.Dispose();
				args._iacc = default; //not necessary, but let it throw objectdisposedexception if the callback assigned Args to Acc and will try to use it (must use Args.ToAcc)
			}
		}
	}
#else
			using(var c = new _Children(parent)) {
				for(int i = 0; i < c.count; i++) {
					if(0 != parent.FromVARIANT(ref c.v[i], out var a, out int e)) continue;
					try {
						args.LibSetBeforeCallback(a, e, 0, level);
						f(args);
						if(args.stop) return;
						if(args.skipChildren) continue;
						if(!allDescendants) continue;
						if(e != 0) continue;
						_EnumChildren(a, true, f, args, level + 1);
						if(args.stop) return;
					}
					finally {
						if(e == 0) a.Dispose();
						args._iacc = default; //not necessary, but let it throw objectdisposedexception if the callback assigned Args to Acc and will try to use it (must use Args.ToAcc)
					}
				}
			}
#endif
}

/// <summary>
/// Gets direct children of an accessible object as VARIANT array.
/// Allocates the array, and frees in Dispose.
/// Uses API <msdn>AccessibleChildren</msdn>.
/// </summary>
struct _Children2 :IDisposable
{
	IAccessible _parent;
	IEnumVARIANT _ev;
	int _n;
	int _i;

	public _Children2(IAccessible parent) : this()
	{
		_parent = parent;
		int hr = Marshal.QueryInterface(_parent, ref IID_IEnumVARIANT, out var ip);
		if(hr == 0 && ip != default) {
			_ev = Unsafe.As<IEnumVARIANT>(Marshal.GetObjectForIUnknown(parent));
			Marshal.Release(ip);
			//Print("ok");
			//_ev.Reset(); //TODO: need this?
		} else if(0 == parent.get_accChildCount(out int n) && n > 0) { // in Firefox makes 10% slower
			_n = n;
		}
	}

	public void Dispose()
	{
		if(_ev != null) {
			Marshal.ReleaseComObject(_ev);
			_ev = null;
		}
	}

	public bool Next(out IAccessible a, out int e)
	{
		a = default; e = 0;
		if(_ev != null) {
			if(0 == _ev.Next(1, out var v, out int n) && n == 1) { //slower. Need array, to get multiple.
				if(0 == _parent.FromVARIANT(ref v, out a, out e)) {
					if(e == 0) return true;
					if(0 == _parent.get_accChild(e, out IntPtr di)) {
						if(0 != IAccessible.FromIDispatch(di, out a)) return false;
						e = 0;
					}
					return true;
				}
			}
		} else if(_i < _n) {
			if(0 == _parent.get_accChild(++_i, out IntPtr di)) {
				if(0 == IAccessible.FromIDispatch(di, out a)) return true;
			}
		}

		return false;
	}

	internal static Guid IID_IEnumVARIANT = new Guid(0x00020404, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

	[ComImport, Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IEnumVARIANT
	{
		[PreserveSig] int Next(int celt, out VARIANT rgVar, out int pCeltFetched);
		[PreserveSig] int Skip(int celt);
		[PreserveSig] int Reset();
		[PreserveSig] int Clone(out IEnumVARIANT ppEnum);
	}
}













#if true
public string ClassName
{
	get
	{
		const int stackSize = 260;
		var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
		int n = Api.GetClassName(this, b, stackSize);
		if(n > 0) return _String(b, n);
		return null;
	}
}
#elif true
		public string ClassName
		{
			get
			{
				const int stackSize = 260;
				var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
				int n = Api.GetClassName(this, b, stackSize);
				if(n > 0) return new string(b, 0, n);
				return null;
			}
		}
#elif true
		//This version uses a simple hwnd-classname cache. More info below #endif.
		public string ClassName
		{
			get
			{
				lock(_stringCache) { //slightly slower than concurrent dictionary, but easier and uses less memory
					if(!_stringCache.TryGetTarget(out var dict)) _stringCache.SetTarget(dict = new Dictionary<Wnd, _StringCacheEntry>());

					bool isCached = dict.TryGetValue(this, out var x);
					if(isCached && (Time.Milliseconds - x.time < 1500)) {
						if(IsAlive) return x.className;
						dict.Remove(this);
						return null;
					}

					const int stackSize = 260;
					var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
					int n = Api.GetClassName(this, b, stackSize);
					if(n == 0) {
						if(isCached) dict.Remove(this);
						return null;
					}

					var cn = x.className;
					if(isCached) isCached = Util.CharPtr.Equals(b, n, cn);
					if(!isCached) cn = new string(b, 0, n);

					dict[this] = new _StringCacheEntry() { className = cn, time = Time.Milliseconds };
					return cn;
				}
			}
		}

		struct _StringCacheEntry { public string className; public long time; }
		static WeakReference<Dictionary<Wnd, _StringCacheEntry>> _stringCache = new WeakReference<Dictionary<Wnd, _StringCacheEntry>>(null);
#elif true
		//This version uses a hwnd-classname cache that avoids creating duplicate strings.
		//For example, probably there are only ~80 unique classnames for 320 windows.
		//Rejected because uses about the same amount of memory.
		public string ClassName
		{
			get
			{
				lock(_stringCache) { //slightly slower than concurrent dictionary, but easier and uses less memory
					if(!_stringCache.TryGetTarget(out var cache)) _stringCache.SetTarget(cache = new _StringCache());

					bool isCached = cache.Dict.TryGetValue(this, out var x);
					string cn = isCached ? cache.ClassNames[x.className] : null;
					if(isCached && ((int)(Time.Milliseconds / 1000) - x.time < 3)) {
						if(IsAlive) return cn;
						cache.Dict.Remove(this);
						return null;
					}

					const int stackSize = 260;
					var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
					int n = Api.GetClassName(this, b, stackSize);
					if(n == 0) {
						if(isCached) cache.Dict.Remove(this);
						return null;
					}

					if(isCached) isCached = Util.CharPtr.Equals(b, n, cn);
					if(!isCached) cn = new string(b, 0, n);

					cache.AddOrUpdate(this, cn, isCached ? x.className : -1);
					//PrintList(cache.Dict.Count, cache.ClassNames.Count);
					return cn;
				}
			}
		}

		struct _StringCacheEntry { public int className; public int time; }
		static WeakReference<_StringCache> _stringCache = new WeakReference<_StringCache>(null);

		class _StringCache
		{
			public Dictionary<Wnd, _StringCacheEntry> Dict;
			public List<string> ClassNames;

			public _StringCache()
			{
				Dict = new Dictionary<Wnd, _StringCacheEntry>(100);
				ClassNames = new List<string>(100);
			}

			public void AddOrUpdate(Wnd w, string className, int stringIndex)
			{
				if(stringIndex < 0) {
					int len = className.Length; char c0 = className[0];
					for(int i = 0; i < ClassNames.Count; i++) {
						var v = ClassNames[i];
						if(v.Length == len && v[0] == c0 && v == className) { stringIndex = i; break; }
					}
					//for ClassNames can instead use Dictionary<int, string> and its ContainsValue. But is slow etc.
					if(stringIndex < 0) {
						stringIndex = ClassNames.Count;
						ClassNames.Add(className);
					}
				}

				var x = new _StringCacheEntry() { className = stringIndex, time = (int)(Time.Milliseconds / 1000) };
				Dict[w] = x;
			}
		}
#elif true
		//This version uses a simplest hwnd-classname cache that does not make faster.
		public string ClassName
		{
			get
			{
				lock(_stringCache) { //slightly slower than concurrent dictionary, but easier and uses less memory
					if(!_stringCache.TryGetTarget(out var dict)) _stringCache.SetTarget(dict = new Dictionary<Wnd, _StringCacheEntry>());
					bool isCached = dict.TryGetValue(this, out var x);

					const int stackSize = 260;
					var b = stackalloc char[stackSize]; //tested: same speed with Util.Buffers
					int n = Api.GetClassName(this, b, stackSize);
					if(n == 0) {
						if(isCached) dict.Remove(this);
						return null;
					}

					var cn = x.className;
					if(isCached && Util.CharPtr.Equals(b, n, cn)) return cn;

					x.className = cn = new string(b, 0, n);
					dict[this] = x;
					return cn;
				}
			}
		}

		struct _StringCacheEntry { public string className; }
		static WeakReference<Dictionary<Wnd, _StringCacheEntry>> _stringCache = new WeakReference<Dictionary<Wnd, _StringCacheEntry>>(null);
#endif
//The hwnd-classname cache allows to:
//	1. Avoid creating megabytes of garbage in seconds when calling frequently (eg while waiting for a window).
//	2. Get classname much faster when calling frequently.
//Tested how soon OS recycles window handles when creating-destroying windows in a loop:
//	Message-only window: ~9 s, ~65000 handles.
//	Normal hidden popup window: ~5 minutes, ~95000 handles (it seems it depends on the time).
//There is no API to get window creation time.
//Tried to measure how long a busy thread is suspended when GC runs. It seems about 250 mcs. Not bad. But the PC is fast, has CPU with 4 logical CPU. Need to test on a 1-CPU PC.
//rejected: use cache for Name and ProcessName. Maybe in the future, if will notice that GC is a problem.
//	Name: 1. Can use only the simplest version (without time), which does not make faster. 2. Names usually create less garbage, because most windows are nameless.
//	ProcessName: 1. Not so easy. 2. Not so often used.
//On my PC normally there are about 350 top-level windows, 1000 total windows, 180 unique class names.
//	It means need 16 KB for the array if searching for a control in all windows (hidden too).




//this version can use 1.5 byte for xy. Rejected because more difficult and saves just ~3% of string length.
//public static string EncodeMultipleRecordedMoves(IEnumerable<uint> moves)
//{
//	var a = new List<byte>();
//	a.Add(0);

//	int pdx = 0, pdy = 0, fourBits = -1;
//	foreach(var t in moves) {
//		int dx = Calc.LoShort(t), x = dx - pdx; pdx = dx;
//		int dy = Calc.HiShort(t), y = dy - pdy; pdy = dy;

//		int nbits = 8;
//		if(x < -64 || x > 63 || y < -64 || y > 63) nbits = 32; //~0%
//		else if(x < -16 || x > 15 || y < -16 || y > 15) nbits = 16; //~1%
//		else if(x < -4 || x > 3 || y < -4 || y > 3) nbits = 12; //~10%

//		int v, mask, yshift;
//		switch(nbits) {
//		case 8: v = 0; mask = 0x7; yshift = 5; break;
//		case 12: v = 0x1; mask = 0x1f; yshift = 7; break;
//		case 16: v = 0x2; mask = 0x7f; yshift = 9; break;
//		default: v = 0x3; mask = 0x7fff; yshift = 17; break;
//		}
//		v |= ((x & mask) << 2) | ((y & mask) << yshift);

//		if(fourBits >= 0) {
//			a.Add((byte)((fourBits & 0xf) | ((v & 0xf) << 4)));
//			v >>= 4;
//			nbits -= 4;
//		}

//		switch(nbits) {
//		case 4:
//			fourBits = (byte)v;
//			break;
//		case 8:
//			a.Add((byte)v);
//			fourBits = -1;
//			break;
//		case 12:
//			a.Add((byte)v);
//			fourBits = (byte)(v >> 8);
//			break;
//		case 16:
//			a.Add((byte)v); a.Add((byte)(v >> 8));
//			break;
//		default:
//			a.Add((byte)v); a.Add((byte)(v >> 8)); a.Add((byte)(v >> 16));
//			v >>= 24;
//			if(nbits == 32) a.Add((byte)v); else fourBits = (byte)v;
//			break;
//		}
//	}
//	if(fourBits >= 0) a.Add((byte)(fourBits & 0xf));

//	return null;
//}



#if false //initially was used by SharedMemory
	/// <summary>
	/// Manages a named kernel handle (mutex, event, memory mapping, etc).
	/// Normally calls CloseHandle when dies or is called Close.
	/// But does not call CloseHandle for the variable that uses the name first time in current process.
	/// Therefore the kernel object survives, even when the first appdomain ends.
	/// It ensures that all variables in all appdomains will use the same kernel object (although different handle to it) if they use the same name.
	/// Most CreateX API work in "create or open" way. Pass such a created-or-opened object handle to the constructor.
	/// </summary>
	[DebuggerStepThrough]
	class LibInterDomainHandle
	{
		IntPtr _h;
		bool _noClose;

		public IntPtr Handle { get => _h; }

		/// <param name="handle">Kernel object handle</param>
		/// <param name="name">Kernel object name. Note: this function adds local atom with that name.</param>
		public LibInterDomainHandle(IntPtr handle, string name)
		{
			_h = handle;

			if(_h != Zero && 0 == Api.FindAtom(name)) {
				Api.AddAtom(name);
				_noClose = true;
			}
		}

		~LibInterDomainHandle() { Close(); }

		public void Close()
		{
			if(_h != Zero && !_noClose) { Api.CloseHandle(_h); _h = Zero; }
		}
	}
#endif




/// <summary>
/// Memory that can be used by multiple processes.
/// Wraps Api.CreateFileMapping(), Api.MapViewOfFile().
/// Faster and more "unsafe" than System.IO.MemoryMappedFiles.MemoryMappedFile.
/// </summary>
[DebuggerStepThrough]
public unsafe class SharedMemory
{
	void* _mem;
	LibInterDomainHandle _hmap;

	/// <summary>
	/// Pointer to the base of the shared memory.
	/// </summary>
	public void* mem { get => _mem; }

	/// <summary>
	/// Creates shared memory of specified size. Opens if already exists.
	/// Calls API <msdn>CreateFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="size"></param>
	/// <exception cref="Win32Exception">When fails.</exception>
	/// <remarks>
	/// Once the memory is created, it is alive until this process (not variable or appdomain) dies, even if you call Close.
	/// All variables in all appdomains will get the same physical memory for the same name, but they will get different virtual address.
	/// </remarks>
	public SharedMemory(string name, uint size)
	{
		IntPtr hm = Api.CreateFileMapping((IntPtr)(~0), Zero, 4, 0, size, name);
		if(hm != Zero) {
			_mem = Api.MapViewOfFile(hm, 0x000F001F, 0, 0, 0);
			if(_mem == null) Api.CloseHandle(hm); else _hmap = new LibInterDomainHandle(hm, name);
		}
		if(_mem == null) throw new Win32Exception();
		//todo: option to use SECURITY_ATTRIBUTES to allow low IL processes open the memory.
		//todo: use single handle/address for all appdomains.
		//PrintList(_hmap, _mem);
	}

	//This works but not useful.
	///// <summary>
	///// Opens shared memory.
	///// Calls API <msdn>OpenFileMapping</msdn> and API <msdn>MapViewOfFile</msdn>.
	///// </summary>
	///// <param name="name"></param>
	///// <exception cref="Win32Exception">When fails, eg the memory does not exist.</exception>
	//public SharedMemory(string name)
	//{
	//	_hmap = Api.OpenFileMapping(0x000F001F, false, name);
	//	if(_hmap != Zero) {
	//		_mem = Api.MapViewOfFile(_hmap, 0x000F001F, 0, 0, 0);
	//	}
	//	if(_mem == Zero) throw new Win32Exception();
	//}

	~SharedMemory() { if(_mem != null) Api.UnmapViewOfFile(_mem); }

	/// <summary>
	/// Unmaps the memory.
	/// D
	/// </summary>
	public void Close()
	{
		if(_mem != null) { Api.UnmapViewOfFile(_mem); _mem = null; }
		if(_hmap != null) { _hmap.Close(); _hmap = null; }
	}
}



//Almost complete. Need just to implement screen and EndThread option. Now some used library functions deleted/moved/renamed/etc.
//Instead use TaskDialog. If need classic message box, use MessageBox.Show(). Don't need 3 functions for the same.
#region MessageDialog

/// <summary>
/// MessageDialog return value (user-clicked button).
/// </summary>
public enum MDResult
{
	OK = 1, Cancel = 2, Abort = 3, Retry = 4, Ignore = 5, Yes = 6, No = 7/*, Timeout = 9*/, TryAgain = 10, Continue = 11,
}

/// <summary>
/// MessageDialog buttons.
/// </summary>
public enum MDButtons
{
	OK = 0, OKCancel = 1, AbortRetryIgnore = 2, YesNoCancel = 3, YesNo = 4, RetryCancel = 5, CancelTryagainContinue = 6,
}

/// <summary>
/// MessageDialog icon.
/// </summary>
public enum MDIcon
{
	None = 0, Error = 0x10, Question = 0x20, Warning = 0x30, Info = 0x40, Shield = 0x50, App = 0x60,
}

/// <summary>
/// MessageDialog flags.
/// </summary>
[Flags]
public enum MDFlag :uint
{
	DefaultButton2 = 0x100, DefaultButton3 = 0x200, DefaultButton4 = 0x300,
	SystemModal = 0x1000, DisableThreadWindows = 0x2000, HelpButton = 0x4000,
	TryActivate = 0x10000, DefaultDesktopOnly = 0x20000, Topmost = 0x40000, RightAlign = 0x80000, RtlLayout = 0x100000, ServiceNotification = 0x200000,
	//not API flags
	NoSound = 0x80000000,
	//todo: EndThread.
}

public static class MessageDialog
{
	/// <summary>
	/// Shows classic message box dialog.
	/// Like System.Windows.Forms.MessageBox.Show but has more options and is always-on-top by default.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="buttons">Example: MDButtons.YesNo.</param>
	/// <param name="icon">One of standard icons. Example: MDIcon.Info.</param>
	/// <param name="flags">One or more options. Example: MDFlag.NoTopmost|MDFlag.DefaultButton2.</param>
	/// <param name="owner">Owner window or null.</param>
	/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
	/// </remarks>
	public static MDResult Show(string text, MDButtons buttons, MDIcon icon = 0, MDFlag flags = 0, IWin32Window owner = null, string title = null)
	{
		//const uint MB_SYSTEMMODAL = 0x1000; //same as MB_TOPMOST + adds system icon in title bar (why need it?)
		const uint MB_USERICON = 0x80;
		const uint IDI_APPLICATION = 32512;
		const uint IDI_ERROR = 32513;
		const uint IDI_QUESTION = 32514;
		const uint IDI_WARNING = 32515;
		const uint IDI_INFORMATION = 32516;
		const uint IDI_SHIELD = 106; //32x32 icon. The value is undocumented, but we use it instead of the documented IDI_SHIELD value which on Win7 displays clipped 128x128 icon. Tested: the function does not fail with invalid icon resource id.

		var p = new MSGBOXPARAMS();
		p.cbSize = Api.SizeOf(p);
		p.lpszCaption = _Util.Title(title);
		p.lpszText = text;

		Wnd ow =
		bool alien = (flags & (MDFlag.DefaultDesktopOnly | MDFlag.ServiceNotification)) != 0;
		if(alien) owner = Wnd0; //API would fail. The dialog is displayed in csrss process.

		if(icon == MDIcon.None) { } //no sound
		else if(icon == MDIcon.Shield || icon == MDIcon.App || flags.HasFlag(MDFlag.NoSound)) {
			switch(icon) {
			case MDIcon.Error: p.lpszIcon = (IntPtr)IDI_ERROR; break;
			case MDIcon.Question: p.lpszIcon = (IntPtr)IDI_QUESTION; break;
			case MDIcon.Warning: p.lpszIcon = (IntPtr)IDI_WARNING; break;
			case MDIcon.Info: p.lpszIcon = (IntPtr)IDI_INFORMATION; break;
			case MDIcon.Shield: p.lpszIcon = (IntPtr)IDI_SHIELD; break;
			case MDIcon.App:
				p.lpszIcon = (IntPtr)IDI_APPLICATION;
				if(Util.Misc.GetAppIconHandle(32) != Zero) p.hInstance = Util.Misc.GetModuleHandleOfAppdomainEntryAssembly();
				//info: C# compiler adds icon to the native resources as IDI_APPLICATION.
				//	If assembly without icon, we set hInstance=0 and then the API shows common app icon.
				//	In any case, it will be the icon displayed in File Explorer etc.
				break;
			}
			p.dwStyle |= MB_USERICON; //disables sound
			icon = 0;
		}

		if(Script.Option.dialogRtlLayout) flags |= MDFlag.RtlLayout;
		if(owner.Is0) {
			flags |= MDFlag.TryActivate; //if foreground lock disabled, activates, else flashes taskbar button; without this flag the dialog woud just sit behind other windows, often unnoticed.
			if(Script.Option.dialogTopmostIfNoOwner) flags |= MDFlag.SystemModal; //makes topmost, always works, but also adds an unpleasant system icon in title bar
																				  //if(Script.Option.dialogTopmostIfNoOwner) flags|=MDFlag.Topmost; //often ignored, without a clear reason and undocumented, also noticed other anomalies
		}
		//tested: if owner is child, the API disables its top-level parent.
		//consider: if owner 0, create hidden parent window to:
		//	Avoid adding taskbar icon.
		//	Apply Option.dialogScreenIfNoOwner.
		//consider: if owner 0, and current foreground window is of this thread, let it be owner. Maybe a flag.
		//consider: if owner of other thread, don't disable it. But how to do it without hook? Maybe only inherit owner's monitor.
		//consider: play user-defined sound.

		p.hwndOwner = owner;

		flags &= ~(MDFlag.NoSound); //not API flags
		p.dwStyle |= (uint)buttons | (uint)icon | (uint)flags;

		int R = MessageBoxIndirect(ref p);
		if(R == 0) throw new AuException();

		_Util.DoEventsAndWaitForAnActiveWindow();

		return (MDResult)R;

		//tested:
		//user32:MessageBoxTimeout. Undocumented. Too limited etc to be useful. If need timeout, use TaskDialog.
		//shlwapi:SHMessageBoxCheck. Too limited etc to be useful.
		//wtsapi32:WTSSendMessageW. In csrss process, no themes, etc. Has timeout.
	}

	/// <summary>
	/// Shows classic message box dialog.
	/// Returns clicked button's character (as in style), eg 'O' for OK.
	/// You can specify buttons etc in style string, which can contain:
	/// <para>Buttons: OC OKCancel, YN YesNo, YNC YesNoCancel, ARI AbortRetryIgnore, RC RetryCancel, CTE CancelTryagainContinue.</para>
	/// <para>Icon: x error, ! warning, i info, ? question, v shield, a app.</para>
	/// <para>Flags: s no sound, t topmost, d disable windows.</para>
	/// <para>Default button: 2 or 3.</para>
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="style">Example: "YN!".</param>
	/// <param name="owner">Owner window or null.</param>
	/// <param name="title">Title bar text. If omitted, null or "", uses ScriptOptions.DisplayName (default is appdomain name).</param>
	/// <remarks>
	/// These script options are applied: Script.Option.dialogRtlLayout, Script.Option.dialogTopmostIfNoOwner, ScriptOptions.DisplayName (title).
	/// </remarks>
	public static char Show(string text, string style = null, IWin32Window owner = null, string title = null)
	{
		MDButtons buttons = 0;
		MDIcon icon = 0;
		MDFlag flags = 0;

		if(!Empty(style)) {
			if(style.Contains("OC")) buttons = MDButtons.OKCancel;
			else if(style.Contains("YNC")) buttons = MDButtons.YesNoCancel;
			else if(style.Contains("YN")) buttons = MDButtons.YesNo;
			else if(style.Contains("ARI")) buttons = MDButtons.AbortRetryIgnore;
			else if(style.Contains("RC")) buttons = MDButtons.RetryCancel;
			else if(style.Contains("CT")) buttons = MDButtons.CancelTryagainContinue; //not CTC, because Continue returns E

			if(style.Contains("x")) icon = MDIcon.Error;
			else if(style.Contains("?")) icon = MDIcon.Question;
			else if(style.Contains("!")) icon = MDIcon.Warning;
			else if(style.Contains("i")) icon = MDIcon.Info;
			else if(style.Contains("v")) icon = MDIcon.Shield;
			else if(style.Contains("a")) icon = MDIcon.App;

			if(style.Contains("t")) flags |= MDFlag.SystemModal; //MDFlag.Topmost often ignored etc
			if(style.Contains("s")) flags |= MDFlag.NoSound;
			if(style.Contains("d")) flags |= MDFlag.DisableThreadWindows;

			if(style.Contains("2")) flags |= MDFlag.DefaultButton2;
			else if(style.Contains("3")) flags |= MDFlag.DefaultButton3;
		}

		int r = (int)Show(text, buttons, icon, flags, owner, title);

		return (r > 0 && r < 12) ? "COCARIYNCCTE"[r] : 'C';
	}

	struct MSGBOXPARAMS
	{
		public uint cbSize;
		public Wnd hwndOwner;
		public IntPtr hInstance;
		public string lpszText;
		public string lpszCaption;
		public uint dwStyle;
		public IntPtr lpszIcon;
		public LPARAM dwContextHelpId;
		public IntPtr lpfnMsgBoxCallback;
		public uint dwLanguageId;
	}

	[DllImport("user32.dll", EntryPoint = "MessageBoxIndirectW")]
	static extern int MessageBoxIndirect([In] ref MSGBOXPARAMS lpMsgBoxParams);

}
#endregion MessageDialog






public partial class Files
{

	/// <summary>
	/// Gets shell icon of a file or protocol etc where SHGetFileInfo would fail.
	/// Also can get icons of sizes other than 16 or 32.
	/// Cannot get file extension icons.
	/// If pidl is not Zero, uses it and ignores file, else uses file.
	/// Returns Zero if failed.
	/// </summary>
	[HandleProcessCorruptedStateExceptions]
	static unsafe IntPtr _Icon_GetSpec(string file, IntPtr pidl, int size)
	{
		IntPtr R = Zero;
		bool freePidl = false;
		Api.IShellFolder folder = null;
		Api.IExtractIcon eic = null;
		try { //possible exceptions in shell32.dll or in shell extensions
			if(pidl == Zero) {
				pidl = Misc.PidlFromString(file);
				if(pidl == Zero) return Zero;
				freePidl = true;
			}

			IntPtr pidlItem;
			int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
			if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }

			object o;
			hr = folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o);
			//if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			if(0 != hr) {
				if(hr == Api.REGDB_E_CLASSNOTREG) return Zero;
				PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}");
				return Zero;
			}
			eic = o as Api.IExtractIcon;

			var sb = new StringBuilder(300); int ii; uint fl;
			hr = eic.GetIconLocation(0, sb, 300, out ii, out fl);
			if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			string loc = sb.ToString();

			if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) != 0 || 1 != Api.PrivateExtractIcons(loc, ii, size, size, out R, Zero, 1, 0)) {
				IntPtr* hiSmall = null, hiBig = null;
				if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
				hr = eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16));
				if(0 != hr) { PrintDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			}
		}
		catch(Exception e) { PrintDebug($"Exception in _Icon_GetSpec: {file}, {e.Message}, {e.TargetSite}"); }
		finally {
			if(eic != null) Marshal.ReleaseComObject(eic);
			if(folder != null) Marshal.ReleaseComObject(folder);
			if(freePidl) Marshal.FreeCoTaskMem(pidl);
		}
		return R;
	}

}
}






//-------------------------------------------------------------------------- 
//  
//  Copyright (c) Microsoft Corporation.  All rights reserved.  
//  
//  File: StaTaskScheduler.cs 
// 
//-------------------------------------------------------------------------- 

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace System.Threading.Tasks.Schedulers
{
	/// <summary>Provides a scheduler that uses STA threads.</summary> 
	public sealed class StaTaskScheduler :TaskScheduler, IDisposable
	{
		/// <summary>Stores the queued tasks to be executed by our pool of STA threads.</summary> 
		private BlockingCollection<Task> _tasks;
		/// <summary>The STA threads used by the scheduler.</summary> 
		private readonly List<Thread> _threads;

		/// <summary>Initializes a new instance of the StaTaskScheduler class with the specified concurrency level.</summary> 
		/// <param name="numberOfThreads">The number of threads that should be created and used by this scheduler.</param> 
		public StaTaskScheduler(int numberOfThreads)
		{
			// Validate arguments 
			if(numberOfThreads < 1) throw new ArgumentOutOfRangeException("concurrencyLevel");

			// Initialize the tasks collection 
			_tasks = new BlockingCollection<Task>();

			// Create the threads to be used by this scheduler 
			_threads = Enumerable.Range(0, numberOfThreads).Select(i =>
			{
				var thread = new Thread(() =>
				{
					// Continually get the next task and try to execute it. 
					// This will continue until the scheduler is disposed and no more tasks remain. 
					foreach(var t in _tasks.GetConsumingEnumerable()) {
						TryExecuteTask(t);
					}
				});
				thread.IsBackground = true;
				thread.SetApartmentState(ApartmentState.STA);
				//thread.Priority = ThreadPriority.Lowest; //perhaps makes the UI thread slightly faster, but sometimes may slow down icon display too much
				return thread;
			}).ToList();

			// Start all of the threads 
			_threads.ForEach(t => t.Start());
		}

		/// <summary>Queues a Task to be executed by this scheduler.</summary> 
		/// <param name="task">The task to be executed.</param> 
		protected override void QueueTask(Task task)
		{
			// Push it into the blocking collection of tasks 
			_tasks.Add(task);
		}

		/// <summary>Provides a list of the scheduled tasks for the debugger to consume.</summary> 
		/// <returns>An enumerable of all tasks currently scheduled.</returns> 
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			// Serialize the contents of the blocking collection of tasks for the debugger 
			return _tasks.ToArray();
		}

		/// <summary>Determines whether a Task may be inlined.</summary> 
		/// <param name="task">The task to be executed.</param> 
		/// <param name="taskWasPreviouslyQueued">Whether the task was previously queued.</param> 
		/// <returns>true if the task was successfully inlined; otherwise, false.</returns> 
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			// Try to inline if the current thread is STA 
			return
				Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
				TryExecuteTask(task);
		}

		/// <summary>Gets the maximum concurrency level supported by this scheduler.</summary> 
		public override int MaximumConcurrencyLevel
		{
			get => _threads.Count;
		}

		/// <summary> 
		/// Cleans up the scheduler by indicating that no more tasks will be queued. 
		/// This method blocks until all threads successfully shutdown. 
		/// </summary> 
		public void Dispose()
		{
			if(_tasks != null) {
				// Indicate that no new tasks will be coming in 
				_tasks.CompleteAdding();

				// Wait for all threads to finish processing tasks 
				foreach(var thread in _threads) thread.Join();

				// Cleanup 
				_tasks.Dispose();
				_tasks = null;
			}
		}
	}
}


