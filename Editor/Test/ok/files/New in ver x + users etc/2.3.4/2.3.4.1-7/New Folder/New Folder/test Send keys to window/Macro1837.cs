out
 int w=win("Untitled - Notepad" "Notepad")
 int c=id(15 w)
 int w=win("Document4 - Microsoft Word" "OpusApp")
 int c=child("Microsoft Word Document" "_WwG" w)
 int w=win("Adobe Dreamweaver CS4 - [test.html]" "_macr_dreamweaver_frame_window_")
 int c=id(59893 w)
 act w
int c=0

str s="Test: Keys Microsoft Word Document[]Line2: Microsoft Word Document[]"
 key (s); ret
s.getmacro("Macro1837")
 s.lcase
 s="a[]b"
 opt keychar 1
 hid w; 0.1
 opt slowkeys 1
 SendKeysToWindow c key(CaX(0.5)(s))
  SendKeysToWindow c key(CaX(s))
  hid- w
  SendKeysToWindow c key(CaX(0.5)A{ed})
 ret

SendKeysToWindow c key(CaX(0.5))
int t1=timeGetTime
SendTextToWindow c s
out timeGetTime-t1
 SendTextToWindow c "Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines"
 SendTextToWindow c "Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines, Text Without Newlines.[]Line2[]Line3[]"
 SendKeysToWindow c key(Y)

 opt keychar 1
 key (s)
