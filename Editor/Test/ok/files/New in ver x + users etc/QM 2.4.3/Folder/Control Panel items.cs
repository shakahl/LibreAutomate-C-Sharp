 How to enumerate Control Panel items normally on Vista+? Could not find on internet.

 run ":: 14001F706806EE260AA0D7449371BEB064C98683 0C0001008421DE3902000000"
 run "$3$ 1E0071800000000000000000000082FCDDF2128FDD4CB7DCD4FE1425AA4D" ;;Sound

 run "$system$\control.exe" "/name Microsoft.AudioDevicesAndSoundThemes /page Recording"
run "$system$\control.exe" "/name Microsoft.Mouse /page Pointer Options"

 Shell32.Shell k._create
 Shell32.Folder f=k.NameSpace(3)
 Shell32.FolderItems ims=f.Items
  out ims.Count
  ims.Item
 Shell32.FolderItem im
 foreach im ims
	 out im.Name


 SHDocVw.
 SHDocVw.ShellNameSpace ns=