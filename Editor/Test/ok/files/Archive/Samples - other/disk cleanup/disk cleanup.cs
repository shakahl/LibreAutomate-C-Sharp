sel list("Create/change disk cleanup preset #100[]Run disk cleanup preset #100")
	case 1 run "cleanmgr.exe" "/sageset:100" "" "" 0x10400
	case 2 run "cleanmgr.exe" "/sagerun:100" "" "" 0x10400
