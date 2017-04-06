out
str s=
 Date:
 {{}D}  - short date ({D})
 {{}DD} - long date ({DD})
 
 Time:
 {{}T}  - time without seconds ({T})
 {{}TT} - time with seconds ({TT})
 
 Custom date:
 {{}
 d    - day, 1-2 digits ({d})
 dd   - day, 2 digits ({dd})
 ddd  - day of week, abbreviated name ({ddd})
 dddd - day of week, full name ({dddd})
 M    - month, 1-2 digits ({M})
 MM   - month, 2 digits ({MM})
 MMM  - month, abbreviated name ({MMM})
 MMMM - month, full name ({MMMM})
 y    - year, 1-2 digits ({y})
 yy   - year, 2 digits ({yy})
 yyyy - year, 4-5 digits ({yyyy})
 gg   - period/era or nothing ({gg})
 }
 
 Custom time:
 {{}
 h    - time, 12 hours, 1-2 digits ({h})
 hh   - time, 12 hours, 2 digits ({hh})
 H    - time, 24 hours, 1-2 digits ({H})
 HH   - time, 24 hours, 2 digits ({HH})
 m    - minutes, 1-2 digits ({m})
 mm   - minutes, 2 digits ({mm})
 s    - seconds, 1-2 digits ({s})
 ss   - seconds, 2 digits ({ss})
 t    - A/P ({t})
 tt   - AM/PM ({tt})
 }

out _s.timeformat(s)
