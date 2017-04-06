 /exe
out
 2
rep
	out "inicio-uma"
	HtmlDoc dBRA
	dBRA.SetOptions(2)
	dBRA.InitFromWeb("http://br.weather.com/agora/BRXX0043:1:BR") ;;leak. No leak with other pages. Also leaks in normal web browser control in dialog, even on Refresh.
	 dBRA.InitFromWeb("http://www.yahoo.com")
	 dBRA.InitFromWeb("http://www.quickmacros.com/index.html")
	 dBRA.InitFromWeb("http://www.quickmacros.com/forum/viewtopic.php?f=4&t=5514")
	 dBRA.InitFromWeb("http://www.lrt.lt/mediateka/laidos")
	 dBRA.InitFromWeb("http://www.youtube.com/")
	 dBRA.InitFromWeb("http://www.last.fm/user/qgindi")
	 1
	0.1

 BEGIN PROJECT
 main_function  Macro2062
 exe_file  $my qm$\Macro2062.qmm
 flags  6
 guid  {C6523C5B-8B7C-457F-BBB7-01DF87FFA6C2}
 END PROJECT
