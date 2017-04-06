out

str s se
 s=""
 s="''''"
 s="c:\folder\file.txt"
 s="file.txt"
 s="''c:\folder\file.txt''"
 s="''file''"
 s="''c:\file.c'' more"
 s="$desktop$\test.dll"
 s="$desktop$\folder\test.dll"
 s="$desktop$"
 s="%env var%"
 s="%env var%\file.v"
 s="$desktop$\"
 s="$desktop\file.c"
 s="$desktop\fi$le.c"
 s="''$desktop\file.c''"
 s="c:\folder\"
 s=".\file.txt"
 s="c:"
 s="c:\"
 s="''c:''"
 s="c:file.xml"
 s="cc:file.xml"
 s="$desktop$\test.dll:fs"
 s="c:\folder\file.txt:fs"
 s="c:\folder\file.txt:stream1:$DATA"
 s="$desktop\file.c:stream1:$DATA"
 s="c:\folder\file.txt:stre.am1:$DATA"
 s="\file"
 s="\\aa\b\c\d.ext"
 s="\\aa\b\d.ext"
 s="\\aa\d.ext"
 s="\\aa"
 s="http:\\www.qm.com"
 s="mailto:a@c.d"
 s="$my qm$:fs:$DATA"
 s="q:\d:fs:$DATA"

out _s.getpath(s)
 out _s.GetPath(s)
out _s.getpath(s "")
 out _s.GetPath(s 1)

out _s.getfilename(s)
 out _s.GetFilename(s)
out _s.getfilename(s 1)
 out _s.GetFilename(s 1)

 out PathFindFileName(s)
