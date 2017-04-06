out

str s="ab 9**...&&yo"
 s[6]=0
str rx="\d(\W+)"
out findrx(s rx)
out findrx(s rx 0 0 0 1)
out findrx(s rx 0 0 _i); out _i
out findrx(s rx 0 0 _i 1); out _i
out findrx(s rx 0 0 _s); out _s;; out _s.len
out findrx(s rx 0 0 _s 1); out _s
 out findrx("" rx 0 128 _s)
 out _s.nc
out "---"
