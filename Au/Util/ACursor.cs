using Au.Types;
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
using System.Drawing;
//using System.Linq;

namespace Au.Util
{
	/// <summary>
	/// Helps to load cursors, etc.
	/// </summary>
	/// <remarks>
	/// To load cursors for winforms can be used <see cref="System.Windows.Forms.Cursor"/> constructors, but they don't support colors, ani cursors and custom size.
	/// Don't use this class to load cursors for WPF. Its <b>Cursor</b> class loads cursors correctly.
	/// </remarks>
	public class ACursor
	{
		IntPtr _handle;

		/// <summary>
		/// Sets native cursor handle.
		/// The cursor will be destroyed when disposing this variable or when converting to object of other type.
		/// </summary>
		public ACursor(IntPtr hcursor) { _handle = hcursor; }

		/// <summary>
		/// Destroys native cursor handle.
		/// </summary>
		public void Dispose() {
			if (_handle != default) { Api.DestroyCursor(_handle); _handle = default; }
		}

		/// <summary>
		/// Returns true if <b>Handle</b>==default(IntPtr).
		/// </summary>
		public bool Is0 => _handle == default;

		/// <summary>
		/// Gets native cursor handle.
		/// </summary>
		public IntPtr Handle => _handle;

		/// <summary>
		/// Gets native cursor handle.
		/// </summary>
		public static implicit operator IntPtr(ACursor cursor) => cursor._handle;

		/// <summary>
		/// Loads cursor from file.
		/// </summary>
		/// <returns>Returns default(ACursor) if failed.</returns>
		/// <param name="file">.cur or .ani file. If not full path, uses <see cref="AFolders.ThisAppImages"/>.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI.</param>
		public static ACursor Load(string file, int size = 0)
		{
			file = APath.Normalize(file, AFolders.ThisAppImages);
			if(file == null) return null;
			uint fl = Api.LR_LOADFROMFILE; if(size == 0) fl |= Api.LR_DEFAULTSIZE;
			return new ACursor(Api.LoadImage(default, file, Api.IMAGE_CURSOR, size, size, fl));
		}

		/// <summary>
		/// Creates cursor from cursor file data in memory, for example from a managed resource.
		/// </summary>
		/// <returns>Returns default(ACursor) if failed.</returns>
		/// <param name="cursorData">Data of .cur or .ani file.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI.</param>
		/// <remarks>
		/// This function creates/deletes a temporary file, because there is no good API to load cursor from memory.
		/// </remarks>
		public static ACursor Load(byte[] cursorData, int size = 0)
		{
			var s = AFolders.Temp + Guid.NewGuid().ToString();
			File.WriteAllBytes(s, cursorData);
			var c = Load(s, size);
			AFile.Delete(s);
			return c;

			//If want to avoid temp file, can use:
			//1. CreateIconFromResourceEx.
			//	But quite much unsafe code (at first need to find cursor offset (of size) and set hotspot), less reliable (may fail for some .ani files), in some cases works not as well (may get wrong-size cursor).
			//	The code moved to the Unused project.
			//2. CreateIconIndirect. Not tested. No ani. Need to find cursor offset (of size).
		}

		/// <summary>
		/// Converts native cursor to winforms cursor object.
		/// Returns null if <i>Handle</i> is default(IntPtr).
		/// </summary>
		/// <param name="destroyCursor">If true (default), the returned variable owns the unmanaged cursor and destroys it when disposing. If false, the returned variable just uses the cursor handle and will not destroy; later will need to dispose this variable.</param>
		public System.Windows.Forms.Cursor ToWinformsCursor(bool destroyCursor = true)
		{
			if(_handle == default) return null;
			var R = new System.Windows.Forms.Cursor(_handle);
			if(destroyCursor) AIcon.LetObjectDestroyIconOrCursor_(R);
			return R;
		}

		/// <summary>
		/// Calculates 64-bit FNV1 hash of cursor's mask bitmap.
		/// Returns 0 if fails.
		/// </summary>
		public unsafe long Hash()
		{
			if(!Api.GetIconInfo(_handle, out var ii)) return 0;
			if(ii.hbmColor != default) Api.DeleteObject(ii.hbmColor);
			var hb = Api.CopyImage(ii.hbmMask, Api.IMAGE_BITMAP, 0, 0, Api.LR_COPYDELETEORG | Api.LR_CREATEDIBSECTION);
			if(hb == default) { Api.DeleteObject(ii.hbmMask); return 0; }
			long R = 0; Api.BITMAP b;
			if(0 != Api.GetObject(hb, sizeof(Api.BITMAP), &b) && b.bmBits != default)
				R = AHash.Fnv1Long((byte*)b.bmBits, b.bmHeight * b.bmWidthBytes);
			Api.DeleteObject(hb);
			return R;
		}

		/// <summary>
		/// Gets the native handle of the current global mouse cursor.
		/// Returns false if cursor is hidden.
		/// </summary>
		/// <remarks>
		/// Don't destroy the cursor.
		/// </remarks>
		public static bool GetCurrentVisibleCursor(out ACursor cursor)
		{
			Api.CURSORINFO ci = default; ci.cbSize = Api.SizeOf(ci);
			if(Api.GetCursorInfo(ref ci) && ci.hCursor != default && 0 != (ci.flags & Api.CURSOR_SHOWING)) { cursor = new ACursor(ci.hCursor); return true; }
			cursor = default; return false;
		}

		/// <summary>
		/// Workaround for brief 'wait' cursor when mouse enters a window of current thread first time.
		/// Reason: default thread's cursor is 'wait'. OS shows it before the first WM_SETCURSOR sets correct cursor.
		/// Call eg on WM_CREATE.
		/// </summary>
		internal static void SetArrowCursor_()
		{
			var h = Api.LoadCursor(default, MCursor.Arrow);
			if(Api.GetCursor() != h) Api.SetCursor(h);
		}
	}
}
