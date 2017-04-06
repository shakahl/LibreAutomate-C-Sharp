int w2

	int k
	for k 0 1000000
		act w2
		err
			if(k<50) ;;5 s
				wait 0.1
			else
				OnScreenDisplay _error.description 5 0 0 "" 0 0xFF
				ret
