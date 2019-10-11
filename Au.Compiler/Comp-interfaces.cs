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

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Compiler
{
	public interface IWorkspaceFile
	{
		uint Id { get; }

		string IdString { get; }

		string Name { get; }

		string ItemPath { get; }

		string FilePath { get; }

		bool IsScript { get; }

		bool IsClass { get; }

		string IfwText { get; }

		IWorkspaceFiles IwfWorkspace { get; }

		IWorkspaceFile IwfFindRelative(string relativePath, bool? folder);

		IEnumerable<IWorkspaceFile> IwfEnumProjectClassFiles(IWorkspaceFile fSkip = null);

		bool IwfFindProject(out IWorkspaceFile folder, out IWorkspaceFile main);

		void IwfTriggers(List<CompTriggerData> a);
	}

	public interface IWorkspaceFiles
	{
		/// <summary>
		/// Data used by compiler. It sets and gets this property.
		/// Implementation: public object IcfCompilerContext { get; set; }
		/// </summary>
		object IwfCompilerContext { get; set; }

		string IwfFilesDirectory { get; }

		string IwfWorkspaceDirectory { get; }

		IWorkspaceFile IwfFindById(uint id);
	}

	public struct CompTriggerData
	{
		public string method;
		public string triggerType;
		public KeyValuePair<string, object>[] args;

		public CompTriggerData(string method, string attribute, KeyValuePair<string, object>[] args)
		{
			this.method = method;
			triggerType = attribute.RemoveSuffix(9); //remove suffix "Attribute"
			this.args = args;
		}
	}
}
