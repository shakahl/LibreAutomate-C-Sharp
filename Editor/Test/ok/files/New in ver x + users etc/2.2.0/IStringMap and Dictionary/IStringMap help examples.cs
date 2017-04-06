IStringMap m=CreateStringMap(0)
lpstr s=
 key1 value1
 key2 value2
 key3 value3
m.AddList(s "")

 lpstr v=m.Get("key2")
 if(v) out v
 else out "not found"
 
 ARRAY(str) ak av
 m.GetAll(ak av)
 int i
 for(i 0 ak.len)
	 out "%s %s" ak[i] av[i]
 
 str sk sv
 m.EnumBegin
 rep
	 if(!m.EnumNext(sk sv)) break
	 out "%s %s" sk sv

 if(!m.Get("key2"))
	 m.Add("key2" "new value")
 else
	 out "already exists"
 
 m.Add("key2" "new value")
 err
	 out "already exists"

str s1; lpstr s2
s1=5; m.Add("key5" s1)
s2=m.Get("key5"); if(s2) out val(s2)
