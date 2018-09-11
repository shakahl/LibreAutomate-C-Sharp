//Small extension classes for .NET classes. Except those that have own files.
//Naming:
//	Class name: related .NET class name with _ suffix.
//	Extension method name: related .NET method name with _ suffix. Or new name with _ suffix.
//	Static method name: any name without _ suffix.

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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Linq;
using System.Xml.Linq;
using System.Security; //for XML comments
using System.Globalization;
using System.Windows.Forms;

using Au.Types;
using static Au.NoClass;

//Why Forms extension methods are in separate class, not in ExtensionMethods?
//Initially it was in ExtensionMethods. Then I noticed: appdomains are loaded much slower if the code uses ExtensionMethods.Has_.
//Reason: .NET loaded Forms and Drawing dlls, although Has_ does not use them. Moving Forms extensions to a separate class fixed it.

namespace Au.Types
{
	/// <summary>
	/// Adds extension methods to some .NET classes.
	/// </summary>
	public static class ExtensionMethods_Forms
	{
		#region Control

		/// <summary>
		/// If control handle still not created, creates handle.
		/// Like <see cref="Control.CreateHandle"/>, which is protected.
		/// Unlike <see cref="Control.CreateControl"/>, creates handle even if invisible, and does not create child control handles.
		/// </summary>
		public static void CreateHandle_(this Control t)
		{
			if(!t.IsHandleCreated) {
				var h = t.Handle;
			}
		}

		/// <summary>
		/// Gets mouse cursor position in client area coordinates.
		/// Returns default(POINT) if handle not created.
		/// </summary>
		public static POINT MouseClientXY_(this Control t)
		{
			return ((Wnd)t).MouseClientXY;
		}

		/// <summary>
		/// Gets mouse cursor position in window coordinates.
		/// </summary>
		public static POINT MouseWindowXY_(this Control t)
		{
			POINT p = Mouse.XY;
			POINT k = t.Location;
			return (p.x - k.x, p.y - k.y);
		}

		/// <summary>
		/// Sets the textual cue, or tip, that is displayed by the control when it does not have text.
		/// Sends API <msdn>EM_SETCUEBANNER</msdn>.
		/// Does nothing if Multiline.
		/// </summary>
		public static void SetCueBanner_(this TextBox t, string text, bool showWhenFocused = false)
		{
			Debug.Assert(!t.Multiline);
			_SetCueBanner_(t, Api.EM_SETCUEBANNER, showWhenFocused, text);
		}

		/// <summary>
		/// Sets the textual cue, or tip, that is displayed by the edit control when it does not have text.
		/// Sends API <msdn>CB_SETCUEBANNER</msdn>.
		/// </summary>
		public static void SetCueBanner_(this ComboBox t, string text)
		{
			_SetCueBanner_(t, Api.CB_SETCUEBANNER, false, text);
		}

		static void _SetCueBanner_(Control c, int message, bool showWhenFocused, string text)
		{
			if(c.IsHandleCreated) {
				((Wnd)c).SendS(message, showWhenFocused, text);
			} else if(!Empty(text)) {
				c.HandleCreated += (unu, sed) => _SetCueBanner_(c, message, showWhenFocused, text);
			}
		}

		//Currently not used.
		//note: when any function of this class is used, the presence of this function somehow makes to load Forms and Drawing dlls at run time, even if that function does not use Forms.
		///// <summary>
		///// Creates a control, sets its commonly used properties (Bounds, Text, tooltip, Anchor) and adds it to the Controls collection of this.
		///// </summary>
		///// <typeparam name="T">Control class.</typeparam>
		///// <param name="t"></param>
		///// <param name="x">Left.</param>
		///// <param name="y">Top.</param>
		///// <param name="width">Width.</param>
		///// <param name="height">Height.</param>
		///// <param name="text">The <see cref="Control.Text"/> property.</param>
		///// <param name="tooltip">Tooltip text.
		///// This function creates a ToolTip component and assigns it to the Tag property of this.</param>
		///// <param name="anchor">The <see cref="Control.Anchor"/> property.</param>
		//public static T Add_<T>(this ContainerControl t, int x, int y, int width, int height, string text = null, string tooltip = null, AnchorStyles anchor = AnchorStyles.None/*, string name = null*/) where T : Control, new()
		//{
		//	var c = new T();
		//	//if(!Empty(name)) c.Name = name;
		//	c.Bounds = new System.Drawing.Rectangle(x, y, width, height);
		//	if(anchor != AnchorStyles.None) c.Anchor = anchor;
		//	if(text != null) c.Text = text;
		//	if(!Empty(tooltip)) {
		//		var tt = t.Tag as ToolTip;
		//		if(tt == null) {
		//			t.Tag = tt = new ToolTip();
		//			//t.Disposed += (o, e) => Print((o as ContainerControl).Tag as ToolTip);
		//			//t.Disposed += (o, e) => ((o as ContainerControl).Tag as ToolTip)?.Dispose(); //it seems tooltip is auto-disposed when its controls are disposed. Anyway, this event is only if the form is disposed explicitly, but nobody does it.
		//		}
		//		tt.SetToolTip(c, tooltip);
		//	}
		//	t.Controls.Add(c);
		//	return c;
		//}

		#endregion
	}
}
