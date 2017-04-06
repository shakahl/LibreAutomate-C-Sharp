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
using static Catkeys.NoClass;

//[DebuggerStepThrough]
public static class EResources
{
	/// <summary>
	/// Call this from other thread (than the UI thread that will use resources) at the very startup of the app.
	/// It makes the UI thread start faster.
	/// </summary>
	public static void Init()
	{
		//if(_initializingResources == 0) _initializingResources = 1;
		//var p = new Perf.Inst(true);
		Project.Properties.Resources.Culture = System.Globalization.CultureInfo.InvariantCulture; //makes 3 times faster. Default culture is en-US.
																						  //p.Next();
		GetImage("_new"); //5-8 ms. Without InvariantCulture 15-20 ms.

		//var b = Properties.Resources.il_tv;
		//p.Next();
		//_initializingResources = 2;
		//ImageList il = new ImageList();
		//il.LoadFromImage(b);
		//p.NW();
		//_ilFile = il;
	}
	//static int _initializingResources;
	//static ImageList _ilFile;

	///// <summary>
	///// Gets ImageList used for the files/scripts list pane.
	///// </summary>
	//public static ImageList ImageList_Files
	//{
	//	get
	//	{
	//		if(_ilFile == null) DebugPrint("need to wait for __ilFile");
	//		while(_ilFile == null) Thread.Sleep(5);
	//		return _ilFile;
	//	}
	//}

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

	/// <summary>
	/// Gets a non-string resource (eg Bitmap) from project resources.
	/// Uses memory cache. Gets the same object when called multiple times for the same name.
	/// Calling ResourceManager.GetObject would create object copies.
	/// Thread-safe.
	/// If not found (bad name), asserts and returns null.
	/// </summary>
	/// <param name="name">Resource name. If fileIcon - file path.</param>
	public static object GetObject(string name)
	{
		lock("3moj2pOaoUGgRILqYfBGPw") {
			object R;
			if(_cache == null) {
				_cache = new Hashtable();
			} else {
				R = _cache[name];
				if(R != null) return R;
			}
			//#if DEBUG
			//				if(_initializingResources == 1) DebugPrint($"warning: the resource initialization thread still not finished. The first get-resource then is slow.");
			//#endif
			//var p = new Perf.Inst(true);
			R = Project.Properties.Resources.ResourceManager.GetObject(name, Project.Properties.Resources.Culture);
			//p.NW();
			Debug.Assert(R != null);
			if(R != null) _cache[name] = R;
			return R;
		}
	}
	static Hashtable _cache;

	/// <summary>
	/// Gets a Bitmap resource from project resources.
	/// Calls <see cref="GetObject"/> and casts to Bitmap.
	/// </summary>
	/// <param name="name">Image resource name.</param>
	public static Bitmap GetImage(string name)
	{
		return GetObject(name) as Bitmap;
	}
}
