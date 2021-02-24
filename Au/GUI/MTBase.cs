using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace Au
{
	/// <summary>
	/// Base class of <see cref="AMenu"/> and <see cref="AToolbar"/>.
	/// </summary>
	/// <remarks>
	/// <i>image</i> argument of "add item" functions can be:
	/// - file/folder path (string) - the "show" function calls <see cref="AIcon.OfFile"/> to get its icon. It also supports file type icons like ".txt", etc.
	/// - file path with prefix "imagefile:" or resource path that starts with "resources/" or has prefix "resource:" - the "show" function loads .png or .xaml image file or resource.
	/// - string with prefix "image:" - Base-64 encoded png file. Can be created with the "Find image..." dialog.
	/// - <see cref="FolderPath"/> - same as folder path string.
	/// - <see cref="Image"/> - image object.
	/// - <see cref="AIcon"/> - variable containing native icon handle. The "add item" function disposes it (actually the <see cref="MTImage"/> implicit conversion operator disposes it).
	/// - <see cref="StockIcon"/> - the "show" function calls <see cref="AIcon.Stock"/>.
	/// - null - if <see cref="ExtractIconPathFromCode"/> true, the "show" function tries to extract a file path from action code; then calls <see cref="AIcon.OfFile"/>. Else no image.
	/// - string "" - no image, even if <b>ExtractIconPathFromCode</b> true.
	/// 
	/// Item images should be of size 16x16 (small icon size). If high DPI, will scale images automatically, which makes them slightly blurred. To avoid scaling, can be used XAML images, but then slower.
	///
	/// Images are loaded on demand, when showing the menu or submenu etc. If fails to load, prints warning (<see cref="AWarning.Write"/>).
	/// 
	/// For icon/image files use full path, unless they are in <see cref="AFolders.ThisAppImages"/>
	/// 
	/// To add an image resource in Visual Studio, use build action "Resource" for the image file.
	/// </remarks>
	public abstract class MTBase
	{
		protected private readonly string _sourceFile;
		protected private readonly int _sourceLine;
		protected private int _dpi;

		internal MTBase() { }

		internal MTBase(string name, string f, int l) {
			_sourceFile = f;
			_sourceLine = l;
			ExtractIconPathFromCode = true;
		}

		/// <summary>
		/// When adding items without explicitly specified image, extract file path from item action code (for example <see cref="AFile.Run"/> argument) and use icon of that file.
		/// This property is applied to items added afterwards.
		/// </summary>
		/// <remarks>
		/// Gets file path from code that contains a string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%AFolders.System%\notepad.exe"</c> or URL/shell.
		/// Also supports code patterns like <c>AFolders.System + "notepad.exe"</c>, <c>AFolders.Virtual.RecycleBin</c>.
		/// 
		/// If extracts file path, also in the context menu adds item "Find file" which selects the file in Explorer.
		/// 
		/// Also can extract script file name or path in workspace, like @"\Folder\Script20.cs". It is used to open the script from the context menu.
		/// </remarks>
		public bool ExtractIconPathFromCode { get; set; }

		/// <summary>
		/// Gets or sets image cache.
		/// </summary>
		/// <remarks>
		/// Use if there are many same images. Then makes faster.
		/// 
		/// The <b>AIconImageCache</b> object can be shared with other code (menus etc).
		/// </remarks>
		public AIconImageCache ImageCache { get; set; }

		/// <summary>
		/// Execute item actions asynchronously in new threads.
		/// This property is applied to items added afterwards.
		/// </summary>
		/// <remarks>
		/// If current thread is a UI thread (has windows etc) or has triggers or hooks, and item action functions execute some long automations etc in current thread, current thread probably is hung during that time. Set this property = true to avoid it.
		/// </remarks>
		public bool ActionThread { get; set; }

		/// <summary>
		/// Whether to handle exceptions in item action code. If false (default), handles exceptions and on exception calls <see cref="AWarning.Write"/>.
		/// This property is applied to items added afterwards.
		/// </summary>
		public bool ActionException { get; set; }

		//protected private int _SourceLine(MTItem x) => x?.sourceLine ?? _sourceLine;
		protected private string _SourceLink(MTItem x, string text) => _sourceFile == null ? null : $"<open {_sourceFile}|{x.sourceLine}>{text}<>";

		/// <summary>
		/// Converts x.image (object containing string, Image, etc or null) to Image. Extracts icon path from code if need.
		/// </summary>
		protected private (Image image, bool dispose) _GetImage(MTItem x) {
			Image im = null; bool dontDispose = false;
			g1:
			switch (x.image) {
			case Image g: im = g; dontDispose = true; break;
			case string s when s.Length > 0:
				try {
					if (dontDispose = ImageCache != null) im = ImageCache.Get(s, _dpi, onException: _OnException);
					else if (AImageUtil.HasImageOrResourcePrefix(s)) im = AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(s, (new(16, 16), _dpi));
					else im = AIcon.OfFile(s).ToGdipBitmap();

					if (im == null) _OnException(s, null);
				}
				catch (Exception e1) { _OnException(null, e1); }

				void _OnException(string s, Exception e) {
					AOutput.Write($"<>Failed to load image. {e?.ToStringWithoutStack() ?? s}. {_SourceLink(x, "Edit")}");
				}
				break;
			case StockIcon si:
				im = AIcon.Stock(si).ToGdipBitmap();
				break;
			case null:
				if (x.extractIconPath == 1 && x.clicked != null) {
					x.file = AIcon.IconPathFromCode_(x.clicked.Method, out bool cs);
					if (x.file != null && !cs) { x.image = x.file; x.extractIconPath = 2; } else x.extractIconPath = cs ? 4 : 3;
					//				APerf.Next('c');
				}
				//if(x.image==null && x.checkType==0) x.image = (x.submenu ? DefaultSubmenuImage : DefaultImage).Value;
				if (x.image != null) goto g1;
				break;
			}
			return (im, im != null && !dontDispose);
		}

		/// <summary>
		/// Base of <see cref="AMenu.MenuItem"/> etc.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract class MTItem
		{
			internal Delegate clicked;
			internal object image;
			/// <summary>1 if need to extract, 2 if already extracted (the image field is the path), 3 if failed to extract, 4 if extracted "script.cs"</summary>
			internal byte extractIconPath; //from MTBase.ExtractIconPathFromCode
			internal bool actionThread; //from MTBase.ActionThread
			internal bool actionException; //from MTBase.ActionException
			internal int sourceLine;
			internal string file;

			/// <summary>
			/// Item text.
			/// </summary>
			public string Text { get; set; }

			/// <summary>
			/// Item tooltip.
			/// </summary>
			public string Tooltip { get; set; }

			/// <summary>
			/// Any value. Not used by this library.
			/// </summary>
			public object Tag { get; set; }

			/// <summary>
			/// Gets file path extracted from item action code or sets file path as it would be extracted from action code.
			/// </summary>
			/// <remarks>
			/// Can be used to set file path when it cannot be extracted from action code (the item does not have an action or action code does not contain the path string). See <see cref="MTBase.ExtractIconPathFromCode"/>.
			/// When you set this property, the menu/toolbar item uses icon of the specified file, and its context menu contains "Find file".
			/// </remarks>
			public string File {
				get => file;
				set {
					file = value;
					if (file == null) {
						image = null; extractIconPath = 3;
					} else if (file.Ends(".cs") && !APath.IsFullPath(file)) {
						image = null; extractIconPath = 4;
					} else {
						image = file; extractIconPath = 2;
					}
				}
			}

			internal void GoToFile_() {
				if (file.NE()) return;
				if (extractIconPath == 2) AFile.SelectInExplorer(file);
				else AScriptEditor.GoToEdit(file, 0);
			}

			internal static (bool edit, bool go, string goText) CanEditOrGoToFile_(string _sourceFile, MTItem item) {
				if (_sourceFile != null) {
					if (AScriptEditor.Available) {
						if (item?.file == null) return (true, false, null);
						return (true, true, item.extractIconPath == 2 ? "Find file" : "Open script");
					} else if (item != null && item.extractIconPath == 2) {
						return (false, true, "Find file");
					}
				}
				return default;
			}

			/// <summary>
			/// Call when adding menu/toolbar item.
			/// Sets text and tooltip (from text). Sets clicked, image and sourceLine fields.
			/// Sets extractIconPath, actionThread and actionException fields from mt properties.
			/// </summary>
			internal void Set_(MTBase mt, string text, Delegate click, MTImage im, int l) {
				if (!text.NE()) {
					int i = text.IndexOf('\0');
					if (i >= 0) {
						int j = i + 1; if (text.Eq(j, ' ')) j++;
						Tooltip = text[j..];
						text = text[..i];
					}
				}
				if (mt is AMenu) Text = text ?? ""; else if (!text.NE()) Text = text;

				image = im.Value;
				if (image is AIcon ic) { image = ic.ToGdipBitmap(); image ??= ""; } //DestroyIcon now; don't extract from code.

				clicked = click;
				sourceLine = l;

				extractIconPath = mt.ExtractIconPathFromCode ? 1 : 0;
				actionThread = mt.ActionThread;
				actionException = mt.ActionException;
			}

			///
			public override string ToString() => Text;
		}

	}

}

namespace Au.Types
{
	/// <summary>
	/// Used for menu/toolbar function parameters to specify an image in different ways (file path, Image object, etc).
	/// </summary>
	/// <remarks>
	/// Has implicit conversions from string, <see cref="Image"/>, <see cref="AIcon"/>, <see cref="StockIcon"/>, <see cref="FolderPath"/>.
	/// More info: <see cref="MTBase"/>.
	/// </remarks>
	public struct MTImage
	{
		readonly object _o;
		MTImage(object o) { _o = o; }

		///
		public static implicit operator MTImage(string pathEtc) => new MTImage(pathEtc);
		///
		public static implicit operator MTImage(Image image) => new MTImage(image);
		///
		public static implicit operator MTImage(AIcon icon) => new MTImage(icon.ToGdipBitmap());
		///
		public static implicit operator MTImage(StockIcon icon) => new MTImage(icon);
		///
		public static implicit operator MTImage(FolderPath path) => new MTImage((string)path);

		/// <summary>
		/// Gets the raw value stored in this variable. Can be string, Image, StockIcon or null.
		/// </summary>
		public object Value => _o;
	}
}