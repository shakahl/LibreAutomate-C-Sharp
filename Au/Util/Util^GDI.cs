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
using System.Windows.Forms;
using System.Drawing;
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
		public void Dispose() => Api.ReleaseDC(default, _dc);
	}

	/// <summary>
	/// Helps to get and release window DC with the 'using(...){...}' pattern.
	/// Uses API GetDC and ReleaseDC.
	/// If w is default(Wnd), gets screen DC.
	/// </summary>
	internal struct LibWindowDC :IDisposable
	{
		IntPtr _dc;
		Wnd _w;

		public LibWindowDC(Wnd w) => _dc = Api.GetDC(_w = w);
		public static implicit operator IntPtr(LibWindowDC dc) => dc._dc;
		public void Dispose() => Api.ReleaseDC(_w, _dc);
		public bool Is0 => _dc == default;
	}

	/// <summary>
	/// Helps to create and delete screen DC with the 'using(...){...}' pattern.
	/// Uses API CreateCompatibleDC and DeleteDC.
	/// </summary>
	internal struct LibCompatibleDC :IDisposable
	{
		IntPtr _dc;

		public LibCompatibleDC(IntPtr dc) => _dc = Api.CreateCompatibleDC(dc);
		public static implicit operator IntPtr(LibCompatibleDC dc) => dc._dc;
		public void Dispose() => Api.DeleteDC(_dc);
	}

	/// <summary>
	/// Creates and manages native bitmap handle and memory DC (GDI device context).
	/// The bitmap is selected in the DC.
	/// </summary>
	public sealed class MemoryBitmap :IDisposable
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
		/// <exception cref="AuException">Failed. Probably there is not enough memory for bitmap of specified size (need with*height*4 bytes).</exception>
		public MemoryBitmap(int width, int height)
		{
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
		~MemoryBitmap() { Delete(); }
		//info: calls DeleteDC. MSDN says that ReleaseDC must be called from the same thread. But does not say it about DeleteDC and others.

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
		/// <param name="width">Width, pixels.</param>
		/// <param name="height">Height, pixels.</param>
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
						HeightOnScreen = z.Height;
						Api.SelectObject(dcMem, of);
					}
				}
			}
		}
	}

}
