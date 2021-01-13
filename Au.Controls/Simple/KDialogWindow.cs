using Au.Types;
using System;
using System.Windows;
using System.Windows.Threading;

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

		/// <summary>
		/// Sets <b>Owner</b>, calls <b>Show</b> and waits until closed.
		/// Unlike <b>ShowDialog</b>, does not disable thread windows. Also, <b>DialogResult</b> cannot be used.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="hideOwner">Temporarily hide owner.</param>
		public void ShowAndWait(Window owner, bool hideOwner = false) {
			Owner = owner;
			if (hideOwner) owner.Hide();
			Show();
			_dispFrame = new DispatcherFrame();
			Dispatcher.PushFrame(_dispFrame);
			if (hideOwner) owner.Show();
		}
		DispatcherFrame _dispFrame;

		void _EndModal() {
			if (_dispFrame != null) {
				_dispFrame.Continue = false;
				_dispFrame = null;
			}
		}

		///
		protected override void OnClosed(EventArgs e) {
			_EndModal();
			base.OnClosed(e);
		}
	}
}
