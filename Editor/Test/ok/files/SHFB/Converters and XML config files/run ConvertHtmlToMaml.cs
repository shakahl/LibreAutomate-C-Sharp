out
str f1.expandpath("$qm$\Catkeys\Help\HelpHtml")
str f2.expandpath("$qm$\Catkeys\Help\CatkeysHelp\Converted")
 del- f2; err ;;Windows bug: often does not show the new folder in the "browse folders" dialog
if(FileExists(f2 1)) del- F"{f2}\*"; else mkdir f2; 1

str cl=F"''{f1}'' ''{f2}'' /moveIntro"
if(RunConsole2(F"''$program files$\EWSoftware\Sandcastle Help File Builder\Extras\ConvertHtmlToMaml.exe'' {cl}")) end "failed"
ret

int w=win("CatkeysHelp\*? - Sandcastle Help File Builder" "*.Window.*" 0 0x200)
act w
key F10 fpfae
int w1=wait(30 WA win("Browse For Folder" "#32770"))
int c=id(100 w1) ;;outline 'Select the folder to add'
act c
key R
Acc a.Find(w1 "OUTLINEITEM" "Converted" "class=SysTreeView32[]id=100" 0x1015)
a.Select(3)
key Y
