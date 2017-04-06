function [x] [y]

if !getopt(nargs)
	SetRectEmpty &rect
else
	SetRectangle(x y 1 1)
