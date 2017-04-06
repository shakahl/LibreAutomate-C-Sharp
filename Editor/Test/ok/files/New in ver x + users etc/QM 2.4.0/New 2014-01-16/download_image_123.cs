 allow single instance
if(getopt(nthreads)>1) end "The thread is already running. You can end it in the 'Running items' pane."

str sf sd ;;variables

 download image data into variable sd
int downloaded
rep 60 ;;repeat max 10 minutes every 10 s until success
	IntGetFile "http://image.weather.com/images/maps/forecast/precfcst_600x405.jpg" sd
	err 10; continue ;;if error, retry after 10 s
	downloaded=1
	break
if(!downloaded) end "failed to download"

 format filename and save to file
sf.timeformat("$my qm$\Image-{yyyy-MM-dd}.jpg")
sd.setfile(sf)

run sf ;;open the image. Delete this line if don't need.
