out
str process_pth="\RR_QM_enhancements\__ACTIVE__\QM_popupmenu"
ARRAY(int) a aLevel
GetQmItemsInFolder(process_pth a aLevel)
int i; str fullpath
for i 0 a.len
	 out _s.getmacro(a[i] 1)
	GetQmItemPath a[i] fullpath
	out fullpath
	out aLevel[i]
