/exe 4
 out
 IntGetFile "http://quickmacros.com/forum/index.php" _s
Http h.Connect("quickmacros.com")
str headers
 h.Get2("/forum/index.php" _s 0 INTERNET_FLAG_NO_COOKIES 0 headers)
h.Get("/forum/index.php" _s)
 out _s
 out _s.len
 out findw(_s "Login")
out findw(_s "Logout")


 BEGIN PROJECT
 main_function  Macro2080
 exe_file  $my qm$\Macro2080.qmm
 flags  6
 guid  {FEB53220-C8D8-4D08-A1D9-AB5B93BB2DCF}
 END PROJECT
