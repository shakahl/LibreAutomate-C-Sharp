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

using Au.Types;
using static Au.NoClass;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member //TODO
#pragma warning disable CS0169 // field never used //TODO

namespace Au
{
	//TODO
	public static class BlockUserInput
	{
		public static BIUnblock All()
		{
			return new BIUnblock(BIUnblock.What.All);
		}

		public static BIUnblock Keys()
		{
			return new BIUnblock(BIUnblock.What.Keys);
		}

		public static BIUnblock Mouse()
		{
			return new BIUnblock(BIUnblock.What.Mouse);
		}

		//note: don't use Api.BlockInput because:
		//	UAC. Fails if our process has Medium IL.
		//	Too limited, eg cannot block only keys or only mouse.
		///// <summary>Infrastructure.</summary>
		///// <tocexclude />
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

namespace Au.Types
{
	/// <summary>Infrastructure.</summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct BIUnblock :IDisposable
	{
		[Flags]
		public enum What { None, Keys, Mouse, All }

		//What _blocked;
		IntPtr _kHook, _mHook;

		//note: don't use Api.BlockInput because:
		//	UAC. Fails if our process has Medium IL.
		//	Too limited, eg cannot block only keys or only mouse.

		public BIUnblock(What what) : this()
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
}
