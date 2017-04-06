out _s.searchpath("rundll32.exe")

#compile "____Wow64DisableWow64FsRedirection"
__Wow64DisableWow64FsRedirection x.DisableRedirection
out _s.searchpath("rundll32.exe")
x.Revert
