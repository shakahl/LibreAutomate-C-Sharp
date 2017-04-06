 /
function# nbAll nbRead str&s fparam

 Progress callback function for internet functions.
 Called repeatedly while downloading or uploading. Can be used to show progress.

 nbAll - file size, or -1 if unknown.
 nbRead - the number of bytes already downloaded or uploaded.
 s - variable that contains:
    When downloading to string - downloaded data.
    When uploading from string - all data that is being uploaded.
    When using file - file path.
 fparam - fparam that was passed to the function.

 Return: 0 continue, 1 cancel.

 Note: this function may run in other thread.


 <add your code here>
