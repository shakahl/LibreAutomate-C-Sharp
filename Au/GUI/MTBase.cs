using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;

namespace Au {
/// <summary>
/// Base class of <see cref="AMenu"/> and <see cref="AToolbar"/>.
/// </summary>
/// <remarks>
/// <i>image</i> argument of "add item" functions can be:
/// - file/folder path (string) - the "show" function calls <see cref="AIcon.OfFile"/> to get its icon. It also supports file type icons like ".txt", etc.
/// - file path with prefix "imagefile:" or resource path that starts with "resources/" or has prefix "resource:" - the "show" function loads .png or .xaml image file or resource.
/// - <see cref="FolderPath"/> - same as folder path string.
/// - <see cref="System.Drawing.Image"/> - image object.
/// - <see cref="AIcon"/> - variable containing native icon handle. The "add item" function disposes it.
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
public abstract class MTBase {
	protected private readonly string _sourceFile;
	protected private readonly int _sourceLine;
	protected private int _dpi;
	
	internal MTBase() {  }
	
	internal MTBase(string name, string f, int l) {
		_sourceFile = f;
		_sourceLine = l;
		ExtractIconPathFromCode=true;
	}

	/// <summary>
	/// When adding items without explicitly specified image, extract file path from item action code (for example <see cref="AFile.Run"/> argument) and use icon of that file.
	/// This property is applied to items added afterwards.
	/// </summary>
	/// <remarks>
	/// Gets icon path from code that contains string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%AFolders.System%\notepad.exe"</c> or URL/shell.
	/// Also supports code patterns like <c>AFolders.System + "notepad.exe"</c> or <c>AFolders.Virtual.RecycleBin</c>.
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
		Image im=null; bool dontDispose=false;
		g1:
		switch(x.image) {
		case Image g: im=g; dontDispose=true; break;
		case string s when s.Length>0:
			try {
				if(dontDispose=ImageCache!=null) im=ImageCache.Get(s, _dpi, onException: _OnException);
				else if(AImageUtil.HasImageOrResourcePrefix(s)) im=AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(s, (new(16, 16), _dpi));
				else im=AIcon.OfFile(s).ToGdipBitmap();
				
				if(im==null) _OnException(s, null);
			}
			catch(Exception e1) { _OnException(null, e1); }
			
			void _OnException(string s, Exception e) {
				AOutput.Write($"<>Failed to load image. {e?.ToStringWithoutStack() ?? s}. {_SourceLink(x, "Edit")}");
			}
			break;
		case StockIcon si:
			im=AIcon.Stock(si).ToGdipBitmap();
			break;
		case null:
			if(x.extractIconPath && x.clicked!=null) {
				x.extractIconPath=false;
				x.image=AIcon.IconPathFromCode_(x.clicked.Method);
//				APerf.Next('c');
			}
			//if(x.image==null && x.checkType==0) x.image = (x.submenu ? DefaultSubmenuImage : DefaultImage).Value;
			if(x.image!=null) goto g1;
			break;
		}
		return (im, im!=null && !dontDispose);
	}
	
	/// <summary>
	/// Base of <see cref="AMenu.MenuItem"/> etc.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class MTItem {
		internal Delegate clicked;
		internal object image;
		internal bool separator;
		internal bool extractIconPath; //from MTBase.ExtractIconPathFromCode
		internal bool actionThread; //from MTBase.ActionThread
		internal bool actionException; //from MTBase.ActionException
		internal int sourceLine;
		
		/// <summary>
		/// Item text.
		/// </summary>
		public string Text { get; set; }
		
		/// <summary>
		/// Any value. Not used by this library.
		/// </summary>
		public object Tag { get; set; }
		
		internal void SetImage_(MTImage i) {
			image=i.Value;
			if(image is AIcon ic) { image=ic.ToGdipBitmap(); image??=""; } //DestroyIcon now; don't extract from code.
		}
		
		///
		public override string ToString() => Text;
	}
	
}

}