using Au;
using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Au.Util
{
	/// <summary>
	/// Loads <see cref="Bitmap"/> images of size 16x16 from files/resources/strings. Also can get file icons.
	/// Uses memory cache to avoid loading same image multiple times.
	/// Thread-safe.
	/// </summary>
	public sealed class AIconImageCache : IDisposable
	{
		List<(int dpi, Dictionary<string, object> images)> _images; //object is Bitmap if loaded, or null if failed to load, or List<(Action<Bitmap, object>, object)> if loading async
		Dictionary<AHash.MD5Result, Bitmap> _dmb = new();
		MemoryStream _ms = new();
		const int c_imageSize = 16;

		/// <summary>
		/// Gets image from memory cache or file or resource.
		/// </summary>
		/// <param name="imageSource">File path, or resource path that starts with "resources/" or has prefix "resource:", etc. See <i>isImage</i> parameter.</param>
		/// <param name="dpi">DPI of window that will display the image.</param>
		/// <param name="isImage">
		/// true - load image from xaml/png/etc file, resource or string with <see cref="AImageUtil.LoadGdipBitmapFromFileOrResourceOrString"/> or <see cref="AImageUtil.LoadWpfImageElementFromFileOrResourceOrString"/>.
		/// false - get file icon with <see cref="AIcon.OfFile"/>.
		/// null (default) - call <see cref="AImageUtil.HasImageOrResourcePrefix"/> to determine whether it is image.
		/// </param>
		/// <param name="asyncCompletion">
		/// If not null, if the image is still not in cache, <b>Get</b> returns null and loads it asynchronously in other thread. When loaded, calls <i>asyncCompletion</i> in this thread.
		/// Can be used to avoid slow startup when need to get many file icons.
		/// This thread must dispatch messages, else the callback will not be called.
		/// Used only for icons, not for images.
		/// </param>
		/// <param name="acData">Something to pass to the <i>asyncCompletion</i> callback function. The function is called once for a unique asyncCompletion/acData.</param>
		/// <param name="onException">Action to call when fails to load image. If null, calls <see cref="AWarning.Write"/>. Parameters are image source string and exception.</param>
		public Bitmap Get(string imageSource, int dpi, bool? isImage = null, Action<Bitmap, object> asyncCompletion = null, object acData = null, Action<string, Exception> onException = null) {
			lock (this) {
				bool isIm = isImage ?? AImageUtil.HasImageOrResourcePrefix(imageSource);
				bool isXaml = isIm && imageSource.Ends(".xaml", true);
				if (!isXaml) dpi = 96;
				if (isIm) asyncCompletion = null;

				_images ??= new List<(int dpi, Dictionary<string, object> images)>();

				//get dictionary for dpi
				int i; for (i = 0; i < _images.Count; i++) if (_images[i].dpi == dpi) goto g1;
				_images.Add((dpi, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)));
				g1:
				var d = _images[i].images;

				//find or load
				if (!d.TryGetValue(imageSource, out var ba)) {
					if (asyncCompletion != null) {
						d[imageSource] = new List<(Action<Bitmap, object>, object)> { (asyncCompletion, acData) }; //prevent async-load same image multiple times
						var post = PostToThisThread_.OfThisThread; //use this instead of ContinueWith which requires an UI SynchronizationContext which we cannot ensure
						Task.Factory.StartNew(() => {
							var b = _Load();
							post.Post(() => {
								lock (this) {
									if (_images != null && d[imageSource] is List<(Action<Bitmap, object>, object)> a) _AsyncToBitmap(b, a, false);
									else b?.Dispose();
								}
							});
						}, default, 0, StaTaskScheduler_.Default);
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
						if (!isIm) {
							b = AIcon.OfFile(imageSource).ToGdipBitmap();
							if (b != null) {
								lock (this) {
									var hash = _Hash(b);
									if (!_dmb.TryAdd(hash, b)) { b.Dispose(); b = _dmb[hash]; }
									//AOutput.Write(_images[0].images.Count, _dmb.Count, imageSource);
								}
							}
						} else if (isXaml) b = AImageUtil.LoadGdipBitmapFromXaml(imageSource, (c_imageSize, c_imageSize), dpi);
						else b = AImageUtil.LoadGdipBitmapFromFileOrResourceOrString(imageSource);
					}
					catch (Exception ex) { if (onException != null) onException(imageSource, ex); else AWarning.Write(ex.ToStringWithoutStack()); }
					return b;
				}

				void _AsyncToBitmap(Bitmap b, List<(Action<Bitmap, object>, object)> a, bool post) {
					d[imageSource] = b;
					if (b == null) return;
					if (post) PostToThisThread_.OfThisThread.Post(() => { foreach (var v in a) v.Item1(b, v.Item2); });
					else foreach (var v in a) v.Item1(b, v.Item2);
				}
			}
		}

		AHash.MD5Result _Hash(Bitmap b) {
			_ms.Position = 0;
			b.Save(_ms, System.Drawing.Imaging.ImageFormat.Bmp); //fast, if compared with AIcon.OfFile
			AHash.MD5 md = default;
			unsafe { fixed (byte* p = _ms.GetBuffer()) md.Add(p, (int)_ms.Position); } //fast
			return md.Hash;
		}
		//also tested hashing bits from hicon directly, to avoid ToGdipBitmap for duplicates.
		//	It makes faster for duplicates, else slower.
		//	Anyway AIcon.OfFile is much slower. Better use smaller code.

		/// <summary>
		/// Disposes all image objects.
		/// </summary>
		public void Dispose() {
			lock (this) {
				if (_images != null) {
					_images = null;
					foreach (var b in _dmb.Values) b.Dispose();
					_dmb.Clear();
				}
			}
		}
	}
}
