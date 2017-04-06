 /
function $prog [$cl] [flags]

EnableDisk2

if(flags&0x400) ret run(prog cl "" "" flags)
run(prog cl "" "" flags)
err+ end _error
