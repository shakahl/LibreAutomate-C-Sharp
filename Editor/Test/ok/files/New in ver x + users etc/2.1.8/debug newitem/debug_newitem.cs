spe -1
int x y
for x 50 1000 50
	for y 50 700 50
		key AZ
		1
		mac "EA_Main"
		1
		int h=win("Find accessible")
		lef+ 30 30 h 1
		mou+ -100 0
		mou x y
		0.5
		lef-
		1
		wait 10 WE h
		but id(41 h)
		5
		clo h
		1
	