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

			_grid.ZValueChanged += _grid_ZValueChanged;
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

		private void _bCapture_Click(object sender, EventArgs e)
		{
			if(!_CaptureImageOrRect(false, out var r)) return;
			_imageFile = null;
			_SetImage(r);
		}

		void _SetImage(WICResult r)
		{
			if(_isColor = (r.image == null)) using(var g = Graphics.FromImage(r.image = new Bitmap(16, 16))) g.Clear((Color)r.color);
			_color = r.color.color & 0xffffff;
			_image = r.image;
			var wndOld = _wnd;
			_wnd = r.wnd;
			_rect = r.rect;

			//set _pict
			var oldImage = _pict.Image;
			_pict.Image = _image;
			oldImage?.Dispose();

			//set _code
			_FormatCode(newWindow: _wnd != wndOld);

			_bTest.Enabled = true; _bOK.Enabled = true;

			if(_MultiIsActive && AuDialog.ShowYesNo("Add to array?", owner: this)) _MultiAdd();

			_errorProvider.Clear();
		}

		void _FillGrid()
		{
			_noeventGridValueChanged = true;
			var g = _grid;
			g.Clear();

			//g.ZAdd("wnd", "Window", "Edit...", null, etype: ParamGrid.EditType.Button, buttonAction: (unu, sed) => Print("FUTURE"));
			g.ZAdd("rect", "Search in rectangle", "(left, top, width, height)", etype: ParamGrid.EditType.TextButton, buttonAction: (unu, sed) =>
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
			}, tt: "Limit the search area to this rectangle in window client area.\nSmaller = faster.");
			g.ZAddCheck("WindowDC", "Window can be in background", tt: "Flag WindowDC.\nMakes faster etc.");
			g.ZAdd("colorDiff", "Allow color difference", "10", tt: "Parameter colorDiff.\nValid values: 0 - 250.\nBigger = slower.");
			g.ZAdd("skip", "Skip matching images", "1", tt: "0-based index of matching image.\nFor example, if 1, gets the second matching image.");

			//g.ZAddHeaderRow("Results, wait, action");
			g.ZAddCheck("all", "Find all", tt: "Find all matching images");
			g.ZAdd("Wait", "Wait", "5", tt: c_infoWait);
			g.ZAdd("WaitNot", "Wait until disappears", "5", tt: c_infoWait);
			g.ZAddCheck("orThrow", "Exception if not found", true);
			g.ZAdd("Mouse", "Mouse", "Move|Click|Right click", true, tt: "When found, call MouseMove or MouseClick", etype: ParamGrid.EditType.ComboList, comboIndex: 0);

			_noeventGridValueChanged = false;
			g.ZAutoSize();
		}

		void _grid_ZValueChanged(SG.CellContext sender)
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
						_errorProvider.Icon = Icons.GetStockIcon(StockIcon.INFO, 16);
						_errorProvider.SetError(_bTest, "After changing 'Window can be in background' may need to capture again.\nClick Test. If not found, click Capture.");
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
			_UpdateCodeBox();
		}
		bool _noeventGridValueChanged;

		string _FormatCode(bool forTest = false, bool newWindow = false)
		{
			if(_image == null) return null;

			var b = new StringBuilder();

			bool findAll = !forTest && _IsChecked("all");
			bool orThrow = !forTest && _IsChecked("orThrow");
			bool isMulti = !forTest && _multi != null && _multi.Count != 0;
			bool isColor = _isColor && !isMulti;

			int waitFunc = 0; string waitTime = null;
			if(!forTest) {
				if(_grid.ZGetValue("Wait", out waitTime, false)) waitFunc = 1;
				else if(_grid.ZGetValue("WaitNot", out waitTime, false)) waitFunc = 2;
			}
			if(waitFunc != 0) {
				b.Append(waitFunc == 1 ? "WinImage.Wait(" : "WinImage.WaitNot(").AppendWaitTime(waitTime, orThrow).Append(", ");
			} else {
				b.Append("WinImage.Find(");
			}

			var (wndCode, wndVar) = _code.FormatWndFindCode(_wnd, newWindow);

			if(_grid.ZGetValue("rect", out var sRect, true)) b.Append("(").Append(wndVar).Append(", ").Append(sRect).Append(")");
			else b.Append(wndVar);

			b.AppendOtherArg(isColor ? "0x" : "image");
			if(isColor) b.Append(_color.ToString("X6"));

			if(_IsChecked("WindowDC")) b.AppendOtherArg("WIFlags.WindowDC");
			if(_grid.ZGetValue("colorDiff", out var colorDiff, true) && colorDiff != "0") b.AppendOtherArg(colorDiff, "colorDiff");

			string also = null;
			if(findAll) {
				also = "o => { all.Add(o); return WIAlso.OkFindMore; }";
			} else if(_grid.ZGetValue("skip", out var skip, true)) {
				also = "o => o.Skip(" + skip + ")";
			}
			if(also != null) b.AppendOtherArg(also, "also");

			b.Append(")");
			if(orThrow && waitFunc == 0) b.Append(".OrThrow()");
			b.Append(";");

			var bb = new StringBuilder();
			bb.AppendLine(wndCode);

			if(!isColor) {
				if(isMulti) {
					bb.AppendLine("object[] image = {");
					foreach(var v in _multi) bb.Append('\t').Append(v).AppendLine(",");
					bb.Append('}');
				} else {
					bb.Append("string image = ").Append(_CurrentImageString());
				}
				bb.AppendLine(";");
			}

			if(!forTest) {
				if(_grid.ZGetValue("Mouse", out string mouse, true)) {
					switch(mouse) {
					case "Move": mouse = "im.MouseMove();"; break;
					case "Click": mouse = "im.MouseClick();"; break;
					default: mouse = "im.MouseClick(button: MButton.Right);"; break;
					}
				}

				if(findAll) {
					bb.AppendLine("var all = new List<WinImage>();");
					b.Append("\r\nforeach(var im in all) { ");
					if(mouse != null) b.Append(mouse).Append(" 100.ms(); ");
					b.Append("}");
				} else if(waitFunc != 2) {
					bb.Append("var im = ");
					if(!orThrow || mouse != null) b.AppendLine();
					if(!orThrow) b.Append("if(im != null) { ");
					if(mouse != null) b.Append(mouse);
					if(!orThrow) b.Append(" } else { Print(\"not found\"); }");
				} else if(!orThrow) {
					bb.Append("bool ok = ");
				}
			}

			bb.Append(b.ToString());
			var R = bb.ToString();

			if(!forTest) _code.ZSetText(R, wndCode.Length);

			return R;
		}

		#region util, misc

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

		string _CurrentImageString()
		{
			if(_isColor) return "0x" + _color.ToString("X6");
			if(_imageFile != null) return "@\"" + _imageFile + "\"";
			using(var ms = new MemoryStream()) {
				_image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
				return "@\"image:" + Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length) + "\"";
			}
		}

		void _UpdateCodeBox()
		{
			_FormatCode();
		}

		#endregion

		#region menu, open, save, multi

		private void _bEtc_Click(object sender, EventArgs e)
		{
			bool isImage = _image != null && !_isColor;

			var m = new AuMenu();
			m["Use file..."] = o => _OpenFile(false);
			m["Embed file..."] = o => _OpenFile(true);
			m["Save as file..."] = o => _SaveFile(); m.LastItem.Enabled = isImage;
			m.Separator();
			m["Add to array"] = o => _MultiMenuAdd(); m.LastItem.Enabled = _image != null;
			m["Remove from array"] = o => _MultiRemove(); m.LastItem.Enabled = _MultiIsActive;

			//if(isImage) {
			//	m.Separator();
			//	using(m.Submenu("Make transparent")) {
			//		m["Pixels of top-left color"] = o => _MakeTransparent(0);
			//		//m["Pixels of bottom-left color"] = o => _MakeTransparent(1);
			//	}
			//}

			m.Show(this);
		}

		void _OpenFile(bool embed)
		{
			if(_image == null) {
				AuDialog.ShowInfo("At first please capture window", "Close this dialog, click Capture, click the window.", owner: this);
				return;
			}

			var d = new OpenFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png" };
			if(d.ShowDialog(this) != DialogResult.OK) return;
			var f = d.FileName;
			var im = Image.FromFile(f) as Bitmap;
			_imageFile = embed ? null : f;
			var r = new WICResult() { image = im, wnd = _wnd, rect = (0, 0, im.Width, im.Height) };
			_SetImage(r);
		}
		const string c_fileDialogFilter = "png, bmp|*.png;*.bmp";

		bool _SaveFile()
		{
			var d = new SaveFileDialog { Filter = c_fileDialogFilter, DefaultExt = "png" };
			if(d.ShowDialog(this) != DialogResult.OK) return false;
			var f = d.FileName;
			_image.Save(f, f.EndsWith_(".bmp", true) ? System.Drawing.Imaging.ImageFormat.Bmp : System.Drawing.Imaging.ImageFormat.Png);
			_MultiRemove(true);
			_imageFile = f;
			_MultiAdd();
			_UpdateCodeBox();
			return true;
		}

		void _MultiMenuAdd()
		{
			if(_multi == null) _multi = new HashSet<string>();
			_MultiAdd();
		}

		bool _MultiIsActive => _multi != null && _image != null;

		void _MultiAdd()
		{
			if(!_MultiIsActive) return;
			_multi.Add(_CurrentImageString());
			_UpdateCodeBox();
		}
		HashSet<string> _multi;

		void _MultiRemove(bool noUpdateCode = false)
		{
			if(!_MultiIsActive) return;
			_multi.Remove(_CurrentImageString());
			if(!noUpdateCode) _UpdateCodeBox();
		}

		//FUTURE: make transparent.
		//	This code is not useful because images are often alpha-blended with background. Need to make transparent near-color pixels too.
		//	Ideally let the user set color difference with preview. Maybe even draw or flood-fill clicked areas.
		//void _MakeTransparent(int corner)
		//{
		//	int x = 0, y = 0;

		//	_image.MakeTransparent(_image.GetPixel(x, y));
		//	_pict.Invalidate();
		//}

		#endregion

		#region OK, Test

		/// <summary>
		/// When OK clicked, contains C# code.
		/// </summary>
		public string ResultCode { get; private set; }

		private void _bOK_Click(object sender, EventArgs e)
		{
			ResultCode = _code.Text;
			if(Empty(ResultCode)) this.DialogResult = DialogResult.Cancel;
		}

		private async void _bTest_Click(object sender, EventArgs e)
		{
			_errorProvider.Clear();
			var code = _FormatCode(true); if(code == null) return;
			_wnd.ActivateLL(); Time.SleepDoEvents(200);
			var r = await TUtil.RunTestFindObject(code, _wnd, _bTest, _lSpeed, o => (o as WinImage).RectInScreen);
			//if(r.obj != null) _testedAndFound = true;
		}
		//bool _testedAndFound;

		#endregion

		#region info

		const string c_infoForm =
@"Creates code to <help M_Au_WinImage_Find>find image or color<> in <help M_Au_Wnd_Find>window<>. Your script can click it, etc.
1. Click the Capture button. Mouse-draw rectangle around the image.
2. Click the Test button. It finds and shows the image and the search time.
3. If need, check/uncheck/edit some fields; click Test.
4. Click OK, it inserts C# code in the editor. Or copy/paste.
5. If need, edit the code in the editor: rename variables, delete duplicate Wnd.Find lines, replace part of window name with *, etc.";
		const string c_infoWait = @"Wait timeout, seconds.
If unchecked, does not wait. Else if 0 or empty, waits infinitely. Else waits max this time interval; on timeout returns null or throws exception, depending on the 'Exception...' checkbox.";

		#endregion
	}
}
