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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.Controls;

using SG = SourceGrid;

//FUTURE:
//Option to use child window.

//TODO: option to get base64 strings of all captured images, as array. Maybe automatically print.

namespace Au.Tools
{
	public partial class Form_WinImage :Form_
	{
		Wnd _wnd;
		Bitmap _image;
		RECT _rect;
		bool _isColor;
		int _color;
		string _imageFile;

		public Form_WinImage()
		{
			InitializeComponent();

			_grid.ZValueChanged += _OnValueChanged;
			_tWnd.WndVarNameChanged += (unu, sed) => _OnGridChanged();
		}

		const string c_registryKey = @"\Tools\WinImage";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Wnd w = (Wnd)this;
			if(Registry_.GetString(out var wndPos, "wndPos", c_registryKey))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }

			_info.Text = c_infoForm;
			_FillGrid();

			//test grid combo
			//_grid.ZAddOptional(null, "Text", "TEXT");
			//_grid.ZAddOptional(null, "ComboText", "one|two|three", etype: ParamGrid.EditType.ComboText);
			//_grid.ZAddOptional(null, "ComboList", "one|two|three", etype: ParamGrid.EditType.ComboList);
			//int i=_grid.ZAddOptional(null, "TextButton", "TEXT", etype: ParamGrid.EditType.TextButton, buttonAction: () => Print(1));

			//_grid.ZAutoSizeRows();
			//_grid.ZAutoSizeColumns();
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			Wnd w = (Wnd)this;
			Registry_.SetString(w.SavePositionSizeState(), "wndPos", c_registryKey);

			base.OnFormClosing(e);
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			if(_image != null) {
				_pict.Image = null;
				_image.Dispose();
				_image = null;
			}

			base.OnFormClosed(e);
		}

		#region capture, save, open

		private void _bCapture_Click(object sender, EventArgs e)
		{
			if(!_CaptureImageOrRect(false, out var r)) return;
			_SetImage(r);
			if(_imageFile != null && !_isColor) {
				try { if(!_SaveFile()) _imageFile = null; } catch { _imageFile = null; throw; }
			}
		}

		void _SetImage(WICResult r)
		{
			if(_isColor = (r.image == null)) using(var g = Graphics.FromImage(r.image = new Bitmap(16, 16))) g.Clear((Color)r.color);
			_color = r.color.color & 0xffffff;
			_image = r.image;
			_rect = r.rect;

			//set _pict
			var oldImage = _pict.Image;
			_pict.Image = _image;
			oldImage?.Dispose();

			//set _tWnd
			_wnd = r.wnd;
			_tWnd.Hwnd = _wnd;

			//set _tFind
			_OnGridChanged();

			_bTest.Enabled = true; _bOK.Enabled = true; _bCopy.Enabled = true; _bEtc.Enabled = !_isColor;
		}

		bool _SaveFile()
		{
			var d = new SaveFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png", FileName = _imageFile };
			if(d.ShowDialog(this) != DialogResult.OK) return false;
			var f = d.FileName;
			_image.Save(f, f.EndsWith_(".bmp", true) ? System.Drawing.Imaging.ImageFormat.Bmp : System.Drawing.Imaging.ImageFormat.Png);
			_imageFile = f;
			return true;
		}

		void _OpenFile(bool embed)
		{
			var d = new OpenFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png" };
			if(d.ShowDialog(this) != DialogResult.OK) return;
			var f = d.FileName;
			var im = Image.FromFile(f) as Bitmap;
			_imageFile = embed ? null : f;
			var r = new WICResult() { image = im, wnd = _wnd, rect = (0, 0, im.Width, im.Height) };
			_SetImage(r);
		}

		private void _bEtc_Click(object sender, EventArgs e)
		{
			if(_image != null && !_isColor) {
				var m = new AuMenu();
				m["Save as file..."] = o => _SaveFile();
				//the 'open' items should be added when _image==null, but then need to capture window. Better let the user at first capture image+window in the familiar way.
				m["Use file..."] = o => _OpenFile(false);
				m["Embed file..."] = o => _OpenFile(true);

				//FUTURE: make transparent.
				//	This code is not useful because images are often alpha-blended with background. Need to make transparent near-color pixels too.
				//	Ideally let the user set color difference with preview. Maybe even draw or flood-fill clicked areas.
				//m.Separator();
				//using(m.Submenu("Make transparent")) {
				//	m["Pixels of top-left color"] = o => _MakeTransparent(0);
				//	//m["Pixels of bottom-left color"] = o => _MakeTransparent(1);

				//	void _MakeTransparent(int corner)
				//	{
				//		int x=0, y=0;

				//		_image.MakeTransparent(_image.GetPixel(x, y));
				//		_pict.Invalidate();
				//	}
				//}

				m.Show(this);
			}
		}
		const string c_fileDialogFilter = "png, bmp|*.png;*.bmp";

		#endregion

		void _FillGrid()
		{
			_noeventGridValueChanged = true;
			_grid.Clear();

			_AddProp("rect", "Search in rectangle", "(left, top, width, height)", tt: "Limit the search area to this rectangle in window client area.\nSmaller = faster.", etype: ParamGrid.EditType.TextButton, buttonAction: () =>
			{
				if(_wnd.Is0) return;
				var m = new AuMenu();
				m["Rectangle of the captured image"] = o => _SetRect(_rect);
				m["Select rectangle..."] = o => { if(_CaptureImageOrRect(true, out var r)) _SetRect(r.rect); };
				m.Show(this);
				void _SetRect(RECT k)
				{
					_grid.ZSetCellText("rect", 1, $"({k.left}, {k.top}, {k.Width}, {k.Height})");
					_Check("rect", true);
				}
			});
			_AddFlag("WindowDC", "Window can be in background", tt: "Flag WindowDC.\nMakes faster etc.");
			_AddProp("colorDiff", "Allow color difference", "10", tt: "Parameter colorDiff.\nValid values: 0 - 250.\nBigger = slower.");
			_AddProp("skip", "Skip matching images", "1", tt: "0-based index of matching image.\nFor example, if 1, gets the second matching image.");
			_grid.ZAddHeaderRow("Results, wait, action");
			_AddFlag("all", "Find all", tt: "Find all matching images and get array of rectangles etc.");
			_AddProp("Wait", "Wait for image", "5", tt: c_infoWait);
			_AddProp("WaitNot", "Wait until disappears", "5", tt: c_infoWait);
			_AddFlag("orThrow", "Throw exception if not found (or on timeout)", true, tt: "Checked - throw exception.\nUnchecked - return null.");
			_AddProp(null, "Mouse", "Move|Click|Right click", tt: "Move the mouse to the found image and optionally click.\nIf unchecked, gets rectangle etc.", etype: ParamGrid.EditType.ComboList, check: true);

			_noeventGridValueChanged = false;
			_grid.ZAutoSizeRows();
			_grid.ZAutoSizeColumns();

			void _AddProp(string key, string name, string value, string tt = null, ParamGrid.EditType etype = ParamGrid.EditType.Text, Action buttonAction = null, bool check = false)
			{
				_grid.ZAddOptional(key, name, value, check, tt, etype: etype, buttonAction: buttonAction);
			}

			void _AddFlag(string flag, string name, bool check = false, string tt = null)
			{
				_grid.ZAddFlag(flag, name, check, tt: tt);
			}
		}

		void _OnValueChanged(SG.CellContext sender)
		{
			if(_noeventGridValueChanged) return; _noeventGridValueChanged = true;
			var g = sender.Grid as ParamGrid;
			var pos = sender.Position;
			switch(pos.Column) {
			case 0:
				bool on = (sender.Cell as SG.Cells.CheckBox).Checked.GetValueOrDefault();
				switch(g.ZGetRowKey(pos.Row)) {
				case "WindowDC":
					if(_image != null) {
						_info.Text = "After changing 'Window can be in background' may need to capture again.\nClick Test. If not found, click Capture.";
						Timer_.After(15_000, () => _info.Text = c_infoForm);
					}
					break;
				case "skip": if(on) _Check("all", false); break;
				case "all": if(on) { _Check("skip", false); _Check("WaitNot", false); } break;
				case "Wait": if(on) _Check("WaitNot", false); break;
				case "WaitNot": if(on) { _Check("Wait", false); _Check("all", false); _Check("Mouse", false); } break;
				case "Mouse": if(on) _Check("WaitNot", false); break;
				}
				break;
			case 1:
				break;
			}
			_noeventGridValueChanged = false;
			_OnGridChanged();
		}
		bool _noeventGridValueChanged;

		void _OnGridChanged()
		{
			//Print("_OnGridChanged");
			_tFind.Text = _FormatFindCode(_FormatCaller.Display);
		}

		enum _FormatCaller { Display, Test, OK }

		string _FormatFindCode(_FormatCaller caller)
		{
			if(_image == null) return null;

			bool forTest = caller == _FormatCaller.Test, forOK = caller == _FormatCaller.OK, forDisplay = caller == _FormatCaller.Display;

			var b = new StringBuilder();

			bool orThrow = !forTest && _IsChecked("orThrow");
			bool findAll = !forTest && _IsChecked("all");

			int waitFunc = 0; string waitTime = null;
			if(!forTest) {
				if(_grid.ZGetValue("Wait", out waitTime, false)) waitFunc = 1;
				else if(_grid.ZGetValue("WaitNot", out waitTime, false)) waitFunc = 2;
			}
			if(waitFunc != 0) {
				b.Append(waitFunc == 1 ? "WinImage.Wait(" : "WinImage.WaitNot(");
				if(waitTime == null) waitTime = "0"; else if(!orThrow && waitTime != "0" && !waitTime.StartsWith_('-')) b.Append('-');
				b.Append(waitTime).Append(", ");
			} else {
				b.Append("WinImage.Find(");
			}

			if(_grid.ZGetValue("rect", out var sRect, true)) {
				b.Append("(").Append(_tWnd.WndVar).Append(", ").Append(sRect).Append(")");
			} else b.Append(_tWnd.WndVar);

			ToolsUtil.AppendOtherArg(b, _isColor ? "0x" : "image");
			if(_isColor) b.Append(_color.ToString("X6"));

			if(_IsChecked("WindowDC")) ToolsUtil.AppendOtherArg(b, "WIFlags.WindowDC");
			if(_grid.ZGetValue("colorDiff", out var colorDiff, true) && colorDiff != "0") ToolsUtil.AppendOtherArg(b, colorDiff, "colorDiff");

			string also = null;
			if(findAll) {
				also = "o => { all.Add(o); return WIAlso.FindMoreAndReturn; }";
			} else if(_grid.ZGetValue("skip", out var skip, true)) {
				also = "o => o.Skip(" + skip + ")";
			}
			if(also != null) ToolsUtil.AppendOtherArg(b, also, "also");

			b.Append(")");
			if(orThrow && waitFunc == 0) b.Append(".OrThrow()");
			b.Append(";");

			if(forDisplay) return b.ToString();

			var bb = new StringBuilder();
			if(!forTest) bb.AppendLine(_tWnd.Text);
			if(!_isColor) {
				bb.Append("string image = ");
				if(_imageFile != null) {
					bb.Append("@\"").Append(_imageFile);
				} else {
					using(var ms = new MemoryStream()) {
						_image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
						bb.Append("\"image:").Append(Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length));
					}
				}
				bb.AppendLine("\";");
			}

			if(forTest) {
				bb.Append(b.ToString());
			} else { //forOK
				if(_grid.ZGetValue("Mouse", out string mouse, true)) {
					switch(mouse) {
					case "Move": mouse = "wi.MouseMove();"; break;
					case "Click": mouse = "wi.MouseClick();"; break;
					default: mouse = "wi.MouseClick(button: MButton.Right);"; break;
					}
				}

				if(findAll) {
					bb.AppendLine("var all = new List<WinImage>();");
					b.Append("\r\nforeach(var wi in all) { ");
					if(mouse != null) b.Append(mouse).Append(" 100.ms(); ");
					b.Append("}");
				} else if(waitFunc != 2) {
					bb.Append("var wi = ");
					if(!orThrow || mouse != null) b.AppendLine();
					if(!orThrow) b.Append("if(wi != null) { ");
					if(mouse != null) b.Append(mouse);
					if(!orThrow) b.Append(" } else { Print(\"not found\"); }");
				} else if(!orThrow) {
					bb.Append("bool ok = ");
				}

				bb.Append(b.ToString());
			}

			Output.Clear(); Print(bb.ToString()); Print("---"); //TODO
			return bb.ToString();
		}

		#region util

		bool _IsChecked(string rowKey) => _grid.ZIsChecked(rowKey);
		void _Check(string rowKey, bool check) => _grid.ZCheck(rowKey, check);

		bool _CaptureImageOrRect(bool rect, out WICResult r)
		{
			r = null;
			WICFlags fl = 0;
			if(rect) fl = WICFlags.Rectangle;
			else if(_IsChecked("WindowDC")) fl |= WICFlags.WindowDC; //FUTURE: how rect is if DPI-scaled window?
			if(!WinImage.CaptureUI(out r, fl, this)) return false;
			r.wnd = r.wnd.WndWindow;

			string es = null;
			if(rect) {
				if(r.wnd != _wnd) es = "Whole rectangle must be in the client area of the captured image's window.";
			} else if(r.wnd.Is0) {
				r.image?.Dispose(); r.image = null;
				es = "Whole image must be in the client area of a single window.";
			}
			if(es != null) {
				AuDialog.ShowError(null, es, owner: this);
				return false;
			}

			r.wnd.MapScreenToClient(ref r.rect);
			return true;
		}

		#endregion

		#region OK, Copy

		private void _bCopy_Click(object sender, EventArgs e)
		{
			var s = _FormatFindCode(_FormatCaller.OK);
			if(s != null) Clipboard.SetText(s);
		}

		private void _bOK_Click(object sender, EventArgs e)
		{
			ResultCode = _FormatFindCode(_FormatCaller.OK);
			if(ResultCode == null) this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// When OK clicked, contains C# code.
		/// </summary>
		public string ResultCode { get; private set; }

		#endregion

		#region test

		private async void _bTest_Click(object sender, EventArgs e)
		{
			var code = _FormatFindCode(_FormatCaller.Test);
			if(code == null) return;
			_wnd.ActivateLL(); Time.SleepDoEvents(200);
			var r = await ToolsUtil.RunTestFindObject(code, _tWnd, _bTest, _lSpeed, o => (o as WinImage).RectInScreen);
		}

		#endregion

		#region info

		const string c_infoForm =
@"Creates code to find image or color in window. Then your script can click it, etc. See <help M_Au_WinImage_Find>WinImage.Find<>, <help M_Au_Wnd_Find>Wnd.Find<>. How to use:
1. Click the Capture button. Mouse-draw rectangle around the image.
2. Click the Test button. It finds and shows the image and the search time.
3. If need, check/uncheck/edit some fields; click Test.
4. Click OK, it inserts C# code in the editor. Or Copy to the clipboard.
5. If need, edit the code in the editor: rename variables, delete duplicate Wnd.Find lines, replace part of window name with *, etc.";
		const string c_infoWait = @"Wait max this time interval, seconds.
If 0 or empty, no timeout. Else on timeout the function returns null or throws exception, depending on the 'Throw...' checkbox.";

		#endregion
	}
}
