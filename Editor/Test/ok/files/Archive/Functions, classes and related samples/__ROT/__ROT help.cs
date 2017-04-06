 Gets active COM objects from ROT, like _getactive with moniker.

 Init() retrieves monikers of all objects and returns the number.
 Then you can call GetStr and/or GetObj for each object by index.

 EXAMPLE
#compile ____ROT
__ROT x
int i n=x.Init
for i 0 n
	str s=x.GetStr(i) ;;moniker string
	IUnknown u=x.GetObj(i) ;;the object
	out s
	out u
