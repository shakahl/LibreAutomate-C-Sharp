using Au;
using Au.Types;
using Au.Controls;
using Au.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

public partial class PanelFind : UserControl
{
	//private ErrorProvider _errorProvider;

	public PanelFind() {
		InitializeComponent();
		//this.AccessibleName = this.Name = "Find";
		this.IsVisibleChanged += _IsVisibleChanged;

		_tFind.TextChanged += _tFind_TextChanged;
		var a1 = new TextBox[] { _tFind, _tReplace };
		foreach (var v in a1) {
			v.GotKeyboardFocus += _tFindReplace_KeyboardFocus;
			v.LostKeyboardFocus += _tFindReplace_KeyboardFocus;
			v.ContextMenu = new AWpfMenu();
			v.ContextMenuOpening += _tFindReplace_ContextMenuOpening;
			v.KeyDown += _tFindReplace_KeyDown;
			v.PreviewMouseUp += (o, e) => {
				//use up, else up will close popup. Somehow on up ClickCount always 1.
				if (e.ChangedButton == MouseButton.Middle) { var tb = o as TextBox; if (tb.Text.NE()) _Recent(tb); else tb.Clear(); }
			};
		}

		var a2 = new CheckBox[] { _cCase, _cWord, _cRegex, _cName };
		foreach (var v in a2) {
			v.Checked += _CheckedChanged;
			v.Unchecked += _CheckedChanged;
		}

		_bFind.Click += _bFind_Click;
		_bFindIF.Click += _bFindIF_Click;
		_bReplace.Click += _bReplace_Click;
		_bOptions.Click += _bOptions_Click;
	}

	#region control events

	private void _tFind_TextChanged(object sender, TextChangedEventArgs e) {
		ZUpdateQuickResults(false);
	}

	private void _tFindReplace_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
		var c = sender as TextBox;
		var m = c.ContextMenu as AWpfMenu;
		m.Items.Clear();
		m["_Undo\0" + "Ctrl+Z", c.CanUndo] = o => c.Undo();
		m["_Redo\0" + "Ctrl+Y", c.CanRedo] = o => c.Redo();
		m["Cu_t\0" + "Ctrl+X", c.SelectionLength > 0] = o => c.Cut();
		m["_Copy\0" + "Ctrl+C", c.SelectionLength > 0] = o => c.Copy();
		m["_Paste\0" + "Ctrl+V", Clipboard.ContainsText()] = o => c.Paste();
		m["_Select All\0" + "Ctrl+A"] = o => c.SelectAll();
		m["Cl_ear\0" + "M-click"] = o => c.Clear();
		m["Rece_nt\0" + "M-click"] = o => _Recent(c);
	}

	private void _tFindReplace_KeyDown(object sender, KeyEventArgs e) {
		//AOutput.Write(e.Key, Keyboard.Modifiers);
		switch ((e.Key, Keyboard.Modifiers)) {
		case (Key.F1, 0):
			_ShowRegexInfo(sender as TextBox, true);
			break;
		default: return;
		}
		e.Handled = true;
	}

	private void _tFindReplace_KeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
		if (!_cRegex.IsCheck()) return;
		var tb = sender as TextBox;
		if (e.NewFocus == tb) {
			//use timer to avoid temporary focus problems, for example when tabbing quickly or closing active Regex window (this was for forms, now not tested without)
			ATimer.After(70, _ => { if (tb.IsFocused) _ShowRegexInfo(tb, false); });
		} else {
			if (_regexWindow.Window.Visible) {
				var c = Keyboard.FocusedElement;
				if (c == null || (c != _tFind && c != _tReplace /*&& c != _regexWindow.Window && c.TopLevelControl != _regexWindow.Window*/)) {//TODO
					_regexWindow.Hide();
				}
			}
		}
	}

	private void _CheckedChanged(object sender, RoutedEventArgs e) {
		if (sender == _cWord) {
			if (_cWord.IsCheck()) _cRegex.IsChecked = false;
		} else if (sender == _cRegex) {
			if (_cRegex.IsCheck()) {
				_cWord.IsChecked = false;
				if (_regexWindow == null) {
					_regexWindow = new RegexWindow(this.Hwnd());
				}
			} else {
				_regexWindow?.Dispose();
				_regexWindow = null;
			}
		} else if (sender == _cName) {
			Panels.Found.ZControl.Z.ClearText();
			if (_cName.IsCheck()) {
				_aEditor.Clear();
				Panels.Editor.ZActiveDoc?.InicatorsFind_(null);
			}
		}
		ZUpdateQuickResults(false);
	}

	RegexWindow _regexWindow;
	string _regexTopic;

	void _ShowRegexInfo(TextBox tb, bool F1) {
		//if (_regexWindow == null || !_cRegex.IsCheck()) return;
		//if (_regexWindow.UserClosed) { if (!F1) return; _regexWindow.UserClosed = false; }

		//if (!_regexWindow.Window.IsHandleCreated) {
		//	var r = this.Hwnd().Rect;
		//	r.Offset(0, -20);
		//	_regexWindow.Show(App.Wmain, r, true);
		//} else _regexWindow.Window.Show();

		//_regexWindow.InsertInControl = tb;

		//bool replace = tb == _tReplace;
		//var s = _regexWindow.CurrentTopic;
		//if (s == "replace") {
		//	if (!replace) _regexWindow.CurrentTopic = _regexTopic;
		//} else if (replace) {
		//	_regexTopic = s;
		//	_regexWindow.CurrentTopic = "replace";
		//}
	}

	private void _bFind_Click(object sender, RoutedEventArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f, false)) return;
		_FindNextInEditor(f, false);
	}

	private void _bFindIF_Click(object sender, RoutedEventArgs e) {
		_cName.IsChecked = false;
		//using var _ = new _TempDisableControl(_bFindIF);
		_FindAllInFiles(false);

		//SHOULDDO: disabled button view now not updated because UI is blocked. When in text, should search in other thread; at least get text.
	}

	private void _bReplace_Click(object sender, RoutedEventArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f, true)) return;
		_FindNextInEditor(f, true);
	}

	private void _bOptions_Click(object sender, RoutedEventArgs e) {
		var b = new AWpfBuilder("Find Options").WinSize(350)
			.R.StartGrid<GroupBox>("Find in files")
			.R.Add("Search in", out ComboBox cbFileType, true).Items("All files|C# files (*.cs)|C# script files|C# class files|Other files").Select(_SearchIn)
			.R.Add<Label>("Skip files where path matches wildcard")
			.R.Add(out TextBox tSkip, string.Join("\r\n", _SkipWildcards)).Multiline(100, TextWrapping.NoWrap)
			.End()
			.R.AddOkCancel()
			.End();
		if (!b.ShowDialog()) return;
		App.Settings.find_searchIn = _searchIn = cbFileType.SelectedIndex;
		App.Settings.find_skip = tSkip.Text; _aSkipWildcards = null;
	}

	#endregion

	#region common

	/// <summary>
	/// Called when changed find text or options. Also when activated another document.
	/// Async-updates find-hiliting in editor or 'find name' results.
	/// </summary>
	public void ZUpdateQuickResults(bool onlyEditor) {
		if (!IsVisible) return;
		if (onlyEditor && _cName.IsCheck()) return;
		//AOutput.Write("UpdateQuickResults", Visible);

		_timerUE ??= new ATimer(_ => {
			if (_cName.IsCheck()) {
				_FindAllInFiles(true);
			} else {
				_FindAllInEditor();
				Panels.Editor.ZActiveDoc?.InicatorsFind_(_aEditor);
			}
		});

		_timerUE.After(150);
	}
	ATimer _timerUE;

	struct _TextToFind
	{
		public string findText;
		public string replaceText;
		public ARegex rx;
		public bool wholeWord;
		public bool matchCase;
	}

	bool _GetTextToFind(out _TextToFind f, bool forReplace, bool noRecent = false) {
		//_errorProvider.Clear();
		f = default;
		f.findText = _tFind.Text;
		if (f.findText.Length == 0) return false;
		f.matchCase = _cCase.IsCheck();
		if (_cRegex.IsCheck()) {
			try {
				var fl = RXFlags.MULTILINE;
				if (!f.matchCase) fl |= RXFlags.CASELESS;
				f.rx = new ARegex(f.findText, flags: fl);
			}
			catch (ArgumentException e) {
				_SetErrorProvider(_tFind, e.Message);
				return false;
			}
		} else f.wholeWord = _cWord.IsCheck();
		if (forReplace) f.replaceText = _tReplace.Text;

		_AddToRecent(f, noRecent);

		if (forReplace && (Panels.Editor.ZActiveDoc?.Z.IsReadonly ?? true)) return false;
		return true;
	}

	void _FindAllInString(string text, in _TextToFind f, List<Range> a, bool one = false) {
		a.Clear();
		if (f.rx != null) {
			foreach (var g in f.rx.FindAllG(text)) {
				a.Add(g.Start..g.End);
				if (one) break;
			}
		} else {
			for (int i = 0; i < text.Length; i += f.findText.Length) {
				i = f.wholeWord ? text.FindWord(f.findText, i.., !f.matchCase, "_") : text.Find(f.findText, i, !f.matchCase);
				if (i < 0) break;
				a.Add(i..(i + f.findText.Length));
				if (one) break;
			}
		}
	}

	void _SetErrorProvider(Control c, string text) {
		//_errorProvider.SetIconAlignment(c, ErrorIconAlignment.BottomRight);
		//_errorProvider.SetIconPadding(c, -16);
		//_errorProvider.SetError(c, text);
	}

	#endregion

	#region in editor

	void _FindNextInEditor(in _TextToFind f, bool replace) {
		var doc = Panels.Editor.ZActiveDoc; if (doc == null) return;
		var z = doc.Z;
		var text = doc.Text; if (text.Length == 0) return;
		int i, len = 0, from8 = replace ? z.SelectionStart8 : z.SelectionEnd8, from = doc.Pos16(from8);
		RXMatch rm = null;
		bool retryFromStart = false, retryRx = false;
		g1:
		if (f.rx != null) {
			if (f.rx.Match(text, out rm, from..)) {
				i = rm.Start;
				len = rm.Length;
				if (i == from && len == 0 && !(replace | retryRx | retryFromStart)) {
					if (++i > text.Length) i = -1;
					else {
						if (i < text.Length) if (text.Eq(i - 1, "\r\n") || char.IsSurrogatePair(text, i - 1)) i++;
						from = i; retryRx = true; goto g1;
					}
				}
				if (len == 0) doc.Focus();
			} else i = -1;
		} else {
			i = f.wholeWord ? text.FindWord(f.findText, from.., !f.matchCase, "_") : text.Find(f.findText, from, !f.matchCase);
			len = f.findText.Length;
		}
		//AOutput.Write(from, i, len);
		if (i < 0) {
			if (retryFromStart || from8 == 0) return;
			from = 0; retryFromStart = true; replace = false; goto g1;
		}
		int to = doc.Pos8(i + len);
		i = doc.Pos8(i);
		if (replace && i == from8 && to == z.SelectionEnd8) {
			var repl = f.replaceText;
			if (rm != null) repl = rm.ExpandReplacement(repl);
			//z.ReplaceRange(i, to, repl); //also would need to set caret pos = to
			z.ReplaceSel(repl);
			_FindNextInEditor(f, false);
		} else {
			z.Select(false, i, to, true);
		}
	}

	private void _bReplace_MouseUp(object sender, MouseEventArgs e) {
		if (e.RightButton == MouseButtonState.Pressed) _bFind_Click(sender, e);
	}

	private void _bReplaceAll_Click(object sender, EventArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f, true)) return;
		var doc = Panels.Editor.ZActiveDoc;
		var text = doc.Text;
		var repl = f.replaceText;
		if (f.rx != null) {
			if (!f.rx.FindAll(text, out var ma)) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for (int i = ma.Length - 1; i >= 0; i--) {
				var m = ma[i];
				doc.Z.ReplaceRange(true, m.Start, m.End, m.ExpandReplacement(repl));
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		} else {
			var a = _aEditor;
			_FindAllInString(text, f, a);
			if (a.Count == 0) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for (int i = a.Count - 1; i >= 0; i--) {
				var v = a[i];
				doc.Z.ReplaceRange(true, v.Start.Value, v.End.Value, repl);
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		}
		//Easier/faster would be to create new text and call Z.SetText. But then all non-text data is lost: markers, folds, caret position...
	}

	List<Range> _aEditor = new(); //all found in editor text

	void _FindAllInEditor() {
		_aEditor.Clear();
		if (!_GetTextToFind(out var f, false, noRecent: true)) return;
		var text = Panels.Editor.ZActiveDoc?.Text; if (text.NE()) return;
		_FindAllInString(text, f, _aEditor);
	}

	void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
		if (!_cName.IsCheck()) Panels.Editor.ZActiveDoc?.InicatorsFind_(IsVisible ? _aEditor : null);
	}

	/// <summary>
	/// Makes visible and sets find text = s (should be selected text of a control; can be null/"").
	/// </summary>
	public void ZCtrlF(string s) {
		Panels.PanelManager[this].Visible = true;
		_tFind.Focus();
		if (s.NE()) return;
		_tFind.Text = s;
		_tFind.SelectAll();
	}

	/// <summary>
	/// Makes visible and sets find text = selected text of focused control.
	/// </summary>
	public void ZCtrlF() { if (Keyboard.FocusedElement is TextBox t) ZCtrlF(t.SelectedText); }

	#endregion

	#region in files

	int _SearchIn => _searchIn >= 0 ? _searchIn : (_searchIn = App.Settings.find_searchIn);
	int _searchIn = -1;

	string[] _SkipWildcards => _aSkipWildcards ??= (App.Settings.find_skip ?? "").Lines(true);
	string[] _aSkipWildcards;
	readonly string[] _aSkipImages = new string[] { ".png", ".bmp", ".jpg", ".jpeg", ".gif", ".tif", ".tiff", ".ico", ".cur", ".ani" };
	bool _init1;
	const int c_indic = 0;

	void _FindAllInFiles(bool names/*, bool forReplace*/) {
		if (!_GetTextToFind(out var f, false, noRecent: names)) {
			Panels.Found.ZControl.Z.ClearText();
			return;
		}

		Panels.PanelManager["Found"].Visible = true;

		if (!_init1) {
			_init1 = true;
			var c = Panels.Found.ZControl;
			App.Model.WorkspaceLoadedAndDocumentsOpened += () => Panels.Found.ZControl.Z.ClearText();

			c.ZTags.AddLinkTag("+open", s => {
				_OpenLinkClicked(s);
			});
			c.ZTags.AddLinkTag("+ra", s => {
				if (!_OpenLinkClicked(s)) return;
				ATimer.After(10, _ => _bReplaceAll_Click(null, null));
				//info: without timer sometimes does not set cursor pos correctly
			});
			c.ZTags.AddLinkTag("+f", s => {
				var a = s.Split(' ');
				if (!_OpenLinkClicked(a[0])) return;
				var doc = Panels.Editor.ZActiveDoc;
				//doc.Focus();
				int from = a[1].ToInt(), to = a[2].ToInt();
				ATimer.After(10, _ => doc.Z.Select(true, from, to, true));
				//info: scrolling works better with async when now opened the file
			});
			bool _OpenLinkClicked(string file) {
				var f = App.Model.Find(file, null); //<id>
				if (f == null) return false;
				if (f.IsFolder) f.SelectSingle();
				else if (!App.Model.SetCurrentFile(f)) return false;
				//add indicator to make it easier to find later
				var z = Panels.Found.ZControl.Z;
				z.IndicatorClear(c_indic);
				var v = z.LineStartEndFromPos(false, z.CurrentPos8);
				z.IndicatorAdd(false, c_indic, v.start..v.end);
				return true;
			}
			c.Call(Sci.SCI_INDICSETSTYLE, c_indic, Sci.INDIC_BOX);
			c.Call(Sci.SCI_INDICSETFORE, c_indic, 0x0080e0);
		}

		var b = new StringBuilder();
		var a = new List<Range>();
		var bSlow = !names && f.rx != null ? new StringBuilder() : null;
		bool jited = false;
		int searchIn = names ? 0 : _SearchIn;

		foreach (var v in App.Model.Root.Descendants()) {
			//using var perf = new _Perf();
			string text = null, path = null;
			if (names) {
				text = v.Name;
			} else {
				//APerf.First();
				if (v.IsCodeFile) {
					switch (searchIn) { //0 all, 1 C#, 2 script, 3 class, 4 other
					case 4: continue;
					case 2 when !v.IsScript: continue;
					case 3 when !v.IsClass: continue;
					}
				} else {
					if (searchIn >= 1 && searchIn <= 3) continue;
					if (v.IsFolder) continue;
					if (0 != v.Name.Ends(true, _aSkipImages)) continue;
				}
				var sw = _SkipWildcards; if (sw.Length != 0 && 0 != (path = v.ItemPath).Like(true, sw)) continue;
				//perf.Start(v.Name);
				text = v.GetText();
				if (text.Length == 0) continue;
				if (text.Contains('\0')) continue;
				//APerf.NW();
			}

			long time = bSlow != null ? ATime.PerfMilliseconds : 0;

			_FindAllInString(text, f, a, one: names);

			if (a.Count != 0) {
				if (!names) b.Append("<Z 0xC0E0C0>");
				path ??= v.ItemPath;
				string link = v.IdStringWithWorkspace;
				if (v.IsFolder) {
					b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<><>    <c 0x008000>//folder<>", link, path);
				} else {
					int i1 = path.LastIndexOf('\\') + 1;
					string s1 = path.Remove(i1), s2 = path.Substring(i1);
					if (names) {
						b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<>{2}<>", link, s1, s2);
					} else {
						int ns = 120 - path.Length * 7 / 4;
#if true //open and select the first found text
						b.AppendFormat("<+f \"{0} {1} {2}\"><c 0x808080>{3}<><b>{4}{5}      <><>    <+ra \"{0}\"><c 0x80ff>Replace all<><>",
							link, a[0].Start.Value, a[0].End.Value, s1, s2, ns > 0 ? new string(' ', ns) : null);
#else //just open
						b.AppendFormat("<+open \"{0}\"><c 0x808080>{1}<><b>{2}{3}      <><>    <+ra \"{0}\"><c 0x80ff>Replace all<><>",
							link, s1, s2, ns > 0 ? new string(' ', ns) : null);
#endif
					}
				}
				if (!names) b.Append("<>");
				b.AppendLine();
				if (names) {
					//FUTURE: icon; maybe hilite.
				} else {
					if (b.Length < 10_000_000) {
						for (int i = 0; i < a.Count; i++) {
							var range = a[i];
							int start = range.Start.Value, end = range.End.Value, lineStart = start, lineEnd = end;
							int lsMax = Math.Max(lineStart - 100, 0), leMax = Math.Min(lineEnd + 200, text.Length); //start/end limits like in VS
							for (; lineStart > lsMax; lineStart--) { char c = text[lineStart - 1]; if (c == '\n' || c == '\r') break; }
							bool limitStart = lineStart == lsMax && lineStart > 0;
							for (; lineEnd < leMax; lineEnd++) { char c = text[lineEnd]; if (c == '\r' || c == '\n') break; }
							bool limitEnd = lineEnd == leMax && lineEnd < text.Length;
							b.AppendFormat("<+f \"{0} {1} {2}\">", link, start.ToString(), end.ToString())
								.Append(limitStart ? "…<\a>" : "<\a>").Append(text, lineStart, start - lineStart).Append("</\a>")
								.Append("<z 0xffff5f><\a>").Append(text, start, end - start).Append("</\a><>")
								.Append("<\a>").Append(text, end, lineEnd - end).Append(limitEnd ? "</\a>…" : "</\a>")
								.AppendLine("<>");
						}
					}
				}
			}

			if (bSlow != null) {
				time = ATime.PerfMilliseconds - time;
				if (!jited) jited = true;
				else if (time >= 100) {
					if (bSlow.Length == 0) bSlow.AppendLine("<Z orange>Slow in these files<>");
					bSlow.Append(time).Append(" ms in <open>").Append(v.ItemPath).Append("<> , length ").Append(text.Length).AppendLine();
				}
			}
		}

		if (searchIn > 0) b.Append("<Z orange>Note: searched only in ")
			 .Append(searchIn switch { 1 => "C#", 2 => "C# script", 3 => "C# class", _ => "non-C#" })
			 .AppendLine(" files. It is set in Find options (the ... button).<>");
		b.Append(bSlow);

		Panels.Found.ZControl.Z.SetText(b.ToString());
	}

	//struct _Perf : IDisposable
	//{
	//	long _t;
	//	string _file;

	//	public void Start(string file) {
	//		_t = ATime.PerfMicroseconds;
	//		_file = file;
	//	}

	//	public void Dispose() {
	//		if (_t == 0) return;
	//		long t=ATime.PerfMicroseconds-_t;
	//		if (t < 20000) return;
	//		AOutput.Write(t, _file);
	//	}
	//}

	//rejected: replace in files.
	//	Rarely used, dangerous, need much work to make more useful. It's easy to click each file in results and click 'Replace' or 'all'.
	//	FUTURE: in found results, at bottom add link "Replace in all files". But also need checkboxes.
	//private void _bReplaceIF_Click(object sender, EventArgs e)
	//{
	//}

	//struct _TempDisableControl : IDisposable
	//{
	//	UIElement _e;
	//	int _enableAfter;

	//	public _TempDisableControl(UIElement e, int enableAfter = 0) {
	//		_e = e;
	//		_enableAfter = enableAfter;
	//		e.IsEnabled = false;
	//	}

	//	public void Dispose() {
	//		if (_enableAfter == 0) _e.IsEnabled = true;
	//		else { var e = _e; ATimer.After(_enableAfter, _ => e.IsEnabled = true); }
	//	}
	//}

	#endregion

	#region recent

	string _recentPrevFind, _recentPrevReplace;
	int _recentPrevOptions;

	//temp is false when clicked a button, true when changed the find text or a checkbox.
	void _AddToRecent(in _TextToFind f, bool temp) {
		if (temp) return; //not implemented. Was implemented, but was not perfect and probably not useful. Adds too many intermediate garbage, although filtered.

		int k = f.matchCase ? 1 : 0; if (f.wholeWord) k |= 2; else if (f.rx != null) k |= 4;

		if (f.findText != _recentPrevFind || k != _recentPrevOptions) _Add(false, _recentPrevFind = f.findText, _recentPrevOptions = k);
		if (!f.replaceText.NE() && f.replaceText != _recentPrevReplace) _Add(true, _recentPrevReplace = f.replaceText, 0);

		static void _Add(bool replace, string text, int options) {
			if (text.Length > 1000) {
				//if(0 != (options & 4)) AWarning.Write("The find text of length > 1000 will not be saved to 'recent'.", -1);
				return;
			}
			var a = (replace ? App.Settings.find_recentReplace : App.Settings.find_recent) ?? new FRRecentItem[0];
			for (int i = a.Length; --i >= 0;) if (a[i].t == text) a = a.RemoveAt(i); //avoid duplicates
			if (a.Length >= 20) a = a[0..19]; //limit count
			a = a.InsertAt(0, new FRRecentItem { t = text, o = options });
			if (replace) App.Settings.find_recentReplace = a; else App.Settings.find_recent = a;
		}
	}

	void _Recent(TextBox tb) {
		bool replace = tb == _tReplace;
		var a = replace ? App.Settings.find_recentReplace : App.Settings.find_recent;
		if (a == null) return;
		var p = new KPopupListBox { PlacementTarget = tb };
		var k = p.Control;
		foreach (var v in a) k.Items.Add(v);
		p.OK += o => {
			var r = o as FRRecentItem;
			tb.Text = r.t;
			if (!replace) {
				int k = r.o;
				_cCase.IsChecked = 0 != (k & 1);
				_cWord.IsChecked = 0 != (k & 2);
				_cRegex.IsChecked = 0 != (k & 4);
			}
		};
		Dispatcher.InvokeAsync(() => p.IsOpen = true);
	}

	#endregion
}

class FRRecentItem //not nested in PanelFind because used with ASettings (would load UI dlls).
{
	[System.Text.Json.Serialization.JsonInclude]
	public string t;
	[System.Text.Json.Serialization.JsonInclude]
	public int o;

	public override string ToString() => t.Limit(200); //ListBox item display text
}
