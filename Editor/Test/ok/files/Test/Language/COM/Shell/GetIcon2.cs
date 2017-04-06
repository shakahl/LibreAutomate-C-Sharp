 /Test GetIcon2
function# $file [size]

typelib IShellFolderEx_TLB {B96BCBE1-F886-11D0-9C63-A06801C10627} 1.2 ;;IShellFolder Extended Type Library v1.2, ver 1.2
typelib olelib {3181A65A-CC39-4CDE-A4DF-2E889E6F1AF1} 1.4 ;;Edanmo's OLE interfaces & functions v1.4, ver 1.4

dll shell32 #SHGetDesktopFolder IShellFolder*pshf

IShellFolder isf isfd
IExtractIconA iei
ITEMIDLIST* pidl
str s s1 s2
int i fl hi hr

s.searchpath(file); if(s.len=0) ret
s1.getpath(s); if(s1.len<s.len) s2.getfilename(s 1)
if(size=0) size=16

SHGetDesktopFolder &isfd
hr=isfd.ParseDisplayName(0 0 s.unicode(s1) 0 &pidl 0); if(hr) ret
hr=isfd.BindToObject(pidl 0 uuidof(IShellFolder) &isf); if(hr) ret
if(pidl) CoTaskMemFree pidl; pidl=0
hr=isf.ParseDisplayName(0 0 s.unicode(s2) 0 &pidl 0); if(hr) ret
hr=isf.GetUIObjectOf(0 1 &pidl uuidof(IExtractIconA) 0 &iei); if(hr) ret
if(pidl) CoTaskMemFree pidl; pidl=0
fl=iei.GetIconLocation(0 s.all(260) 260 &i)

if(size<=24) hr=iei.Extract(s i 0 &hi size<<16)
else hr=iei.Extract(s i &hi 0 size)

ret hi