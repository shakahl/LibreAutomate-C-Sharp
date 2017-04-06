 Translates multiple PowerPoint files specified in the PowerPointFiles variable.
 Saves translated files in another folder. Does not modify the original files.
 Before running this macro, close PowerPoint application. If fails, look in Task Manager and close hidden PowerPoint processes.

 change these if need. Here $documents$ is the Documents folder.
str ExcelFile="$documents$\dictionary.xls" ;;Excel file. Must contain the table of words, in columns A and B of the first sheet.
str PowerPointFiles="$documents$\*.ppt" ;;source files (all .ppt files in Documents folder). The files will not be modified.
str folderForTranslatedFiles="$documents$\translated" ;;folder where to save translated files. The macro will create it if does not exist.

out ;;clear QM output
 ____________________________________

 create folder for translated files, if does not exist
folderForTranslatedFiles.expandpath
mkdir folderForTranslatedFiles

 get table from Excel file columns A-B
ExcelSheet es.Init("" 8 ExcelFile)
ARRAY(str) a
es.CellsToArray(a "A:B")
es.Close
int i
 for(i 0 a.len) out F"{a[0 i]} = {a[1 i]}" ;;debug-output words

 run hidden PowerPoint process
typelib PowerPoint {91493440-5A91-11CF-8700-00AA0060263B} 2.8
PowerPoint.Application ap._create

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

 out nReplaced
