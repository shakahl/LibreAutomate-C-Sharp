using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;
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
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

/// <summary>
/// Misc util functions.
/// </summary>
static class EdUtil
{
	public static void MinimizeProcessPhysicalMemory(int afterMS)
	{
		Task.Delay(afterMS).ContinueWith(_ => {
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), -1, -1);
		});
	}
}

/// <summary>
/// Extension methods for .NET classes.
/// See also <see cref="AExtensions"/>.
/// </summary>
static class EdNetExtensions
{
	/// <summary>
	/// Shows ToolStripDropDownMenu menu at the cursor or caret position, like it would be a ContextMenuStrip menu.
	/// </summary>
	/// <remarks>
	/// This way is undocumented and possibly will stop working in future .NET versions. I did not find a better way.
	/// </remarks>
	public static void ZShowAsContextMenu(this ToolStripDropDownMenu t, bool caret = false)
	{
		POINT p; if(caret && Api.GetCaretPos(out p)) Api.GetFocus().MapClientToScreen(ref p); else p = AMouse.XY;
		var oi = t.OwnerItem; t.OwnerItem = null; //to set position
		try { t.Show(p); }
		finally { t.OwnerItem = oi; }
	}

	///// <summary>
	///// Checks or unchecks item by name.
	///// See also: <see cref="FMain.CheckCmd"/>
	///// </summary>
	///// <exception cref="NullReferenceException">Item does not exist.</exception>
	//public static void ZCheckItem(this ToolStripDropDownMenu t, string itemName, bool check)
	//{
	//	(t.Items[itemName] as ToolStripMenuItem).Checked = check;
	//}

}

static class EdResources
{
	//static EdResources()
	//{
	//	//rejected. Instead use [assembly: NeutralResourcesLanguage("en-US")].
	//	//This makes the first access of managed resources 3 times faster. Then app starts ~10 ms faster. Default culture is en-US.
	//	Au.Editor.Properties.Resources.Culture = System.Globalization.CultureInfo.InvariantCulture;
	//}

#if false //currently not used. Imagelists have problems with high DPI.
	/// <summary>
	/// Loads imagelist from Image.
	/// Appends if the ImageList is not empty.
	/// Calls <see cref="ImageList.ImageCollection.AddStrip"/>.
	/// </summary>
	/// <param name="image">Horizontal strip of images, eg Properties.Resources.il_tv.</param>
	public static void LoadFromImage(this ImageList t, Image image)
	{
		t.ColorDepth = ColorDepth.Depth32Bit;
		int k = image.Height; t.ImageSize = new Size(k, k); //AddStrip throws exception if does not match
		t.Images.AddStrip(image);
		var h = t.Handle; //workaround for the lazy ImageList behavior that causes exception later because the Image is disposed when actually used

		//note: could add imageSize parameter and resize the image if need.
		//	But it is slow, and also need to create a Resize method.
		//	Better use several image files with different icon sizes.
	}

	/// <summary>
	/// Loads imagelist from a file, eg .png.
	/// Appends if the ImageList is not empty.
	/// Calls <see cref="Image.FromFile"/>, then <see cref="ImageList.ImageCollection.AddStrip"/>.
	/// Exception if fails, eg file not found or bad format.
	/// </summary>
	/// <param name="file">File containing horizontal strip of images, eg .png.</param>
	/// <remarks>
	/// This is faster than loading from managed resources if managed resources are not used altogether in the project. Else it is by 1 ms slower.
	/// </remarks>
	public static void LoadFromImageFile(this ImageList t, string file)
	{
		using(Image img = Image.FromFile(file)) {
			t.LoadFromImage(img);
		}

		//This is the fastest found way to load an imagelist when the project does not use managed resources. 3ms, tested non-ngened, with SSD.
		//The .png file can be created with a function in DevTools.cs.
		//With .bmp also works, but need to use a transparent color, eg il.TransparentColor=Color.Black;
		//With multiple ico files ~24 ms, tested non-ngened.
		//With managed resources slow, ~40 ms first time, tested non-ngened. But slightly faster (than file) if managed resources were already used.
		//The same (~40 ms) with a designer-added ImageList with designer-added images. Also then noticed an anomaly when closing the form.
	}
#endif

	/// <summary>
	/// Gets a non-string resource (eg Bitmap) from project resources or cache. Don't dispose it.
	/// If not found (bad name), returns null.
	/// </summary>
	/// <param name="name">Resource name. Use <c>nameof(Au.Editor.Properties.Resources.name)</c>.</param>
	/// <remarks>
	/// Uses memory cache. Gets the same object when called multiple times for the same name. Don't dispose it.
	/// Calling Au.Editor.Properties.Resources.name or ResourceManager.GetObject would create object copies.
	/// Thread-safe.
	/// </remarks>
	public static object GetObjectUseCache(string name) => s_cache.GetOrAdd(name, n => GetObjectNoCache(n));
	static ConcurrentDictionary<string, object> s_cache = new ConcurrentDictionary<string, object>();
	//note: don't use WeakReference for s_cache. Maybe could use weekreferences for each image etc, but not tested whether it is good.

	/// <summary>
	/// Gets a Bitmap resource from project resources or cache. Don't dispose it.
	/// If not found (bad name), returns null.
	/// Calls <see cref="GetObjectUseCache"/> and casts to Bitmap.
	/// </summary>
	/// <param name="name">Image resource name. Use <c>nameof(Au.Editor.Properties.Resources.name)</c>.</param>
	public static Bitmap GetImageUseCache(string name) => GetObjectUseCache(name) as Bitmap;

	/// <summary>
	/// Same as <see cref="GetImageUseCache"/>, but appends suffix "_20" or "_24" or "_32" if high DPI.
	/// </summary>
	public static Bitmap GetImageUseCacheDpi(string name) => GetImageUseCache(_DpiImage(name));

	/// <summary>
	/// Gets a non-string resource (eg Bitmap) from project resources. Each time returns a new copy.
	/// If not found (bad name), returns null.
	/// </summary>
	/// <param name="name">Resource name. Use <c>nameof(Au.Editor.Properties.Resources.name)</c>.</param>
	public static object GetObjectNoCache(string name) => Au.Editor.Properties.Resources.ResourceManager.GetObject(name, Au.Editor.Properties.Resources.Culture);

	/// <summary>
	/// Gets a Bitmap resource from project resources. Each time returns a new copy.
	/// If not found (bad name), returns null.
	/// </summary>
	/// <param name="name">Image resource name. Use <c>nameof(Au.Editor.Properties.Resources.name)</c>.</param>
	public static Bitmap GetImageNoCache(string name) => GetObjectNoCache(name) as Bitmap;

	/// <summary>
	/// Same as <see cref="GetImageNoCache"/>, but appends suffix "_20" or "_24" or "_32" if high DPI.
	/// </summary>
	public static Bitmap GetImageNoCacheDpi(string name) => GetImageNoCache(_DpiImage(name));

	static string _DpiImage(string name)
	{
		int dpi = Au.Util.ADpi.BaseDPI;
		if(dpi >= 120) name += dpi < 144 ? "_20" : (dpi < 192 ? "_24" : "_32");
		return name;
	}
}

/// <summary>
/// Various cached GDI+ objects etc.
/// </summary>
static class EdStock
{
	/// <summary>
	/// Cached standard font used by most windows and controls.
	/// On Windows 10 it is "Segoe UI" 9 by default.
	/// </summary>
	public static readonly Font FontRegular = Au.Util.AFonts.Regular;

	/// <summary>
	/// Bold version of <see cref="Regular"/> font.
	/// </summary>
	public static readonly Font FontBold = Au.Util.AFonts.OfStyle(FontStyle.Bold);

	static Icon _iconAppNormal, _iconTrayNormal, _iconTrayDisabled, _iconTrayRunning;

	public static Icon IconAppNormal => _iconAppNormal ??= Au.Editor.Properties.Resources.app_normal; //contains icons of multiple sizes

	public static Icon IconTrayNormal => _iconTrayNormal ??= _Icon(IconAppNormal);

	public static Icon IconAppDisabled => _iconTrayDisabled ??= _Icon(Au.Editor.Properties.Resources.app_disabled);

	public static Icon IconAppRunning => _iconTrayRunning ??= _Icon(Au.Editor.Properties.Resources.app_running);

	static Icon _Icon(Icon icon)
	{
		int size = AIcon.GetShellIconSize(IconSize.SysSmall);
		return new Icon(icon, size, size);
	}

	//rejected. Use EdResources.
	//static Image _imageNamespace, _imageClass, _imageStruct, _imageEnum, _imageDelegate, _imageInterface,
	//	_imageMethod, _imageProperty, _imageEvent, _imageField, _imageConst, _imageVar, _imageKeyword, _imageSnippet;
	//public static Image ImageNamespace => _imageNamespace ??= Au.Editor.Properties.Resources.Namespace_16x;
}

/// <summary>
/// Static XName instances for XML tag/attribute names used in this app.
/// XML functions much faster with XName than with string. When with string, .NET looks for it in its static hashtable.
/// The variable names match the strings.
/// </summary>
class XN
{
	public static readonly XName f = "f", n = "n";
}

#if DEBUG

static class EdDebug
{
	public static void PrintTabOrder(Control c, int level = 0)
	{
		var tabs = "".PadLeft(level, ' ');
		Print($"{tabs}{c.GetType().Name} \"{c.Name}\"  {c.TabStop} {c.TabIndex}");
		foreach(Control v in c.Controls) PrintTabOrder(v, level + 1);
	}
}

#endif
