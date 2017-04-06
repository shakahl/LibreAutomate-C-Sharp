/exe 1
 str url="http://www.quickmacros.com"
str url="http://www.google.com"
 str url.expandpath("$desktop$\test.htm")

 web "" 8
 web url 1
 web url 9
 web url 2
 web url 4 ;;opens inactive and behind existing window
web url 5 ;;ok
 SHDocVw.IWebBrowser2 b=web(url 4)
 web url 4 "" "" 0 _i
 web url 12|1

 web url 1 win("Internet Explorer")
 web url 5 win("Internet Explorer")
 web url 12 win("Internet Explorer")

 SHDocVw.IWebBrowser2 b=web("")
 out b.LocationURL

 int h
  web url 0 "" "" 0 h
 web url 32 "" "" 0 h
 zw h

 web url 0 "" "" _s; out _s
 web url 1 "" "" _s; out _s

 SHDocVw.IWebBrowser2 b=web(url)
 b.Navigate("www.google.com")

 web url 128|8
 web("" 8|128)

 web url 4
 web url 4|64
 web url 12
 web url 12|64

 web url

 web url 0 "" "www.quickmacros.com"
 web url 1|16 "" "*www.quickmacros.com*"

 BEGIN PROJECT
 main_function  Macro403
 exe_file  $my qm$\Macro403.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {470602AE-9BD1-48F8-AED6-ADC4DC27409D}
 END PROJECT
