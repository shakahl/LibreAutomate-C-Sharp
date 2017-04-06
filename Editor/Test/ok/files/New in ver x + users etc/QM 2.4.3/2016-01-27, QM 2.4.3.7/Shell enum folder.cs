out

IShellFolder2 sfD sfRB
ITEMIDLIST* pidlRB pidl
SHGetDesktopFolder(&sfD)
 SHGetSpecialFolderLocation(0 CSIDL_BITBUCKET &pidlRB)
pidlRB=PidlFromStr("$17$")
sfD.BindToObject(pidlRB 0 IID_IShellFolder2 &sfRB)
CoTaskMemFree pidlRB

str s1 s2 s3 s4; str* sp=&s1; int i
SHELLDETAILS sd

 column headers
for i 0 4
	sfRB.GetDetailsOf(0 i &sd)
	sp[i].FromSTRRET(sd.str)
out "<><Z 0xff00>%-25s %-50s %-20s %-20s</Z>" s1 s2 s3 s4

IEnumIDList en
sfRB.EnumObjects(0 SHCONTF_FOLDERS|SHCONTF_NONFOLDERS|SHCONTF_INCLUDEHIDDEN &en)
rep
	en.Next(1 &pidl &_i); if(_hresult) break
	
	  location
	 STRRET sr; sfRB.GetDisplayNameOf(pidl 0 &sr)
	 str s.FromSTRRET(sr pidl)
	 out s
	
	 columns
	for i 0 4
		sfRB.GetDetailsOf(pidl i &sd)
		sp[i].FromSTRRET(sd.str pidl)
	s3.findreplace("[0xe2][0x80][0x8e]"); s3.findreplace("[0xe2][0x80][0x8f]") ;;right-to-left mark etc
	out "%-25s %-50s %-20s %-20s" s1 s2 s3 s4
	
	CoTaskMemFree pidl
	
