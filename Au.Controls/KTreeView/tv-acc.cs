//Implements MSAA IAccessible.

//Problem: UIA "object from point" returns CLIENT, not item, although has items in tree. MSAA works well.
//	For UIA to work correctly, need to implement UIA (AutomationPeer for control and items).
//		But then problem: if not implemented MSAA, MSAA "object from point" does not return item, although has items in tree.
//			If implemented both, tree contains duplicate subtrees; example - SysListView32 control.
//		Also, cannot implement it directly for HwndHost, because WPF then throws exception. But can implement for its parent.
//	I did not find a way to make both UIA and MSAA work correctly with WPF HwndHost.
//	Good: NVDA now works anyway.

//Strange: QM2 does not find items and client unless checked +invisible, although no invisible state. Never mind.

using Au.Types;
using System;
using System.Collections.Generic;
using AccNAVDIR = Au.Types.Api.AccNAVDIR;

namespace Au.Controls
{
	public partial class KTreeView
	{
		class _Accessible : HwndHostAccessibleBase_
		{
			KTreeView _tv;

			internal _Accessible(KTreeView tv) : base(tv, tv.Hwnd) {
				_tv = tv;
			}

			ITreeViewItem _Item(int child) {
				if (child == -1) return null;
				if ((uint)child < _tv.CountVisible) return _tv._avi[child].item;
				throw new ArgumentException();
			}

			public override int ChildCount => _tv.CountVisible;

			public override string Name(int child) {
				var k = _Item(child);
				if (k == null) return _tv.Name;
				return k.DisplayText;
			}

			public override AccROLE Role(int child) {
				return child == -1 ? AccROLE.TREE : AccROLE.TREEITEM;
			}

			public override AccSTATE State(int child) {
				if (child == -1) return base.State(child);
				var k = _Item(child);
				AccSTATE r = 0;
				if (_tv.Focusable) {
					r |= AccSTATE.FOCUSABLE;
					if (child == _tv._focusedIndex && _tv.IsKeyboardFocused) r |= AccSTATE.FOCUSED;
				}
				if (k.IsSelectable) {
					r |= AccSTATE.SELECTABLE;
					if (_tv.MultiSelect) r |= AccSTATE.MULTISELECTABLE;
					if (_tv.IsSelected(child)) r |= AccSTATE.SELECTED;
				}
				if (k.IsFolder) r |= k.IsExpanded ? AccSTATE.EXPANDED : AccSTATE.COLLAPSED;
				var (from, to) = _tv._GetViewRange(); if (child < from || child >= to) r |= AccSTATE.INVISIBLE | AccSTATE.OFFSCREEN;
				if (k.IsDisabled) r |= AccSTATE.DISABLED;
				switch (k.CheckState) { case TVCheck.Checked: case TVCheck.RadioChecked: r |= AccSTATE.CHECKED; break; case TVCheck.Mixed: r |= AccSTATE.MIXED; break; }
				//if(child==_tv._hotIndex) r|=AccSTATE.HOTTRACKED;
				//if(!k.IsEditable) r|=AccSTATE.READONLY;
				return r;
			}

			public override int FocusedChild => _tv._focusedIndex;

			public override List<int> SelectedChildren => _tv.SelectedIndices;

			public override string DefaultAction(int child) {
				var k = _Item(child);
				if (k == null) return null;
				if (k.IsFolder) return k.IsExpanded ? "Collapse" : "Expand";
				return "Activate";
			}

			public override void SelectChild(AccSELFLAG flagsSelect, int child) {
				//if(flagsSelect.HasAny(AccSELFLAG.ADDSELECTION|AccSELFLAG.EXTENDSELECTION) && !_tv.MultiSelect) throw new InvalidOperationException();
				//int anchor=Math.Max(_tv.FocusedIndex, 0);
				switch (flagsSelect & (AccSELFLAG.TAKESELECTION | AccSELFLAG.ADDSELECTION | AccSELFLAG.EXTENDSELECTION | AccSELFLAG.REMOVESELECTION)) {
				case 0: break;
				case AccSELFLAG.TAKESELECTION:
					_tv.SelectSingle(child, andFocus: false);
					break;
				case AccSELFLAG.REMOVESELECTION:
					_tv.Select(child, false);
					break;
				case AccSELFLAG.ADDSELECTION when _tv.MultiSelect:
					_tv.Select(child, true);
					break;
				//case AccSELFLAG.EXTENDSELECTION: //rarely used
				//case AccSELFLAG.ADDSELECTION|AccSELFLAG.EXTENDSELECTION:
				//	break;
				//case AccSELFLAG.REMOVESELECTION|AccSELFLAG.EXTENDSELECTION:
				//	break;
				default: throw new ArgumentException();
				}
				if (flagsSelect.Has(AccSELFLAG.TAKEFOCUS)) {
					//_tv.Focus();
					_tv.FocusedIndex = child;
				}
			}

			public override RECT ChildRect(int child) => _tv.GetRectPhysical(child);

			public override int? Navigate(AccNAVDIR navDir, int childStart) {
				if (childStart == -1) { //navDir can be only first or last
					int n = _tv.CountVisible; if (n == 0) return null;
					return navDir == AccNAVDIR.FIRSTCHILD ? 0 : n - 1;
				} else { //navDir cannot be first or last
					switch (navDir) {
					case AccNAVDIR.PREVIOUS:
					case AccNAVDIR.UP:
						if (childStart > 0) return childStart - 1;
						break;
					case AccNAVDIR.NEXT:
					case AccNAVDIR.DOWN:
						if (++childStart < _tv.CountVisible) return childStart;
						break;
					}
					return null;
				}
			}

			public override int HitTest(int x, int y) => _tv._ItemFromY(y);

			public override void DoDefaultAction(int child) {
				var k = _Item(child);
				if (k == null) return;
				if (k.IsFolder) _tv.Expand(child, null);
				else {
					_tv.SelectSingle(child, andFocus: true);
					_tv.ItemActivated?.Invoke(_tv, new TVItemEventArgs(k, child));
				}
			}
		}
	}
}
