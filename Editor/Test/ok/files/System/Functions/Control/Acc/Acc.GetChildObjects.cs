function ARRAY(Acc)&array [level] [$role] [$name] [$prop] [flags] ;;level: 0 direct children, >=1 further descendants, -1 all descendants, MakeInt(levelMin levelMax)

 Gets descendant accessible objects.

 array - variable that receives accessible objects.
 level - level of descendants in the object hierarchy, relative to this object.
   If omitted or 0, gets direct children. If >=1, gets further descendants.
   If -1, gets descendants of all levels.
   Can be MakeInt(levelMin levelMax). Then gets descendants between and including levelMin and levelMax.
   You can see the hierarchy in the "Find accessible object" dialog.
 role, name, prop, flags - properties of descendants that must match. Same as with <help>Acc.Find</help>.

 Added in: QM 2.3.3.

 EXAMPLES
 Acc a.FromMouse
 ARRAY(Acc) c; int i
 a.GetChildObjects(c)
 for i 0 c.len
	 c[i].Role(_s); out _s
	 out c[i].Name

  get all links in web page in Firefox
 int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
 Acc a.Find(w "DOCUMENT" "" "" 0x3010 2)
 ARRAY(Acc) c; int i
 a.GetChildObjects(c -1 "LINK")
 for i 0 c.len
	 out c[i].Value

  get name/URL of all Firefox tabs
 int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
 Acc a.Find(w "" "" "" 0x3010 2 0 "parent3") ;;find root web page object and get its parent GROUPING
 ARRAY(Acc) b; int i
 a.GetChildObjects(b 2 "DOCUMENT" "" "" 16) ;;get all child DOCUMENT at level 2, including hidden
 for i 0 b.len
	 out "--------"
	 out b[i].Name ;;page name
	 out b[i].Value ;;page URL
	 if(b[i].State&STATE_SYSTEM_INVISIBLE=0) out "<visible>"


if(!a) end ERR_INIT
array=0

if level!-1
	str s=prop
	s.addline(F"level={level&0xffff} {level>>16}")
	prop=s

Acc _a.Find(a role name prop flags 0 0 "" &sub.Callback &array)

err+ end _error


#sub Callback
function# Acc&a level ARRAY(Acc)&ar
ar[]=a
ret 1
