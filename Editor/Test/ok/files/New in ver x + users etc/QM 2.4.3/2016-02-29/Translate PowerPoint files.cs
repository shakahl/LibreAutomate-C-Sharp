 Translates multiple PowerPoint files. Uses words from the first table in a Word file.
 Saves translated files in another folder. Does not modify source files.

 change these if need. Here $documents$ is the Documents folder.
str WordFile="$documents$\dictionary.doc" ;;Word file containing the table of words
str PowerPointFiles="$documents$\*.ppt" ;;source files (all .ppt files in Documents folder). The files will not be modified.
str folderForTranslatedFiles="$documents$\translated" ;;folder where to save translated files. The macro will create it if does not exist.

out ;;clear QM output
 ____________________________________

 PowerPoint must not be running, it creates problems for this macro
if ProcessNameToId("POWERPNT")
	mes- "Before running this macro, close PowerPoint application. Also close hidden PowerPoint processes in Task Manager." "" "x"

 get table from Word file
ARRAY(str) a
sub.GetTable WordFile a
int i
 for(i 0 a.len) out F"{a[0 i]} = {a[1 i]}" ;;debug-output words

 run hidden PowerPoint process
typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8
PowerPoint.Application ap._create

 create folder for translated files, if does not exist
folderForTranslatedFiles.expandpath
mkdir folderForTranslatedFiles

 repeat for each PowerPoint file
Dir d
foreach(d PowerPointFiles FE_Dir)
	str path=d.FullPath
	sub.TranslateFile ap path a folderForTranslatedFiles


#sub TranslateFile
function PowerPoint.Application'ap str'path ARRAY(str)&a str'folderForTranslatedFiles

out F"<><Z 0xff8080>{path}</Z>" ;;debug-output PowerPoint files

 open PowerPoint file
PowerPoint.Presentation p=ap.Presentations.Open(path 0 0 0)

int i nReplaced

 repeat for each slide
PowerPoint.Slide k
foreach k p.Slides
	str slideName=k.Name; out F"<><Z 0x80ffff>{slideName}</Z>" ;;debug-output slide name
	
	 repeat for each text box
	PowerPoint.Shape h
	foreach h k.Shapes
		if(!h.HasTextFrame) continue
		if(!h.TextFrame.HasText) continue
		str s=h.TextFrame.TextRange.Text
		 out s ;;debug-output slide text
		
		 replace words in this text box
		for i 0 a.len
			nReplaced+=s.findreplace(a[0 i] a[1 i] 1|2|64)
		h.TextFrame.TextRange.Text=s

 save translated file
str newFilePath.from(folderForTranslatedFiles "\" _s.getfilename(path 1))
p.SaveAs(newFilePath 1 -2)
p.Close

 out nReplaced


#sub GetTable
function str'WordFile ARRAY(str)&a

 create empty 2-dim array
a.create(2 0)

 open Word file
typelib Word {00020905-0000-0000-C000-000000000046} 8.0 0x409
Word.Application wa._create
VARIANT v1(WordFile.expandpath) v2(-1)
Word.Document wd=wa.Documents.Open(v1 @ v2)

 repeat for each row in the first table
str s1 s2
Word.Row r
foreach r wd.Tables.Item(1).Rows
	 get cells
	s1=r.Cells.Item(1).Range.Text; s1.fix(s1.len-2)
	s2=r.Cells.Item(2).Range.Text; s2.fix(s2.len-2)
	if(!s1.len) continue
	 add to array a
	int i=a.redim(-1); a[0 i]=s1; a[1 i]=s2

 quit; without this would remain hidden Word processes
wa.Quit
