 opt hidden
 out opt(hidden)

 spe 3
 opt save
	 out spe
	 spe 22
	 opt save
		 out spe
		 spe 33
		 out spe
	 opt restore
	 out spe
 opt restore
  out spe
 out getopt(speed)

opt slowkeys
opt save
	out getopt(slowkeys)
	opt slowkeys 0
	opt save
		out getopt(slowkeys)
		opt slowkeys 1
		out getopt(slowkeys)
	opt restore
	out getopt(slowkeys)
opt restore
out getopt(slowkeys)
 opt restore
