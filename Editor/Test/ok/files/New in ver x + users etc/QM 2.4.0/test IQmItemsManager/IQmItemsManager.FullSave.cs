str s.all(100000 2 'A')
 _qmfile.ResourceAdd("IQmItemsManager.FullSave" 1 "big" s s.len)
 _qmfile.ResourceDelete("IQmItemsManager.FullSave" 1 "big")
 _qmfile.FullSave
