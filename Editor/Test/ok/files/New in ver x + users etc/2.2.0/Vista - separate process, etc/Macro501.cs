int w=win("Notepad")
act w
 "aaa"
 lef 30 30 w
 out rset("test" "test" "software\gindi" HKEY_LOCAL_MACHINE)
 men 33 w ;;Font...
 EnableWindow w 0; 5; EnableWindow w 1
 BlockInput 1; 10; BlockInput 0
