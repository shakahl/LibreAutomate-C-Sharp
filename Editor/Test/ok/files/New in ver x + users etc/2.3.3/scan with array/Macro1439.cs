out
RECT r
 scan "macro:Macro1439.bmp" 0 0 1|2 0 3; ret
ARRAY(RECT) a
scan "macro:Macro1439.bmp" 0 r 1|2 0 a
zRECT r
 scan "macro:Macro1439.bmp" _hwndqm 0 2|0x100 0 a

 __GdiHandle b=LoadPictureFile("$desktop$\Clipboard01.bmp")
 scan "macro:Macro1439.bmp" b 0 0x200|2 0 a

int i
for i 0 a.len
	zRECT a[i]
	mou a[i].left a[i].top; 1
	
