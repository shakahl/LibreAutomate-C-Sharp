All bugs fixed.

 ----------------------

After craeting console exe, cannot create normal gui exe.
   Reason: global variable pe_header is changed when compiling console.
   Quick fix: Use const template. Before writing exe, copy it to variable.
