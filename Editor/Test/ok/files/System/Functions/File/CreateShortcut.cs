 /
function# $shortcutpath $targetpath [$cmdLine]

 Creates shortcut.
 Returns: 1 success, 0 failed.

 shortcutpath - shortcut file (.lnk) path with or without shortcut filename, eg "$desktop$\a.lnk", or only "$desktop$". If the folder does not exist, creates.
 targetpath - target file or folder. For non-file objects, can be <help #IDP_SEARCHPATHS>pidl in QM format</help>.
 cmdLine - command line arguments.

 See also: <CreateShortcutEx>, <GetShortcutInfoEx>.

 EXAMPLES
  Create shortcut to qm.exe on the desktop:
 CreateShortcut("$Desktop$\Shortcut To QM.lnk" "$qm$\qm.exe")

  Get shortcut target:
 SHORTCUTINFO si
 if(GetShortcutInfoEx("$desktop$\test.lnk" &si))
	 out si.target


SHORTCUTINFO si.target=targetpath
si.param=cmdLine
ret CreateShortcutEx(shortcutpath &si)
