 /
function IDispatch'pDisp VARIANT&URL SHDocVw.WebBrowser'c

 out URL

int- t_hdlg
str s=c.LocationURL
s.setwintext(id(4 t_hdlg))
