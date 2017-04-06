out
int ht=mac("Function268" "" 1)
 out SetHandleInformation(ht HANDLE_FLAG_PROTECT_FROM_CLOSE HANDLE_FLAG_PROTECT_FROM_CLOSE)
 out GetHandleInformation(ht &_i); out _i
 out SetHandleInformation(ht HANDLE_FLAG_PROTECT_FROM_CLOSE 0)
 out GetHandleInformation(ht &_i); out _i
out CloseHandle(ht)
