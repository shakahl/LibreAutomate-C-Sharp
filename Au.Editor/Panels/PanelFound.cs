using System.Windows.Controls;
using Au.Controls;

class PanelFound : DockPanel
{
	KScintilla _c;

	public KScintilla ZControl => _c;

	public PanelFound()
	{
		_c = new KScintilla { Name = "Found_list" };
		_c.ZInitReadOnlyAlways = true;
		_c.ZInitTagsStyle = KScintilla.ZTagsStyle.AutoAlways;
		_c.ZAcceptsEnter = true;
		_c.ZHandleCreated += _c_ZHandleCreated;

		this.Children.Add(_c);
	}

	private void _c_ZHandleCreated() {
		_c.zSetMarginWidth(1, 0);
		_c.zStyleFont(Sci.STYLE_DEFAULT, App.Wmain);
		_c.zStyleClearAll();
		_c.ZTags.SetLinkStyle(new SciTags.UserDefinedStyle(), (false, default), false);
	}
}
