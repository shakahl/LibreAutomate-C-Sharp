using Au;
using Au.Types;
using Au.More;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;


namespace Au.More
{
	/// <summary>
	/// Static functions to open a help topic etc.
	/// </summary>
	public static class HelpUtil
	{
		/// <summary>
		/// Opens an Au library help topic online.
		/// </summary>
		/// <param name="topic">Topic file name, like "Au.elm.find" or "elm.find" or "articles/Wildcard expression".</param>
		public static void AuHelp(string topic) {
			run.itSafe(AuHelpUrl(topic));
		}

		/// <summary>
		/// Gets URL of an Au library help topic.
		/// </summary>
		/// <param name="topic">Topic file name, like "Au.elm.find" or "elm.find" or "articles/Wildcard expression".</param>
		public static string AuHelpUrl(string topic) {
			var url = "https://www.quickmacros.com/au/help/";
			if (!topic.NE()) url = url + (topic.Contains('/') ? null : (topic.Starts("Au.") ? "api/" : "api/Au.")) + topic + (topic.Ends('/') || topic.Ends(".html") ? null : ".html");
			return url;
		}
	}
}
