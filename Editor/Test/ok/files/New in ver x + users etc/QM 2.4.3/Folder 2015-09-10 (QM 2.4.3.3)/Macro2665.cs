 act "Word"
 outx pixel(286 40 "Word" 0x1001)
 outx pixel(286 40 "Word" 1)
 outx pixel(286 40 "Word" 1|2)
wait(0 C 0x40C8F0 286 40 "Word" 1|0)
 wait(0 C 0x40C8F0 286 40 "Word" 1|2)
 wait(0 C 0x40C8F0 286 40 "Word" 1|0x1000)


 #region Recorded 2015-08-30 08:17:11
 int w1=win("Untitled - Paint" "MSPaintApp")
  act w1
  lef 15 16 child("" "Afx:dd0000:8" w1) 1
 lef 150 160 w1 1
 #endregion
