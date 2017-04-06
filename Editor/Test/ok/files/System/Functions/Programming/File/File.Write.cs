function !*buffer [count] [itemsize]

 Writes data to the file.

 buffer - data source. Can be string, or address of a variable, or address of first element of array, or pointer to binary data.
 count, itemsize - writes count*itemsize bytes.
   If buffer is string, count can be omitted or 0, then writes whole string.
   Default itemsize is 1 byte.

 REMARKS
 Writes starting from current file pointer, and updates it.


if(!m_file) end ERR_INIT

if(!buffer) end ERR_BADARG
if(itemsize<1) itemsize=1
if(count<1) lpstr ls=+buffer; count=len(ls)
if(fwrite(buffer itemsize count m_file)<count) end ERR_FAILED
