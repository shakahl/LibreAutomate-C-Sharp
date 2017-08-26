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
using static Catkeys.NoClass;

namespace Catkeys
{
	//TODO
	public static class BlockUserInput
	{
		public static _Unblock All()
		{
			return new _Unblock(_Unblock.What.All);
		}

		public static _Unblock Keys()
		{
			return new _Unblock(_Unblock.What.Keys);
		}

		public static _Unblock Mouse()
		{
			return new _Unblock(_Unblock.What.Mouse);
		}

		/// <summary>Infrastructure.</summary>
		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), CLSCompliant(false)]
		public struct _Unblock :IDisposable
		{
			[Flags]
			public enum What { None, Keys, Mouse, All }

			//What _blocked;
			IntPtr _kHook, _mHook;

			//note: don't use Api.BlockInput because:
			//	UAC. Fails if our process has Medium IL.
			//	Too limited, eg cannot block only keys or only mouse.

			public _Unblock(What what) : this()
			{
				//_blocked = what;
			}

			/// <summary>
			/// Starts blocking input.
			/// </summary>
			public void Start(What what)
			{

			}

			/// <summary>
			/// Stops blocking input.
			/// </summary>
			public void Stop()
			{

			}

			/// <summary>
			/// Calls Stop.
			/// </summary>
			public void Dispose() { Stop(); }
		}

		//note: don't use Api.BlockInput because:
		//	UAC. Fails if our process has Medium IL.
		//	Too limited, eg cannot block only keys or only mouse.
		/// <summary>Infrastructure.</summary>
		//[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		//public struct BlockInput_All :IDisposable
		//{
		//	bool _blocked;

		//	public BlockInput_All(bool startNow = true)
		//	{
		//		_blocked = startNow && Api.BlockInput(true);
		//	}

		//	/// <summary>
		//	/// Starts blocking input.
		//	/// </summary>
		//	public void Start()
		//	{
		//		if(!_blocked) _blocked = Api.BlockInput(true);
		//	}

		//	/// <summary>
		//	/// Stops blocking input.
		//	/// </summary>
		//	public void Stop()
		//	{
		//		if(_blocked) { _blocked = false; Api.BlockInput(false); }
		//	}

		//	/// <summary>
		//	/// Calls Stop.
		//	/// </summary>
		//	public void Dispose() { Stop(); }
		//}
	}
}
