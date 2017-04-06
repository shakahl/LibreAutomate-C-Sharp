str sFile="thefile"
str sData
sData.getfile(sFile)
sData.unicode(sData 28592) ;;iso-8859-2 -> UTF-16. The 28592 is from "Code Page Identifiers" table from MSDN library.
sData.ansi(sData CP_UTF8) ;;UTF-16 -> UTF-8
sData.setfile(sFile)
