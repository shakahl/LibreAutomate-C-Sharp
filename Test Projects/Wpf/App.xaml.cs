using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

using Au;
using Au.Types;
using static Au.NoClass;

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
