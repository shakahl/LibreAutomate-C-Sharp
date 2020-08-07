using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
//using System.Linq;

using Au.Types;

namespace Au.Util
{
	/// <summary>
	/// Security-related functions, such as enabling privileges.
	/// </summary>
	public static class ASecurity
	{
		/// <summary>
		/// Enables or disables a privilege for this process.
		/// Returns false if fails. Supports <see cref="ALastError"/>.
		/// </summary>
		/// <param name="privilegeName"></param>
		/// <param name="enable"></param>
		public static bool SetPrivilege(string privilegeName, bool enable)
		{
			bool ok = false;
			var p = new Api.TOKEN_PRIVILEGES { PrivilegeCount = 1, Privileges = new Api.LUID_AND_ATTRIBUTES { Attributes = enable ? 2u : 0 } }; //SE_PRIVILEGE_ENABLED
			if(Api.LookupPrivilegeValue(null, privilegeName, out p.Privileges.Luid)) {
				Api.OpenProcessToken(Api.GetCurrentProcess(), Api.TOKEN_ADJUST_PRIVILEGES, out Handle_ hToken);
				Api.AdjustTokenPrivileges(hToken, false, p, 0, null, default);
				ok = 0 == ALastError.Code;
				hToken.Dispose();
			}
			return ok;
		}
	}
}
