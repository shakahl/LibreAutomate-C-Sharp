out

int updateData=1 ;;delete the folder and recreate all subfolders and script files. Slow.
 str folder="q:\test\ok"
 str folder="e:\test\ok"
str folder="%catkeys%\Editor\test\ok"
if(updateData) del- folder; err
mkdir F"{folder}\files"

IXml xml._create
IXmlNode x=xml.Add("files")
str onlyFolder
 onlyFolder="\System"
 onlyFolder="\QM programming"
onlyFolder="\Catkeys"
if onlyFolder
	int iFolder=qmitem(onlyFolder); if(iFolder=0) end "not found"
sub.AddFolderItems(iFolder x)
xml.ToFile(F"{folder}\Main.xml")
 xml.ToString(_s); out _s


#sub AddFolderItems v
function iFolder IXmlNode'xFolder

ARRAY(QMITEMIDLEVEL) a; int i
if(!GetQmItemsInFolder(iif(iFolder=0 "" +iFolder) &a 1)) end "failed"
for i 0 a.len
	int iid=a[i].id
	QMITEM q; qmitem(iid 0 q 1|2|4|8|32|256); err continue ;;error if in Deleted
	q.name.ReplaceInvalidFilenameCharacters("@")
	
	str name flags guid
	name=q.name
	
	GUID g; _qmfile.SqliteItemProp(+iid 0 g)
	guid.fromn(&g sizeof(GUID)); guid.encrypt(4 guid "" 3); guid.findreplace("/" "-")
	
	int f=0
	if(q.flags&8) f|8 ;;disabled
	flags=iif(f>9 F"0x{f}" iif(f F"{f}" ""))
	
	 XML
	lpstr tag="f"; sel(q.itype) case 5 tag="d" ;;file or directory
	IXmlNode n=xFolder.Add(tag)
	str filename=name; sel(q.itype) case [5,7] case else filename+".cs"
	n.SetAttribute("n" filename)
	n.SetAttribute("g" guid)
	if(flags.len) n.SetAttribute("f" flags)
	if(q.itype=7) n.SetAttribute("path" q.linktarget.expandpath)
	
	 TEXT
	if updateData
		str itempath=sub.GetQmItemPath(iid)
		if(onlyFolder.len) itempath.remove(0 onlyFolder.len)
		str s=F"{folder}\files{itempath}"
		sel q.itype
			case 5 ;;folder
			mkdir s
			case 7 ;;file link
			case else
			q.text.setfile(F"{s}.cs")
	
	 subitems
	if q.itype=5
		sub.AddFolderItems iid n
	
	err+
		out F"<><c 0xff>{q.name}: {_error.description}</c>"


#sub GetQmItemPath
function'str iid

str s
QMITEM qi
rep
	qmitem iid 0 qi 17
	qi.name.ReplaceInvalidFilenameCharacters("@")
	s.from("\" qi.name s)
	iid=qi.folderid
	if(!iid) break
ret s
