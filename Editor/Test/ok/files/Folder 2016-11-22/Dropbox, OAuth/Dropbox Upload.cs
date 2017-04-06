out
#compile "__Dropbox"
Dropbox x
 x.Authorize("kuacy0sv1zqrebo" "mgl8i88y880qxcv")
 x.Authorize("myAppKey" "myAppSecret")
 x.Authorize("kuacy0sv1zqrebo" "myAppSecret")
x.token="bbJgKCNsfDAAAAAAAAAA1cXV1I7w2hblUr2hYr6OHnXbG6hDe9G96qtSk2_NtsT4"
out x.Upload("Q:\Test\test.cs" "/test.cs" "overwrite")
 out x.Upload("Q:\Test\a.txt" "/many/a.txt" "overwrite")

 Dir d
 foreach(d "Q:\Downloads\*" FE_Dir 4)
	 long size=d.FileSize
	 if(size>100) continue
	 str path=d.FullPath
	  out path
	 str name=d.FileName
	 
	 out x.Upload(path F"/many/{name}" "overwrite")

 115
