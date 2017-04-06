 /
function &a size

int b=q_malloc(size)
VirtualProtect +b size PAGE_EXECUTE_READWRITE &_i
memcpy +b +a size
a=b
