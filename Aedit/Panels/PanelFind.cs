using Au;
using Au.Types;
using Au.Controls;
using Au.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Au.Util;
using System.Media;

//CONSIDER: right-click "Find" - search backward. The same for "Replace" (reject "find next"). Rarely used.
//CONSIDER: option to replace and don't find next until next click. Eg Eclipse has buttons "Replace" and "Replace/Find". Or maybe delay to preview.

class PanelFind : UserControl
{
	TextBox _tFind, _tReplace;
	KCheckBox _cFolder, _cName, _cCase, _cWord, _cRegex, _cWildex;
	KPopup _ttRegex, _ttNext;

	public PanelFind() {
		var cstyle = Application.Current.FindResource(ToolBar.CheckBoxStyleKey) as Style;
		var bstyle = Application.Current.FindResource(ToolBar.ButtonStyleKey) as Style;

		var b = new AWpfBuilder(this).Columns(-1).Brush(SystemColors.ControlBrush);
		b.Options(modifyPadding: false, margin: new Thickness(2));
		b.AlsoAll((b, _) => { if (b.Last is Button k) k.Padding = new(1, 0, 1, 1); });
		b.Row((-1, 22..)).Add(out _tFind).Margin(-1, 0, -1, 2).Multiline(wrap: TextWrapping.Wrap).Tooltip("Text to find");
		b.Row((-1, 22..)).Add(out _tReplace).Margin(-1, 0, -1, 2).Multiline(wrap: TextWrapping.Wrap).Tooltip("Replacement text");
		b.R.StartGrid().Columns((-1, ..80), (-1, ..80), (-1, ..80), 0);
		b.R.AddButton("Find", _bFind_Click).Tooltip("Find next match in editor");
		b.AddButton("Replace", _bReplace_Click).Tooltip("Replace single match in editor.\nRight click - find next match.");
		b.AddButton("Repl. all", _bReplaceAll_Click).Tooltip("Replace all matches in editor");

		b.R.AddButton("In files", _bFindIF_Click).Tooltip("Find text in files");
		b.StartStack();
		b.Add(out _cFolder, AResources.GetWpfImageElement("resources/images/folderclosed_16x.xaml")).Padding(1, 0, 1, 1).Tooltip("Let 'In files' search only in current project or root folder");
		_cFolder.Style = cstyle;
		b.AddButton(AResources.GetWpfImageElement("resources/images/settingsgroup_16x.xaml"), _bOptions_Click).Tooltip("More options");
		b.Last.Style = bstyle;
		b.End();

		b.Add(out _cName, "Name").Tooltip("Search in filenames");

		b.R.Add(out _cCase, "Case").Tooltip("Match case")
			.And(0).Add(out _cWildex, "Wildex").Hidden().Tooltip("Wildcard expression.\nExamples: start*.cs, *end.cs, *middle*.cs, **m green.cs||blue.cs.\nF1 - Wildex help.");
		b.Add(out _cWord, "Word").Tooltip("Whole word");
		b.Add(out _cRegex, "Regex").Tooltip("Regular expression.\nF1 - Regex tool and help.");
		b.End().End();

		//this.AccessibleName = this.Name = "Find";
		this.IsVisibleChanged += (_, _) => {
			if (!_cName.IsChecked) Panels.Editor.ZActiveDoc?.InicatorsFind_(IsVisible ? _aEditor : null);
		};

		_tFind.TextChanged += (_, _) => {
			ZUpdateQuickResults(false);
		};

		foreach (var v in new TextBox[] { _tFind, _tReplace }) {
			v.AcceptsTab = true;
			v.IsInactiveSelectionHighlightEnabled = true;
			v.GotKeyboardFocus += _tFindReplace_KeyboardFocus;
			v.LostKeyboardFocus += _tFindReplace_KeyboardFocus;
			v.ContextMenu = new AWpfMenu();
			v.ContextMenuOpening += _tFindReplace_ContextMenuOpening;
			v.PreviewMouseUp += (o, e) => { //use up, else up will close popup. Somehow on up ClickCount always 1.
				if (e.ChangedButton == MouseButton.Middle) {
					var tb = o as TextBox;
					if (tb.Text.NE()) _Recent(tb); else tb.Clear();
				}
			};
		}

		foreach (var v in new KCheckBox[] { _cWildex, _cWord, _cRegex, _cName }) v.CheckChanged += _CheckedChanged;
	}

	#region control events

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

	protected override void OnKeyDown(KeyEventArgs e) {
		base.OnKeyDown(e);
		switch ((e.Key, Keyboard.Modifiers)) {
		case (Key.F1, 0):
			//var os = e.OriginalSource;
			//if (os == _tFind || os == _tReplace || os == _cRegex || os == _cWildex) {
			if (_cRegex.IsChecked) _ShowRegexInfo((e.OriginalSource as TextBox) ?? _tFind, true);
			else if (_cWildex.IsChecked && _cName.IsChecked) AHelp.AuHelp("articles/Wildcard expression");
			//}
			break;
		default: return;
		}
		e.Handled = true;
	}

	private void _tFindReplace_KeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
		if (!_cRegex.IsChecked) return;
		var tb = sender as TextBox;
		if (e.NewFocus == tb) {
			//use timer to avoid temporary focus problems, for example when tabbing quickly or closing active Regex window (this was for forms, now not tested without)
			ATimer.After(70, _ => { if (tb.IsFocused) _ShowRegexInfo(tb, false); });
		} else {
			if (_regexWindow?.IsVisible??false) {
				var c = Keyboard.FocusedElement;
				if (c == null || (c != _tFind && c != _tReplace && !AWnd.ThisThread.IsFocused(_regexWindow.Hwnd))) {
					_regexWindow.Hwnd.ShowL(false);
				}
			}
		}
	}

	private void _CheckedChanged(object sender, RoutedEventArgs e) {
		if (sender == _cWildex) {
			if (_cWildex.IsChecked) {
				_cRegex.IsChecked = false;
				_cWord.IsChecked = false;
			}
		} else if (sender == _cWord) {
			if (_cWord.IsChecked) {
				_cRegex.IsChecked = false;
				_cWildex.IsChecked = false;
			}
		} else if (sender == _cRegex) {
			if (_cRegex.IsChecked) {
				_cWord.IsChecked = false;
				_cWildex.IsChecked = false;
			} else {
				_regexWindow?.Close();
				_regexWindow = null;
			}
		} else if (sender == _cName) {
			Panels.Found.ZControl.zClearText();
			if (_cName.IsChecked) {
				_aEditor.Clear();
				Panels.Editor.ZActiveDoc?.InicatorsFind_(null);
				_cCase.Visibility = Visibility.Hidden;
				_cWildex.Visibility = Visibility.Visible;
			} else {
				_cWildex.Visibility = Visibility.Hidden;
				_cCase.Visibility = Visibility.Visible;
			}
		}
		ZUpdateQuickResults(false);
	}

	RegexWindow _regexWindow;
	string _regexTopic;

	void _ShowRegexInfo(TextBox tb, bool F1) {
		if (F1) {
			_regexWindow ??= new RegexWindow();
			_regexWindow.UserClosed = false;
		} else {
			if (_regexWindow == null || _regexWindow.UserClosed) return;
		}

		if (_regexWindow.Hwnd.Is0) {
			var r = this.RectInScreen();
			r.Offset(0, -20);
			_regexWindow.ShowByRect(App.Wmain, Dock.Right, r, true);
		} else _regexWindow.Hwnd.ShowL(true);

		_regexWindow.InsertInControl = tb;

		bool replace = tb == _tReplace;
		var s = _regexWindow.CurrentTopic;
		if (s == "replace") {
			if (!replace) _regexWindow.CurrentTopic = _regexTopic;
		} else if (replace) {
			_regexTopic = s;
			_regexWindow.CurrentTopic = "replace";
		}
	}

	private void _bFind_Click(WBButtonClickArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f)) return;
		_FindNextInEditor(f, false);
	}

	private void _bFindIF_Click(WBButtonClickArgs e) {
		_cName.IsChecked = false;
		//using var _ = new _TempDisableControl(_bFindIF);
		_FindAllInFiles(false);

		//SHOULDDO: disabled button view now not updated because UI is blocked. When in text, should search in other thread; at least get text.
	}

	private void _bReplace_Click(WBButtonClickArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f, true)) return;
		_FindNextInEditor(f, true);
	}

	private void _bOptions_Click(WBButtonClickArgs e) {
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
		if (onlyEditor && _cName.IsChecked) return;
		//AOutput.Write("UpdateQuickResults", Visible);

		_timerUE ??= new ATimer(_ => {
			if (_cName.IsChecked) {
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
		public AWildex wildex;
		public bool wholeWord;
		public bool matchCase;
	}

	bool _GetTextToFind(out _TextToFind f, bool forReplace = false, bool noRecent = false, bool noTooltip = false, bool names = false) {
		_ttRegex?.Close();
		f = new() { findText = _tFind.Text };
		if (f.findText.Length == 0) return false;
		f.matchCase = !names && _cCase.IsChecked;
		try {
			if (_cRegex.IsChecked) {
				var fl = RXFlags.MULTILINE;
				if (!f.matchCase) fl |= RXFlags.CASELESS;
				f.rx = new ARegex(f.findText, flags: fl);
			} else if (names && _cWildex.IsChecked) {
				f.wildex = new AWildex(f.findText);
			} else {
				f.wholeWord = _cWord.IsChecked;
			}
		}
		catch (ArgumentException e) { //ARegex and AWildex ctors throw if invalid
			if (!noTooltip) TUtil.InfoTooltip(ref _ttRegex, _tFind, e.Message);
			return false;
		}
		if (forReplace) f.replaceText = _tReplace.Text;

		_AddToRecent(f, noRecent);

		if (forReplace && (Panels.Editor.ZActiveDoc?.zIsReadonly ?? true)) return false;
		return true;
	}

	void _FindAllInString(string text, in _TextToFind f, List<Range> a) {
		a.Clear();
		if (f.rx != null) {
			foreach (var g in f.rx.FindAllG(text)) a.Add(g.Start..g.End);
		} else {
			for (int i = 0; i < text.Length; i += f.findText.Length) {
				i = f.wholeWord ? text.FindWord(f.findText, i.., !f.matchCase, "_") : text.Find(f.findText, i, !f.matchCase);
				if (i < 0) break;
				a.Add(i..(i + f.findText.Length));
			}
		}
	}

	//Used to find text in filenames.
	//If found, adds single element to a. In the future a may be used to highlight all matching parts of names in the found list.
	void _FindFirstInString(string text, in _TextToFind f, List<Range> a) {
		a.Clear();
		if (f.rx != null) {
			if (f.rx.MatchG(text, out var g)) a.Add(g.Start..g.End);
		} else if (f.wildex != null) {
			if (f.wildex.Match(text)) a.Add(..);
		} else {
			int i = f.wholeWord ? text.FindWord(f.findText, ignoreCase: true, otherWordChars: "_") : text.Find(f.findText, ignoreCase: true);
			if (i >= 0) a.Add(i..(i + f.findText.Length));
		}
	}

	#endregion

	#region in editor

	void _FindNextInEditor(in _TextToFind f, bool replace) {
		_ttNext?.Close();
		var doc = Panels.Editor.ZActiveDoc; if (doc == null) return;
		var text = doc.zText; if (text.Length == 0) return;
		int i, len = 0, from8 = replace ? doc.zSelectionStart8 : doc.zSelectionEnd8, from = doc.zPos16(from8);
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
			SystemSounds.Asterisk.Play();
			if (retryFromStart || from8 == 0) return;
			from = 0; retryFromStart = true; replace = false;
			goto g1;
		}
		if (retryFromStart) TUtil.InfoTooltip(ref _ttNext, _tFind, "Info: this match is before last position");
		int to = doc.zPos8(i + len);
		i = doc.zPos8(i);
		if (replace && i == from8 && to == doc.zSelectionEnd8) {
			var repl = f.replaceText;
			if (rm != null) repl = rm.ExpandReplacement(repl);
			//doc.zReplaceRange(i, to, repl); //also would need to set caret pos = to
			doc.zReplaceSel(repl);
			_FindNextInEditor(f, false);
		} else {
			doc.zSelect(false, i, to, true);
		}
	}

	private void _bReplace_MouseUp(object sender, MouseEventArgs e) {
		if (e.RightButton == MouseButtonState.Pressed) _bFind_Click(null);
	}

	private void _bReplaceAll_Click(WBButtonClickArgs e) {
		_cName.IsChecked = false;
		if (!_GetTextToFind(out var f, true)) return;
		var doc = Panels.Editor.ZActiveDoc;
		var text = doc.zText;
		var repl = f.replaceText;
		if (f.rx != null) {
			if (!f.rx.FindAll(text, out var ma)) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for (int i = ma.Length - 1; i >= 0; i--) {
				var m = ma[i];
				doc.zReplaceRange(true, m.Start, m.End, m.ExpandReplacement(repl));
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		} else {
			var a = _aEditor;
			_FindAllInString(text, f, a);
			if (a.Count == 0) return;
			doc.Call(Sci.SCI_BEGINUNDOACTION);
			for (int i = a.Count - 1; i >= 0; i--) {
				var v = a[i];
				doc.zReplaceRange(true, v.Start.Value, v.End.Value, repl);
			}
			doc.Call(Sci.SCI_ENDUNDOACTION);
		}
		//Easier/faster would be to create new text and call zSetText. But then all non-text data is lost: markers, folds, caret position...
	}

	List<Range> _aEditor = new(); //all found in editor text

	void _FindAllInEditor() {
		_aEditor.Clear();
		if (!_GetTextToFind(out var f, noRecent: true, noTooltip: true)) return;
		var text = Panels.Editor.ZActiveDoc?.zText; if (text.NE()) return;
		_FindAllInString(text, f, _aEditor);
	}

	/// <summary>
	/// Makes visible and sets find text = s (should be selected text of a control; can be null/"").
	/// </summary>
	public void ZCtrlF(string s/*, bool findInFiles = false*/) {
		Panels.PanelManager[this].Visible = true;
		_tFind.ToolTip = null;
		_tFind.Focus();
		if (s.NE()) return;
		_cName.IsChecked = false;
		_tFind.Text = s;
		//_tFind.SelectAll(); //no, somehow WPF makes selected text gray like disabled when non-focused
		//if (findInFiles) _FindAllInFiles(false); //rejected. Not so useful.
	}

	/// <summary>
	/// Makes visible and sets find text = selected text of e.
	/// Supports KScintilla and TextBox. If other type or null or no selected text, just makes visible etc.
	/// </summary>
	public void ZCtrlF(FrameworkElement e/*, bool findInFiles = false*/) {
		string s = null;
		switch (e) {
		case KScintilla c:
			s = c.zSelectedText();
			break;
		case TextBox c:
			s = c.SelectedText;
			break;
		}
		ZCtrlF(s/*, findInFiles*/);
	}

	//rejected. Could be used for global keyboard shortcuts, but currently they work only if the main window is active.
	///// <summary>
	///// Makes visible and sets find text = selected text of focused control.
	///// </summary>
	//public void ZCtrlF() => ZCtrlF(FocusManager.GetFocusedElement(App.Wmain));

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
		if (!_GetTextToFind(out var f, noRecent: names, names: names)) {
			Panels.Found.ZControl.zClearText();
			return;
		}

		Panels.PanelManager["Found"].Visible = true;

		if (!_init1) {
			_init1 = true;
			var c = Panels.Found.ZControl;
			App.Model.WorkspaceLoadedAndDocumentsOpened += () => Panels.Found.ZControl.zClearText();

			c.ZTags.AddLinkTag("+open", s => {
				_OpenLinkClicked(s);
			});
			c.ZTags.AddLinkTag("+ra", s => {
				if (!_OpenLinkClicked(s)) return;
				ATimer.After(10, _ => _bReplaceAll_Click(null));
				//info: without timer sometimes does not set cursor pos correctly
			});
			c.ZTags.AddLinkTag("+f", s => {
				var a = s.Split(' ');
				if (!_OpenLinkClicked(a[0])) return;
				var doc = Panels.Editor.ZActiveDoc;
				//doc.Focus();
				int from = a[1].ToInt(), to = a[2].ToInt();
				ATimer.After(10, _ => doc.zSelect(true, from, to, true));
				//info: scrolling works better with async when now opened the file
			});
			bool _OpenLinkClicked(string file) {
				var f = App.Model.Find(file, null); //<id>
				if (f == null) return false;
				if (f.IsFolder) f.SelectSingle();
				else if (!App.Model.SetCurrentFile(f)) return false;
				//add indicator to make it easier to find later
				var z = Panels.Found.ZControl;
				z.zIndicatorClear(c_indic);
				var v = z.zLineStartEndFromPos(false, z.zCurrentPos8);
				z.zIndicatorAdd(false, c_indic, v.start..v.end);
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

		var folder = App.Model.Root;
		if (!names && _cFolder.IsChecked && Panels.Editor.ZActiveDoc?.ZFile is FileNode fn) {
			if (fn.FindProject(out var proj, out _, ofAnyScript: true)) folder = proj;
			else folder = fn.AncestorsReverse(noRoot: true).FirstOrDefault() ?? folder;
		}

		foreach (var v in folder.Descendants()) {
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

			if (names) _FindFirstInString(text, f, a);
			else _FindAllInString(text, f, a);

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
				if (time >= (jited ? 100 : 500)) {
					if (bSlow.Length == 0) bSlow.AppendLine("<Z orange>Slow in these files<>");
					bSlow.Append(time).Append(" ms in <open>").Append(v.ItemPath).Append("<> , length ").Append(text.Length).AppendLine();
				}
				jited = true;
			}
		}

		if (folder != App.Model.Root)
			b.Append("<z orange>Note: searched only in folder ").Append(folder.Name).AppendLine(".<>");
		if (searchIn > 0)
			b.Append("<z orange>Note: searched only in ")
			   .Append(searchIn switch { 1 => "C#", 2 => "C# script", 3 => "C# class", _ => "non-C#" })
			   .AppendLine(" files. It is set in Find Options dialog.<>");
		b.Append(bSlow);

		Panels.Found.ZControl.zSetText(b.ToString());
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

	//Not used when Name checked.
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
