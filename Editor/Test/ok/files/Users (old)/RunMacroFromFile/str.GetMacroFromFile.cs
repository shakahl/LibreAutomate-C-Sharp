function $qml_file $macro_name [flags] [str&sData] ;;flags: 0 not folder, 1 folder

 Extracts macro or item of other type from QM macro list file (qml).
 Error if does not find.
 If sData used, extracts from it. It must contain file data. Then qml_file not used.

 EXAMPLE
 str s.GetMacroFromFile("test.qml" "Macro2")
 RunTextAsMacro s


str __s; if(!&sData) &sData=__s; sData.getfile(qml_file)

str sm.from("[]  " macro_name " ")
sm[2]=0; sm[3]=0; sm[sm.len-1]=0
 find name
 g1
int i j=findb(sData sm sm.len j); if(j<0) goto e
 get flags
j+sm.len
if(sData[j]) j=findb(sData "" 1 j); if(j<0) goto e
j+1
int isFolder=val(sData+j)&0x10000!0
if(isFolder!=flags&1) goto g1
 find/get text
i=findb(sData "[]" 2 j)+2; if(i<2) goto e
this.get(sData i len(sData+i)-2)
ret
 e
end "macro not found"