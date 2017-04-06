out
act "Notepad"
str s="String"

 key abTT (0.5) "Text" (s) (32) (0x10000|30) (0x0112) (0x40112)
 key (0x0112) (0x40112)
 KeyToWindow key(abTT (0.5) "Text" (s) (32) (0x10000|30) (0x0112) (0x40112)) id(15 "Notepad")
KeyToWindow key("Text" (s) (0.1) (32) (0x10000|30) (0x0112) (0x40112) Y) id(15 "Notepad")

#if 0

