function str&buffer [nbytes]

 Reads string or other data from the file into a str variable.

 buffer - variable that receives data.
 nbytes - number of bytes to read. If omitted or -1, reads to the end of file.

 REMARKS
 Reads starting from current file pointer, and updates it.


if(!m_file) end ERR_INIT

if(getopt(nargs)<2 or nbytes<0) nbytes=FileLen-GetPos; if(nbytes<1) end F"{ERR_FAILED}. EOF"
else if(!nbytes) buffer.all; ret
buffer.all(nbytes 2)
int r=fread(buffer 1 nbytes m_file)
if(r<nbytes)
	buffer.fix(r)
	if(feof(m_file)) end F"{ERR_FAILED}. EOF"
	end _s.dllerror(ERR_FAILED "C")
