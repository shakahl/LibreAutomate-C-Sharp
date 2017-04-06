function fa [fparam]

 Sets progress callback function.
 All functions that support <help "__Wininet.SetProgressDialog">progress dialog</help> also support callback function.

 fa - address of <help "Callback_internet_progress">callback function</help>.
   A template is available in menu -> File -> New -> Templates.
 fparam - some value to pass to the callback function.

 REMARKS
 With PostFormData, the callback function is called only when downloading response, not when uploading files. For files you can use another callback function.

 Added in: QM 2.3.2.


m_fa=fa
m_fparam=fparam
