//Misc small classes and structs.

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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Inter-domain variables. Allows to share values by all app domains of this process.
	/// Similar to environment variables, but supports more types and does not inherit parent process variables; the speed is similar.
	/// Supports not all types. The type must have one of these properties:
	/// 1. Derived from MarshalByRefObject. The stored value is a virtual reference (proxy), not a copy. The owner domain must be still alive when other domains access (call methods etc) the retrieved value, else AppDomainUnloadedException.
	/// 2. Has [Serializable] attribute. The stored value is a copy, not reference. For example int, IntPtr, string, arrays, List, Dictionary.
	/// </summary>
	[DebuggerStepThrough]
	public static class InterDomain
	{
#if true
		/// <summary>
		/// Adds or modifies an inter-domain variable.
		/// </summary>
		/// <param name="name">Name. Can be any unique string, for example a GUID string. Case-sensitive.</param>
		/// <param name="value">Value.</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The type is not supported because is neither serializable nor MarshalByRefObject-derived.</exception>
		public static void SetVariable(string name, object value)
		{
			Util.AppDomain_.GetDefaultDomain().SetData("Catkeys\x5" + name, value);
		}

		/// <summary>
		/// Gets the value of an inter-domain variable.
		/// Returns null if the variable does not exist. Else returns the value as object; need a cast.
		/// </summary>
		/// <param name="name">Name. Case-sensitive.</param>
		public static object GetVariable(string name)
		{
			return Util.AppDomain_.GetDefaultDomain().GetData("Catkeys\x5" + name);
		}
#else
		//Tried to make faster than AppDomain.GetData/SetData, but same speed, and first time much slower.
		//Uses similar code, ie MarshalByRefObject, just less unnecessary code, but it seems the unnecessary code is not the slowest part.
		//What is better:
		//	1. Maybe it is not good if library users add hundreds of AppDomain properties. But not dangerous because we add a prefix string.
		//	2. Supports Remove(). The above code can set value to null but it does not remove the variable from the dictionary.
		//What is bad:
		//	1. Much slower first time in domain, even if ngened, don't know why.
		//	2. More code.

		/// <summary>
		/// Adds or modifies an inter-domain variable.
		/// </summary>
		/// <param name="name">Name. Can be any unique string, for example a GUID string. Case-sensitive.</param>
		/// <param name="value">Value.</param>
		/// <exception cref="SerializationException">The type is not supported because is neither serializable nor MarshalByRefObject-derived.</exception>
		public static void SetVariable(string name, object value)
		{
			_Mbr.Set(name, value);
		}

		/// <summary>
		/// Gets the value of an inter-domain variable.
		/// Returns null if the variable does not exist. Else returns the value as object; need to cast it to the real type.
		/// </summary>
		/// <param name="name">Name. Case-sensitive.</param>
		public static object GetVariable(string name)
		{
			return _Mbr.Get(name);
		}

		//note: could support null values, but it somehow makes much slower, maybe because need more marshaling.
		//Eg tried 'bool _MBR.TryGet(name, out value)', 'STRUCT _MBR.TryGet(name)'.
		//Also, generic methods in _MBR make many times slower.

		/// <summary>
		/// Removes an inter-domain variable.
		/// Returns true if existed.
		/// </summary>
		/// <param name="name">Name. Case-sensitive.</param>
		public static bool Remove(string name)
		{
			return _Mbr.Remove(name);
		}

		//If called from default appdomain, gets the true _MBR object, else gets its proxy.
		//note: this is old code. Use _ObjectInDefaultDomain instead. It is faster.
		static _MBR _Mbr
		{
			get
			{
				if(_mbr == null) {
					lock ("r1+6n6nPoEeix2QbYV0n+Q") {
						if(_mbr == null) {
							var dd = Util.AppDomain_.GetDefaultDomain(out bool isThisDomainDefault);
							_mbr = dd.GetData("Catkeys_InterDomain") as _MBR;
							if(_mbr == null) {
								if(isThisDomainDefault) _Init();
								else {
									dd.DoCallBack(_Init);
									_mbr = dd.GetData("Catkeys_InterDomain") as _MBR;
		#region }}}}}
								}
							}
						}
					}
				}
		#endregion
				return _mbr;
			}
		}

		static _MBR _mbr;

		static void _Init()
		{
			_mbr = new _MBR();
			AppDomain.CurrentDomain.SetData("Catkeys_InterDomain", _mbr);
		}

		class _MBR :MarshalByRefObject
		{
			Dictionary<string, object> _a = new Dictionary<string, object>();
			//System.Collections.Hashtable _a=new System.Collections.Hashtable(); //same speed; does not support null values.

			public void Set(string name, object value)
			{
				_a[name] = value;
			}

			public object Get(string name)
			{
				object r;
				if(_a.TryGetValue(name, out r)) return r;
				return null;
			}

			public bool Remove(string name)
			{
				return _a.Remove(name);
			}
		}
#endif
		/// <summary>
		/// Gets the value of an inter-domain variable.
		/// Returns true if the variable exists and is not null.
		/// </summary>
		/// <param name="name">Name. Case-sensitive.</param>
		/// <param name="value">Receives the value. If the variable does not exist, receives the default value of that type.</param>
		/// <exception cref="InvalidCastException">Bad value type.</exception>
		public static bool GetVariable<T>(string name, out T value)
		{
			object o = GetVariable(name);
			if(o == null) {
				value = default(T);
				return false;
			}
			value = (T)o;
			return true;
		}

		/// <summary>
		/// Gets the value of an inter-domain variable.
		/// If the variable does not exist or is null, calls a callback function that provides an initial value; then sets the variable = the initial value.
		/// Thread-safe.
		/// </summary>
		/// <param name="name">Name. Case-sensitive.</param>
		/// <param name="initValue">A callback function delegate (eg lambda) that is called when the variable does not exist or is null. It must return an initial value.</param>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The type is not supported because is neither serializable nor MarshalByRefObject-derived.</exception>
		/// <exception cref="InvalidCastException">Bad value type.</exception>
		/// <example><code>int test = InterDomain.GetVariable("test", () => 10);</code></example>
		public static T GetVariable<T>(string name, Func<T> initValue)
		{
			lock("5WzrQPTnqUWvCTbo1GUTsA") {
				object o = GetVariable(name);
				if(o != null) return (T)o;
				T x = initValue();
				SetVariable(name, x);
				return x;
			}
		}

		/// <summary>
		/// Gets a reference to an object that lives in default appdomain. Auto-creates new object there if does not exist.
		/// The object can be used in any appdomain. When a non-default domain calls its methods, the call is marshaled to the default domain and executed there.
		/// The type must be derived from MarshalByRefObject and have default constructor.
		/// Returns true if created new object, false if it already existed.
		/// Thread-safe.
		/// </summary>
		/// <param name="name">Inter-domain variable name. Any unique string. Case-sensitive.</param>
		/// <param name="value">Receives the reference.</param>
		public static bool DefaultDomainVariable<T>(string name, out T value) where T : MarshalByRefObject, new()
		{
			lock("5WzrQPTnqUWvCTbo1GUTsA") {
				object o = GetVariable(name);
				if(o != null) { value = (T)o; return false; }

				var d = Util.AppDomain_.GetDefaultDomain(out bool isThisDomainDefault);
				if(isThisDomainDefault) {
					value = new T();
				} else {
					//d.SetData("Catkeys_DefDomVar", name);
					//d.DoCallBack(() =>
					//{
					//	SetVariable(AppDomain.CurrentDomain.GetData("Catkeys_DefDomVar") as string, new T());
					//});
					//value = (T)GetVariable(name); return true;
					//speed: 1291  179  151  (1622)

					var t = typeof(T);
					value = (T)d.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
					//speed:  791  87  65  (945)

					//CallInDefaultDomain.SetDefaultDomainVariable(name, out value);
					//speed: much slower.

					//CallInDefaultDomain.SetDefaultDomainVariable<T>(name);
					//value = (T)GetVariable(name); return true;
					//speed:  835  97  87  (1020), + 50 for Init called in default domain

					//value = CallInDefaultDomain.SetDefaultDomainVariable<T>(name); return true;
					//speed:  838  92  78  (1010), + 50 for Init called in default domain

					//value = (T)CallInDefaultDomain.SetDefaultDomainVariable<T>(name); return true;
					//speed:  814  89  75  (980), + 50 for Init called in default domain

					//value = (T)CallInDefaultDomain.SetDefaultDomainVariable(typeof(T));
					//speed:  884  86  69  (1042), + 50 for Init called in default domain

					//var t = typeof(T);
					//value = (T)CallInDefaultDomain.SetDefaultDomainVariable(t.Assembly.FullName, t.FullName);
					//speed:  843  72  62  (979), + 50 for Init called in default domain
					//speed:  928  68  59  (1057), without Init

					//var t = typeof(T);
					//value = (T)CallInDefaultDomain.SetDefaultDomainVariable(t.Assembly.FullName, t.FullName).Unwrap();
					//speed:  905  102  85  (1095)

					//var t = typeof(T);
					//value =(T)Activator.CreateInstance(d, t.Assembly.FullName, t.FullName).Unwrap();
					//speed:  865  115  93  (1075)
					//info: some CreateInstance overloads can call constructor with parameters.

					//speed measured ngened. The type was in this assembly. All ~2 times slower if in another ngened or not-ngened assembly.
				}
				SetVariable(name, value);
				return true;
			}

			//note: could use Func<T> initValue instead of out T value. The function could create and return new object.
			//But it is impossible because delegates cannot be marshaled to other domain and executed here.
		}

		internal static LibObjectInDefaultDomain CallInDefaultDomain
		{
			get
			{
				_Init();
				return _inDD;
			}
		}
		static LibObjectInDefaultDomain _inDD;

		//public //could be called from default appdomain. But it saves < 100 mcs. Now we always auto-init on demand.
		static void _Init()
		{
			if(_inDD == null) {
				var d = Util.AppDomain_.GetDefaultDomain(out bool isThisDomainDefault);
				_inDD = d.GetData("Catkeys_InterDomain") as LibObjectInDefaultDomain;
				if(_inDD == null) {
					if(isThisDomainDefault) {
						_inDD = new LibObjectInDefaultDomain();
					} else {
						var t = typeof(LibObjectInDefaultDomain);
						_inDD = (LibObjectInDefaultDomain)d.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
					}
					d.SetData("Catkeys_InterDomain", _inDD);
				}
			}
		}

		//This can be used to execute methods in default appdomain. Unlike with AppDomain.DoCallback, we can have parameters. Also slightly faster. Unlike with AppDomain.CreateInstanceAndUnwrap or Activator.CreateInstance, don't need to create a MarshalByRefObject-derived class for each such method.
		//When will need it, add methods in this class and call through CallInDefaultDomain.
		//note: be careful with the number and type of parameters, as it can make much slower. Eg out parameters make several times slower.
		internal sealed class LibObjectInDefaultDomain :MarshalByRefObject
		{
			//This is the fastest, almost like AppDomain.CreateInstanceAndUnwrap.
			//public object SetDefaultDomainVariable(string assemblyName, string typeName)
			//{
			//	return Activator.CreateInstance(assemblyName, typeName).Unwrap();
			//}
			//public object SetDefaultDomainVariable(Type t)
			//{
			//	return Activator.CreateInstance(t);
			//}
			//public ObjectHandle SetDefaultDomainVariable(string assemblyName, string typeName)
			//{
			//	return Activator.CreateInstance(assemblyName, typeName);
			//         }
			//public void SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//}
			//public T SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//	return x;
			//}
			//public object SetDefaultDomainVariable<T>(string name) where T : MarshalByRefObject, new()
			//{
			//	var x = new T();
			//	SetVariable(name, x);
			//	return x;
			//}
			//public void SetDefaultDomainVariable<T>(string name, out T value) where T : MarshalByRefObject, new()
			//{
			//	value = new T();
			//}
		}
	}
}
