using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Drawing;
using System.Windows.Forms;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Helps to get and release screen DC with the 'using(...){...}' pattern.
	/// Uses API GetDC and ReleaseDC.
	/// </summary>
	internal struct LibScreenDC :IDisposable
	{
		IntPtr _dc;

		public LibScreenDC(int unused) => _dc = Api.GetDC(default);
		public static implicit operator IntPtr(LibScreenDC dc) => dc._dc;
		public void Dispose() { Api.ReleaseDC(default, _dc); _dc = default; }
	}

	/// <summary>
	/// Helps to get and release window DC with the 'using(...){...}' pattern.
	/// Uses API GetDC and ReleaseDC.
	/// If w is default(Wnd), gets screen DC.
	/// </summary>
	internal struct LibWindowDC :IDisposable, IDeviceContext
	{
		IntPtr _dc;
		Wnd _w;

		public LibWindowDC(Wnd w) => _dc = Api.GetDC(_w = w);
		public static implicit operator IntPtr(LibWindowDC dc) => dc._dc;
		public bool Is0 => _dc == default;

		public void Dispose() => ReleaseHdc();

		public IntPtr GetHdc() => _dc;

		public void ReleaseHdc()
		{
			Api.ReleaseDC(_w, _dc);
			_w = default; _dc = default;
		}
	}

	/// <summary>
	/// Helps to create and delete screen DC with the 'using(...){...}' pattern.
	/// Uses API CreateCompatibleDC and DeleteDC.
	/// </summary>
	internal struct LibCompatibleDC :IDisposable, IDeviceContext
	{
		IntPtr _dc;

		public LibCompatibleDC(IntPtr dc) => _dc = Api.CreateCompatibleDC(dc);
		public static implicit operator IntPtr(LibCompatibleDC dc) => dc._dc;
		public bool Is0 => _dc == default;

		public void Dispose() => ReleaseHdc();

		public IntPtr GetHdc()=> _dc;

		public void ReleaseHdc()
		{
			Api.DeleteDC(_dc);
			_dc = default;
		}
	}

	/// <summary>
	/// Creates and manages native bitmap handle and memory DC (GDI device context).
	/// The bitmap is selected in the DC.
	/// </summary>
	public sealed class MemoryBitmap :IDisposable, IDeviceContext
	{
		IntPtr _dc, _bm, _oldbm;

		/// <summary>
		/// DC handle.
		/// </summary>
		public IntPtr Hdc => _dc;

		/// <summary>
		/// Bitmap handle.
		/// </summary>
		public IntPtr Hbitmap => _bm;

		/// <summary>
		/// Does nothing. Later you can call Create or Attach.
		/// </summary>
		public MemoryBitmap() { }

		/// <summary>
		/// Calls <see cref="Create"/>.
		/// </summary>
		/// <exception cref="ArgumentException">width or height is less than 1.</exception>
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		public MemoryBitmap(int width, int height)
		{
			if(width <= 0 || height <= 0) throw new ArgumentException();
			if(!Create(width, height)) throw new AuException("*create memory bitmap of specified size");
		}

		//rejected: not obvious, whether it attaches or copies. Also, attaching is rarely used.
		///// <summary>
		///// Calls <see cref="Attach"/>.
		///// </summary>
		//public MemoryBitmap(IntPtr hBitmap)
		//{
		//	Attach(hBitmap);
		//}

		/// <summary>
		/// Deletes the bitmap and DC.
		/// </summary>
		public void Dispose()
		{
			Delete();
			//GC.SuppressFinalize(this); //no. We allow to Create/Attach after calling this.
		}

		///
		~MemoryBitmap() => Delete();
		//info: calls DeleteDC. MSDN says that ReleaseDC must be called from the same thread. But does not say it about DeleteDC and others.

		///
		public IntPtr GetHdc() => _dc;

		///
		public void ReleaseHdc() => Delete();

		/// <summary>
		/// Deletes the bitmap and DC.
		/// </summary>
		public void Delete()
		{
			if(_dc == default) return;
			if(_bm != default) {
				Api.SelectObject(_dc, _oldbm);
				Api.DeleteObject(_bm);
				_bm = default;
			}
			Api.DeleteDC(_dc);
			_dc = default;
		}

		/// <summary>
		/// Creates new memory DC and bitmap of specified size and selects it into the DC.
		/// Returns false if failed.
		/// In any case deletes previous bitmap and DC.
		/// </summary>
		/// <param name="width">Width, pixels. Must be &gt; 0.</param>
		/// <param name="height">Height, pixels. Must be &gt; 0.</param>
		public bool Create(int width, int height)
		{
			using(var dcs = new LibScreenDC(0)) {
				Attach(Api.CreateCompatibleBitmap(dcs, width, height));
				return _bm != default;
			}
		}

		/// <summary>
		/// Sets this variable to manage an existing bitmap.
		/// Selects the bitmap into a memory DC.
		/// Deletes previous bitmap and DC.
		/// </summary>
		/// <param name="hBitmap">Native bitmap handle.</param>
		public void Attach(IntPtr hBitmap)
		{
			Delete();
			if(hBitmap != default) {
				_dc = Api.CreateCompatibleDC(default);
				_oldbm = Api.SelectObject(_dc, _bm = hBitmap);
			}
		}

		/// <summary>
		/// Deletes memory DC, clears this variable and returns its bitmap (native bitmap handle).
		/// The returned bitmap is not selected into a DC. Will need to delete it with API DeleteObject.
		/// </summary>
		public IntPtr Detach()
		{
			IntPtr bret = _bm;
			if(_bm != default) {
				Api.SelectObject(_dc, _oldbm);
				Api.DeleteDC(_dc);
				_dc = default; _bm = default;
			}
			return bret;
		}
	}

	/// <summary>
	/// Creates and manages native font handle.
	/// </summary>
	internal sealed class LibNativeFont :IDisposable
	{
		public IntPtr Handle { get; private set; }
		public int HeightOnScreen { get; private set; }

		public LibNativeFont(IntPtr handle) { Handle = handle; }

		public static implicit operator IntPtr(LibNativeFont f) => (f == null) ? default : f.Handle;

		~LibNativeFont() { _Dispose(); }
		public void Dispose() { _Dispose(); GC.SuppressFinalize(this); }
		void _Dispose()
		{
			if(Handle != default) { Api.DeleteObject(Handle); Handle = default; }
		}

		public LibNativeFont(string name, int height, bool calculateHeightOnScreen = false)
		{
			using(var dcs = new LibScreenDC(0)) {
				int h2 = -Math_.MulDiv(height, Api.GetDeviceCaps(dcs, 90), 72);
				Handle = Api.CreateFont(h2, iCharSet: 1, pszFaceName: name); //LOGPIXELSY=90
				if(calculateHeightOnScreen) {
					using(var dcMem = new LibCompatibleDC(dcs)) {
						var of = Api.SelectObject(dcMem, Handle);
						Api.GetTextExtentPoint32(dcMem, "A", 1, out var z);
						HeightOnScreen = z.height;
						Api.SelectObject(dcMem, of);
					}
				}
			}
		}
	}

	/// <summary>
	/// Helps to load cursors, etc.
	/// </summary>
	public static class Cursors_
	{
		/// <summary>
		/// Loads cursor from file.
		/// Returns null if fails.
		/// </summary>
		/// <param name="file">.cur or .ani file. If not full path, uses <see cref="Folders.ThisAppImages"/>.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI (the "text size" system setting).</param>
		/// <remarks>
		/// This function exists because <see cref="Cursor"/> constructors don't support colors, ani cursors and custom size.
		/// </remarks>
		public static Cursor LoadCursorFromFile(string file, int size = 0)
		{
			file = Path_.Normalize(file, Folders.ThisAppImages);
			if(file == null) return null;
			uint fl = Api.LR_LOADFROMFILE; if(size == 0) fl |= Api.LR_DEFAULTSIZE;
			var hCur = Api.LoadImage(default, file, Api.IMAGE_CURSOR, size, size, fl);
			return HandleToCursor(hCur);
		}

		/// <summary>
		/// Converts unmanaged cursor to Cursor object.
		/// Returns null if hCur is default(IntPtr).
		/// </summary>
		/// <param name="hCursor">Cursor handle.</param>
		/// <param name="destroyCursor">If true (default), the returned variable owns the unmanaged cursor and destroys it when disposing. If false, the returned variable just uses the unmanaged cursor and will not destroy; if need, the caller later should destroy it with API <msdn>DestroyCursor</msdn>.</param>
		public static Cursor HandleToCursor(IntPtr hCursor, bool destroyCursor = true)
		{
			if(hCursor == default) return null;
			var R = new Cursor(hCursor);
			if(destroyCursor) {
				var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
				Debug.Assert(fi != null);
				fi?.SetValue(R, true);
				Util.GC_.AddObjectMemoryPressure(R, 1000); //see comments in Icons.HandleToIcon
			}
			return R;
		}

		/// <summary>
		/// Creates cursor from cursor file data in memory, for example from a managed resource.
		/// Returns null if fails.
		/// </summary>
		/// <param name="cursorData">Data of .cur or .ani file.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI (the "text size" system setting).</param>
		/// <remarks>
		/// This function exists because <see cref="Cursor"/> constructors don't support colors, ani cursors and custom size.
		/// </remarks>
		public static Cursor LoadCursorFromMemory(byte[] cursorData, int size = 0)
		{
			var s = Folders.Temp + Guid.NewGuid().ToString();
			File.WriteAllBytes(s, cursorData);
			var c = LoadCursorFromFile(s, size);
			Files.Delete(s);
			return c;

			//If want to avoid temp file, can use CreateIconFromResourceEx.
			//	But quite much unsafe code (at first need to find cursor offset and set hotspot), less reliable (may fail for some .ani files), in some cases works not as well (may get wrong-size cursor).
			//	The code moved to the Unused project.
		}

		/// <summary>
		/// Calculates 64-bit FNV1 hash of a mouse cursor's mask bitmap.
		/// Returns 0 if fails.
		/// </summary>
		/// <param name="hCursor">Native cursor handle. See <see cref="GetCurrentCursor"/>.</param>
		public static unsafe long HashCursor(IntPtr hCursor)
		{
			long R = 0; Api.BITMAP b;
			if(!Api.GetIconInfo(hCursor, out var ii)) return 0;
			if(ii.hbmColor != default) Api.DeleteObject(ii.hbmColor);
			var hb = Api.CopyImage(ii.hbmMask, Api.IMAGE_BITMAP, 0, 0, Api.LR_COPYDELETEORG | Api.LR_CREATEDIBSECTION);
			if(hb == default) { Api.DeleteObject(ii.hbmMask); return 0; }
			if(0 != Api.GetObject(hb, sizeof(Api.BITMAP), &b) && b.bmBits != default)
				R = Convert_.HashFnv1_64((byte*)b.bmBits, b.bmHeight * b.bmWidthBytes);
			Api.DeleteObject(hb);
			return R;
		}

		/// <summary>
		/// Gets the native handle of the current mouse cursor.
		/// Returns false if the cursor is currently invisible.
		/// </summary>
		/// <remarks>
		/// It is the system cursor, not the cursor of this thread.
		/// Don't destroy the cursor.
		/// </remarks>
		public static bool GetCurrentCursor(out IntPtr hcursor)
		{
			Api.CURSORINFO ci = default; ci.cbSize = Api.SizeOf(ci);
			if(Api.GetCursorInfo(ref ci) && ci.hCursor != default && 0 != (ci.flags & Api.CURSOR_SHOWING)) { hcursor = ci.hCursor; return true; }
			hcursor = default; return false;
		}
	}

	/// <summary>
	/// Misc GDI util.
	/// </summary>
	internal static class LibGDI
	{
		//rejected: now we use BufferedGraphics. Same speed. With BufferedGraphics no TextRenderer problems.
		///// <summary>
		///// Copies a .NET Bitmap to a native DC in a fast way.
		///// </summary>
		///// <remarks>
		///// Can be used for double-buffering: create Bitmap and Graphics from it, draw in that Graphics, then call this func.
		///// The bitmap should be PixelFormat.Format32bppArgb (normal), else slower etc. Must be top-down (normal).
		///// </remarks>
		//public static unsafe void CopyNetBitmapToDC(Bitmap b, IntPtr dc)
		//{
		//	var r = new Rectangle(0, 0, b.Width, b.Height);
		//	var d = b.LockBits(r, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		//	try {
		//		Api.BITMAPINFOHEADER h = default;
		//		h.biSize = sizeof(Api.BITMAPINFOHEADER);
		//		h.biWidth = d.Width;
		//		h.biHeight = -d.Height;
		//		h.biPlanes = 1;
		//		h.biBitCount = 32;
		//		int k = Api.SetDIBitsToDevice(dc, 0, 0, d.Width, d.Height, 0, 0, 0, d.Height, (void*)d.Scan0, &h, 0);
		//		Debug.Assert(k > 0);
		//	}
		//	finally { b.UnlockBits(d); }

		//	//speed: 6-7 times faster than Graphics.FromHdc/DrawImageUnscaled. When testing, the dc was from BeginPaint.
		//}
		//public static unsafe void CopyNetBitmapToDC2(Bitmap b, IntPtr dc)
		//{
		//	using(var g = Graphics.FromHdc(dc)) {
		//		g.DrawImageUnscaled(b, 0, 0);
		//	}
		//}
	}
}
