using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App :Application
	{
		public App()
		{
			Perf.First();

		}
	}
}
