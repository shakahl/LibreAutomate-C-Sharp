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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Types
{
	/// <summary>
	/// In DocFX-generated help files removes documentation and auto-generated links in TOC and class pages.
	/// </summary>
	public class NoDoc : Attribute
	{
	}

	/// <summary>
	/// Specifies whether to set, add or remove flags.
	/// </summary>
	public enum SetAddRemove
	{
		/// <summary>Set flags = the specified value.</summary>
		Set = 0,
		/// <summary>Add the specified flags, don't change others.</summary>
		Add = 1,
		/// <summary>Remove the specified flags, don't change others.</summary>
		Remove = 2,
		/// <summary>Toggle the specified flags, don't change others.</summary>
		Xor = 3,
	}
}
