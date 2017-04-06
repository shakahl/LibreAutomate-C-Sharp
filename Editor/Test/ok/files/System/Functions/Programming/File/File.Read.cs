function !*buffer count itemsize

 Reads data from the file.

 buffer - destination. Can be str variable, or address of a variable, address of first element of array, etc.
 count, itemsize - reads count*itemsize bytes.

 REMARKS
 Reads starting from current file pointer, and updates it.


if(!m_file) end ERR_INIT

if(!buffer or count<1 or itemsize<1) end ERR_BADARG
if(fread(buffer itemsize count m_file)<count)
	if(feof(m_file)) end F"{ERR_FAILED}. EOF"
	end _s.dllerror(ERR_FAILED "C")
