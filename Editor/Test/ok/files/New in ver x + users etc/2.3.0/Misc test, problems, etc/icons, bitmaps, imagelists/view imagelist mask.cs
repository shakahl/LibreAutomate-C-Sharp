str bmp="$qm$\il_qm.bmp"

 -----------------

type BITMAPFILEHEADER []@bfType [2]bfSize [+4]@bfReserved1 [+2]@bfReserved2 [+2]bfOffBits

str s.getfile(bmp)
BITMAPFILEHEADER* f=+s
 out f.bfSize
s.get(s f.bfSize)
bmp.insert("_mask" bmp.len-4)
s.setfile(bmp)
run bmp
