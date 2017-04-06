File f.Open("$desktop$\test.txt" "w+")
f.WriteLine("line1")
f.Write("line2[]line3")
f.SetPos(0)
str s
rep
	if(!f.ReadLine(s)) break
	out s

out f.GetPos
out f.FileLen
out f.EOF

f.SetPos(7)

f.Read(s.all(3 2) 3 1)
out s

f.ReadToStr(s -1)
out s
