 /hook ToolbarExProc_TWWB /siz 500 300 /set 2
 Back :web "Back" 0 TriggerWindow **
 Back :SHDocVw.WebBrowser b=GetTbWebBrowser; out b; if(b) b.GoBack; err out _error.description **
Back :TbWebBrowserBack **
www.quickmacros.com :OpenInTbWebBrowser "http://www.quickmacros.com" * web.ico
Book1.xls :OpenInTbWebBrowser "$personal$\Book1.xls" * $personal$\Book1.xls
