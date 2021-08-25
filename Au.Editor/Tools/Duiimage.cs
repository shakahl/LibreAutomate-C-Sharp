using System.Windows.Controls;
using Au.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Interop;
using System.Drawing;

//CONSIDER: reorder controls:
/*
Capture		Test	OK	Cancel
...			speed	cbAction	Mouse
Wait		Exception
Image				Properties
*/

namespace Au.Tools
{
	class Duiimage : KDialogWindow
	{
		wnd _wnd, _con;
		bool _useCon;
		System.Drawing.Bitmap _image;
		RECT _rect;
		bool _isColor;
		int _color;
		string _imageFile;

		KSciInfoBox _info;
		KPopup _ttInfo;
		Button _bTest, _bOK;
		Label _speed;
		_PictureBox _pict;
		KSciCodeBoxWnd _code;

		KCheckBox controlC, allC, exceptionC, exceptionnoC;
		KCheckTextBox rectC, diffC, skipC, waitC, waitnoC;
		KCheckComboBox wiflagsC, mouseC;
		ComboBox function;

		public Duiimage() {
			Title = "Find image or color in window";

			_noeventValueChanged = true;
			var b = new wpfBuilder(this).WinSize((500, 450..), (430, 340..)).Columns(180, -1);
			b.R.Add(out _info).Height(60);
			b.R.StartGrid()
				.R.AddButton("_Capture", _bCapture_Click).AddButton("_...", _bEtc_Click).Width(30).Align("L").Tooltip("File, array")
				.R.AddButton(out _bTest, "_Test", _bTest_Click).Width(70..).Disabled().Tooltip("Executes the code now.\nShows rectangle of the found image.\nIgnores options: Find all, wait, Exception, Mouse.")
				.Add(out _speed).Tooltip("The search time (window + image). Red if not found.")
				.R.AddOkCancel(out _bOK, out _, out _).Align("L"); _bOK.IsEnabled = false;
			b.Row(110).xAddInBorder(out _pict);
			b.End();
			b.OkApply += _bOK_Click;

			b.xStartPropertyGrid();
			(controlC = b.xAddCheck("Control")).IsEnabled = false;
			b.xAddButton("Window/control...", _bWnd_Click);
			rectC = b.xAddCheckText("Rectangle", "(left, top, width, height)"); b.And(20).xAddButton("...", _bRect_Click);
			wiflagsC = b.xAddCheckCombo("Window pixels", "IFFlags.WindowDC|IFFlags.PrintWindow");
			diffC = b.xAddCheckText("Color deviation", "10");
			skipC = b.xAddCheckText("Skip", "1");

			b.StartGrid().Columns(-1, 0);
			b.xAddOther(out function).Margin(2, 4, 8, 4); b.Items("Find or wait for image|Wait until image disappears");
			allC = b.xAddCheck("Get all", noNewRow: true);
			b.End();
			waitnoC = b.xAddCheckText("Timeout", "5"); waitnoC.Visible = false;
			(exceptionnoC = b.xAddCheck("Exception on timeout")).IsChecked = true; exceptionnoC.Visibility = System.Windows.Visibility.Collapsed;
			waitC = b.xAddCheckText("Wait", "1", check: true);
			(exceptionC = b.xAddCheck("Exception if not found")).IsChecked = true;
			(mouseC = b.xAddCheckCombo("Mouse", "Move|Click|Right click")).c.IsChecked = true;

			b.xEndPropertyGrid();
			b.Row(-1).xAddInBorder(out _code);
			b.End();
			_noeventValueChanged = false;

			WndSavedRect.Restore(this, App.Settings.tools_Duiimage_wndPos, o => App.Settings.tools_Duiimage_wndPos = o);
		}

		static Duiimage() {
			TUtil.OnAnyCheckTextBoxValueChanged<Duiimage>((d, o) => d._AnyCheckTextBoxComboValueChanged(o), comboToo: true);
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);

			_InitInfo();
		}

		protected override void OnClosed(EventArgs e) {
			if (_image != null) _pict.Image = _image = null;

			base.OnClosed(e);
		}

		private void _bCapture_Click(WBButtonClickArgs e) {
			if (!_CaptureImageOrRect(false, out var r)) return;
			_imageFile = null;
			_SetImage(r);
		}

		private void _bWnd_Click(WBButtonClickArgs e) {
			var r = _code.ZShowWndTool(this, _wnd, _con, !_useCon);
			if (r.ok) _SetWndCon(r.w, r.con, r.useCon, true);
		}

		private void _bRect_Click(WBButtonClickArgs e) {
			if (_wnd.Is0) return;
			var m = new popupMenu();
			m["Rectangle of the captured image"] = o => _SetRect(_rect);
			m["Select rectangle..."] = o => { if (_CaptureImageOrRect(true, out var r)) _SetRect(r.rect); };
			m.Show();
			void _SetRect(RECT k) => rectC.Set(true, k.ToStringFormat("({0}, {1}, {4}, {5})"));
		}

		//Use r on Capture. Use image on Open.
		void _SetImage(ICResult r = null, Bitmap image = null) {
			if (r != null) { //on Capture
				var w = r.w.Window; if (w.Is0) return;
				_SetWndCon(w, r.w, true, false);
				if (_isColor = (r.image == null)) {
					using var g = Graphics.FromImage(r.image = new Bitmap(16, 16));
					g.Clear((Color)r.color);
				}
				_color = r.color.argb & 0xffffff;
				_image = r.image;
				_rect = r.rect;
			} else { //on Open
				_color = 0;
				_image = image;
				_rect = new RECT(0, 0, image.Width, image.Height);
			}

			//set _pict
			_pict.Image = _image;

			//set _code
			_FormatCode();

			_bTest.IsEnabled = true; _bOK.IsEnabled = true;

			if (_MultiIsActive && dialog.showYesNo("Add to array?", owner: this)) _MultiAdd();
		}

		void _SetWndCon(wnd w, wnd con, bool useCon, bool updateCodeIfNeed) {
			var wPrev = _WndSearchIn;
			_wnd = w;
			_con = con == w ? default : con;

			_noeventValueChanged = !updateCodeIfNeed;
			_useCon = useCon && !_con.Is0;
			controlC.IsChecked = _useCon; controlC.IsEnabled = !_con.Is0;
			if (_WndSearchIn != wPrev) rectC.c.IsChecked = false;
			_noeventValueChanged = false;
		}

		//when checked/unchecked any checkbox, and when text changed of any textbox
		void _AnyCheckTextBoxComboValueChanged(object source) {
			if (!_noeventValueChanged) {
				_noeventValueChanged = true;
				//print.it("source", source);
				if (source is KCheckBox c) {
					bool on = c.IsChecked;
					if (c == controlC) {
						if (_useCon = on) _wnd.MapClientToClientOf(_con, ref _rect); else _con.MapClientToClientOf(_wnd, ref _rect);
						rectC.c.IsChecked = false;
					} else if (c == wiflagsC.c) {
						if (_image != null) TUtil.InfoTooltip(ref _ttInfo, c, "After changing 'Window pixels' may need to capture again.\nClick Test. If not found, click Capture.");
					} else if (c == skipC.c) {
						if (on) allC.IsChecked = false;
					} else if (c == allC) {
						if (on) skipC.c.IsChecked = false;
					}
				} else if (source is TextBox t && t.Tag is KCheckTextBox k) {
					_noeventValueChanged = _formattedOnValueChanged = false; //allow auto-check but prevent formatting twice
					k.CheckIfTextNotEmpty();
					if (_formattedOnValueChanged) return;
				} else if (source is ComboBox u && u.Tag is KCheckComboBox m) {
					_noeventValueChanged = _formattedOnValueChanged = false; //allow auto-check but prevent formatting twice
					m.c.IsChecked = true;
					if (_formattedOnValueChanged) return;
				} else if (source == function) {
					bool not = function.SelectedIndex == 1;
					waitnoC.Visible = not;
					exceptionnoC.Visibility = !not ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
					waitC.Visible = !not;
					mouseC.Visible = !not;
					exceptionC.Visibility = not ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
					allC.Visibility = not ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
				}
				_noeventValueChanged = false;
				_formattedOnValueChanged = true;
				//print.it("format");
				_FormatCode();
			}
		}
		bool _noeventValueChanged, _formattedOnValueChanged;

		(string code, string wndVar) _FormatCode(bool forTest = false) {
			//print.it("_FormatCode");
			if (_image == null) return default;

			var b = new StringBuilder();

			string waitTime = null;
			bool waitNot = false, wait = false, orThrow = false, notTimeout = false, findAll = false, isMulti = false;
			if (!forTest) {
				if (waitNot = function.SelectedIndex == 1) {
					notTimeout = waitnoC.GetText(out waitTime, emptyToo: true);
					orThrow = notTimeout && exceptionnoC.IsChecked;
					if (waitTime.NE()) waitTime = "0";
				} else {
					wait = waitC.GetText(out waitTime, emptyToo: true);
					orThrow = exceptionC.IsChecked;
					findAll = allC.IsChecked;
				}
				isMulti = _multi != null && _multi.Count != 0;
			}
			bool isColor = _isColor && !isMulti;

			b.Append(waitNot ? "uiimage.waitNot(" : "uiimage.find(");
			if (wait || waitNot || orThrow) b.AppendWaitTime(waitTime ?? "0", orThrow).Append(", ");

			var (wndCode, wndVar) = _code.ZGetWndFindCode(_wnd, _useCon ? _con : default);

			if (rectC.GetText(out var sRect)) b.AppendFormat("({0}, {1})", wndVar, sRect);
			else b.Append(wndVar);

			b.AppendOtherArg(isColor ? "0x" : "image");
			if (isColor) b.Append(_color.ToString("X6"));

			if (wiflagsC.GetText(out var wiFlag)) b.AppendOtherArg(wiFlag);

			if (diffC.GetText(out var colorDiff) && colorDiff != "0") b.AppendOtherArg(colorDiff, "colorDiff");

			string also = null;
			if (findAll) {
				also = "o => { all.Add(o); return IFAlso.OkFindMore; }";
			} else if (skipC.GetText(out var skip)) {
				also = "o => o.Skip(" + skip + ")";
			}
			if (also != null) b.AppendOtherArg(also, "also");

			b.Append(");");

			var bb = new StringBuilder();
			bb.AppendLine(wndCode);

			if (!isColor) {
				if (isMulti) {
					bb.AppendLine("IFImage[] image = {");
					foreach (var v in _multi) bb.Append('\t').Append(v).AppendLine(",");
					bb.Append('}');
				} else {
					bb.Append("string image = ").Append(_CurrentImageString());
				}
				bb.AppendLine(";");
			}

			if (!forTest) {
				if (waitNot) {
					if (!orThrow && notTimeout) bb.Append("bool ok = ");
				} else {
					mouseC.GetIndex(out int mi);
					string mouse = mi switch {
						0 => "im.MouseMove();",
						1 => "im.MouseClick();",
						2 => "im.MouseClick(button: MButton.Right);",
						_ => null
					};
					if (findAll) {
						bb.AppendLine("var all = new List<uiimage>();");
						b.Append("\r\nforeach(var im in all) { ");
						if (mouse != null) b.Append(mouse).Append(" 250.ms(); ");
						b.Append('}');
					} else {
						bb.Append("var im = ");
						if (!orThrow || mouse != null) b.AppendLine();
						if (!orThrow) b.Append("if(im != null) { ");
						if (mouse != null) b.Append(mouse);
						if (!orThrow) b.Append(" } else { print.it(\"not found\"); }");
					}
				}
			}

			var R = bb.Append(b).ToString();

			if (!forTest) _code.ZSetText(R, wndCode.Length);

			return (R, wndVar);
		}

		#region util, misc

		wnd _WndSearchIn => _useCon ? _con : _wnd;

		bool _CaptureImageOrRect(bool rect, out ICResult r) {
			ICFlags fl = 0;
			if (rect) fl = ICFlags.Rectangle;
			else if (wiflagsC.c.IsChecked) fl = wiflagsC.t.SelectedIndex == 0 ? ICFlags.WindowDC : ICFlags.PrintWindow; //FUTURE: how rect is if DPI-scaled window?

			if (!uiimage.captureUI(out r, fl, this)) return false;

			string es = null;
			if (rect) {
				bool otherWindow = (_useCon ? r.w : r.w.Window) != (_useCon ? _con : _wnd);
				if (otherWindow) es = "Whole rectangle must be in the client area of the captured image's window or control.";
			} else if (r.w.Is0) {
				r.image?.Dispose(); r.image = null;
				es = "Whole image must be in the client area of a single window.";
			}
			if (es != null) {
				dialog.showError(null, es, owner: this);
				return false;
			}

			r.w.MapScreenToClient(ref r.rect);
			return true;
		}

		string _CurrentImageString() {
			if (_isColor) return "0x" + _color.ToString("X6");
			if (_imageFile != null) return "@\"" + _imageFile + "\"";
			var ms = new MemoryStream();
			_image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			return "@\"image:" + Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length) + "\"";
		}

		#endregion

		#region menu, open, save, multi

		void _bEtc_Click(WBButtonClickArgs e) {
			bool isImage = _image != null && !_isColor;

			var m = new popupMenu();
			m["Use file..."] = o => _OpenFile(false, e.Button);
			m["Embed file..."] = o => _OpenFile(true, e.Button);
			m["Save as file...", disable: !isImage] = o => _SaveFile();
			m.Separator();
			m["Add to array", disable: _image == null] = o => _MultiMenuAdd();
			m["Remove from array", disable: !_MultiIsActive] = o => _MultiRemove();
			m.Show();
		}

		void _OpenFile(bool embed, Button button) {
			if (_wnd.Is0) {
				TUtil.InfoTooltip(ref _ttInfo, button, "At first please select a window with button 'Capture' or 'Window/control'.");
				return;
			}

			var d = new OpenFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png" };
			if (d.ShowDialog(this) != true) return;
			var f = d.FileName;
			var im = new Bitmap(f);
			_imageFile = embed ? null : f;
			_SetImage(null, im);
		}
		const string c_fileDialogFilter = "png, bmp|*.png;*.bmp";

		bool _SaveFile() {
			var d = new SaveFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png" };
			if (d.ShowDialog(this) != true) return false;
			var f = d.FileName;
			_image.Save(f, f.Ends(".bmp", true) ? System.Drawing.Imaging.ImageFormat.Bmp : System.Drawing.Imaging.ImageFormat.Png);
			_MultiRemove(true);
			_imageFile = f;
			_MultiAdd();
			_FormatCode();
			return true;
		}

		void _MultiMenuAdd() {
			if (_multi == null) _multi = new HashSet<string>();
			_MultiAdd();
		}

		bool _MultiIsActive => _multi != null && _image != null;

		void _MultiAdd() {
			if (!_MultiIsActive) return;
			_multi.Add(_CurrentImageString());
			_FormatCode();
		}
		HashSet<string> _multi;

		void _MultiRemove(bool noUpdateCode = false) {
			if (!_MultiIsActive) return;
			_multi.Remove(_CurrentImageString());
			if (!noUpdateCode) _FormatCode();
		}

		//FUTURE: make transparent.
		//	This code is not useful because images are often alpha-blended with background. Need to make transparent near-color pixels too.
		//	Let the user set color difference with preview. Maybe even draw or flood-fill clicked areas.
		//void _MakeTransparent(int corner)
		//{
		//	int x = 0, y = 0;

		//	_image.MakeTransparent(_image.GetPixel(x, y));
		//	_pict.Invalidate();
		//}

		#endregion

		#region OK, Test

		/// <summary>
		/// When OK clicked, contains C# code. Else null.
		/// </summary>
		public string ZResultCode { get; private set; }

		private void _bOK_Click(WBButtonClickArgs e) {
			var s = _code.zText;
			if (s.NE()) { ZResultCode = null; e.Cancel = true; return; }
			ZResultCode = s = s.Replace("@\"image:", "@\"image:\r\n");
			InsertCode.Statements(s, fold: true);
		}

		private void _bTest_Click(WBButtonClickArgs ea) {
			var (code, wndVar) = _FormatCode(true); if (code == null) return;
			TUtil.RunTestFindObject(code, wndVar, _WndSearchIn, _bTest, _speed, _info, o => (o as uiimage).RectInScreen, activateWindow: true);
		}

		#endregion

		#region info

		//TUtil.CommonInfos _commonInfos;
		void _InitInfo() {
			//_commonInfos = new TUtil.CommonInfos(_info);

			_info.zText = c_dialogInfo;
			_info.AddElem(this, c_dialogInfo);

			_info.InfoC(controlC,
@"Search only in control (if captured), not in whole window.
To change window or/and control name etc, click 'Window/control...' or edit it in the code field.");
			_info.InfoCT(rectC,
@"Limit the search area to this rectangle in the client area of the window or control. Smaller = faster.");
			_info.InfoCO(wiflagsC,
@"Get pixels from window, not from screen.
Usually it makes faster. Also, window can be in background.
Works not with all windows. WindowDC is fastest. PrintWindow works with more windows.");
			_info.InfoCT(diffC,
@"Parameter colorDiff.
Valid values: 0 - 250.
Bigger = slower.");
			_info.InfoCT(skipC,
@"0-based index of matching image.
For example, if 1, gets the second matching image.");
			_info.Info(function, "Function", "Create code for function find() or waitNot().");
			_info.InfoC(allC, "Find all matching images.");
			_info.InfoCT(waitC, @"The wait timeout, seconds.
The function waits max this time interval. On timeout throws exception if 'Exception...' checked, else returns null. If empty, uses 1e11 (3251 years).");
			_info.InfoCT(waitnoC, @"The wait timeout, seconds.
The function waits max this time interval. On timeout throws exception if 'Exception...' checked, else returns false. No timeout if unchecked or 0 or empty.");
			_info.InfoC(exceptionC,
@"Throw exception if not found.
If unchecked, returns null.");
			_info.InfoC(exceptionnoC,
@"Throw exception on timeout.
If unchecked, returns false.");
			_info.InfoCO(mouseC, "When found, call MouseMove or MouseClick.");
		}

		const string c_dialogInfo =
@"This dialog creates code to find/click <help uiimage.find>image or color<> in <help wnd.find>window<>.
1. Click the Capture button. Mouse-draw rectangle around the image.
2. Click the Test button. It finds and shows the image.
3. If need, change some fields.
4. Click OK, it inserts C# code in editor. Or copy/paste.
5. If need, edit the code in editor: rename variables, delete duplicate wnd.find lines, replace part of window name with *, etc.";

		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			_ttInfo?.Close();
			base.OnPreviewKeyDown(e);
		}

		#endregion

		//display image in HwndHost.
		//	Tried WPF Image, but blurry when high DPI, even if bitmap's DPI is set to match window's DPI.
		//	UseLayoutRounding does not help in this case.
		//	RenderOptions.SetBitmapScalingMode(NearestNeighbor) helps it seems. But why it tries to scale the image, and how many times?
		//	Tried winforms PictureBox, bust sometimes very slowly starts.
		class _PictureBox : HwndHost
		{
			wnd _w;

			protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
				var wParent = (wnd)hwndParent.Handle;
				_w = WndUtil.CreateWindow(_wndProc = _WndProc, false, "Static", null, WS.CHILD | WS.CLIPCHILDREN, 0, 0, 0, 10, 10, wParent);

				return new HandleRef(this, _w.Handle);
			}

			protected override void DestroyWindowCore(HandleRef hwnd) {
				Api.DestroyWindow(_w);
			}

			public Bitmap Image {
				get => _b;
				set {
					_b?.Dispose();
					_b = value;
					Api.InvalidateRect(_w);
				}
			}
			Bitmap _b;

			WNDPROC _wndProc;
			nint _WndProc(wnd w, int msg, nint wParam, nint lParam) {
				//var pmo = new PrintMsgOptions(Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x10c1);
				//if (WndUtil.PrintMsg(out string s, _w, msg, wParam, lParam, pmo)) print.it("<><c green>" + s + "<>");

				switch (msg) {
				case Api.WM_NCHITTEST:
					return Api.HTTRANSPARENT;
				case Api.WM_PAINT:
					var dc = Api.BeginPaint(w, out var ps);
					_WmPaint(dc);
					Api.EndPaint(w, ps);
					return default;
				}

				return Api.DefWindowProc(w, msg, wParam, lParam);
			}

			void _WmPaint(IntPtr dc) {
				using var g = Graphics.FromHdc(dc);
				g.Clear(System.Drawing.SystemColors.AppWorkspace);
				if (_b == null) return;

				g.DrawImage(_b, 0, 0);
			}
		}
	}
}
