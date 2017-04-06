function func [param]

 Sets <help "::/Other/IDP_ENUMWIN.html">callback function</help> for WaitX functions.

 func - callback function address.
 param - some value to pass to the callback function.

 REMARKS
 The callback function not called for AddMsgBox windows.


m_func=func
m_param=iif(func param 0)
