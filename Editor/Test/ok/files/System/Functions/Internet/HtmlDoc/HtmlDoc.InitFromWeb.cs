function $URL

 Initializes the variable from a web page.
 Downloads the page from the Internet.

 URL - web page address.

 REMARKS
 By default, downloads page with IntGetFile and calls InitFromText.
 If called SetOptions with flag 2 or 1, instead creates hidden web browser control and calls its Navigate function.


if m_flags&3
	CreateDocument(URL)
else
	IntGetFile URL _s
	InitFromText(_s)

err+ end _error
