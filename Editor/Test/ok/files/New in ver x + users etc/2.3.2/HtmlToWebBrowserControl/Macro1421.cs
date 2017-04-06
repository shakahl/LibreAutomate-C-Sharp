 parameters
str url="http://rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar"
str save_folder="$desktop$"

 -------------------

out

 IntGetFile url _s; out _s
 ret


HtmlDoc hd.InitFromWeb(url)
 out hd.GetHtml

str s url2 sd
hd.GetForm(0 url2 sd)
 out url2
out sd

