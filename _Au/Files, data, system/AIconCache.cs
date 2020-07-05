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
//using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

using Au.Types;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Gets icons of files etc as Bitmap. Uses 2-level cache - memory and file.
	/// </summary>
	/// <threadsafety static="true" instance="true"/>
	public sealed class AIconCache
	{
		XElement _x;
		Hashtable _table;
		string _cacheFile;
		int _iconSize;
		bool _dirty;

		///
		public AIconCache(string cacheFile, int iconSize)
		{
			_cacheFile = cacheFile;
			_iconSize = iconSize;
			AProcess.Exit += (_, _) => SaveCacheFileNow();
		}

		/// <summary>
		/// Saves to the cache file now, if need.
		/// Automatically called on process exit.
		/// </summary>
		public void SaveCacheFileNow()
		{
			if(_dirty) {
				lock(this) {
					if(_dirty) {
						_dirty = false;
						AFile.WaitIfLocked(() => _x.Save(_cacheFile));
					}
				}
			}
		}

		/// <summary>
		/// Clears the memory cache and deletes the cache file.
		/// </summary>
		public void ClearCache()
		{
			lock(this) {
				_dirty = false;
				_x = null;
				_table = null;
				AFile.Delete(_cacheFile);
			}
		}

		/// <summary>
		/// Gets file icon as <b>Bitmap</b>.
		/// Returns null if the icon is not cached and failed to get it, eg file does not exist.
		/// </summary>
		/// <param name="file">Any file or folder.</param>
		/// <param name="useExt">
		/// Get file type icon, depending on filename extension. Use this to avoid getting separate image object for each file of same type.
		/// This is ignored if filename extension is ".ico" or ".exe" or starts with ".exe," or ".dll,".
		/// </param>
		/// <param name="giFlags">Flags for <see cref="AIcon.GetFileIconImage"/>.</param>
		/// <param name="autoUpdate">
		/// If not null, the cached image will be auto-updated when changed. Then will be called this function. It can update the image in UI.
		/// How it works: If this function finds cached image, it sets timer that after ~50 ms loads that icon/image from file again and compares with the cached image. If different, updates the cache. Does it once, not periodically.
		/// Use only in UI threads. Does not work if this thread does not retrieve/dispatch posted messages.
		/// </param>
		/// <param name="auParam">Something to pass to the <i>autoUpdate</i> callback function.</param>
		/// <remarks>
		/// If the icon is in the memory cache, gets it from there.
		/// Else if it is in the file cache, gets it from there and adds to the memory cache.
		/// Else gets from file (uses <see cref="AIcon.GetFileIconImage"/> and adds to the file cache and to the memory cache.
		/// </remarks>
		public Bitmap GetImage(string file, bool useExt, IconGetFlags giFlags = 0, Action<Bitmap, object> autoUpdate = null, object auParam = null)
		{
			if(useExt) {
				var ext = APath.GetExtension(file);
				if(ext.Length == 0) {
					if(AFile.ExistsAsDirectory(file)) ext = file;
					else ext = ".no-ext";
				} else {
					//ext = ext.Lower();
					if(ext.Eqi(".ico") || ext.Eqi(".exe") || ext.Starts(".exe,", true) || ext.Starts(".dll,", true)) ext = file;
				}
				file = ext;
			} else if(APath.IsFullPathExpandEnvVar(ref file)) {
				file = APath.Normalize_(file, noExpandEV: true);
			}

			return _GetImage(file, giFlags, null, autoUpdate, auParam, true);
		}

		/// <summary>
		/// Gets any icon or image using callback function.
		/// Returns null if the icon is not cached and the callback function returns null.
		/// </summary>
		/// <param name="name">Some unique name. It is used to identify this image in cache.</param>
		/// <param name="callback">Called to get image. To convert icon handle to image, use <see cref="AIcon.HandleToImage"/>.</param>
		/// <param name="autoUpdate"></param>
		/// <param name="auParam"></param>
		/// <param name="auDispose">If true (default), auto-updating can dispose unused image returned by <i>callback</i>.</param>
		/// <remarks>
		/// If the icon is in the memory cache, gets it from there.
		/// Else if it is in the file cache, gets it from there and adds to the memory cache.
		/// Else calls callback function, which should return image, and adds to the file cache and to the memory cache.
		/// </remarks>
		public Bitmap GetImage(string name, Func<Bitmap> callback, Action<Bitmap, object> autoUpdate = null, object auParam = null, bool auDispose = true)
		{
			return _GetImage(name, 0, callback, autoUpdate, auParam, auDispose);
		}

		Bitmap _GetImage(string file, IconGetFlags giFlags, Func<Bitmap> callback, Action<Bitmap, object> autoUpdate, object auParam, bool auDispose)
		{
			bool cached = true;
			lock(this) {
				Bitmap R = null;

				//is in memory cache?
				if(_table == null) _table = new Hashtable(StringComparer.OrdinalIgnoreCase);
				else R = _table[file] as Bitmap;

				if(R == null) {
					//is in file cache?
					try {
						if(_x == null && AFile.ExistsAsFile(_cacheFile)) {
							_x = AExtXml.LoadElem(_cacheFile);
							if(_iconSize != _x.Attr("size", 0) || ADpi.OfThisProcess != _x.Attr("dpi", 0)) {
								_x = null;
								ADebug.Print("info: cleared icon cache");
							}

							//FUTURE: Delete unused entries. Maybe try to auto-update changed icons.
							//	Not very important, because there is ClearCache.
						}
						if(_x != null) {
							var x = _x.Elem("i", "name", file, true);
							if(x != null) {
								using var ms = new MemoryStream(Convert.FromBase64String(x.Value), false);
								R = new Bitmap(ms);
							}
						}
					}
					catch(Exception ex) {
						ADebug.Print(ex.Message);
					}

					if(R != null) {
						_table[file] = R; //add to memory cache
					} else if(_LoadImage(out R, file, giFlags, callback)) { //get file icon
						_AddImage(file, R, false); //add to file cache and memory cache
						cached = false;
					}
				}

				//auto-update
				if(cached && autoUpdate != null) {
					var d = new _AUData() {
						cache = this, oldImage = R, file = file, callback = callback,
						autoUpdated = autoUpdate, auParam = auParam, giFlags = giFlags, canDispose = auDispose,
					};
					_AutoUpdateAdd(d);
				}

				return R;
			}
		}

		bool _LoadImage(out Bitmap b, string file, IconGetFlags giFlags, Func<Bitmap> callback)
		{
			if(callback != null) b = callback();
			else b = AIcon.GetFileIconImage(file, _iconSize, giFlags);
			return b != null;
		}

		void _AddImage(string file, Bitmap b, bool replace)
		{
			using(var ms = new MemoryStream()) {
				b.Save(ms, ImageFormat.Png);
				var s = Convert.ToBase64String(ms.ToArray()); //SHOULDDO: GetBuffer
				XElement d = null;
				if(replace) {
					d = _x?.Elem("i", "name", file, true);
					Debug.Assert(d != null); //possible, eg if called ClearCache after GetImage
					if(d != null) d.Value = s;
				}
				if(d == null) {
					d = new XElement("i", s);
					d.SetAttributeValue("name", file);
					if(_x == null) {
						_x = new XElement("images");
						_x.SetAttributeValue("size", _iconSize);
						_x.SetAttributeValue("dpi", ADpi.OfThisProcess);
					}
					_x.Add(d);
				}
			}
			_dirty = true;
			_table[file] = b; //add to memory cache
		}

		#region auto update

		[ThreadStatic] static Queue<_AUData> t_auList;
		[ThreadStatic] static ATimer t_auTimer;

		static void _AutoUpdateAdd(_AUData d)
		{
			if(t_auTimer == null) {
				t_auTimer = new ATimer(t => _AutoUpdateTimer());
				t_auList = new Queue<_AUData>();
			}
			t_auList.Enqueue(d);
			t_auTimer.Every(50);
		}

		static void _AutoUpdateTimer()
		{
			var t = ATime.PerfMilliseconds;
			while(t_auList.Count > 0) {
				if(ATime.PerfMilliseconds - t > 20) return; //don't block UI thread
				var k = t_auList.Dequeue();
				k.cache._AutoUpdateSingle(k);
			}
			t_auTimer.Stop();
		}

		void _AutoUpdateSingle(_AUData k)
		{
			//FUTURE: optimize.
			//	Don't load ico/exe file if not modified.
			//	Don't do too frequently for same file.
			//	Skip duplicate paths.
			//	Don't do for files where getting icon is very slow.

			if(!_LoadImage(out var b, k.file, k.giFlags, k.callback)) return;
			if(b == k.oldImage) return; //callback returned the same object
			if(_CompareImages(b, k.oldImage)) {
				if(k.canDispose) b.Dispose();
				return;
			}
			lock(this) _AddImage(k.file, b, true);
			//k.autoUpdated?.Invoke(b, k.auParam);

			//AOutput.Write(k.auParam);
			//APerf.First();
			k.autoUpdated?.Invoke(b, k.auParam);
			//APerf.NW();
		}

		class _AUData
		{
			public AIconCache cache;
			public Bitmap oldImage;
			public string file;
			public Func<Bitmap> callback;
			public Action<Bitmap, object> autoUpdated;
			public object auParam;
			public IconGetFlags giFlags;
			public bool canDispose;
		}

		static unsafe bool _CompareImages(Bitmap b1, Bitmap b2)
		{
			var pf = b1.PixelFormat;
			if(b1.Size != b2.Size || pf != b2.PixelFormat) return false;
			var r = new Rectangle(0, 0, b1.Width, b1.Height);
			var d1 = b1.LockBits(r, ImageLockMode.ReadOnly, pf);
			var d2 = b2.LockBits(r, ImageLockMode.ReadOnly, pf);
			try {
				if(d1.Stride != d2.Stride) return false;
				int n = Math.Abs(d1.Stride) * d1.Height;
				return 0 == Api.memcmp((void*)d1.Scan0, (void*)d2.Scan0, n);
			}
			finally {
				b1.UnlockBits(d1);
				b2.UnlockBits(d2);
			}
		}

		#endregion
	}
}
