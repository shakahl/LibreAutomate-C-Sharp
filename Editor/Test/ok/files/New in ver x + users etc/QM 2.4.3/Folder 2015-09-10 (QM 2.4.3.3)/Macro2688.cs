str s="m a5;"
str rx
out findrx("" "a(\d)" 0 128 rx)
 out "%s %i %i" rx.lpstr rx.nc rx.len
out findrx(s "a(\d)")
out findrx(s rx)

_s=s
out s.replacerx("a(\d)" "M$1"); out s
out _s.replacerx(rx "M$1"); out _s
