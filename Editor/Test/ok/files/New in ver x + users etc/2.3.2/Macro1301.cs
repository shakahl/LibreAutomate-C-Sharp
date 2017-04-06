 int w1=win("Microsoft Access" "OMain")

int w1 = win("2009 Q1 Data")
out w1

int activeWindow=win
out activeWindow

	int k
	for k 0 1000000
		act w2
		err
			if(k<50) ;;5 s
				wait 0.1
			else
				OnScreenDisplay
				ret
