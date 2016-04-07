using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public partial struct Wnd
	{
		#region find

		/// <summary>
		/// Finds window and returns its handle as Wnd.
		/// Can be used like <c>Wnd w=Wnd.Find("Name");</c>
		/// </summary>
		public static Wnd Find(string name)
		{
			return Api.FindWindow(null, name);
			// return Wnd0;
		}

		/// <summary>
		/// Finds first or n-th child control that has the specified class name.
		/// </summary>
		/// <param name="hwnd">Parent window (top-level or control).</param>
		/// <param name="className">Class name. Case-insensitive, wildcard (uses String.LikeI_()).</param>
		/// <param name="directChild">Must be direct child, not a grandchild.</param>
		/// <param name="matchIndex">1-based match index. For example, if 2, will get the second matching control.</param>
		public static Wnd FindControlOfClass(Wnd w, string className, bool directChild = false, int matchIndex = 1)
		{
			bool wild = _IsWildcard(className);
			Wnd R = Wnd0;

			Api.WNDENUMPROC ep = delegate (Wnd c, LPARAM param)
			{
				if(c._ClassNameIs(className, wild) && (--matchIndex == 0)) { R = c; return 0; }
				return 1;
			};

			if(directChild) {
				foreach(Wnd c in All.DirectChildControlsEnum(w))
					if(ep(c, Zero) == 0) break;
			} else {
				Api.EnumChildWindows(w, ep, Zero);
			}
			return R;
		}

		#endregion

		/// <summary>
		/// Static functions that get arrays or enumerables of windows/controls.
		/// </summary>
		public static class All
		{
			//#if use_first_next
			/// <summary>
			/// Enumerates direct child controls of w.
			/// Example: <c>foreach(Wnd c in Wnd.Get.DirectChildren(w) { Out(c); }</c>
			/// In first loop it calls Get.FirstChild, then in each next loop calls Get.NextSibling; therefore, if controls are zordered while enumerating, some controls can be skipped or retrieved more than once.
			/// </summary>
			//TODO: instead use just DirectChildControls, which will use Api.EnumChildWindows. (compare speed)
			public static IEnumerable<Wnd> DirectChildControlsEnum(Wnd w)
			{
				for(Wnd t = Get.FirstChild(w); !t.Is0; t = Get.NextSibling(t)) {
					yield return t;
				}
			}

			//TODO: maybe better use Api.EnumChildWindows.
			public static List<Wnd> DirectChildControls(Wnd hwnd)
			{
				var a = new List<Wnd>();
				for(Wnd t = Get.FirstChild(hwnd); !t.Is0; t = Get.NextSibling(t)) {
					a.Add(t);
				}
				return a;
			}
			//#endif

			/// <summary>
			/// Gets array of child controls of a top-level window or control.
			/// Uses Api.EnumChildWindows().
			/// </summary>
			/// <param name="w">A top-level window or control.</param>
			/// <param name="directChild">Get only direct children, not grandchildren.</param>
			/// <param name="className">If not null, gets only controls of this class. Case-insensitive, wildcard (uses String.LikeI_()).</param>
			public static List<Wnd> Controls(Wnd w, bool directChild = false, string className = null)
			{
				bool wild = _IsWildcard(className);
				var a = new List<Wnd>();
				Api.EnumChildWindows(w, (c, param) =>
				{
					if(!directChild || c.DirectParentOrOwner == w)
						if(className == null || c._ClassNameIs(className, wild))
							a.Add(c);
					return 1;
				}, Zero);
				return a;
			}

			/// <summary>
			/// Calls callback function for each child control of a top-level window or control.
			/// Uses Api.EnumChildWindows().
			/// </summary>
			/// <param name="f">Lambda or other callback function to call for each matching control. Can return true to stop enumeration.</param>
			/// <param name="w">A top-level window or control.</param>
			/// <param name="directChild">Get only direct children, not grandchildren.</param>
			/// <param name="className">If not null, gets only controls of this class. Case-insensitive, wildcard (uses String.LikeI_()).</param>
			public static void Controls(Func<Wnd, bool> f, Wnd w, bool directChild = false, string className = null)
			{
				bool wild = _IsWildcard(className);
				Api.EnumChildWindows(w, (c, param) =>
				{
					if(!directChild || c.DirectParentOrOwner == w)
						if(className == null || c._ClassNameIs(className, wild))
							if(f(c) == true) return 0;
					return 1;
				}, Zero);
			}

			//Cannot use this because need a callback function. Unless we at first get all and store in an array (maybe similar speed).
			//public static IEnumerable<Wnd> Controls(Wnd hwnd)
			//{
			//	Api.EnumChildWindows(hwnd, (t, param)=>
			//	{
			//		yield return t; //error, yield cannot be in an anonymous method etc
			//		return 1;
			//	}, Zero);
			//}

			//Don't use this because it is less reliable than EnumWindows (which we cannot use with foreach).
			//Enumerating child windows in this way is more reliable because they are less often zordered.
			//public static IEnumerable<Wnd> Windows()
			//{
			//	for(Wnd t = Get.FirstToplevel(); !t.Is0; t = Get.NextSibling(t)) {
			//		yield return t;
			//	}
			//}


			//public static Wnd[] MainWindows(Wnd w) { return Wnd0; }

			//public static Wnd[] All(bool visibleOnly=false) { return Wnd0; }

			//public static Wnd[] All(string name, ...) { return Wnd0; }

			//public static Wnd[] AllControls() { return Wnd0; }

			//public static Wnd[] AllControls(string className, ...) { return Wnd0; }

			//public static Wnd[] AllDirectChild() { return Wnd0; }

			//public static Wnd[] AllOfThisThread(Wnd w) { return Wnd0; }
		}

		#region util

		static bool _IsWildcard(string s)
		{
			return s != null && s.IsLikeWildcard_();
		}

		bool _ClassNameIs(string className, bool wild)
		{
			if(Is0) return false;
			string cn = ClassName; if(cn == null) return false;
			return wild ? cn.LikeI_(className) : cn.EqualsI_(className);
		}

		#endregion
	}
}
