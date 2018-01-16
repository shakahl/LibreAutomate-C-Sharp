using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Xml;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;

/// <summary>
/// Extension methods for .NET classes.
/// See also <see cref="ExtensionMethods"/>.
/// </summary>
static class EDotNetExtensions
{
	/// <summary>
	/// Shows ToolStripDropDownMenu menu at current mouse position, like it would be a ContextMenuStrip menu.
	/// </summary>
	/// <remarks>
	/// This way is undocumented and possibly will stop working in future .NET versions. I did not find a better way.
	/// </remarks>
	public static void ShowAsContextMenu_(this ToolStripDropDownMenu t)
	{
		var oi = t.OwnerItem; t.OwnerItem = null; //to set position
		try { t.Show(Mouse.XY); }
		finally { t.OwnerItem = oi; }
	}

	///// <summary>
	///// Checks or unchecks item by name.
	///// See also: <see cref="EForm.CheckCmd"/>
	///// </summary>
	///// <exception cref="NullReferenceException">Item does not exist.</exception>
	//public static void CheckItem(this ToolStripDropDownMenu t, string itemName, bool check)
	//{
	//	(t.Items[itemName] as ToolStripMenuItem).Checked = check;
	//}

}

//[DebuggerStepThrough]
static class EResources
{
	static EResources()
	{
		//this makes the first access of managed resources 3 times faster. Then app starts ~10 ms faster. Default culture is en-US.
		Project.Properties.Resources.Culture = System.Globalization.CultureInfo.InvariantCulture;
	}

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
	/// Gets a non-string resource (eg Bitmap) from project resources.
	/// Uses memory cache. Gets the same object when called multiple times for the same name. Don't dispose it.
	/// Calling ResourceManager.GetObject would create object copies.
	/// Thread-safe.
	/// If not found (bad name), asserts and returns null.
	/// </summary>
	/// <param name="name">Resource name. If fileIcon - file path.</param>
	public static object GetObjectUseCache(string name)
	{
		lock(_cache) {
			object R = _cache[name];
			if(R != null) return R;
			//var p = Perf.StartNew();
			R = Project.Properties.Resources.ResourceManager.GetObject(name, Project.Properties.Resources.Culture);
			//p.NW();
			Debug.Assert(R != null);
			if(R != null) _cache[name] = R;
			return R;
		}
	}
	static Hashtable _cache = new Hashtable();

	/// <summary>
	/// Gets a Bitmap resource from project resources or cache.
	/// Uses memory cache. Gets the same object when called multiple times for the same name. Don't dispose it.
	/// Calls <see cref="GetObjectUseCache"/> and casts to Bitmap.
	/// </summary>
	/// <param name="name">Image resource name.</param>
	public static Bitmap GetImageUseCache(string name)
	{
		return GetObjectUseCache(name) as Bitmap;
	}

	/// <summary>
	/// Gets a Bitmap resource from project resources.
	/// If called multiple times, creates new object each time.
	/// </summary>
	/// <param name="name">Image resource name.</param>
	public static Bitmap GetImageNoCache(string name)
	{
		return Project.Properties.Resources.ResourceManager.GetObject(name, Project.Properties.Resources.Culture) as Bitmap;
	}
}
