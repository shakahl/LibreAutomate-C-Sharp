//Interfaces that must implement apps using this library.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	public interface ICollectionFile
	{
		string Guid { get; }

		string Name { get; }

		string FilePath { get; }

		bool IsScript { get; }

		ICollectionFiles IcfCollection { get; }

		ICollectionFile IcfFindRelative(string relativePath, bool? folder);

		IEnumerable<ICollectionFile> IcfEnumProjectFiles(ICollectionFile fSkip = null);
	}

	public interface ICollectionFiles
	{
		/// <summary>
		/// Data used by compiler. It sets and gets this property.
		/// Implementation: public object IcfCompilerContext { get; set; }
		/// </summary>
		object IcfCompilerContext { get; set; }

		string IcfFilesDirectory { get; }

		string IcfCollectionDirectory { get; }

		ICollectionFile IcfFindByGUID(string guid);
	}
}
