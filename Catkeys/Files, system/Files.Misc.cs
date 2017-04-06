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
//using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	partial class Files
	{
		/// <summary>
		/// Miscellaneous functions.
		/// </summary>
		public static class Misc
		{
			//TODO: Use class Files.Shell.

			#region pidl

			/// <summary>
			/// Converts file path or URL or shell object parsing name to PIDL.
			/// Returns Zero if s is empty. If failed, returns Zero or throws exception.
			/// Later call Marshal.FreeCoTaskMem();
			/// </summary>
			/// <param name="s"></param>
			/// <param name="canThrow">If fails, throw CatException.</param>
			/// <exception cref="CatException">Failed, and canThrow is true. Probably invalid s.</exception>
			public static unsafe IntPtr PidlFromString(string s, bool canThrow = false)
			{
				if(!Empty(s)) {
					var hr = Api.SHParseDisplayName(s, Zero, out IntPtr R, 0, null);
					if(hr == 0) return R;
					if(canThrow) throw new CatException(hr);
				}
				return Zero;
				//the same as
				//Api.IShellFolder isf; if(0 != Api.SHGetDesktopFolder(out isf)) return Zero;
				//try { if(0 != isf.ParseDisplayName(Wnd0, Zero, s, null, out R, null)) return Zero; } finally { Marshal.ReleaseComObject(isf); }
			}

			/// <summary>
			/// Converts PIDL to file path or URL or shell object parsing name.
			/// Returns null if pidl is Zero. If failed, returns null or throws exception.
			/// </summary>
			/// <param name="pidl"></param>
			/// <param name="stringType">
			/// A value from the <see cref="Native.SIGDN"/> enumeration that specifies the type of string to retrieve.
			/// With the default value returns string that can be passed to PidlFromString. It can be a path, URL, "::{CLSID}", etc.
			/// Other often used values:
			/// Native.SIGDN.SIGDN_FILESYSPATH - returns null if pidl is not of a file system object.
			/// Native.SIGDN.SIGDN_URL - returns null if pidl is not of a URL or path. If path, returns its URL form, like "file:///C:/a/b.txt".
			/// Native.SIGDN.SIGDN_NORMALDISPLAY - returns string that is best to display in UI but cannot be passed to PidlFromString.
			/// </param>
			/// <param name="canThrow">If fails, throw CatException.</param>
			/// <exception cref="CatException">Failed, and canThrow is true.</exception>
			public static string PidlToString(IntPtr pidl, Native.SIGDN stringType = Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, bool canThrow = false)
			{
				if(pidl != Zero) {
					var hr = Api.SHGetNameFromIDList(pidl, stringType, out string R);
					if(hr == 0) return R;
					if(canThrow) throw new CatException(hr);
				}
				return null;
			}

			#endregion

#if false //currently not used
			/// <summary>
			/// Gets HKEY_CLASSES_ROOT registry key of file type or protocol.
			/// The key usually contains subkeys "shell", "DefaultIcon", sometimes "shellex" and more.
			/// For example, for ".txt" can return "txtfile", for ".cs" - "VisualStudio.cs.14.0".
			/// Looks in "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts" and in HKEY_CLASSES_ROOT.
			/// Returns null if the type/protocol is not registered.
			/// Returns null if fileType does not end with ".extension" and does not start with "protocol:"; also if starts with "shell:".
			/// </summary>
			/// <param name="fileType">
			/// File type extension like ".txt" or protocol like "http:".
			/// Can be full path or URL; the function gets extension or protocol from the string.
			/// Can start with %environment variable%.
			/// </param>
			/// <param name="isFileType">Don't parse fileType, it does not contain full path or URL or environment variables. It is ".ext" or "protocol:".</param>
			/// <param name="isURL">fileType is URL or protocol like "http:". Used only if isFileType == true, ie it is protocol.</param>
			internal static string GetFileTypeOrProtocolRegistryKey(string fileType, bool isFileType, bool isURL)
			{
				if(!isFileType) fileType = GetExtensionOrProtocol(fileType, out isURL);
				else if(isURL) fileType = fileType.Remove(fileType.Length - 1); //"proto:" -> "proto"
				if(Empty(fileType)) return null;

				string R, userChoiceKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + fileType + @"\UserChoice";
				if(Registry_.GetString(out R, "ProgId", userChoiceKey)) return R;
				if(isURL) return fileType;
				if(Registry_.GetString(out R, "", fileType, Registry.ClassesRoot)) return R;
				return null;

				//note: IQueryAssociations.GetKey is very slow.
			}

			/// <summary>
			/// Gets file path extension like ".txt" or URL protocol like "http".
			/// Returns null if path does not end with ".extension" and does not start with "protocol:"; also if starts with "shell:".
			/// </summary>
			/// <param name="path">File path or URL. Can be just extension like ".txt" or protocol like "http:".</param>
			/// <param name="isProtocol">Receives true if URL or protocol.</param>
			internal static string GetExtensionOrProtocol(string path, out bool isProtocol)
			{
				isProtocol = false;
				if(Empty(path)) return null;
				if(!PathIsExtension(path)) {
					int i = path.IndexOf(':');
					if(i > 1) {
						path = path.Remove(i); //protocol
						if(path == "shell") return null; //eg "shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"
						isProtocol = true;
					} else {
						try { path = Path.GetExtension(path); } catch { return null; }
						if(Empty(path)) return null;
					}
				}
				return path;
			}
#endif

#pragma warning disable 660, 661 //no Equals()
			/// <summary>
			/// Contains file properties that can be used to uniquely identify the file on a single computer.
			/// Can be used with files and directories.
			/// To get it, use <see cref="GetFileId"/>.
			/// </summary>
			/// <remarks>
			/// Path to the same file or directory can be specified in many different ways. To determine whether two paths represent to the same file, get and compare FileId of them.
			/// </remarks>
			public struct FileId
			{
				/// <summary>The serial number of the volume (aka disk drive) that contains the file.</summary>
				public int VolumeSerialNumber;
				/// <summary>An identifier that is associated with the file. It is unique in that volume.</summary>
				public long FileIndex;

				///
				public static bool operator ==(FileId a, FileId b)
				{
					return a.FileIndex == b.FileIndex && a.VolumeSerialNumber == b.VolumeSerialNumber;
				}

				///
				public static bool operator !=(FileId a, FileId b) { return !(a == b); }
			}
#pragma warning restore 660, 661

			/// <summary>
			/// Gets <see cref="FileId"/> of a file or directory.
			/// Returns false if fails. Supports <see cref="Native.GetError"/>.
			/// </summary>
			/// <param name="path">Full path. This function does not expand environment variables etc; it passes unmodified path to the API.</param>
			/// <param name="fileId"></param>
			public static bool GetFileId(string path, out FileId fileId)
			{
				fileId = new FileId();
				IntPtr h = (IntPtr)(-1);
				try {
					h = _Api.CreateFile(path, 0x80, 7, Zero, 3, 0x02000000, Zero);
					//FILE_READ_ATTRIBUTES, all sharing, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS
					if(h == (IntPtr)(-1)) return false;
					if(!_Api.GetFileInformationByHandle(h, out var k)) return false;
					fileId.VolumeSerialNumber = (int)k.dwVolumeSerialNumber;
					fileId.FileIndex = (long)((ulong)k.nFileIndexHigh << 32 | k.nFileIndexLow);
					return true;
				}
				finally {
					if(h != (IntPtr)(-1)) Api.CloseHandle(h);
				}
			}

			/// <summary>
			/// Calls <see cref="GetFileId"/> for two paths and returns true if both calls succeed and the ids are equal.
			/// Paths should be normalized. They are passed to API unmodified.
			/// If DEBUG is defined, calls DebugPrint if some GetFileId call fails.
			/// </summary>
			/// <param name="path1"></param>
			/// <param name="path2"></param>
			internal static bool LibIsSameFile(string path1, string path2)
			{
				//try to optimize. No, it's unreliable.
				//int i1 = path1.LastIndexOfAny(_sep3), i2 = path2.LastIndexOfAny(_sep3);
				//if(i1 >= 0 && i2 >= 0 && path1[i1] != '~' && path2[i2] != '~') {
				//	i1++; i2++;
				//	if()
				//}

				var ok1 = GetFileId(path1, out var fid1);
				var ok2 = GetFileId(path2, out var fid2);
				DebugPrintIf(!ok1 || !ok2, "GetFileId failed");
				return ok1 && ok2 && fid1 == fid2;
			}

			//static char[] _sep3 = new char[] { '\\', '/', '~' };
#if false
		//this is ~300 times slower than File.Move. SHFileOperation too. Use only for files or other shell items in virtual folders. Unfinished.
		public static void RenameFileOrDirectory(string path, string newName)
		{
			Perf.First();
			if(Path_.IsInvalidFileName(newName)) throw new ArgumentException("Invalid filename.", nameof(newName));
			path = _PreparePath(path, nameof(path));

			Perf.Next();
			var si = _ShellItem(path, "*rename");
			Perf.Next();
			var fo = new _Api.FileOperation() as _Api.IFileOperation;
			Perf.Next();
			try {
				fo.SetOperationFlags(4); //FOF_SILENT. Without it shows a hidden dialog that becomes the active window.
				CatException.ThrowIfFailed(fo.RenameItem(si, newName, null), "*rename");
				Perf.Next();
				CatException.ThrowIfFailed(fo.PerformOperations(), "*rename");
				Perf.Next();
			}
			finally {
				Api.ReleaseComObject(fo);
				Api.ReleaseComObject(si);
			}
			Perf.NW();
		}

		static _Api.IShellItem _ShellItem(string path, string errMsg)
		{
			var pidl = Misc.PidlFromString(path, true);
			try {
				var guid = typeof(_Api.IShellItem).GUID;
				CatException.ThrowIfFailed(_Api.SHCreateItemFromIDList(pidl, ref guid, out var R), errMsg);
				return R;
			}
			finally { Marshal.FreeCoTaskMem(pidl); }
		}

		static class _Api
		{
			[DllImport("shell32.dll", PreserveSig = true)]
			internal static extern int SHCreateItemFromIDList(IntPtr pidl, [In] ref Guid riid, out IShellItem ppv);

			[ComImport, Guid("3ad05575-8857-4850-9277-11b85bdb8e09"), ClassInterface(ClassInterfaceType.None)]
			internal class FileOperation { }

			[ComImport, Guid("947aab5f-0a5c-4c13-b4d6-4bf7836fc9f8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IFileOperation
			{
				[PreserveSig] int Advise(IFileOperationProgressSink pfops, out uint pdwCookie);
				[PreserveSig] int Unadvise(uint dwCookie);
				[PreserveSig] int SetOperationFlags(uint dwOperationFlags);
				[PreserveSig] int SetProgressMessage([MarshalAs(UnmanagedType.LPWStr)] string pszMessage);
				[PreserveSig] int SetProgressDialog(IOperationsProgressDialog popd);
				[PreserveSig] int SetProperties(IntPtr pproparray); //IPropertyChangeArray
				[PreserveSig] int SetOwnerWindow(Wnd hwndOwner);
				[PreserveSig] int ApplyPropertiesToItem(IShellItem psiItem);
				[PreserveSig] int ApplyPropertiesToItems([MarshalAs(UnmanagedType.IUnknown)] Object punkItems);
				[PreserveSig] int RenameItem(IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IFileOperationProgressSink pfopsItem);
				[PreserveSig] int RenameItems([MarshalAs(UnmanagedType.IUnknown)] Object pUnkItems, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
				[PreserveSig] int MoveItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, IFileOperationProgressSink pfopsItem);
				[PreserveSig] int MoveItems([MarshalAs(UnmanagedType.IUnknown)] Object punkItems, IShellItem psiDestinationFolder);
				[PreserveSig] int CopyItem(IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszCopyName, IFileOperationProgressSink pfopsItem);
				[PreserveSig] int CopyItems([MarshalAs(UnmanagedType.IUnknown)] Object punkItems, IShellItem psiDestinationFolder);
				[PreserveSig] int DeleteItem(IShellItem psiItem, IFileOperationProgressSink pfopsItem);
				[PreserveSig] int DeleteItems([MarshalAs(UnmanagedType.IUnknown)] Object punkItems);
				[PreserveSig] int NewItem(IShellItem psiDestinationFolder, uint dwFileAttributes, [MarshalAs(UnmanagedType.LPWStr)] string pszName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, IFileOperationProgressSink pfopsItem);
				[PreserveSig] int PerformOperations();
				[PreserveSig] int GetAnyOperationsAborted([MarshalAs(UnmanagedType.Bool)] out bool pfAnyOperationsAborted);
			}

			[ComImport, Guid("04b0f1a7-9490-44bc-96e1-4296a31252e2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IFileOperationProgressSink
			{
				[PreserveSig] int StartOperations();
				[PreserveSig] int FinishOperations(int hrResult);
				[PreserveSig] int PreRenameItem(uint dwFlags, IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
				[PreserveSig] int PostRenameItem(uint dwFlags, IShellItem psiItem, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, int hrRename, IShellItem psiNewlyCreated);
				[PreserveSig] int PreMoveItem(uint dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
				[PreserveSig] int PostMoveItem(uint dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, int hrMove, IShellItem psiNewlyCreated);
				[PreserveSig] int PreCopyItem(uint dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
				[PreserveSig] int PostCopyItem(uint dwFlags, IShellItem psiItem, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, int hrCopy, IShellItem psiNewlyCreated);
				[PreserveSig] int PreDeleteItem(uint dwFlags, IShellItem psiItem);
				[PreserveSig] int PostDeleteItem(uint dwFlags, IShellItem psiItem, int hrDelete, IShellItem psiNewlyCreated);
				[PreserveSig] int PreNewItem(uint dwFlags, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName);
				[PreserveSig] int PostNewItem(uint dwFlags, IShellItem psiDestinationFolder, [MarshalAs(UnmanagedType.LPWStr)] string pszNewName, [MarshalAs(UnmanagedType.LPWStr)] string pszTemplateName, uint dwFileAttributes, int hrNew, IShellItem psiNewItem);
				[PreserveSig] int UpdateProgress(uint iWorkTotal, uint iWorkSoFar);
				[PreserveSig] int ResetTimer();
				[PreserveSig] int PauseTimer();
				[PreserveSig] int ResumeTimer();
			}

			[ComImport, Guid("0C9FB851-E5C9-43EB-A370-F0677B13874C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IOperationsProgressDialog
			{
				[PreserveSig] int StartProgressDialog(Wnd hwndOwner, uint flags);
				[PreserveSig] int StopProgressDialog();
				[PreserveSig] int SetOperation(SPACTION action);
				[PreserveSig] int SetMode(uint mode);
				[PreserveSig] int UpdateProgress(ulong ullPointsCurrent, ulong ullPointsTotal, ulong ullSizeCurrent, ulong ullSizeTotal, ulong ullItemsCurrent, ulong ullItemsTotal);
				[PreserveSig] int UpdateLocations(IShellItem psiSource, IShellItem psiTarget, IShellItem psiItem);
				[PreserveSig] int ResetTimer();
				[PreserveSig] int PauseTimer();
				[PreserveSig] int ResumeTimer();
				[PreserveSig] int GetMilliseconds(out ulong pullElapsed, out ulong pullRemaining);
				[PreserveSig] int GetOperationStatus(out PDOPSTATUS popstatus);
			}

			internal enum SPACTION
			{
				SPACTION_NONE,
				SPACTION_MOVING,
				SPACTION_COPYING,
				SPACTION_RECYCLING,
				SPACTION_APPLYINGATTRIBS,
				SPACTION_DOWNLOADING,
				SPACTION_SEARCHING_INTERNET,
				SPACTION_CALCULATING,
				SPACTION_UPLOADING,
				SPACTION_SEARCHING_FILES,
				SPACTION_DELETING,
				SPACTION_RENAMING,
				SPACTION_FORMATTING,
				SPACTION_COPY_MOVING
			}

			internal enum PDOPSTATUS
			{
				PDOPS_RUNNING = 1,
				PDOPS_PAUSED,
				PDOPS_CANCELLED,
				PDOPS_STOPPED,
				PDOPS_ERRORS
			}

			[ComImport, Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			internal interface IShellItem
			{
				[PreserveSig] int BindToHandler(IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv); //IBindCtx
				[PreserveSig] int GetParent(out IShellItem ppsi);
				[PreserveSig] int GetDisplayName(Native.SIGDN sigdnName, out IntPtr ppszName);
				[PreserveSig] int GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
				[PreserveSig] int Compare(IShellItem psi, uint hint, out int piOrder);
			}


		}
#endif
		}
	}
}
