using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Xml.XPath;

using Catkeys;
using Catkeys.Types;
using static Catkeys.NoClass;
using static Program;

partial class FilesModel
{
	public class AutoSave
	{
		FilesModel _model;
		int _collAfterS, _stateAfterS, _curFileAfterS;
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
			Debug.Assert(_curFileAfterS == 0);
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
			if(_curFileAfterS < 1 || _curFileAfterS > afterS) _curFileAfterS = afterS;
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
		/// </summary>
		public void TextNowIfNeed()
		{
			if(_curFileAfterS > 0) _SaveTextNow();
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
			_curFileAfterS = 0;
			Debug.Assert(_model != null); if(_model == null) return;
			Debug.Assert(Panels.Editor.IsOpen);
			if(!Panels.Editor.Save()) _curFileAfterS = 300; //if fails, retry later
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
			if(_curFileAfterS > 0 && --_curFileAfterS == 0) _SaveTextNow();
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
			//Perf.First();
			Xml.Save(CollectionFile);
			//Perf.NW();
			return true;
		}
		catch(Exception ex) { //XElement.Save exceptions are undocumented
			TaskDialog.ShowError("Failed to save", CollectionFile, expandedText: ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Used only by the Save class.
	/// </summary>
	bool _SaveStateNow()
	{
		try {
			//Perf.First();
			var xr = new XElement("state");
			xr.Add(new XElement("expanded", string.Join(" ", TV.AllNodes.Where(n => n.IsExpanded).Select(n => (n.Tag as FileNode).GUID))));
			xr.Add(new XElement("open", string.Join(" ", OpenFiles.Select(f => f.GUID))));
			if(_currentFile != null) xr.Add(new XElement("current", _currentFile.GUID));
			//var sb = new StringBuilder();
			//XElement x = new XElement("windows");
			//Print(xr);
			xr.Save(this.StateFile);
			//Perf.NW();
			return true;
		}
		catch(Exception ex) { //XElement.Save exceptions are undocumented
			TaskDialog.ShowError("Failed to save file states", StateFile, expandedText: ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Called at the end of opening this collection.
	/// </summary>
	public void LoadState()
	{
		//Call LoadState when form loaded, ie when control handles created but form still invisible. Because:
		//	1. TV does not update scrollbars if folders expanded before creating handle.
		//	2. SciControl handle must be created because _SetCurrentFile sets its text etc.
		Debug.Assert(MainForm.IsHandleCreated);

		if(!Files.ExistsAsFile(StateFile)) return;
		try {
			Save.LoadingState = true;
			var xr = XElement.Load(StateFile);
			//expanded folders
			var s = xr.Element("expanded")?.Value;
			if(!Empty(s)) {
				//Perf.First();
				TV.BeginUpdate();
				foreach(var guid in s.Segments_(" ")) {
					var fn = this.FindByGUID(guid.Value);
					if(fn == null) continue; //unexpected, but it's ok, we'll rebuild XML eventually
					fn.TreeNodeAdv.Expand();
				}
				TV.EndUpdate();
				//Perf.NW();
			}
			//open files
			s = xr.Element("open")?.Value;
			if(!Empty(s)) {
				//Perf.First();
				foreach(var guid in s.Segments_(" ")) {
					var fn = this.FindByGUID(guid.Value);
					if(fn == null) continue;
					OpenFiles.Add(fn);
				}
				Panels.Open.UpdateList();
				//Perf.NW();
			}
			s = xr.Element("current")?.Value;
			if(!Empty(s)) {
				var fn = this.FindByGUID(s);
				if(fn != null) _SetCurrentFile(fn);
			}
		}
		catch(Exception ex) { Debug_.Print(ex.Message); }
		finally { Save.LoadingState = false; }
	}

}
