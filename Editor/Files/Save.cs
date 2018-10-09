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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using LiteDB;

partial class FilesModel
{
	public class AutoSave
	{
		FilesModel _model;
		int _collAfterS, _stateAfterS, _textAfterS;
		internal bool LoadingState;

		public AutoSave(FilesModel model)
		{
			_model = model;
			Timer1s += _Program_Timer1s;
			MainForm.VisibleChanged += _MainForm_VisibleChanged;
		}

		public void Dispose()
		{
			_model = null;
			Timer1s -= _Program_Timer1s;
			MainForm.VisibleChanged -= _MainForm_VisibleChanged;

			//must be all saved or unchanged
			Debug.Assert(_collAfterS == 0);
			Debug.Assert(_stateAfterS == 0);
			Debug.Assert(_textAfterS == 0);
		}

		/// <summary>
		/// Sets timer to save files.xml later, if not already set.
		/// </summary>
		/// <param name="afterS">Timer time, seconds.</param>
		public void CollectionLater(int afterS = 5)
		{
			if(_collAfterS < 1 || _collAfterS > afterS) _collAfterS = afterS;
		}

		/// <summary>
		/// Sets timer to save state.xml later, if not already set.
		/// </summary>
		/// <param name="afterS">Timer time, seconds.</param>
		public void StateLater(int afterS = 30)
		{
			if(LoadingState) return;
			if(_stateAfterS < 1 || _stateAfterS > afterS) _stateAfterS = afterS;
		}

		/// <summary>
		/// Sets timer to save editor text later, if not already set.
		/// </summary>
		/// <param name="afterS">Timer time, seconds.</param>
		public void TextLater(int afterS = 60)
		{
			if(_textAfterS < 1 || _textAfterS > afterS) _textAfterS = afterS;
		}

		/// <summary>
		/// If files.xml is set to save (CollectionLater), saves it now.
		/// </summary>
		public void CollectionNowIfNeed()
		{
			if(_collAfterS > 0) _SaveCollectionNow();
		}

		/// <summary>
		/// If state.xml is set to save (StateLater), saves it now.
		/// </summary>
		public void StateNowIfNeed()
		{
			if(_stateAfterS > 0) _SaveStateNow();
		}

		/// <summary>
		/// If editor text is set to save (TextLater), saves it now.
		/// Also saves markers, folding, etc, unless onlyText is true.
		/// </summary>
		public void TextNowIfNeed(bool onlyText = false)
		{
			if(_textAfterS > 0) _SaveTextNow();
			if(onlyText) return;
			Panels.Editor.SaveEditorData();
		}

		void _SaveCollectionNow()
		{
			_collAfterS = 0;
			Debug.Assert(_model != null); if(_model == null) return;
			if(!_model._SaveCollectionNow()) _collAfterS = 60; //if fails, retry later
		}

		void _SaveStateNow()
		{
			_stateAfterS = 0;
			Debug.Assert(_model != null); if(_model == null) return;
			if(!_model._SaveStateNow()) _stateAfterS = 300; //if fails, retry later
		}

		void _SaveTextNow()
		{
			_textAfterS = 0;
			Debug.Assert(_model != null); if(_model == null) return;
			Debug.Assert(Panels.Editor.IsOpen);
			if(!Panels.Editor.SaveText()) _textAfterS = 300; //if fails, retry later
		}

		/// <summary>
		/// Calls CollectionNowIfNeed, StateNowIfNeed, TextNowIfNeed.
		/// </summary>
		public void AllNowIfNeed()
		{
			CollectionNowIfNeed();
			StateNowIfNeed();
			TextNowIfNeed();
		}

		void _Program_Timer1s()
		{
			if(_collAfterS > 0 && --_collAfterS == 0) _SaveCollectionNow();
			if(_stateAfterS > 0 && --_stateAfterS == 0) _SaveStateNow();
			if(_textAfterS > 0 && --_textAfterS == 0) _SaveTextNow();
		}

		void _MainForm_VisibleChanged(object sender, EventArgs e)
		{
			if(!MainForm.Visible) AllNowIfNeed();
		}
	}

	/// <summary>
	/// Used only by the Save class.
	/// </summary>
	bool _SaveCollectionNow()
	{
		try {
			//Print("saving");
			var perf = Perf.StartNew();
			Root.Save(CollectionFile);
			perf.NW('S'); //TODO
			return true;
		}
		catch(Exception ex) { //XElement.Save exceptions are undocumented
			AuDialog.ShowError("Failed to save", CollectionFile, expandedText: ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Used only by the Save class.
	/// </summary>
	bool _SaveStateNow()
	{
		if(TableMisc == null) return true;
		try {
			TableMisc.Upsert(new DBMisc("expanded", string.Join(" ", _control.AllNodes.Where(n => n.IsExpanded).Select(n => (n.Tag as FileNode).IdString))));

			using(new Au.Util.LibStringBuilder(out var b)) {
				var a = OpenFiles;
				b.Append(a.IndexOf(_currentFile));
				foreach(var v in a) b.Append(' ').Append(v.IdString);
				TableMisc.Upsert(new DBMisc("open", b.ToString()));
			}
			return true;
		}
		catch(Exception ex) {
			Debug_.Print(ex);
			return false;
		}
	}

	/// <summary>
	/// Called at the end of opening this collection.
	/// </summary>
	public void LoadState()
	{
		//Call LoadState when form loaded, ie when control handles created but form still invisible. Because:
		//	1. _control does not update scrollbars if folders expanded before creating handle.
		//	2. SciControl handle must be created because _SetCurrentFile sets its text etc.
		Debug.Assert(MainForm.IsHandleCreated);

		if(TableMisc == null) return;
		try {
			Save.LoadingState = true;

			//expanded folders
			var s = TableMisc.FindById("expanded")?.s;
			if(!Empty(s)) {
				_control.BeginUpdate();
				foreach(var seg in s.Segments_(" ")) {
					var fn = FindById(seg.Value);
					fn?.TreeNodeAdv.Expand();
				}
				_control.EndUpdate();
			}

			//open files
			s = TableMisc.FindById("open")?.s;
			if(!Empty(s)) {
				//format: indexOfActiveDocOrMinusOne id1 id2 ...
				int i = -2, iActive = s.ToInt_();
				FileNode fnActive = null;
				//Perf.First();
				foreach(var seg in s.Segments_(" ")) {
					i++; if(i < 0) continue;
					var fn = FindById(seg.Value); if(fn == null) continue;
					OpenFiles.Add(fn);
					if(i == iActive) fnActive = fn;
				}
				//Perf.Next();
				if(fnActive == null || !_SetCurrentFile(fnActive)) Panels.Open.UpdateList();
				//Perf.NW();
			}
		}
		catch(Exception ex) { Debug_.Print(ex.Message); }
		finally { Save.LoadingState = false; }
	}
}

/// <summary>
/// Type of LiteDB database editor.db table 'misc' items.
/// In that table we save expanded folders, open documents, etc.
/// </summary>
class DBMisc
{
	[BsonId]
	public string name { get; set; }
	public string s { get; set; }

	public DBMisc() { } //need for LiteDB
	public DBMisc(string name, string s) { this.name = name; this.s = s; }
}

class DBEdit
{
	//info:
	//	LiteDB uses only properties, not fields.
	//	The unique id property must be named Id or id or _id or have [BsonId].

	public int id { get; set; }
	public List<int> folding { get; set; }
	public List<int> bookmarks { get; set; }
	public List<int> breakpoints { get; set; }

#if DEBUG
	public override string ToString()
	{
		var s1 = folding == null ? "null" : string.Join(" ", folding);
		var s2 = bookmarks == null ? "null" : string.Join(" ", bookmarks);
		var s3 = breakpoints == null ? "null" : string.Join(" ", breakpoints);
		return $"folding={s1},  bookmarks={s2},  breakpoints={s3}";
	}
#endif
}
