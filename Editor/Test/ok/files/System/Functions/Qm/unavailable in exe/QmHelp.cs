 /
function# $contextid [index] [action] ;;action: 0 help, 1 open, 2 ShowText, 3 tip, 4 F1, 5 F2, 6 text in Tips.

 Shows help.

 contextid - one of:
   If action is 0:
     1. QM Help topic name, eg "IDP_KEY".
        QM 2.4.1. Can be with #anchor, like "IDP_QMDLL#IsUserAdmin".
     2. Name of a user-defined function. Shows its help section in Tips.
        Also can be a QM item of other type. Shows whole text in Tips.
     3. QM 2.3.3. Anything that can be used with action 4.
   If action is 1:
     Name of a function or QM item of other type. Opens in editor (mac+).
   If action is 2:
     Name of a function or QM item of other type. Shows in text box (ShowText).
   If action is 3:
     Tip name from <link>$qm$\tips.txt</link>. Shows in Tips.
   If action is 4 (QM 2.3.3):
     Any function, constant, macro, etc. Shows help, like if you click it in code editor and press F1.
     For class functions use "classname.functionname". Example: "str.format".
   If action is 5 (QM 2.3.3):
     Same as with action 4. Shows definition, like if you click it in code editor and press F2.
   If action is 6 (QM 2.3.4):
     Text to display in Tips. Can contain <help #IDP_F1>tags</help>.
 index - if used, contextid is list of contextids, and index is 0-based index in the list.
   If several list items have same contextid, use *. For example, "A[]*[]*[]B" will show help for A if index is 0, 1 or 2.

 QM 2.3.3. When shows QM help topic, returns QM help window handle. Only if action 0.


#if EXE
#warning __ME_W1
#else

if(empty(contextid)) ret
if getopt(nargs)>1 and action!6 and (index>0 or findc(contextid 10)>=0)
	ARRAY(str) a=contextid
	contextid=a[index]
	rep() if(StrCompare(contextid "*")) break; else index-1; contextid=a[index]
if(empty(contextid)) ret

int q=-1
sel action&7
	case 0 if(!findrx(contextid "^(ID[HP]_|::)")) q=0; else q=2
	case 1 mac+ contextid
	case 2 ShowText contextid "" 0 2|(qmitem(contextid)<<16)
	case 3 q=1
	case 4 q=3
	case 5 q=4
	case 6 q=5
if(q>=0) ret __QmHelp(q contextid)

err+
