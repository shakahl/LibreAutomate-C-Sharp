 str s
 IntGetFile "http://www.quickmacros.com/index.html" s
 ShowText "" s

str localfile="$desktop$\quickm21.ex_"
IntGetFile "http://www.quickmacros.com/quickm21.exe" localfile 16 0 1
ren- localfile "$desktop$\quickm21.exe"

 str files=
  index.html
  images\tucows.gif
 str sf sfloc localfolder="$desktop$\qm Http"
 mkdir localfolder
 mkdir _s.from(localfolder "\images")
 Http h.Connect("www.quickmacros.com")
 foreach sf files
	 sfloc.from(localfolder "\" sf)
	 h.FileGet(sf sfloc 16)
	  h.FileGet(sf _s)
	  ShowText "" _s
	  h.FileGet(sf sfloc 16 0 &Function39 0)
