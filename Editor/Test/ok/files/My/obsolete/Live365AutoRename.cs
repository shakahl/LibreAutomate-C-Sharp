function event $name [$newname]
 event: 1 added, 2 removed, 4 renamed, 8 modified
str s(name) ss
if(!s.endi("\temp.mp3")) ret
spe 200

 get song info from Live365 player
15 ;;usually the list is updated earlier, but not always, so we should wait a while
Htm el=htm("A" "" "<A class=playlist onclick=''BuyIt('buy', 2)''*" "Live365 - Player Window" "5" 0 0x24)
err ret
s=el.Text
ARRAY(str) a
if(findrx(s "^(.+?) - (.+) \[\d+:\d+\] - (.+)$" 0 0 a)) ret
ss.format("%s - %s.mp3" a[1] a[2])
ss.ReplaceInvalidFilenameCharacters("_")
s.getpath(newname)
s+ss

 process duplicates
str+ g_live365_file
ss=g_live365_file
g_live365_file=s
if(dir(s))
	if(s=ss) UniqueFileName s ;;failed to split, so we will have more than one part of the same song. The first part will have incorrect filename/tags (from prev song).
	else ;;this song is already recorded in the past. If size is similar, delete new one, else make unique file name.
		int sizold=_dir.fd.nFileSizeLow
		dir(newname)
		int siznew=_dir.fd.nFileSizeLow
		int dif=sizold-siznew; if(dif<0) dif=-dif
		 out "Live365AutoRename: %i" dif
		if(dif<100000) del- newname; ret
		else UniqueFileName s

 rename
 out "%s   %s" newname s
ren newname s; err ret

 add id3 tags
typelib Id3Lib "id3com.dll"
Id3Lib.ID3ComTag t._create
BSTR b=s
t.Link(&b)
t.Artist=a[1]
t.Title=a[2]
t.Album=a[3]
t.SaveV1Tag
t.SaveV2Tag
