str s.getfile("$documents$\words.txt")
out numlines(s)

str s2 s3
foreach s2 s
	s3.addline(s2.stem)

IStringMap m=CreateStringMap(3)
m.AddList(s3)
m.GetList(s3)
 out s3

out numlines(s3)
s3.setfile("$documents$\words_stem.txt")
