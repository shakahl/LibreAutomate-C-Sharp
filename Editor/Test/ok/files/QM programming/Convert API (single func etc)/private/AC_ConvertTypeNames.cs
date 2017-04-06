function str&s

s.replacerx("(\w+) *(\*+) *(?=\w)" "$1$2")

s.replacerx("\b(UINT|DWORD|int|long|INT|LONG|ULONG|WPARAM|LPARAM|LRESULT|BOOL|HWND|HANDLE|HMODULE|HDC|HKEY|COLORREF|ULONG_PTR|UINT32)\b *(\**) *(?=\w)" "$2") 
s.replacerx("\b(LPDWORD|PLONG)\b *(\**) *(?=\w)" "$2*") 
s.replacerx("\b(LPBYTE|LPVOID)\b *(\**) *(?=\w)" "!$2*") 

s.replacerx("\b(LPSTR|LPTSTR|LPCSTR|LPCTSTR|char*)\b *(\**) *(?=\w)" "$$$2")
s.replacerx("\b(LPCOLESTR|LPOLESTR|PWSTR)\b *(\**) *(?=\w)" "@*$2")
s.replacerx("\b(BYTE|TCHAR)\b *(\**) *(?=\w)" "!$2")
s.replacerx("\b(WORD|short|USHORT)\b *(\**) *(?=\w)" "@$2")
s.replacerx("\b(__int64|INT64|UINT64)\b *(\**) *(?=\w)" "long$2")
s.replacerx("\b(double)\b *(\**) *(?=\w)" "^$2")
s.replacerx("\b(REFIID)\b *(\**) *(?=\w)" "GUID*$2")
 
s.replacerx("\b(void|VOID) *\* *(?=\w)" "!*")
s.replacerx("\bvoid *" "" 1)

s.replacerx("\bLPC?(\w+)\b *" "$1*")

s.replacerx("\b(const|CONST|IN|OUT) +")
