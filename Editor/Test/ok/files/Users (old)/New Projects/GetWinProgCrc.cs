 /
function [hwnd]

if(!hwnd) hwnd=win
str se.getwinexe(hwnd 1)
str s.getfile(se)
ret Crc32(s s.len)
