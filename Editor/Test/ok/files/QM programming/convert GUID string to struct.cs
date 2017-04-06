str s="{557cf400-1a04-11d3-9a73-0000f81ef32e}"
out s
s.findreplace("-")
s.replacerx("(\w{8})(\w{4})(\w{4})(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)" "0x$1,0x$2,0x$3,0x$4,0x$5,0x$6,0x$7,0x$8,0x$9,0x$10,0x$11")
out s
