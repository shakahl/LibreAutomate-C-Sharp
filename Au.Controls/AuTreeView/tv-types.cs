using Au;
using Au.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Au.Util;
using System.Windows.Input;


namespace Au.Controls
{
	/// <summary>
	/// Interface for <see cref="AuTreeView"/> items. Provides text and other properties.
	/// </summary>
	public interface ITreeViewItem
	{
		//	bool HasItems { get; }
		//	int ItemsCount { get; }
		//	ITreeViewItem Parent { get; }

		/// <summary>
		/// Folder's child items. Return null if not folder.
		/// </summary>
		IEnumerable<ITreeViewItem> Items => null;

		/// <summary>
		/// Is folder.
		/// </summary>
		bool IsFolder => false;

		/// <summary>
		/// Is expanded folder.
		/// </summary>
		bool IsExpanded => false;

		/// <summary>
		/// Called after expanding or collapsing this folder.
		/// </summary>
		void SetIsExpanded(bool yes) { }

		/// <summary>
		/// Text to display.
		/// </summary>
		string DisplayText { get; }

		/// <summary>
		/// Called after label editing.
		/// </summary>
		void SetNewText(string text) { }

		/// <summary>
		/// Image to display.
		/// Alternatively use <see cref="ImageSource"/>.
		/// </summary>
		/// <remarks>
		/// If image's DPI does not match control's DPI (<see cref="AuTreeView.Dpi"/>), draws it auto-scaled.
		/// </remarks>
		System.Drawing.Bitmap Image => null;

		/// <summary>
		/// Where to find image or icon to display.
		/// Can have prefix:
		/// "imagefile:" - path of png/bmp/jpg/gif/tif or xaml file.
		/// "resource:" - path of png/bmp/jpg/gif/tif or xaml resource, like "resources/file.png".
		/// "image:" - Base-64 encoded png/bmp/jpg/gif/tif image.
		/// If without prefix, gets file icon; see <see cref="AIcon.OfFile"/>.
		/// </summary>
		/// <remarks>
		/// To load image the control calls <see cref="AImageUtil.LoadWinformsImageFromFileOrResourceOrString"/> or <see cref="AImageUtil.LoadWpfImageElementFromFileOrResourceOrString"/>.
		/// The control uses <see cref="AIconImageCache"/>; see <see cref="AuTreeView.ImageCache"/>.
		/// </remarks>
		string ImageSource => null;

		/// <summary>
		/// How to draw checkbox when <see cref="AuTreeView.HasCheckboxes"/> true. Default <b>Unchecked</b>.
		/// </summary>
		TVCheck CheckState => default;

		/// <summary>
		/// Draw text and checkbox like disabled (usually gray).
		/// </summary>
		bool IsDisabled => false;

		/// <summary>
		/// Draw bold text.
		/// </summary>
		bool IsBold => false;

		/// <summary>
		/// Can be selected. Default true.
		/// </summary>
		bool IsSelectable => true;

		/// <summary>
		/// Background color in 0xBBGGRR format. If -1 (default), uses default colors, depending on state (normal, selected, hot).
		/// </summary>
		int Color => -1;

		/// <summary>
		/// Text color in 0xBBGGRR format. If -1 (default), uses default colors, depending on state (normal, disabled).
		/// </summary>
		int TextColor => -1;
	}

	/// <summary>
	/// <see cref="AuTreeView"/> checkbox state.
	/// </summary>
#pragma warning disable 1591
	public enum TVCheck : byte { Unchecked, Checked, Mixed, Excluded, RadioUnchecked, RadioChecked, None }
#pragma warning restore 1591

	/// <summary>
	/// <see cref="AuTreeView"/> item parts.
	/// </summary>
	[Flags]
	public enum TVParts
	{
		/// <summary>Empty area at the left (indentation).</summary>
		Left = 1,
		/// <summary>Get checkbox rectangle.</summary>
		Checkbox = 2,
		/// <summary>Get left margin rectangle.</summary>
		MarginLeft = 4,
		/// <summary>Get image rectangle.</summary>
		Image = 8,
		/// <summary>Get text rectangle, including area to the right. Can be combined with <b>Image</b>.</summary>
		Text = 16,
		/// <summary>Get left margin rectangle.</summary>
		MarginRight = 32,
		/// <summary>Empty area at the right.</summary>
		Right = 64,
	}

	/// <summary>
	/// <see cref="AuTreeView.HitTest"/> results.
	/// </summary>
	public struct TVHitTest
	{
		/// <summary>Item, or null if not on an item.</summary>
		public ITreeViewItem item;

		/// <summary>Item index, or -1 if not on an item.</summary>
		public int index;

		/// <summary>Item part.</summary>
		public TVParts part;
	}

#pragma warning disable 1591
	public class TVItemEventArgs : EventArgs
	{
		internal TVItemEventArgs(ITreeViewItem item, int index, TVParts part = 0, MouseButton button = 0, int clickCount = 0, POINT xy = default, ModifierKeys mk = 0) {
			Item = item; Index = index; ClickedPart = part; MouseButton = button; ClickCount = clickCount; XY = xy; ModifierKeys = mk;
		}

		public ITreeViewItem Item { get; }
		public int Index { get; }
		public TVParts ClickedPart { get; }
		public MouseButton MouseButton { get; }
		public int ClickCount { get; }

		//rejected. Most properties are not useful and on button up incorrect.
		//	/// <summary>
		//	/// Mouse button down event data.
		//	/// null if activated not with mouse.
		//	/// </summary>
		//	public MouseButtonEventArgs MouseEventArgs { get; }

		/// <summary>
		/// Keyboard modifier keys.
		/// On click it is keys that were pressed at mouse button down event time.
		/// </summary>
		public ModifierKeys ModifierKeys { get; }

		/// <summary>
		/// Mouse position in control at mouse button down event time.
		/// </summary>
		public POINT XY { get; }
	}

	/// <summary>
	/// Custom-draw interface used with <see cref="AuTreeView.CustomDraw"/>.
	/// </summary>
	/// <remarks>
	/// When drawing the control, the first and last called functions are <b>Begin</b> and <b>End</b>. For each actually visible item are called functions in this order: <b>DrawBackground</b>, <b>DrawCheckbox</b>, <b>DrawImage</b>, <b>DrawText</b>, <b>DrawMarginLeft</b>, <b>DrawMarginRight</b>. If a bool function returns false, the control draws that part of the item.
	/// </remarks>
	public interface ITVCustomDraw
	{
		void Begin(TVDrawInfo cd, GdiTextRenderer tr);
		bool DrawBackground();
		bool DrawCheckbox();
		bool DrawImage(System.Drawing.Bitmap image);
		bool DrawText();
		void DrawMarginLeft();
		void DrawMarginRight();
		void End();
	}

	/// <summary>
	/// Custom-draw info passed to <see cref="ITVCustomDraw"/>.
	/// </summary>
	public class TVDrawInfo
	{
		public readonly AuTreeView control;

		/// <summary>GDI device context handle.</summary>
		public readonly IntPtr dc;

		public readonly System.Drawing.Graphics graphics;
		public readonly int dpi;

		/// <summary>Index of current item.</summary>
		public int index;

		/// <summary>Current item.</summary>
		public ITreeViewItem item;

		/// <summary>Background drawing rectangle of current item.</summary>
		public RECT rect;

		/// <summary>Image drawing rectangle of current item.</summary>
		public RECT imageRect;

		/// <summary>Text drawing X offset of current item.</summary>
		public int xText;

		/// <summary>Text drawing Y offset of current item.</summary>
		public int yText;

		/// <summary>Left margin drawing X offset of current item.</summary>
		public int xLeft;

		/// <summary>Right margin drawing X offset of current item.</summary>
		public int xRight;

		/// <summary>Left margin width of current item.</summary>
		public int marginLeft;

		/// <summary>Right margin width of current item.</summary>
		public int marginRight;

		/// <summary>Checkbox size.</summary>
		public SIZE checkSize;

		public bool isSelected, isHot, isFocusedItem, isFocusedControl;

		public TVDrawInfo(AuTreeView tv, IntPtr dc, System.Drawing.Graphics graphics, int dpi) {
			control = tv;
			this.dc = dc;
			this.graphics = graphics;
			this.dpi = dpi;
		}
	}

	/// <summary>See <see cref="AuTreeView.GetDropInfo"/>.</summary>
	public struct TVDropInfo
	{
		/// <summary>Point relative to the top-left of the control without border. Physical pixels.</summary>
		public POINT xy;

		/// <summary>Item at <b>targetIndex</b>, or null if the control is empty.</summary>
		public ITreeViewItem targetItem;

		/// <summary>Drop target item index, or index of the last item if <b>xy</b> is not on an item, or -1 if the control is empty.</summary>
		public int targetIndex;

		/// <summary>Index of item from <b>xy</b>, or -1 if <b>xy</b> is not on an item.</summary>
		public int rawIndex;

		/// <summary>If true, should insert after the drop target item. Else before. Not used if <b>intoFolder</b> is true.</summary>
		public bool insertAfter;

		/// <summary><b>xy</b> is on a folder image, therefore should move to the folder.</summary>
		public bool intoFolder;
	}

}
