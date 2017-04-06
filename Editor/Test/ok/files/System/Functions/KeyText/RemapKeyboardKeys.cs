 /
function $map

 Remaps keyboard keys.

 map - text copied from the Remap Keys dialog. To copy, press Ctrl+A, Ctrl+C. If "", clears all remappings.

 REMARKS
 It will be applied after restarting Windows.
 Applies to all user accounts and keyboards.
 Error if fails. Fails if QM is running not as administrator, or if map format is invalid.


ICsv c._create
c.FromString(map)
if(!sub_sys.RKK_Remap(c)) end iif(GetLastError=ERROR_ACCESS_DENIED ERR_ADMIN ERR_FAILED)

err+ end _error

out "Info: Keyboard remapping will be applied after restarting Windows."
