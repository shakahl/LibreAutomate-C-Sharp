function $_file

 Initializes the variable with HTML from a file.

 _file - file containing HTML.
   If called SetOptions with flag 2 or 1, can be a special string supported by Internet Explorer. For example, can load a topic page from a chm file.

 REMARKS
 By default, gets file data with str.getfile and calls InitFromText.
 If called SetOptions with flag 2 or 1, instead creates hidden web browser control and calls its Navigate function.


if m_flags&3
	CreateDocument(_s.expandpath(_file))
else
	_s.getfile(_file)
	InitFromText(_s)

err+ end _error
