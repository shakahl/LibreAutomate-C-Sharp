function hwnd idObject idChild
Acc a.ObjectFromEvent(hwnd idObject idChild)
 a.State(_s); out _s
if a.State&STATE_SYSTEM_BUSY
	out "started loading a page"
else
	str name url
	name=a.Name
	url=a.Value
	out F"loaded:[][9]name=''{name}''[][9]url=''{url}''"

