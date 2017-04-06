 /
function htv [flags] [hItem] ;;flags: don't collapse first item

hItem=SendMessage(htv TVM_GETNEXTITEM iif(hItem TVGN_CHILD TVGN_ROOT) hItem)
rep
	if(!hItem) break
	if(flags&1) flags~1
	else SendMessage(htv TVM_EXPAND TVE_COLLAPSE hItem)
	TreeViewCollapseAll htv 0 hItem
	hItem=SendMessage(htv TVM_GETNEXTITEM TVGN_NEXT hItem)
