 /exe 1

 This macro is not completely reliable. Run it several times until it will not find more images. Then review the document.
 I did not find a better way to identify images. Need to use scan. But can use Word COM interfaces to select images.
 At first need to capture the image:
   1. Select one of the images in Word.
   2. In QM floating toolbar find dialog "Find image", and capture the image in Word. The image must be in selected state.
   3. Save as "word_image_to_delete.bmp".
   4. On OK the dialog will insert 2 lines of code in current macro. Don't need them, remove.
   5. Run this macro.
   6. If error in line 'typelib Word': Open menu -> Tools -> Type Libraties. Find Microsoft Word xxx Object Library. Create declaration and replace the typelib line in this macro with the new declaration.

 find and activate Word window
int w=win("Word" "OpusApp")
act w
int c=child("Microsoft Word Document" "_WwG" w)
 connect to Word COM interface
typelib Word {00020905-0000-0000-C000-000000000046} 8.3 1
Word.Application a._getactive
 get graphic objects
ARRAY(Word.InlineShape) b
Word.InlineShape s
foreach(s a.ActiveDocument.InlineShapes)
	s.Select ;;make to display the object now, it makes more reliable later
	b[]=s
wait 0.5
 repeat for each object, starting from end
int i n k
for i b.len-1 -1 -1
	b[i].Select ;;select the object
	 find image captured with dialog "Find image".
	 It must be captured in selected state.
	 If not found, it is other image.
	if scan("word_image_to_delete.bmp" c 0 16)
		 select-delete line
		spe 10 ;;make faster
		key SE X       ;; Shift+End Delete
		n+1

OnScreenDisplay F"Deleted {n} images" 3 0 0 "" 0 0 2|4

 BEGIN PROJECT
;
 END PROJECT
