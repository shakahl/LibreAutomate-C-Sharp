 find and activate Word window
int w=win("Word" "OpusApp")
act w
 scroll to top
key CH          ;; Ctrl+Home
0.5
 repeat
rep
	 1
	 find image captured with dialog "Find image"
	scan "Macro2098.bmp" child("Microsoft Word Document" "_WwG" w) 0 1|2|16 ;; 'Microsoft Word Document'
	err ;;no more images in this page
		
		 end macro if this is the bottom of the document
		Acc a.Find(w "PUSHBUTTON" "Page down" "class=ScrollBar" 0x1025)
		err break
		
		 scroll to next page and continue 
		key Q           ;; Page_Down
		continue
	
	 click and select-delete line
	lef
	key SE X       ;; Shift+End Delete
