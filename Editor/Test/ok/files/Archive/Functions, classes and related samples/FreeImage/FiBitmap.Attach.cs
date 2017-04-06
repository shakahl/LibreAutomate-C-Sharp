function FIMG.FIBITMAP*fib

 Sets this variable to manage FIBITMAP object (auto-delete etc).

 REMARKS
 Many FreeImage API functions create new FIBITMAP and return FIBITMAP*. You can attach it to a FiImage variable, if want to auto-delete or call FiBitmap functions.


Delete
b=fib
