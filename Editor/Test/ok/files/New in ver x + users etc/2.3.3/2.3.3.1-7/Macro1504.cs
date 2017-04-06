int col=GetSysColor(COLOR_BTNFACE)
word H L S
ColorRGBToHLS(col, &H, &L, &S);
out "%i %i" L S
