 if(!BrowseForFolder(_s "" 0)) ret
 if(!BrowseForFolder(_s "" 2)) ret
 if(!BrowseForFolder(_s "" 4)) ret
 if(!BrowseForFolder(_s "" 6)) ret

 if(!BrowseForFolder(_s "$qm$" 0)) ret
 if(!BrowseForFolder(_s "$qm$" 2)) ret
 if(!BrowseForFolder(_s "$qm$" 4)) ret
 if(!BrowseForFolder(_s "$qm$" 6)) ret

 if(!BrowseForFolder(_s ":: 14001F50E04FD020EA3A6910A2D808002B30309D" 0)) ret
 if(!BrowseForFolder(_s ":: 14001F50E04FD020EA3A6910A2D808002B30309D" 2)) ret
 if(!BrowseForFolder(_s ":: 14001F50E04FD020EA3A6910A2D808002B30309D" 4)) ret
 if(!BrowseForFolder(_s ":: 14001F50E04FD020EA3A6910A2D808002B30309D" 6)) ret

 if(!BrowseForFolder(_s ":: 14001F5425481E03947BC34DB131E946B44C8DD5" 0)) ret
 if(!BrowseForFolder(_s ":: 14001F5425481E03947BC34DB131E946B44C8DD5" 2)) ret
 if(!BrowseForFolder(_s ":: 14001F5425481E03947BC34DB131E946B44C8DD5" 4)) ret
 if(!BrowseForFolder(_s ":: 14001F5425481E03947BC34DB131E946B44C8DD5" 6)) ret

 if(!BrowseForFolder(_s "$17$" 0)) ret
 if(!BrowseForFolder(_s "$17$" 2)) ret
 if(!BrowseForFolder(_s "$17$" 4)) ret
 if(!BrowseForFolder(_s "$17$" 6)) ret

if(!BrowseForFolder(_s "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}" 4)) ret ;;not on XP/7

out _s
