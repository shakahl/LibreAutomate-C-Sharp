function# $macro $name [action] ;;action: 0 read, 1 write, 2 delete

 Reads, writes or deletes a value from a macro that contains list of name/value pairs.
 If action is 0, returns 1 if found the name in the macro, or 0 if not found or the macro does not exist.
 If action is 1 or 2, returns 1.

 Macro format:
 name1 value1
 name2 value2
 ...

 macro - macro name.
 name - the first word in a line.
 action:
   0 - reads the value (the rest of the line) from the macro to this variable (str variable for which the function is called).
   1 - writes this variable to the macro as the value.
   2 - removes the name/value pair from the macro. Does not use and does not change this variable.

 EXAMPLE
 str s s1 s2
  write two values
 s="value1"; s.ValueFromMacro("testx" "name1" 1)
 s="value2"; s.ValueFromMacro("testx" "name2" 1)
  read two values
 s1.ValueFromMacro("testx" "name1")
 s2.ValueFromMacro("testx" "name2")
 out "%s %s" s1 s2


 macro to string map
IStringMap m=CreateStringMap(1|4)
str s.getmacro(macro); err if(!action) ret
if(s.len) m.AddList(s "")

 get, add/replace or remove
sel action
	case 0 ret m.Get2(name this)!=0
	case 1 m.Add(name this)
	case 2 m.Remove(name)
	case else ret

 string map to macro
m.GetList(s "")
int i=qmitem(macro)
if(i) s.setmacro(i)
else newitem macro s "" "" "" 3

err+ end _error
ret 1
