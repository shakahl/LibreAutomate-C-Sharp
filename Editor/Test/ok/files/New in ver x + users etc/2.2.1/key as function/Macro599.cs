 int hwnd=id(15 "Notepad")
int hwnd=child("" "TPSSynEdit" win("PSPad - [New2.php *]" "TfPSPad.UnicodeClass") 0x5)
 int hwnd=child("Microsoft Word Document" "_WwG" win("Document1 - Microsoft Word" "OpusApp") 0x1)
 SendCharactersToWindow hwnd "Line1[]Line2[]"
SendKeysToWindow2 hwnd key("Line1[]Line2[]")
 SendKeysToWindow2 hwnd key("Line1")
 win
