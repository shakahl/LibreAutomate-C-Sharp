/// Use class <google C# class Registry>Registry</google>.
/// 
/// Set a string value.

Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Au\Test", "A", "text");

/// Get a string value if exists.

if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Au\Test", "A", null) is string s1) {
	print.it(s1);
}

/// Set an int (DWORD) value.

int i1 = 100;
Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Au\Test", "B", i1);

/// Get an int (DWORD) value if exists.

if (Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Au\Test", "B", null) is int i2) {
	print.it(i2);
}

/// Delete a value if exists.

using (var k1 = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Au\Test", writable: true)) k1.DeleteValue("A", throwOnMissingValue: false);

/// Enumerate subkeys.

using (var k1 = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft")) {
	foreach (var s2 in k1.GetSubKeyNames()) {
		print.it(s2);
	}
}

/// Sometimes you want to save values of some variables used in a script or class, and next time use the saved values. For it you can use the registry, but it is a slow and awkward way. Usually it's better to use a file, and the fastest possible way is to put the variables in a record class derived from <help><.k>JSettings<><>. Run this script two times to see how it works.

MySettings sett = MySettings.Load(); //in a class you would use a static field or property, but this example uses a local variable for simplicity

print.it(sett.i);
sett.i++;

if (dialog.showInput(out string s, "example", editText: sett.s)) {
	sett.s = s;
}


record MySettings : JSettings { //you may want to change the name (MySettings)
	public static readonly string File = folders.ThisAppDocuments + @"MySettings.json"; //change this

	public static MySettings Load() => Load<MySettings>(File);

	// examples of settings
	public int i;
	public string s = "default";
	
	//Can be used most types, not only int and string. For example bool, DateTime, array, Dictionary. More examples in the JSettings webpage.
}
