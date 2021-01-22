using Au;
using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Au.Util
{
	/// <summary>
	/// Loads <see cref="Bitmap"/> images of size 16x16 from files/resources/strings. Also can get file icons.
	/// Uses memory cache to avoid loading same image multiple times.
	/// </summary>
	public sealed class AIconImageCache : IDisposable
	{
		List<(int dpi, Dictionary<string, object> images)> _images; //object is Bitmap if loaded, or null if failed to load, or List<(Action<Bitmap, object>, object)> if loading async
		const int c_imageSize = 16;

		/// <summary>
		/// Gets image from memory cache or file.
		/// </summary>
		/// <param name="imageSource">File path, or resource path that starts with "resources/" or has prefix "resource:", etc.</param>
		/// <param name="dpi">DPI of window that will display the image.</param>
		/// <param name="isImage">
		/// true - load image from xaml/png/etc file, resource or string with <see cref="AImageUtil.LoadGdipBitmapFromFileOrResourceOrString"/> or <see cref="AImageUtil.LoadWpfImageElementFromFileOrResourceOrString"/>.
		/// false - get file icon with <see cref="AIcon.OfFile"/>.
		/// null (default) - call <see cref="AImageUtil.HasImageOrResourcePrefix"/> to determine whether it is image.
		/// </param>
		/// <param name="asyncCompletion">If not null, if the image is still not in cache, <b>Get</b> returns null and loads it asynchronously in other thread. When loaded, calls <i>asyncCompletion</i> in this thread. Can be used to avoid slow startup when need to get many file icons.</param>
		/// <param name="acData">Something to pass to the <i>asyncCompletion</i> callback function. The function is called once for a unique asyncCompletion/acData.</param>
		/// <param name="onException">Action to call when fails to load image. If null, calls <see cref="AWarning.Write"/>. Parameters are image source string and exception.</param>
		/// <remarks>
		/// When <i>isImage</i> false, returns same <b>Bitmap</b> object for all files of that type, except if file type is ico, exe, scr, lnk.
		/// </remarks>
		public Bitmap Get(string imageSource, int dpi, bool? isImage = null, Action<Bitmap, object> asyncCompletion = null, object acData = null, Action<string, Exception> onException = null) {
			bool isIm = isImage ?? AImageUtil.HasImageOrResourcePrefix(imageSource);
			if (!isIm) {
				switch (AFile.ExistsAs(imageSource)) {
				case FileDir.Directory:
					imageSource = ".";
					break;
				case FileDir.File when !_IsIconSource(imageSource):
					int j = APath.FindExtension(imageSource);
					if (j >= 0) imageSource = imageSource[j..]; else imageSource = ".*";
					break;
				}
			}

			//get dictionary for dpi
			bool isXaml = isIm && imageSource.Ends(".xaml", true);
			if (!isXaml) dpi = 96;
			_images ??= new List<(int dpi, Dictionary<string, object> images)>();
			int i; for (i = 0; i < _images.Count; i++) if (_images[i].dpi == dpi) goto g1;
			_images.Add((dpi, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)));
			g1:
			var d = _images[i].images;

			//find or load
			if (!d.TryGetValue(imageSource, out var ba)) {
				if (asyncCompletion != null) {
					d[imageSource] = new List<(Action<Bitmap, object>, object)> { (asyncCompletion, acData) }; //prevent async-load same image multiple times
					Task.Factory
						.StartNew(() => _Load(), default, 0, StaTaskScheduler_.Default)
						.ContinueWith(task => {
							var b = task.Result;
							//AOutput.Write(imageSource, d[imageSource]);
							if (_images != null && d[imageSource] is List<(Action<Bitmap, object>, object)> a) _AsyncToBitmap(b, a, false);
							else b?.Dispose();
						}, TaskScheduler.FromCurrentSynchronizationContext());
				} else {
					var b = _Load();
					d[imageSource] = b;
					return b;
				}
			} else if (ba is Bitmap b) {
				return b;
			} else if (ba is List<(Action<Bitmap, object>, object)> a) {
				if (asyncCompletion != null) {
					var t = (asyncCompletion, acData);
					foreach (var v in a) if (v == t) return null;
					a.Add(t);
				} else {
					b = _Load();
					_AsyncToBitmap(b, a, true); //call callbacks async, because now may be unsafe
					return b;
				}
			} //else failed to load
			return null;

			Bitmap _Load() {
				Bitmap b = null;
				try {
					if (!isIm) b = AIcon.OfFile(imageSource).ToGdipBitmap();
					else if (isXaml) b = AImageUtil.LoadGdipBitmapFromXaml(imageSource, (c_imageSize, c_imageSize), dpi);
					else b = AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(imageSource);
				}
				catch (Exception ex) { if (onException != null) onException(imageSource, ex); else AWarning.Write(ex.ToStringWithoutStack()); }
				return b;
			}

			void _AsyncToBitmap(Bitmap b, List<(Action<Bitmap, object>, object)> a, bool post) {
				d[imageSource] = b;
				if (b == null) return;
				var c = post ? SynchronizationContext.Current : null;
				if (c != null) c.Post(o => { foreach (var v in a) v.Item1(b, v.Item2); }, null);
				else foreach (var v in a) v.Item1(b, v.Item2);
			}
		}

		/// <summary>
		/// Disposes all image objects.
		/// </summary>
		public void Dispose() {
			if (_images != null) {
				foreach (var (_, images) in _images) {
					foreach (var v in images.Values) if (v is Bitmap b) b.Dispose();
				}
				_images = null;
			}
		}

		static bool _IsIconSource(string s) {
			if (s.Ends(".ico", true) || s.Ends(".exe", true) || s.Ends(".scr", true) || s.Ends(".lnk", true)) return true;
			if (!AIcon.ParsePathIndex(s, out string path, out _)) return false;
			return path.Ends(".dll", true) || path.Ends(".exe", true);
		}
	}
}
