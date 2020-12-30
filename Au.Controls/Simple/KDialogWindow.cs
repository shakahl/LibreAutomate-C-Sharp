using Au.Types;
using System;
using System.Windows;

namespace Au.Controls
{
	/// <summary>
	/// Can be used as base class for WPF windows used as dialogs.
	/// Adds WS_POPUP style, which prevents activating an unrelated window when closing this active owned nonmodal window (OS bug).
	/// </summary>
	public class KDialogWindow : Window
	{
		protected override void OnSourceInitialized(EventArgs e) {
			this.Hwnd().SetStyle(WS.POPUP, WSFlags.Add);
			base.OnSourceInitialized(e);
		}
	}
}
