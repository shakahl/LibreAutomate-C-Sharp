 dll user32 #GetKeyboardLayoutList nBuff *lpList
 
 ARRAY(int) a.create(10)
 int n=GetKeyboardLayoutList(10 &a[0])
 int i
 for i 0 n
	 out "0x%X" a[i]

def LOCALE_SLANGUAGE  0x2
dll user32 #GetKeyboardLayout idThread
dll kernel32 #GetLocaleInfo Locale LCType $lpLCData cchData

int hkl=GetKeyboardLayout(GetWindowThreadProcessId(win, 0))
str s
s.fix(GetLocaleInfo(hkl&0xffff LOCALE_SLANGUAGE s.all(100) 100)-1)
out "0x%X" hkl
out s
