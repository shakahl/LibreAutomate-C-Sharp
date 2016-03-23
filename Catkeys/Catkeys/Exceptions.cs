using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;
using System.Runtime.Serialization;

using Catkeys;
using static Catkeys.NoClass;
using Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	[Serializable]
	public class CatkeysException :Exception
	{
		const string _m ="Failed.";

		public CatkeysException() : base(_m) { }

		public CatkeysException(string message) : base(message) { }

		public CatkeysException(string message, Exception innerException) : base(message ?? _m, innerException) { }

		protected CatkeysException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class WaitTimeoutException :CatkeysException
	{
		const string _m ="Wait timeout.";

		public WaitTimeoutException() : base(_m) { }

		public WaitTimeoutException(string message) : base(message) { }

		public WaitTimeoutException(string message, Exception innerException) : base(message ?? _m, innerException) { }

		protected WaitTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
