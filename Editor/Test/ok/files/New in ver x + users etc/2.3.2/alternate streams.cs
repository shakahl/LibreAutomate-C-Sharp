 str s="one"
 s.setfile("$desktop$\test streams.txt")
 
 str ss="this is stream data"
 ss.setfile("$desktop$\test streams.txt:stream1")

 str sss.getfile("$desktop$\test streams.txt")
 out sss

 str sss.getfile("$desktop$\test streams.txt:stream1")
 out sss

str sss.getfile("$desktop$\test streams - copy.txt:stream1")
out sss

 Dir d
 foreach(d "$Desktop$\*" FE_Dir 0x2)
	 str sPath=d.FileName(1)
	 out sPath
	 
