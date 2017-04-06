out
 clear
RECT x; GetWindowRect win &x
outb &x sizeof(x)
__test x
outb &x sizeof(x)
