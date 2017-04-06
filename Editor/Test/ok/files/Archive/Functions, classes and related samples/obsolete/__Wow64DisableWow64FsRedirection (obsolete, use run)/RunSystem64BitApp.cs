 /
function $_file [$par] [$verb] [$dir_] [flags] ;;obsolete, use run with flag 0x4000

 Runs a program or other file in system32 folder on 64-bit Windows.
 The same as <help>run</help>, but temporarily disables redirection to the 32-bit folder SysWOW64.
 On 32-bit Windows also works.

 Everything is same as with run(), except:
   1. The program will run with same integrity level (Admin/uiAccess/User) as current process (QM or exe). Ignores flags 0x10000 and 0x20000.
   2. Does not have window_ and hwnd parameters.


#compile "____Wow64DisableWow64FsRedirection"
__Wow64DisableWow64FsRedirection x.DisableRedirection

run _file par verb dir_ flags|0x30000
err end _error
