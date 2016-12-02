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

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Editor
{
	//[DebuggerStepThrough]
	public static class ImageList_
	{
		/// <summary>
		/// Loads imagelist from Image, eg png resource.
		/// Appends if the ImageList is not empty.
		/// Calls <see cref="ImageList.ImageCollection.AddStrip"/> and returns its return value (index of the first new image).
		/// </summary>
		/// <param name="pngImage">Png image as horizontal strip of images, eg Properties.Resources.il_tv. Don't dispose, because ImageList will use it later on demand (lazy).</param>
		public static int LoadFromImage_(this ImageList t, Image pngImage)
		{
			t.ColorDepth = ColorDepth.Depth32Bit;
			return t.Images.AddStrip(pngImage);
		}

		/// <summary>
		/// Loads imagelist from a png file.
		/// Appends if the ImageList is not empty.
		/// Calls <see cref="Image.FromFile"/>, then <see cref="ImageList.ImageCollection.AddStrip"/> and returns its return value (index of the first new image).
		/// Exception if fails, eg file not found or bad format.
		/// </summary>
		/// <param name="pngFile">.png file as horizontal strip of images. Actually also can be .bmp etc, but then possible problems with transparency.</param>
		/// <remarks>
		/// This is faster than LoadFromImageResource_ if managed resources are not used altogether in the project. Else it is by 1 ms slower.
		/// </remarks>
		public static int LoadFromImageFile_(this ImageList t, string pngFile)
		{
			int R = 0;
			t.ColorDepth = ColorDepth.Depth32Bit;
			using(Image img = Image.FromFile(pngFile)) {
				R=t.Images.AddStrip(img);
				//OutList(il.ImageSize, il.Images.Count);
				var h = t.Handle; //workaround for the lazy ImageList behavior that causes exception later because the Image is disposed when actually used
			}
			return R;

			//This is the fastest found way to load an imagelist when the project does not use managed resources. 3ms, tested non-ngened, with SSD.
			//The .png file can be created with QM2 macro "Create png imagelist strip from icons".
			//With .bmp also works, but need to use a transparent color, eg il.TransparentColor=Color.Black;
			//With multiple ico files ~24 ms, tested non-ngened.
			//With managed resources slow, ~40 ms first time, tested non-ngened. But slightly faster if managed resources were already used.
			//The same (~40 ms) with a designer-added ImageList with designer-added images. Also then noticed an anomaly when closing the form.
		}
	}
}
