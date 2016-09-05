//Classes and functions that were (almost) finished, but rejected for some reason. Maybe still can be useful in the future.
//For example, when tried to make faster/better than existing .NET classes/functions, but the result was not fast/good enough.


public partial class Files
{

	/// <summary>
	/// Gets shell icon of a file or protocol etc where SHGetFileInfo would fail.
	/// Also can get icons of sizes other than 16 or 32.
	/// Cannot get file extension icons.
	/// If pidl is not Zero, uses it and ignores file, else uses file.
	/// Returns Zero if failed.
	/// </summary>
	[HandleProcessCorruptedStateExceptions]
	static unsafe IntPtr _Icon_GetSpec(string file, IntPtr pidl, int size)
	{
		IntPtr R = Zero;
		bool freePidl = false;
		Api.IShellFolder folder = null;
		Api.IExtractIcon eic = null;
		try { //possible exceptions in shell32.dll or in shell extensions
			if(pidl == Zero) {
				pidl = Misc.PidlFromString(file);
				if(pidl == Zero) return Zero;
				freePidl = true;
			}

			IntPtr pidlItem;
			int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
			if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }

			object o;
			hr = folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o);
			//if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			if(0 != hr) {
				if(hr == Api.REGDB_E_CLASSNOTREG) return Zero;
				OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}");
				return Zero;
			}
			eic = o as Api.IExtractIcon;

			var sb = new StringBuilder(300); int ii; uint fl;
			hr = eic.GetIconLocation(0, sb, 300, out ii, out fl);
			if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			string loc = sb.ToString();

			if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) != 0 || 1 != Api.PrivateExtractIcons(loc, ii, size, size, out R, Zero, 1, 0)) {
				IntPtr* hiSmall = null, hiBig = null;
				if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
				hr = eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16));
				if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
			}
		}
		catch(Exception e) { OutDebug($"Exception in _Icon_GetSpec: {file}, {e.Message}, {e.TargetSite}"); }
		finally {
			if(eic != null) Marshal.ReleaseComObject(eic);
			if(folder != null) Marshal.ReleaseComObject(folder);
			if(freePidl) Marshal.FreeCoTaskMem(pidl);
		}
		return R;
	}

}
}
