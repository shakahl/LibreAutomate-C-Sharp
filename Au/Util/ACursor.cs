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
using Forms = System.Windows.Forms;
//using System.Linq;

using Au;
using Au.Types;

namespace Au.Util
{
	//TODO: like AIcon
	/// <summary>
	/// Helps to load cursors, etc.
	/// </summary>
	public static class ACursor
	{
		/// <summary>
		/// Loads cursor from file.
		/// Returns null if fails.
		/// </summary>
		/// <param name="file">.cur or .ani file. If not full path, uses <see cref="AFolders.ThisAppImages"/>.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI (the "text size" system setting).</param>
		/// <remarks>
		/// This function exists because <see cref="Forms.Cursor"/> constructors don't support colors, ani cursors and custom size.
		/// </remarks>
		public static Forms.Cursor LoadCursorFromFile(string file, int size = 0)
		{
			file = APath.Normalize(file, AFolders.ThisAppImages);
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
		public static Forms.Cursor HandleToCursor(IntPtr hCursor, bool destroyCursor = true)
		{
			if(hCursor == default) return null;
			var R = new Forms.Cursor(hCursor);
			if(destroyCursor) AIcon.LetObjectDestroyIconOrCursor_(R);
			return R;
		}

		/// <summary>
		/// Creates cursor from cursor file data in memory, for example from a managed resource.
		/// Returns null if fails.
		/// </summary>
		/// <param name="cursorData">Data of .cur or .ani file.</param>
		/// <param name="size">Width and height. If 0, uses system default size, which depends on DPI (the "text size" system setting).</param>
		/// <remarks>
		/// This function exists because <see cref="Forms.Cursor"/> constructors don't support colors, ani cursors and custom size.
		/// </remarks>
		public static Forms.Cursor LoadCursorFromMemory(byte[] cursorData, int size = 0)
		{
			var s = AFolders.Temp + Guid.NewGuid().ToString();
			File.WriteAllBytes(s, cursorData);
			var c = LoadCursorFromFile(s, size);
			AFile.Delete(s);
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
				R = AHash.Fnv1Long((byte*)b.bmBits, b.bmHeight * b.bmWidthBytes);
			Api.DeleteObject(hb);
			return R;
		}

		/// <summary>
		/// Gets the native handle of the current global mouse cursor.
		/// Returns false if cursor is hidden.
		/// </summary>
		/// <remarks>
		/// It is not the cursor of this thread (<see cref="Forms.Cursor.Current"/>).
		/// Don't destroy the cursor.
		/// </remarks>
		public static bool GetCurrentCursor(out IntPtr hcursor)
		{
			Api.CURSORINFO ci = default; ci.cbSize = Api.SizeOf(ci);
			if(Api.GetCursorInfo(ref ci) && ci.hCursor != default && 0 != (ci.flags & Api.CURSOR_SHOWING)) { hcursor = ci.hCursor; return true; }
			hcursor = default; return false;
		}

		/// <summary>
		/// Workaround for brief 'wait' cursor when mouse enters a window of current thread first time.
		/// Reason: default thread's cursor is 'wait'. OS shows it before the first WM_SETCURSOR sets correct cursor.
		/// Call eg on WM_CREATE.
		/// </summary>
		internal static void SetArrowCursor_()
		{
			var h = Forms.Cursors.Arrow.Handle;
			if(Api.GetCursor() != h) Api.SetCursor(h);
		}
	}
}
