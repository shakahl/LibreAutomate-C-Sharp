act "Word"; 0.3
str s.getclip(CF_UNICODETEXT)

KeyUnicodeS s 0

 BSTR b.alloc(s.len/2)
 memcpy b.pstr s s.len
 KeyUnicodeB b 1
 