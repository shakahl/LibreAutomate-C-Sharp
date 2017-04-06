str save_location="$temp$\temp_filename.tmp"
str download="http://www.quickmacros.com/test/test.php"
str headers
IntGetFile download save_location 16 0 1 0 0 headers
 out headers
str fn
if(findrx(headers "(?mi)^Content-Disposition:.*\bfilename *= *([^;[]]+)" 0 0 fn 1)<0) out "filename unknown"; ret
 out fn
fn-_s.getpath(save_location)
 out fn
FileMove save_location fn 2
 run fn


 Http h.Connect("www.quickmacros.com")
 h