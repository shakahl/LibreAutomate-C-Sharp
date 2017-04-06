out

str k1("Akey1") k2("Bkey2") k3("Ckey3")

IStringMap m=CreateStringMap(8)
m.Add(k1 "value1")
m.Add(k2 "value2")
m.Add(k3 "value3")
 m.Add(k1 "value1 2")
 m.Add(k1 "value1 3")
 m.Set(k1 "value1 set")
 m.Set(k1 "value4")
 m.Set(k1 "v")
 m.Rename(k2 "zip")
 m.Rename(k1 "k1 new")
 m.Remove(k1)
 m.AddList("Akey1 aaaa[]Dkey4 dd" "")
 m.RemoveAll
 out m.Count

 int iv
 m.IntAdd("k4" 5)
 if(m.IntGet("k4" iv)) m.IntSet("k4" iv+1)
 m.IntAdd("k5" -2000000000)
 if(m.IntGet("k5" iv)) out iv

lpstr v
v=m.Get(k1)
 v=m.Get(k2)
 v=m.Get(k3)
 v=m.Get2(k1 _s)
out v
 out _s

out "---"

ARRAY(str) ak av
int i
 m.GetAll(ak av)
 for i 0 ak.len
	 out "%s=%s" ak[i] av[i]
 m.GetAll(ak 0)
 for i 0 ak.len
	 out ak[i]
 m.GetAll(0 av)
 for i 0 av.len
	 out av[i]

str sk sv
m.EnumBegin
rep
	if(!m.EnumNext(sk sv)) break
	out "%s=%s" sk sv

 out m.GetAllOf(k1 av)
 for i 0 av.len
	 out "%s=%s" k1 av[i]
