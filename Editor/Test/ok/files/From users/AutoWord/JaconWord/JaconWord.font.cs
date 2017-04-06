function $fname [size] [style] ;;style: 1 bold, 2 italic, 4 underline, 8 strikeout

jaw_rng.Text=0
if(style)
	jaw_rng.Font.Reset
	if(style&1) jaw_rng.Font.Bold=1
	if(style&2) jaw_rng.Font.Italic=1
	if(style&4) jaw_rng.Font.Underline=1
	if(style&8) jaw_rng.Font.StrikeThrough=1
if(size)
	jaw_rng.Font.Size=size
jaw_rng.Font.Name=fname

