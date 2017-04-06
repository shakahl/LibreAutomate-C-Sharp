 this code loads a string list from a file to variable m
str test2.getfile("$desktop$\test.txt")
IStringMap m=CreateStringMap(1|2)
m.AddList(test2 "[]")

 then you can use m.Get (any number of times) to search for a string in the list
str test="result"
if(m.Get(test))
	out "found"
else
	out "not found"
