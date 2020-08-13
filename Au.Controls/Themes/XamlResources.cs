using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Au.Controls.WPF
{
	public class XamlResources
	{
		public static ResourceDictionary Dictionary = Application.LoadComponent(new Uri("/Au.Controls;component/themes/generic.xaml", UriKind.Relative)) as ResourceDictionary;
		//build action = default (Page).

		//public static ResourceDictionary Dictionary = XamlReader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Au.Controls.Themes.Generic.xaml")) as ResourceDictionary;
		//build action = embedded resource. Normally works, but somehow exception when debugger.
	}
}
