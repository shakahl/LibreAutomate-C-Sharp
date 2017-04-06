function [~action] [~error] [~server]

int h=id(7 __sfmain)
if(getopt(nargs)=0)
	if(iserr) SendMessage h LVM_DELETEALLITEMS 0 0; iserr=0
else
	SF_LvAdd h -1 0 action error server
	iserr+1

err+
