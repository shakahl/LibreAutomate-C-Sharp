 wait 0 "Notepad"
 wait 30 WP "Notepad"
 wait 0 WE " Notepad"
 wait 0 K (VK_RETURN)
 wait 0 KF <
 int m=wait(0 M)
 out m
 wait 0 P 5
 int+ g_wait=0
 int i=wait(0 V g_wait)
 out i
 int m=mac("Macro109")
 wait 10 H m; err ErrMsg(1)

 wait 0 C 0xFF0000 390 60
 wait 0 C 0xFF0000 394 63 "app - Microsoft Visual C++ [design] - RunWait.cpp"
 wait 0 C 0xC8D0D4 317 58 "Document - WordPad"
 wait 0 C 0xC8D0D4 317 0.5 "Document - WordPad"
 wait 0 C 0x800000 246 16 id(59392 "Document - WordPad") 2
 WaitPixelColor 0 0x800000 246 16 id(59392 "Document - WordPad")
 wait 0 C 0x818181 40 4 0 1

 web "http://www.quickmacros.com/forum/viewforum.php?f=1&sid=327213ec0ed6bf91dcd60055df537e7f"
 web "msdn.microsoft.com/library"
web "http://msdn.microsoft.com/isapi/gomscom.asp?TARGET=/downloads/details.aspx?FamilyID=00600351-4c8f-43cd-b3e3-a9975ecda0ce&displaylang=en"
 1
 web "http://msdn.microsoft.com/library"
wait 0 I "microsoft.com"

 wait 0 CU IDC_IBEAM
 spe 555
