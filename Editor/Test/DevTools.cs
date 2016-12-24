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

namespace Editor.Test
{
	//[DebuggerStepThrough]
	public static class DevTools
	{
		public static void CreatePngImagelistFileFromIconFiles_il_tb()
		{
			var a = new string[]
				{
@"Q:\app\new.ico",
@"Q:\app\properties.ico",
@"Q:\app\save.ico",
@"Q:\app\icons\run.ico",
@"Q:\app\icons\compile.ico",
@"Q:\app\deb next.ico",
@"Q:\app\icons\deb into.ico",
@"Q:\app\icons\deb out.ico",
@"Q:\app\deb cursor.ico",
@"Q:\app\deb run.ico",
@"Q:\app\deb end.ico",
@"Q:\app\undo.ico",
@"Q:\app\redo.ico",
@"Q:\app\cut.ico",
@"Q:\app\copy.ico",
@"Q:\app\paste.ico",
@"Q:\app\icons\back.ico",
@"Q:\app\icons\active_items.ico",
@"Q:\app\icons\images.ico",
@"Q:\app\icons\annotations.ico",
@"Q:\app\help.ico",
@"Q:\app\droparrow.ico",
@"Q:\app\icons\record.ico",
@"Q:\app\find.ico",
@"Q:\app\icons\mm.ico",
@"Q:\app\icons\tags.ico",
@"Q:\app\icons\resources.ico",
@"Q:\app\icons\icons.ico",
@"Q:\app\options.ico",
@"Q:\app\icons\output.ico",
@"Q:\app\tip.ico",
@"Q:\app\icons\tip_book.ico",
@"Q:\app\delete.ico",
@"Q:\app\icons\back2.ico",
@"Q:\app\open.ico",
@"Q:\app\icons\floating.ico",
@"Q:\app\icons\clone dialog.ico",
@"Q:\app\dialog.ico",
				};

			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tb.png", 16);
		}

		public static void CreatePngImagelistFileFromIconFiles_il_tv()
		{
			var a = new string[]
				{
@".cs",
@"Q:\app\folder.ico",
@"Q:\app\folder_open.ico",
				};

			_CreatePngImagelistFileFromIconFiles(a, @"Q:\app\Catkeys\Editor\Resources\il_tv.png", 16);
		}

		static void _CreatePngImagelistFileFromIconFiles(string[] iconFiles, string pngFile, int imageSize)
		{
			var bAll = new Bitmap(imageSize * iconFiles.Length, imageSize);
			var g = Graphics.FromImage(bAll);
			int x = 0;

			foreach(var s in iconFiles) {
				var im = Icons.GetIconImage(s, imageSize);
				g.DrawImage(im, x, 0);
				x += imageSize;
			}

			bAll.Save(pngFile);
		}
	}
}
