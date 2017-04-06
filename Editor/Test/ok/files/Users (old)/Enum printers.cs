Wsh.WshNetwork n._create
Wsh.WshCollection coll=n.EnumPrinterConnections
knt i
for i 0 coll.Count 2
	VARIANT vi=i+1 ;;odd items are printer names
	str printername=coll.Item(vi)
	out printername
