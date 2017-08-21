//Classes and functions that were (almost) finished, but rejected for some reason. Maybe still can be useful in the future.
//For example, when tried to make faster/better than existing .NET classes/functions, but the result was not fast/good enough.







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
		if(R == 0) throw new CatException();

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


