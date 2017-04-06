 parameters
str url="http://rapidshare.com/files/19870643/Electrical.Engineering.Dictionary.PA.Laplante_CRC.rar"
str save_folder="$desktop$"

 -------------------

 download first page
HtmlDoc hd.InitFromWeb(url)

 get URL of second page
str s url2 sd
hd.GetForm(0 url2 sd)
 out url2
 out sd

 download second page
IntPost url2 "dl.start=Free" s
 out s ;;html of second page

 extract direct download link
str rx=
 <form name="dlf" action="(.+?)"
str url3
if(findrx(s rx 0 0 url3 1)<0) end "failed"
 out url3

 wait (because error if does not wait) and download
40
IntPost url3 "mirror=" s
 out s.len
 out s ;;file data if ok, or html error page if error

 save
str filename.getfilename(url3 1)
str save.from(save_folder "\" filename)
s.setfile(save)
