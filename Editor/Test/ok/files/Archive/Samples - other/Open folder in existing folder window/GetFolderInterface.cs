 /
function'SHDocVw.InternetExplorer hwnd

 Returns InternetExplorer interface of Windows Explorer window.


SHDocVw.ShellWindows sw._create
SHDocVw.InternetExplorer ie
foreach(ie sw)
	int h=ie.HWND; err continue
	if(h=hwnd) ret ie
