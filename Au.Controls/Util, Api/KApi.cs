using Au.Types;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Au.Controls
{
	[DebuggerStepThrough]
	static unsafe class KApi
	{

		[DllImport("uxtheme.dll")]
		internal static extern IntPtr OpenThemeData(AWnd hwnd, string pszClassList);

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int CloseThemeData(IntPtr hTheme);

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, RECT* prc, THEMESIZE eSize, out SIZE psz);
		internal enum THEMESIZE
		{
			TS_MIN,
			TS_TRUE,
			TS_DRAW
		}

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, in RECT pRect, RECT* pClipRect = null);

		[DllImport("user32.dll")]
		internal static extern bool DrawFrameControl(IntPtr hdc, in RECT r, int type, int state);

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int BufferedPaintInit();
		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int BufferedPaintUnInit();
		[DllImport("uxtheme.dll")]
		internal static extern IntPtr BeginBufferedPaint(IntPtr hdcTarget, in RECT prcTarget, BP_BUFFERFORMAT dwFormat, ref BP_PAINTPARAMS pPaintParams, out IntPtr phdc);
		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int EndBufferedPaint(IntPtr hBufferedPaint, bool fUpdateTarget);
		internal enum BP_BUFFERFORMAT
		{
			BPBF_COMPATIBLEBITMAP,
			BPBF_DIB,
			BPBF_TOPDOWNDIB,
			BPBF_TOPDOWNMONODIB
		}
		internal struct BP_PAINTPARAMS
		{
			public int cbSize;
			public uint dwFlags;
			public RECT* prcExclude;
			//public BLENDFUNCTION* pBlendFunction;
			uint pBlendFunction;
		}
		//internal struct BLENDFUNCTION {
		//	public byte BlendOp;
		//	public byte BlendFlags;
		//	public byte SourceConstantAlpha;
		//	public byte AlphaFormat;
		//}

		[DllImport("shlwapi.dll")]
		internal static extern void ColorRGBToHLS(int clrRGB, out ushort pwHue, out ushort pwLuminance, out ushort pwSaturation);

		[DllImport("shlwapi.dll")]
		internal static extern int ColorHLSToRGB(ushort wHue, ushort wLuminance, ushort wSaturation);



	}

}
