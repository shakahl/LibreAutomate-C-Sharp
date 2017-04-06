rep
	1
	int w=win("WordPad" "WordPadClass")
	int w1=win("Document - WordPad" "WordPadClass")
	int cid=id(59648 w) ;;editable text 'Rich Text Window'
	str text.getwintext(cid)
	 out text
	 if findrx(text "\bred\b") > -1
	RichEditHighlight(cid "\bred\b" 4 ColorFromRGB(255 255 255) ColorFromRGB(255 0 0))
	RichEditHighlight(cid "\bgreen\b" 4 ColorFromRGB(255 255 255) ColorFromRGB(0 255 0))
	