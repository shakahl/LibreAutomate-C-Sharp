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

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public partial class AToolbar
	{
		class _Settings : Util.JSettings
		{
			public static _Settings Load(string file) => _Load<_Settings>(file);

			public RECT bounds { get => _bounds; set => Set(ref _bounds, value); }
			RECT _bounds = new RECT(100, 0, 200, 100);

			public TBBorder border  { get => _border; set => Set2(ref _border, value); }
			TBBorder _border = TBBorder.Sizable2;

			public ColorInt borderColor  { get => _borderColor; set => Set(ref _borderColor, value); }
			ColorInt _borderColor;

			//CONSIDER: don't save properties like border, borderColor. Only bounds. Or add option to save; if true, then show in context menu.
		}
	}
}
