str s w t t2 f folder loc ffound
int c i iend
byte flag
if inp(folder "Path to winapiQM.txt and type_names.txt?" "" "C:\Quick Macros 2\") = 0; end
ffound.searchpath(loc.from(folder "type_names.txt"))
if(!ffound) mes("%s not found." "" "" loc); end; else s.getfile(loc)
ffound.searchpath(loc.from(folder "winapiQM.txt"))
if(!ffound) mes("%s not found." "" "" loc); end; else w.getfile(loc)
mac "Working_Message"
 loop:
	flag=0
	if(t.getl(s c)<0) goto finalize:
	 if(c=5) goto finalize:
	c+1
	t.trim
	rep
		i=findw(w t i+1 "" 1)
		if(i<0) goto loop:		 	;;See NOTE1
		iend=findcs(w " *'[]" i)	;;See NOTE2
		t2.get(w i iend-i)
		if(t2<>t)
			if(!flag)
				f+"[][][]"; f+t; f+" "; flag=1
			t2+" "
			f+t2
 finalize:
f.setclip
f.setfile(loc.from(folder "type_checkit.txt"))
clo "WORKING ..."
mes "Text copied to clipboard and to type_checkit.txt"
 NOTE1:
 "break" breaks out of inner and outer loops, so i couldnt use 2 "rep" loops here
 -bug has been noted in "Loops" Folder

 Less important notes: 
 NOTE2:
 Since findw already found the word,
 is there any better way to extract the word, 
 without another find operation?

 Also in Step 1:
 want to 1) find "type" 2) get next word
 Since findw has string tokenized, how to just extract the token 
 we need (the next one) without doing more steps (like find)
 
 Update: November 19, its 1:25pm here, and i havent looked into what exactly i was doing above.
 Dont know if NOTE2 is still relevant, and i'm outtta time to find out
