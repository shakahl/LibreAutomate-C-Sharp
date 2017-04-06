 stdole.Picture p
stdole.IPictureDisp p
str s.expandpath("$qm$\bitmap1.bmp")
str s2.expandpath("$desktop$\bm2.bmp")
 VARIANT v=s
out stdole.LoadPicture(s 0 0 0 &p) ;;was exception if passes str, but works when VARIANT. Now ok.
out p
out stdole.SavePicture(p s2)
