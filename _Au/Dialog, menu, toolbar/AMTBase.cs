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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Reflection.Emit;

using Au.Types;
using static Au.AStatic;

namespace Au.Types
{
	interface _IAuToolStrip
	{
		//ToolStrip ToolStrip { get; } //currently not used; we use MainToolStrip instead.
		bool PaintedOnce { get; }
		List<Util.IconsAsync.Item> SubmenuAsyncIcons => null;
	}

	/// <summary>
	/// Base class of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	public abstract class AMTBase
	{
		struct _ItemData
		{
			public Action<MTClickArgs> action;
			public MTThread threadOpt;
			public MTExcept exceptOpt;
			public int sourceLine;
		}

		Dictionary<object, _ItemData> _itemData;
		EventHandler _onClick;
		private protected bool _inRightClick;
		private protected string _sourceFile;
		private protected int _sourceLine;

		private protected AMTBase(string sourceFile, int sourceLine)
		{
			_onClick = OnClick_;
			_itemData = new Dictionary<object, _ItemData>();
			_sourceFile = sourceFile;
			_sourceLine = sourceLine;
		}

		/// <summary>
		/// Common Click even handler of all items.
		/// Calls the true item's onClick callback if need. Calls it in correct thread.
		/// </summary>
		internal void OnClick_(object sender, EventArgs args = null)
		{
			if(_inRightClick) return;
			var x = _itemData[sender];

			switch(x.threadOpt) {
			case MTThread.Current:
				_ExecItem(sender, x);
				break;
			case MTThread.ThreadPool:
				Task.Run(() => _ExecItem(sender, x));
				break;
			case MTThread.StaThread:
			case MTThread.StaBackgroundThread:
				AThread.Start(() => _ExecItem(sender, x), x.threadOpt == MTThread.StaBackgroundThread);
				break;
			}
		}

		void _ExecItem(object sender, in _ItemData x)
		{
			var ca = new MTClickArgs(sender as ToolStripItem);
			if(x.exceptOpt == MTExcept.Exception && x.threadOpt == MTThread.Current) {
				x.action(ca);
			} else {
				try {
					x.action(ca);
				}
				catch(Exception e) {
					if(x.exceptOpt != MTExcept.Silent) PrintWarning(e.ToString(), -1);
				}
			}
		}

		/// <summary>
		/// In what thread to execute item callback functions.
		/// Default: current thread.
		/// </summary>
		/// <remarks>
		/// If current thread is a UI thread (has windows etc), and item callback functions execute some long automations in the same thread, current thread probably is hung during that time. Use this property to avoid it.
		/// This property is applied to items added afterwards.
		/// </remarks>
		public MTThread ItemThread { get; set; }

		/// <summary>
		/// Whether/how to handle unhandled exceptions in item code.
		/// Default: <see cref="MTExcept.Exception"/> (don't handle exceptions if <see cref="ItemThread"/> is <see cref="MTThread.Current"/> (default), else show warning).
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		/// <seealso cref="Application.ThreadException"/>
		public MTExcept ExceptionHandling { get; set; }
		//FUTURE: public Type[] ExceptionTypes { get; set; }
		//	Or bool ExceptionHandling and Func<Exception, bool> ExceptionFilter.

		/// <summary>
		/// Gets ToolStrip of AMenu and AToolbar, which override this.
		/// </summary>
		private protected abstract ToolStrip MainToolStrip { get; }

		/// <summary>
		/// Gets the last added item as <see cref="ToolStripItem"/>, which is the base type of <see cref="ToolStripMenuItem"/>, <see cref="ToolStripButton"/> and other supported types.
		/// </summary>
		public ToolStripItem LastItem { get; protected set; }

		/// <summary>
		/// Occurs when added a non-separator item.
		/// </summary>
		/// <remarks>
		/// Allows to set item properties in single place instead of after each 'add item' code line.
		/// For example, the event handler can set item properties common to all items.
		/// </remarks>
		public event Action<ToolStripItem> ItemAdded;

		/// <summary>
		/// Flags to pass to <see cref="AIcon.GetFileIcon"/>. See <see cref="GIFlags"/>.
		/// </summary>
		/// <remarks>
		/// This property is applied to all items.
		/// </remarks>
		public GIFlags IconFlags { get; set; }

		/// <summary>
		/// Image width and height.
		/// Also can be enum <see cref="IconSize"/>, cast to int.
		/// </summary>
		/// <exception cref="InvalidOperationException">The 'set' function is called after adding items.</exception>
		/// <remarks>
		/// This property is applied to all items, and can be set only before adding items (else exception).
		/// To set different icon size for a submenu: <c>using(m.Submenu("sub")) { m.LastMenuItem.DropDown.ImageScalingSize = new Size(24, 24);</c>
		/// </remarks>
		public int IconSize {
			get => MainToolStrip.ImageScalingSize.Width;
			set {
				if(MainToolStrip.Items.Count != 0) throw new InvalidOperationException();
				MainToolStrip.ImageScalingSize = new Size(value, value);
			}
		}

		/// <summary>
		/// Sets onClick delegate, image and some properties.
		/// </summary>
		private protected void _SetItemProp(bool isTB, ToolStripItem item, Action<MTClickArgs> onClick, object icon, int sourceLine)
		{
			_itemData[item] = new _ItemData {
				action = onClick,
				threadOpt = this.ItemThread,
				exceptOpt = this.ExceptionHandling,
				sourceLine = sourceLine
			};

			if(onClick != null) {
				if(item is ToolStripSplitButton sb) sb.ButtonClick += _onClick; else item.Click += _onClick;

				//APerf.First();
				if(icon == null && ExtractIconPathFromCode) icon = _IconPathFromCode(onClick.Method);
				//APerf.NW(); //ngened about 10 ms first time, then fast. Else 30-40 ms first time.
				//Print(icon);
			}

#if true //to quickly disable icons when measuring speed
			if(icon != null) {
				try {
					switch(icon) {
					case string path:
						if(Util.Image_.IsImageStringPrefix(path)) item.Image = Util.Image_.TryLoadImageFromString(path, warning: true);
						else _SetItemFileIcon(isTB, item, path);
						break;
					case int index: if(index >= 0) item.ImageIndex = index; break;
					case Image img: item.Image = img; break;
					case Icon ico: item.Image = ico.ToBitmap(); break;
					case FolderPath fp: _SetItemFileIcon(isTB, item, fp); break;
					}
				}
				catch(Exception e) { ADebug.Print(e.Message); } //ToBitmap() may throw
			}
#endif
		}

		/// <summary>
		/// Sets LastItem and calls ItemAdded event handlers.
		/// </summary>
		private protected void _OnItemAdded(ToolStripItem item)
		{
			LastItem = item;
			var e = ItemAdded; if(e != null && !(item is ToolStripSeparator)) e(item);
		}

		/// <summary>
		/// When adding items without explicitly specified icon, extract icon from item code.
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		public bool ExtractIconPathFromCode { get; set; }

		void _SetItemFileIcon(bool isTB, ToolStripItem item, string s)
		{
			if(Empty(s)) return;
			var owner = item.Owner;
			var il = owner.ImageList;
			if(il != null && il.Images.ContainsKey(s)) {
				item.ImageKey = s;
			} else {
				//var perf = APerf.Create();
				item.ImageScaling = ToolStripItemImageScaling.None; //we'll get icons of correct size, except if size is 256 and such icon is unavailable, then show smaller

				_AsyncIcons ??= new Util.IconsAsync(); //used by submenus too
				var submenuIcons = (owner as _IAuToolStrip).SubmenuAsyncIcons;
				bool isFirstImage;

				if(submenuIcons == null) {
					isFirstImage = _AsyncIcons.Count == 0;
					_AsyncIcons.Add(s, item);
				} else {
					isFirstImage = submenuIcons.Count == 0;
					submenuIcons.Add(new Util.IconsAsync.Item(s, item));
				}

				//Reserve space for image.
				//If toolbar, need to do it for each button, else only for the first item (it sets size of all items).
				if(isFirstImage) {
					var z = owner.ImageScalingSize;
					_imagePlaceholder = new Bitmap(z.Width, z.Height);
				}
				if(isTB || isFirstImage) item.Image = _imagePlaceholder;
				//perf.NW();
			}
		}
		Image _imagePlaceholder;

		//This is shared by toolbars and main menus. Submenus have their own.
		Util.IconsAsync _AsyncIcons { get; set; }

		//list - used by submenus.
		internal void GetIconsAsync_(ToolStrip ts, List<Util.IconsAsync.Item> list = null)
		{
			if(_AsyncIcons == null) return;
			if(list != null) _AsyncIcons.AddRange(list);
			if(_AsyncIcons.Count == 0) return;
			_AsyncIcons.GetAllAsync(_AsyncCallback, ts.ImageScalingSize.Width, IconFlags, ts);
		}

		void _AsyncCallback(Util.IconsAsync.Result r, object objCommon, int nLeft)
		{
			var ts = objCommon as ToolStrip;
			var item = r.obj as ToolStripItem;

			//Print(r.image, r.hIcon);
			//Image im = r.image;
			//if(im == null && r.hIcon != default) im = AIcon.HandleToImage(r.hIcon);

			Image im = AIcon.HandleToImage(r.hIcon, true);

			//if(im != null) _SetItemIcon(ts, item, im);
			if(im != null) {
				_SetItemIcon(ts, item, im);

				//to dispose images in our Dispose()
				_images ??= new List<Image>();
				_images.Add(im);
			}

			//#if DEBUG
			//			if(im == null) item.ForeColor = Color.Red;
			//#endif
			if(nLeft == 0) {
				//APerf.Next();
				ts.Update();
				//APerf.NW();
			}
		}

		void _SetItemIcon(ToolStrip ts, ToolStripItem item, Image im)
		{
			AWnd w = default;
			var its = ts as _IAuToolStrip;
			if(its.PaintedOnce) {
				if(_region1 == default) _region1 = Api.CreateRectRgn(0, 0, 0, 0);
				if(_region2 == default) _region2 = Api.CreateRectRgn(0, 0, 0, 0);

				w = (AWnd)ts.Handle;
				Api.GetUpdateRgn(w, _region1, false);
			}

			//RECT u;
			//Api.GetUpdateRect((AWnd)ts.Handle, out u, false); Print(its.PaintedOnce, u);

			ts.SuspendLayout(); //without this much slower, especially when with overflow arrows (when many items)
			item.Image = im;
			ts.ResumeLayout(false);

			if(its.PaintedOnce) {
				Api.ValidateRect(w); //tested: with WM_SETREDRAW 3 times slower
				RECT r = item.Bounds; //r.Inflate(-2, -1);
									  //r.right = r.left + r.Height; //same speed
				Api.SetRectRgn(_region2, r.left, r.top, r.right, r.bottom);
				Api.CombineRgn(_region1, _region1, _region2, Api.RGN_OR);

				//RECT b; GetRgnBox(_region1, out b); Print(b, _region1);

				Api.InvalidateRgn(w, _region1, false);
			}

			//Api.GetUpdateRect((AWnd)ts.Handle, out u, false); Print("after", u);
		}

		IntPtr _region1, _region2;
		List<Image> _images;

		/// <summary>
		/// Gets icon path from code that contains string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%AFolders.System%\notepad.exe"</c> or URL/shell.
		/// Also supports code patterns like 'AFolders.System + "notepad.exe"' or 'AFolders.Virtual.RecycleBin'.
		/// Returns null if no such string/pattern.
		/// </summary>
		static string _IconPathFromCode(MethodInfo mi)
		{
			//support code pattern like 'AFolders.System + "notepad.exe"'.
			//	Opcodes: call(AFolders.System), ldstr("notepad.exe"), FolderPath.op_Addition.
			//also code pattern like 'AFolders.System' or 'AFolders.Virtual.RecycleBin'.
			//	Opcodes: call(AFolders.System), FolderPath.op_Implicit(FolderPath to string).
			//also code pattern like 'AExec.TryRun("notepad.exe")'.
			int i = 0, patternStart = -1; MethodInfo f1 = null; string filename = null, filename2 = null;
			try {
				var reader = new Util.ILReader(mi);
				foreach(var instruction in reader.Instructions) {
					if(++i > 100) break;
					var op = instruction.Op;
					//Print(op);
					if(instruction.Op == OpCodes.Ldstr) {
						var s = instruction.Data as string;
						//Print(s);
						if(i == patternStart + 1) filename = s;
						else {
							if(APath.IsFullPathExpandEnvVar(ref s)) return s; //eg AExec.TryRun(@"%AFolders.System%\notepad.exe");
							if(APath.IsUrl(s) || APath.LibIsShellPath(s)) return s;
							filename = null; patternStart = -1;
							if(i == 1) filename2 = s;
						}
					} else if(op == OpCodes.Call && instruction.Data is MethodInfo f && f.IsStatic) {
						//Print(f, f.DeclaringType, f.Name, f.MemberType, f.ReturnType, f.GetParameters().Length);
						var dt = f.DeclaringType;
						if(dt == typeof(AFolders) || dt == typeof(AFolders.Virtual)) {
							if(f.ReturnType == typeof(FolderPath) && f.GetParameters().Length == 0) {
								//Print(1);
								f1 = f;
								patternStart = i;
							}
						} else if(dt == typeof(FolderPath)) {
							if(i == patternStart + 2 && f.Name == "op_Addition") {
								//Print(2);
								var fp = (FolderPath)f1.Invoke(null, null);
								if((string)fp == null) return null;
								return fp + filename;
							} else if(i == patternStart + 1 && f.Name == "op_Implicit" && f.ReturnType == typeof(string)) {
								//Print(3);
								return (FolderPath)f1.Invoke(null, null);
							}
						}
					}
				}
				if(filename2 != null && filename2.Ends(".exe", true)) return AFile.SearchPath(filename2);
			}
			catch(Exception ex) { ADebug.Print(ex); }
			return null;
		}

		private protected void _Dispose(bool disposing)
		{
			//Print("_Dispose", _isDisposed);
			if(_isDisposed) return;
			_isDisposed = true;

			if(disposing) {
				_AsyncIcons?.Dispose();

				if(_images != null) {
					foreach(var im in _images) im.Dispose();
					_images = null;
				}
			}

			if(_region1 != default) Api.DeleteObject(_region1);
			if(_region2 != default) Api.DeleteObject(_region2);

			LastItem = null;
		}
		bool _isDisposed;

		///
		~AMTBase() { /*Print("base dtor");*/ _Dispose(false); }

		/// <summary>
		/// If item not null, gets its 1-based source line. Else gets that of ctor.
		/// </summary>
		internal int GetItemSourceLine_(ToolStripItem item) => item != null && _itemData.TryGetValue(item, out var d) ? d.sourceLine : _sourceLine;

		/// <summary>
		/// If item not null, goes to its source line in editor. Else goes to that of ctor.
		/// Does nothing if editor process not found.
		/// </summary>
		internal void GoToEdit_(ToolStripItem item)
		{
			var wmsg = ATask.WndMsg; if(wmsg.Is0) return;
			Api.AllowSetForegroundWindow(wmsg.ProcessId);
			AWnd.More.CopyDataStruct.SendString(wmsg, 4, _sourceFile, GetItemSourceLine_(item));
		}

		internal static bool CanGoToEdit_ => !ATask.WndMsg.Is0;

		//internal void GoToEdit_(bool isTB, ToolStripItem tsi)
		//{
		//	var wmsg = ATask.WndMsg; if(wmsg.Is0) return;
		//	Api.AllowSetForegroundWindow(wmsg.ProcessId);
		//	var data = Util.LibSerializer.Serialize(isTB ? 1 : 0, _sourceFile, _sourceLine, tsi?.Text);
		//	AWnd.More.CopyDataStruct.SendBytes(wmsg, 120, data);
		//}
	}

	/// <summary>
	/// Data passed to <b>Click</b> event handler functions of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	public class MTClickArgs
	{
		/// <summary>
		/// Gets the clicked item as ToolStripItem.
		/// </summary>
		public ToolStripItem Item { get; }

		/// <summary>
		/// Gets the clicked item as ToolStripMenuItem.
		/// Returns null if it is not ToolStripMenuItem.
		/// </summary>
		public ToolStripMenuItem MenuItem => Item as ToolStripMenuItem;

		internal MTClickArgs(ToolStripItem item) { Item = item; }

		/// <summary>
		/// Gets item text.
		/// </summary>
		public override string ToString() => Item.ToString();
	}

	/// <summary>
	/// Used with <see cref="AMTBase.ItemThread"/>.
	/// </summary>
	public enum MTThread : byte
	{
		/// <summary>
		/// Execute item callback functions in current thread. This is default.
		/// </summary>
		Current,

		/// <summary>
		/// Execute item callback functions in thread pool threads (<see cref="Task.Run"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		ThreadPool,

		/// <summary>
		/// Execute item callback functions in new STA threads (<see cref="Thread.SetApartmentState"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		StaThread,

		/// <summary>
		/// Execute item callback functions in new STA background threads (<see cref="Thread.IsBackground"/>).
		/// Note: current thread does not wait until the callback function finishes.
		/// </summary>
		StaBackgroundThread,
	}

	/// <summary>
	/// Used with <see cref="AMTBase.ExceptionHandling"/>.
	/// </summary>
	public enum MTExcept : byte
	{
		/// <summary>
		/// Don't handle exceptions. This is default.
		/// However if <see cref="AMTBase.ItemThread"/> is not <see cref="MTThread.Current"/>, handles exceptions and shows warning.
		/// On unhandled exception .NET probably will show an unhandled exception dialog with exception info and "Continue" button, unless your app uses <see cref="Application.ThreadException"/> event.
		/// </summary>
		Exception,

		/// <summary>Handle exceptions. On exception call <see cref="PrintWarning"/>.</summary>
		Warning,

		/// <summary>Handle exceptions. On exception do nothing.</summary>
		Silent,
	}

	/// <summary>
	/// Allows to create <see cref="AMenu"/> submenus easier.
	/// Example: <c>using(m.Submenu("Name")) { add items; }</c> .
	/// </summary>
	public struct MUsingSubmenu : IDisposable
	{
		AMenu _m;

		/// <summary>
		/// Gets <b>ToolStripMenuItem</b> of the submenu-item.
		/// </summary>
		public ToolStripMenuItem MenuItem { get; }

		internal MUsingSubmenu(AMenu m, ToolStripMenuItem mi) { _m = m; MenuItem = mi; }

		/// <summary>
		/// Calls <see cref="AMenu.EndSubmenu"/>.
		/// </summary>
		public void Dispose() { _m.EndSubmenu(); }
	}
}
