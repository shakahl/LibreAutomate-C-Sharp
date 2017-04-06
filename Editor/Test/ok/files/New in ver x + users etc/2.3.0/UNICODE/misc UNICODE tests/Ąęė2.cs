dll user32 @*CharLowerW @*lpsz

out
BSTR s.alloc(100)
int i
for i 0x80 0x10000
	int lo=CharLowerW(+i)
	if(lo=i) continue
	int a1(i) a2(lo) a3(i) a4(lo)
	WINAPI2.wvnsprintfW(s 100 L"0x%X 0x%X %c %c" +&a1)
	 str s.format("0x%X %C %C" i i lo)
	out s
	 out
	