using Au.Util;
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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

namespace Au.Types
{
	interface _IAuToolStrip
	{
		//ToolStrip ToolStrip { get; } //currently not used; we use MainToolStrip instead.
		bool PaintedOnce { get; }
		List<IconsAsync_.Item> SubmenuAsyncIcons => null;
	}

	/// <summary>
	/// Base class of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	public abstract class MTBase
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

		private protected MTBase(string sourceFile, int sourceLine)
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
					if(x.exceptOpt != MTExcept.Silent) AWarning.Write(e.ToString(), -1);
				}
			}
		}

		/// <summary>
		/// In what thread to execute item actions (callback functions).
		/// Default: <see cref="MTThread.Current"/> for <b>AMenu</b> created with the parameterless constructor, else <see cref="MTThread.StaThread"/>.
		/// </summary>
		/// <remarks>
		/// If current thread is a UI thread (has windows etc), and item callback functions execute some long automations in the same thread, current thread probably is hung during that time. Use this property to avoid it.
		/// This property is applied to items added afterwards.
		/// </remarks>
		public MTThread ActionThread { get; set; }

		/// <summary>
		/// Whether/how to handle unhandled exceptions in item action code.
		/// Default: <see cref="MTExcept.Exception"/> (don't handle exceptions if <see cref="ActionThread"/> is <see cref="MTThread.Current"/> (default), else show warning).
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

		///// <summary>
		///// Flags to pass to <see cref="AIcon.OfFile"/>. See <see cref="IconGetFlags"/>.
		///// </summary>
		///// <remarks>
		///// This property is applied to all items.
		///// </remarks>
		//public IconGetFlags IconFlags { get; set; }
		// If not full path, searches in <see cref="AFolders.ThisAppImages"/>; see also <see cref="MTBase.IconFlags"/>.

		/// <summary>
		/// Image width and height. Default 16.
		/// </summary>
		/// <exception cref="InvalidOperationException">The 'set' function is called after adding items.</exception>
		/// <remarks>
		/// This property is applied to all items, and can be set only before adding items (else exception).
		/// To set different icon size for a submenu: <c>using(m.Submenu("sub")) { m.LastMenuItem.DropDown.ImageScalingSize = new Size(24, 24);</c>
		/// </remarks>
		public int IconSize {
			get => _iconSize;
			set {
				if(MainToolStrip.Items.Count != 0) throw new InvalidOperationException();
				_iconSize = value;
				MainToolStrip.ImageScalingSize = new Size(value, value);
			}
		}
		int _iconSize = 16;

		/// <summary>
		/// Gets "invoke method" image from Au library resources. Can be used for <see cref="DefaultIcon"/> of toolbars and menus. Common to <see cref="AMenu"/> and <see cref="AToolbar"/>.
		/// </summary>
		public static Image CommonIcon {
			get => s_commonIcon ??= _GetCommonIcon(false);
			set => s_commonIcon = value;
		}

		/// <summary>
		/// Gets "drop-down menu" image from Au library resources. Can be used for <see cref="DefaultSubmenuIcon"/> of toolbars and menus. Common to <see cref="AMenu"/> and <see cref="AToolbar"/>.
		/// </summary>
		public static Image CommonSubmenuIcon {
			get => s_commonSubmenuIcon ??= _GetCommonIcon(true);
			set => s_commonSubmenuIcon = value;
		}

		static Image s_commonIcon, s_commonSubmenuIcon;

		static Image _GetCommonIcon(bool submenu)
		{
			string name = submenu ? "mtHamburgerMenu" : "mtInvokeMethod";
			int i = ADpi.Scale(16); if(i >= 20) name += i < 24 ? "_20" : (i < 32 ? "_24" : "_32");
			return Resources.Resources.ResourceManager.GetObject(name, Resources.Resources.Culture) as Bitmap; //13 ms first time
		}

		/// <summary>
		/// Image for items that don't have an icon specified or auto-extracted from code.
		/// </summary>
		public MTImage DefaultIcon { get; set; }

		/// <summary>
		/// Image for submenu-items that don't have an icon specified.
		/// </summary>
		public MTImage DefaultSubmenuIcon { get; set; }

		/// <summary>
		/// When adding items without explicitly specified icon, extract icon from item action code.
		/// </summary>
		/// <remarks>
		/// This property is applied to items added afterwards.
		/// </remarks>
		public bool ExtractIconPathFromCode { get; set; }

		/// <summary>
		/// Sets onClick delegate, image and some properties.
		/// </summary>
		private protected void _SetItemProp(bool isTB, bool isSub, ToolStripItem item, Action<MTClickArgs> onClick, MTImage icon, int sourceLine)
		{
			_itemData[item] = new _ItemData {
				action = onClick,
				threadOpt = this.ActionThread,
				exceptOpt = this.ExceptionHandling,
				sourceLine = sourceLine
			};

			var iconObj = icon.Value;

			if(onClick != null) {
				if(item is ToolStripSplitButton sb) sb.ButtonClick += _onClick; else item.Click += _onClick;

				//APerf.First();
				if(iconObj == null && ExtractIconPathFromCode) iconObj = AWpfMenu.IconPathFromCode_(onClick.Method);
				//APerf.NW(); //ngened about 10 ms first time, then fast. Else 30-40 ms first time.
				//AOutput.Write(iconObj);
			}

#if true //to quickly disable icons when measuring speed
			if(iconObj != null) {
				_SetIconObj(iconObj, 0);
			} else if(isSub) {
				if(DefaultSubmenuIcon.Value != null) _SetIconObj(DefaultSubmenuIcon.Value, 2);
			} else if(!(item is ToolStripSeparator)){
				if(DefaultIcon.Value != null) _SetIconObj(DefaultIcon.Value, 1);
			}

			void _SetIconObj(object iconObj, int useDefault)
			{
				try {
					Image imageResult = null;
					switch(iconObj) {
					case Image im:
						item.Image = im;
						break;
					case Icon ic:
						imageResult = ic.ToBitmap();
						break;
					case StockIcon ic:
						imageResult = AIcon.Stock(ic, IconSize).ToGdipBitmap();
						break;
					case string s when s.Length > 0:
						if(AImageUtil.HasImageStringPrefix(s)) {
							imageResult = AImageUtil.TryLoadGdipBitmapFromString(s, warning: true);
						} else if(s.Starts("key:")) {
							item.ImageKey = s[4..];
						} else if(useDefault != 0) {
							imageResult = AIcon.OfFile(s, IconSize).ToGdipBitmap();
						} else {
							_SetItemFileIcon(isTB, item, s); //async
						}
						break;
					case int index:
						item.ImageIndex = index;
						break;
					}

					if(imageResult != null) {
						item.Image = imageResult;
						if(useDefault == 1) DefaultIcon = imageResult; else if(useDefault == 2) DefaultSubmenuIcon = imageResult;
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

		void _SetItemFileIcon(bool isTB, ToolStripItem item, string s)
		{
			//var perf = APerf.Create();
			//item.ImageScaling = ToolStripItemImageScaling.None; //we'll get icons of correct size, except if size is 256 and such icon is unavailable, then show smaller

			_AsyncIcons ??= new IconsAsync_(); //used by submenus too
			var submenuIcons = (item.Owner as _IAuToolStrip).SubmenuAsyncIcons;
			bool isFirstImage;

			if(submenuIcons == null) {
				isFirstImage = _AsyncIcons.Count == 0;
				_AsyncIcons.Add(s, item);
			} else {
				isFirstImage = submenuIcons.Count == 0;
				submenuIcons.Add(new IconsAsync_.Item(s, item));
			}

			//Reserve space for image.
			//If toolbar, need to do it for each button, else only for the first item (it sets size of all items).
			if(isFirstImage) {
				//var z = item.Owner.ImageScalingSize;//TODO
				//_imagePlaceholder = new Bitmap(z.Width, z.Height);
				_imagePlaceholder = new Bitmap(_iconSize, _iconSize);
			}
			if(isTB || isFirstImage) item.Image = _imagePlaceholder;
			//perf.NW();
		}
		Image _imagePlaceholder;

		//This is shared by toolbars and main menus. Submenus have their own.
		IconsAsync_ _AsyncIcons { get; set; }

		//list - used by submenus.
		internal void GetIconsAsync_(ToolStrip ts, List<IconsAsync_.Item> list = null)
		{
			if(_AsyncIcons == null) return;
			if(list != null) _AsyncIcons.AddRange(list);
			if(_AsyncIcons.Count == 0) return;
			//_AsyncIcons.GetAllAsync(_AsyncCallback, ts.ImageScalingSize.Width, 0 /*IconFlags*/, ts);//TODO
			_AsyncIcons.GetAllAsync(_AsyncCallback, IconSize, 0 /*IconFlags*/, ts);
		}

		void _AsyncCallback(IconsAsync_.Result r, object objCommon, int nLeft)
		{
			var ts = objCommon as ToolStrip;
			var item = r.obj as ToolStripItem;

			Image im = r.icon.ToGdipBitmap(true);
			if(im != null) {
				_SetItemImage(ts, item, im);

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

		void _SetItemImage(ToolStrip ts, ToolStripItem item, Image im)
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
			//Api.GetUpdateRect((AWnd)ts.Handle, out u, false); AOutput.Write(its.PaintedOnce, u);

			ts.SuspendLayout(); //without this much slower, especially when with overflow arrows (when many items)
			item.Image = im;
			ts.ResumeLayout(false);

			if(its.PaintedOnce) {
				Api.ValidateRect(w); //tested: with WM_SETREDRAW 3 times slower
				RECT r = item.Bounds; //r.Inflate(-2, -1);
									  //r.right = r.left + r.Height; //same speed
				Api.SetRectRgn(_region2, r.left, r.top, r.right, r.bottom);
				Api.CombineRgn(_region1, _region1, _region2, Api.RGN_OR);

				//RECT b; GetRgnBox(_region1, out b); AOutput.Write(b, _region1);

				Api.InvalidateRgn(w, _region1, false);
			}

			//Api.GetUpdateRect((AWnd)ts.Handle, out u, false); AOutput.Write("after", u);
		}

		IntPtr _region1, _region2;
		List<Image> _images;

		private protected void _Dispose(bool disposing)
		{
			//AOutput.Write("_Dispose", _isDisposed, this);
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
		~MTBase()
		{
			//AOutput.Write("base dtor", this);
			_Dispose(false);
		}

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
			AScriptEditor.GoToEdit(_sourceFile, GetItemSourceLine_(item));
		}

		internal static bool CanGoToEdit_ => !ATask.WndMsg_.Is0;

		//internal void GoToEdit_(bool isTB, ToolStripItem tsi)
		//{
		//	var wmsg = ATask.WndMsg; if(wmsg.Is0) return;
		//	Api.AllowSetForegroundWindow(wmsg.ProcessId);
		//	var data = Serializer_.Serialize(isTB ? 1 : 0, _sourceFile, _sourceLine, tsi?.Text);
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
	/// Used with <see cref="MTBase.ActionThread"/>.
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
	/// Used with <see cref="MTBase.ExceptionHandling"/>.
	/// </summary>
	public enum MTExcept : byte
	{
		/// <summary>
		/// Don't handle exceptions. This is default.
		/// However if <see cref="MTBase.ActionThread"/> is not <see cref="MTThread.Current"/>, handles exceptions and shows warning.
		/// On unhandled exception .NET probably will show an unhandled exception dialog with exception info and "Continue" button, unless your app uses <see cref="Application.ThreadException"/> event.
		/// </summary>
		Exception,

		/// <summary>Handle exceptions. On exception call <see cref="AWarning.Write"/>.</summary>
		Warning,

		/// <summary>Handle exceptions. On exception do nothing.</summary>
		Silent,
	}

	/// <summary>
	/// Used for menu/toolbar function parameters to specify an image in different ways (file path, Image object, etc). More info in Remarks.
	/// </summary>
	/// <remarks>
	/// Has implicit conversions from:
	/// - string - path of .ico or any other file or folder or non-file object. See <see cref="AIcon.OfFile"/>.
	/// - string with prefix "image:" - Base-64 encoded png file. Can be created with the "Find image..." dialog.
	/// - string with prefix "key:" - imagelist image key (<see cref="ToolStripItem.ImageKey"/>).
	/// - int - imagelist image index (<see cref="ToolStripItem.ImageIndex"/>).
	/// - Image - image object.
	/// - Icon - icon object.
	/// - <see cref="AIcon"/> - native icon handle. The <b>AIcon</b> to <b>MTImage</b> impicit conversion operator calls <see cref="AIcon.ToGdipBitmap"/> and disposes the native icon.
	/// - <see cref="StockIcon"/> - a shell icon.
	/// - FolderPath - folder path.
	/// - default - no icon. If <see cref="MTBase.ExtractIconPathFromCode"/> == true, extracts icon path from <i>onClick</i> code like <c>AFile.TryRun(@"c:\path\file.exe")</c> or <c>AFile.TryRun(AFolders.System + "file.exe")</c>.
	/// - "" - no icon.
	/// </remarks>
	[DebuggerStepThrough]
	public struct MTImage
	{
		readonly object _o;
		MTImage(object o) { _o = o; }

		///
		public static implicit operator MTImage(string pathEtc) => new MTImage(pathEtc);
		///
		public static implicit operator MTImage(int index) => new MTImage(index);
		///
		public static implicit operator MTImage(Image image) => new MTImage(image);
		///
		public static implicit operator MTImage(Icon icon) => new MTImage(icon);
		///
		public static implicit operator MTImage(AIcon icon) => new MTImage(icon.ToGdipBitmap());
		///
		public static implicit operator MTImage(StockIcon icon) => new MTImage(icon);
		///
		public static implicit operator MTImage(FolderPath path) => new MTImage((string)path);

		/// <summary>
		/// Gets the raw value stored in this variable. Can be string, int, Image, Icon, StockIcon, null.
		/// </summary>
		public object Value => _o;
	}
}
