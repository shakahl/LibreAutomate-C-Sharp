 /
function# action nbAll nbWritten $_file nbAllF nbWrittenF fparam

 Callback function for Http.PostFormData.
 Called repeatedly. Can be used to show progress.
 Not called while downloading response. If need, use SetProgressCallback to set other function for it.

 action: 0 before posting, 1 sending file, 2 all data sent, 3 starting to receive response, 4 finished.
 nbAll - total size of data being uploaded.
 nbWritten - size of already uploaded part of total.
 _file - current file.
 nbAllF - size of current file.
 nbWrittenF - size of already uploaded part of current file.

 Return: 0 continue, 1 cancel.
 Example: search in forum.


 <add your code here>
