function#

 Gets file size. Obsolete.
 May return incorrect value.
 Use <help>File.FileSize</help>.


if(!m_file) end ERR_INIT

ret _filelength(_fileno(m_file))
