using Au;
using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Au.Util
{
	/// <summary>
	/// Loads <see cref="Bitmap"/> images of size 16x16 from files/resources/strings. Also can get file icons.
	/// Uses memory cache to avoid loading same image multiple times.
	/// </summary>
	public sealed class AIconImageCache : IDisposable
	{
		List<(int dpi, Dictionary<string, BitmapAsync> images)> _images;
		const int _imageSize = 16;

		class BitmapAsync
		{
			public Bitmap b;
			public List<(Action<Bitmap, object>, object)> a;
			public bool failed;

			public void Call() {
				var k = a; a = null;
				if (b != null) foreach (var v in k) v.Item1(b, v.Item2);
			}
		}

		/// <summary>
		/// Gets image from memory cache or file.
		/// </summary>
		/// <param name="imageSource">File path, or resource path with prefix "resource:", etc.</param>
		/// <param name="dpi">DPI of window that will display the image.</param>
		/// <param name="isImage">
		/// true - load image from xaml/png/etc file, resource or string with <see cref="AImageUtil.LoadWinformsImageFromFileOrResourceOrString"/> or <see cref="AImageUtil.LoadWpfImageElementFromFileOrResourceOrString"/>.
		/// false - get file icon with <see cref="AIcon.OfFile"/>.</param>
		/// <param name="asyncCompletion">If not null, <b>Get</b> returns null and loads the image/icon asynchronously in other thread. When loaded, calls the callback function in this thread. Can be used to avoid slow startup when need to get many file icons.</param>
		/// <param name="acData">Something to pass to the <i>asyncCompletion</i> callback function.</param>
		/// <remarks>
		/// When <i>isImage</i> false, returns same <b>Bitmap</b> object for all files of that type, except if file type is ico, exe, scr, lnk.
		/// </remarks>
		public Bitmap Get(string imageSource, int dpi, bool isImage, Action<Bitmap, object> asyncCompletion = null, object acData = null) {
			//get dictionary for dpi
			bool isXaml = isImage && imageSource.Ends(".xaml", true);
			if (!isXaml) dpi = 96;
			_images ??= new List<(int dpi, Dictionary<string, BitmapAsync> images)>();
			int i; for (i = 0; i < _images.Count; i++) if (_images[i].dpi == dpi) goto g1;
			_images.Add((dpi, new Dictionary<string, BitmapAsync>(StringComparer.OrdinalIgnoreCase)));
			g1:
			var d = _images[i].images;
			//find or load
			if (!isImage) {
				switch (AFile.ExistsAs(imageSource)) {
				case FileDir.Directory:
					imageSource = ".";
					break;
				case FileDir.File when !_IsIconSource(imageSource):
					i = APath.FindExtension(imageSource);
					if (i >= 0) imageSource = imageSource[i..];
					else imageSource = ".*";
					break;
				}
			}
			//AOutput.Write(imageSource);
			if (!d.TryGetValue(imageSource, out var ba)) {
				if (asyncCompletion != null) {
					d[imageSource] = ba = new BitmapAsync { a = new List<(Action<Bitmap, object>, object)> { (asyncCompletion, acData) } }; //prevent async-load same image multiple times
					Task.Factory
						.StartNew(() => _Load(), default, 0, StaTaskScheduler_.Default)
						.ContinueWith(task => {
							var b = task.Result;
							//AOutput.Write(imageSource);
							if (ba.a != null && _images != null) {
								ba.failed = null == (ba.b = b);
								ba.Call();
							} else b?.Dispose();
						}, TaskScheduler.FromCurrentSynchronizationContext());
				} else {
					var b = _Load();
					d[imageSource] = new BitmapAsync { b = b, failed = b == null };
					return b;
				}
			} else if (ba.a != null && asyncCompletion != null) {
				Debug.Assert(ba.b == null && !ba.failed);
				ba.a.Add((asyncCompletion, acData));
			} else {
				if (ba.b == null && !ba.failed) ba.failed = null == (ba.b = _Load());
				if (ba.a != null) ba.Call();
				return ba.b;
			}
			return null;

			Bitmap _Load() {
				Bitmap b = null;
				try {
					if (!isImage) b = AIcon.OfFile(imageSource).ToWinformsBitmap();
					else if (isXaml) b = _LoadXamlBitmap(imageSource, _imageSize, dpi);
					else b = AImageUtil.LoadWinformsImageFromFileOrResourceOrString(imageSource);
				}
				catch (Exception ex) { AWarning.Write(ex.ToStringWithoutStack()); }
				return b;
			}
		}

		/// <summary>
		/// Disposes all image objects.
		/// </summary>
		public void Dispose() {
			if (_images != null) {
				foreach (var d in _images) {
					foreach (var v in d.images.Values) v.b?.Dispose();
				}
				_images = null;
			}
		}

		static bool _IsIconSource(string s) {
			if (s.Ends(".ico", true) || s.Ends(".exe", true) || s.Ends(".scr", true) || s.Ends(".lnk", true)) return true;
			if (!AIcon.ParsePathIndex(s, out string path, out _)) return false;
			return path.Ends(".dll", true) || path.Ends(".exe", true);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		static Bitmap _LoadXamlBitmap(string imageSource, int size, int dpi) {
			var e = AImageUtil.LoadWpfImageElementFromFileOrResourceOrString(imageSource);
			e.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
			e.Arrange(new System.Windows.Rect(e.DesiredSize));
			int wid = ADpi.Scale(size, dpi), hei = wid;
			var bs = new System.Windows.Media.Imaging.RenderTargetBitmap(wid, hei, dpi, dpi, System.Windows.Media.PixelFormats.Pbgra32);
			bs.Render(e);
			int stride = wid * 4;
			int msize = hei * stride;
			var m = new _BitmapMemory(msize);
			bs.CopyPixels(new System.Windows.Int32Rect(0, 0, wid, hei), m.pixels, msize, stride);
			var b = new Bitmap(wid, hei, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, m.pixels) { Tag = m }; //only this Bitmap creation method preserves alpha
			b.SetResolution(dpi, dpi);
			return b;
		}

		//Holds memory of System.Drawing.Bitmap created with the scan ctor. Such Bitmap does not own/free the memory. We attach _BitmapMemory to Bitmap.Tag, let GC dispose it.
		unsafe class _BitmapMemory
		{
			public readonly IntPtr pixels;
			public _BitmapMemory(int size) { pixels = (IntPtr)AMemory.Alloc(size); }
			~_BitmapMemory() { AMemory.Free((void*)pixels); }
		}
	}
}
