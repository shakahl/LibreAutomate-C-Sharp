out

int updateData=1
str folder="q:\test\ok"
if(updateData) del- folder; err
mkdir folder

ICsv c._create; c.Separator="|"
 c.AddRowCSV(0 "name,flags,level,guid,td")
IXml x._create; IXmlNode xr=x.Add("x")
str j="[ []"

ARRAY(QMITEMIDLEVEL) a; int i
if(!GetQmItemsInFolder("" &a)) end "failed"
int prevLevel=0
for i 0 a.len
	int iid=a[i].id
	QMITEM q; qmitem(iid 0 q 1|2|4|8|32|256); err continue ;;error if in Deleted
	q.name.ReplaceInvalidFilenameCharacters("@")
	
	str name flags guid td
	name=q.name
	
	GUID g; _qmfile.SqliteItemProp(+iid 0 g)
	guid.fromn(&g sizeof(GUID)); guid.encrypt(4 guid "" 3)
	
	int f=0
	sel(q.itype) case 5 f|1; case 7 f|2
	if(q.flags&8) f|8
	flags=iif(f>9 F"0x{f}" iif(f F"{f}" ""))
	
	int lev(a[i].level) k=(lev-prevLevel); prevLevel=lev
	if(k>0) flags+">"; else if(k<0) flags+"<"; k=-k; if(k>1) flags+k
	
	 CSV
	c.AddRowSA(-1 4 &name)
	
	 XML
	IXmlNode n=xr.Add("s" guid)
	n.SetAttribute("n" name)
	n.SetAttribute("f" flags)
	if(td.len) n.SetAttribute("t" td)
	
	 JSON
	j+F"{{''n'':''{name}'', ''f'':{f}, ''g'':''{guid}''"
	if(td.len) td.findreplace("\" "\\"); j+F", ''t'':''{td}''"
	j+"},[]"
	
	 TEXT
	if updateData
		str itempath=sub.GetQmItemPath(iid)
		str s=F"{folder}{itempath}"
		sel q.itype
			case 5 ;;folder
			mkdir s
			case 7 ;;file link
			CreateShortcut F"{s}.lnk" q.linktarget
			case else
			q.text.setfile(F"{s}.cs")
	
	err+
		out F"<><c 0xff>{q.name}: {_error.description}</c>"

c.ToFile("q:\test\ok\LIST.csv")
 c.ToString(_s); out _s; _s.setclip
x.ToFile("q:\test\ok\LIST.xml")
 str s; x.ToString(s); s-"[][][]"; s.setfile("q:\test\ok\LIST.xml") ;;File.ReadAllText loads at same speed
j+"]"; j.setfile("q:\test\ok\LIST.json")


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
