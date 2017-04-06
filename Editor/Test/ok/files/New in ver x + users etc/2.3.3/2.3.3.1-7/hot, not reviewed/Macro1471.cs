 This function downloads image from mouse from deviantart.com.
 Recommended trigger: F12 or other keyboard trigger.
 How to use:
   Change saveToFolder (see below) to the real path of the folder where to save the images.
   Open "http://browse.deviantart.com" in Internet Explorer (not Firefox). It must show many small images.
   Move mouse to an image. Run this function (press the trigger key). Do it for each image that you need. Don't need to wait until the function ends.
   This function downloads large version of the image, and runs it.

str saveToFolder="$desktop$\deviantart" ;;change this

 _________________________

Acc aImg=acc(mouse) ;;<IMG>
aImg.Navigate("parent") ;;<A> that contains the <IMG>
Htm elImg=htm(aImg)
str s=elImg.HTML
 out s
 most thumbnails have direct url to the large image. Extract it.
str url
if(findrx(s "super_img=''(.+?)''" 0 1 url 1)<0)
	 this thumbnail does not have the url. Need to download page and get the url from there.
	if(findrx(s "href=''(.+?)''" 0 1 url 1)<0) end "failed"
	IntGetFile url s
	 out s
	if(findrx(s "<IMG\s[^>]+\sid=''gmi-ResViewSizer_img''[^>]+\ssrc=''(.+?)''" 0 1 url 1)<0) end "failed"
 out url
s.getfilename(url 1)
str sFile.from(saveToFolder "\" s)
mkdir saveToFolder
IntGetFile url sFile 16

 _________________________

 see what we have
run sFile ;;delete this line if you don't want to see the downloaded image
