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

using Au;
using Au.Types;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Controls;

/// <summary>
/// Adds extension methods for some WPF classes.
/// </summary>
public static class AExtWpf
{
	/// <summary>
	/// Gets native window handle of this <b>Window</b> or <b>Popup</b>, or container window handle of this child object.
	/// Returns <c>default(AWnd)</c> if: called before creating or after closing real window; failed; <i>t</i> is null.
	/// </summary>
	/// <param name="t"></param>
	public static AWnd Hwnd(this DependencyObject t)
	{
		switch(t) {
		case null: return default;
		case Window w: return (AWnd)new WindowInteropHelper(w).Handle; //FromDependencyObject works too, but this is usually slightly faster
		case Popup p: t = p.Child; if(t == null) return default; break; //FromVisual(Popup) returns null; or maybe owner window, not tested.
		}
		if(PresentationSource.FromDependencyObject(t) is HwndSource hs) return (AWnd)hs.Handle;
		return default;
	}
	//rejected: notPopup. Not useful.
	///// <summary>
	///// Gets window handle of this <b>Window</b>, <b>Popup</b> or container window handle of this child object.
	///// Returns <c>default(AWnd)</c> if: called before creating real window; failed; <i>t</i> is null.
	///// </summary>
	///// <param name="t"></param>
	///// <param name="notPopup">If this is <b>Popup</b> or in a <b>Popup</b>, get handle of popup's owner <b>Window</b>.</param>
	//public static AWnd Hwnd(this DependencyObject t, bool notPopup = false)
	//{
	//	switch(t) {
	//	case null: return default;
	//	case Window w: return (AWnd)new WindowInteropHelper(w).Handle; //FromDependencyObject works too, but this is usually slightly faster
	//	case Popup p when !notPopup: t = p.Child; if(t == null) return default; break; //FromVisual(Popup) returns null; or maybe owner window, not tested.
	//	}
	//	if(notPopup) {
	//		var w = Window.GetWindow(t); if(w == null) return default; //if Popup or in Popup, gets owner WIndow
	//		return (AWnd)new WindowInteropHelper(w).Handle;
	//	}
	//	if(PresentationSource.FromDependencyObject(t) is HwndSource hs) return (AWnd)hs.Handle;
	//	return default;
	//}

	/// <summary>
	/// Returns true if <see cref="ToggleButton.IsChecked"/> == true.
	/// </summary>
	/// <param name="t"></param>
	public static bool IsCheck(this CheckBox t) => t.IsChecked.GetValueOrDefault();

}
