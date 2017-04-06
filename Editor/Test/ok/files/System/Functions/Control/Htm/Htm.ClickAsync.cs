
 Clicks the element and does not wait. Does not use the mouse.
 This function is asynchronous, ie does not wait while the page is processing the event.

 REMARKS
 The auto delay (spe) is applied.

 EXAMPLE
  clicks a link that displays a message box "VBScript", and closes the message box
 int w1=win("Internet Explorer" "IEFrame")
 act w1
 Htm el=htm("A" "msgbox" "" w1 0 0 0x21)
 el.ClickAsync
 int w2=wait(10 WV "VBScript")
 clo w2


if(!el) end ERR_INIT

if(CoMarshalInterThreadInterfaceInStream(uuidof(MSHTML.IHTMLElement) el +&_i)) end ERR_FAILED
__Handle ev=CreateEvent(0 0 0 0)
mac "sub.Thread" "" _i ev
wait 30 H ev; err end _error
wait -2


#sub Thread
function is ev
Htm el
CoGetInterfaceAndReleaseStream(+is uuidof(MSHTML.IHTMLElement) &el)
SetEvent ev
el.Click
err out "Htm.ClickAsync failed: %s" _error.description
