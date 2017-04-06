 /
function# selFlags $name [$role] [`Window] [$Class] [$value] [flags] [x] [y] [$navig] [waits] ;;flags: 1 use *, 2 regexp., 4 use * in value, 8 value regexp, 16 +invisible, 32 +useless, 64 immediate, 128 reverse, 0x100 client, 0x200 screen, 0x400 description, 0x800 state, 0x1000 error, 0x8000 callback.

 Obsolete.


spe -1; opt waitmsg -1
Acc a=acc(name role Window Class value flags x y navig waits)
if(!a.a) ret
a.Select(selFlags)
ret 1
err+ end _error
