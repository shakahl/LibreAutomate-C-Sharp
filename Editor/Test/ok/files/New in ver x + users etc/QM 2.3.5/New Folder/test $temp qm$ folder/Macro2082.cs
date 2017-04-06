 SetEnvVar "TMP" "Q:\test\no"
 SetEnvVar "TMP" "" 1
 SetEnvVar "TEMP" "" 1
 SetEnvVar "USERPROFILE" "" 1
 out _s.expandpath("$temp$")
 out _s.expandpath("$temp qm$")

 mkdir "$temp$\ac"
 SetCurDir "$temp$\ac"
 out GetCurDir
  del- "$temp$\ac"
 FileDelete "$temp$\ac"
  FileRename "$temp$\ac" "ac2"
 out GetCurDir

 str s="dddddd"
 s.setfile("$temp qm$\test.txt")

 IXml x._create; x.FromString("<x />")
 x.ToFile("$temp qm$\test.txt")

 __HFile f.Create("$temp qm$\test.txt" CREATE_ALWAYS)
 __HFile f.Create("_delete\test.txt" CREATE_ALWAYS)
 SetCurDir "$temp$"; __HFile f.Create("test.txt" CREATE_ALWAYS)

 del- "$temp qm$\test.txt"; err
 File f.Open("$temp qm$\test.txt" "w")
 SetCurDir "$temp$"; File f.Open("test.txt" "w")

 not "" r r+

Sqlite x.Open("$temp qm$\test.db3")
