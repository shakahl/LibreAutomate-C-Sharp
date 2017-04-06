 This macro downloads index.html from www.quickmacros.com,
 and saves it in My Documents. Filename is formatted from day,
 month and original filename.

 download file to variable s
str s
IntGetFile "http://www.quickmacros.com/index.html" s
 format filename
str fn.time("\%d-%m - index.html")
 prepend folder path (to save to other folder, just prepend full path instead, eg s-"s:\my files")
str folder.expandpath("$personal$")
fn-folder
 save
s.setfile(fn)
 report
out "index.html downloaded and saved to %s" fn

 If password is required, instead of IntGetFile you can use
 Http h.Connect("www.quickmacros.com" "username" "password")
 h.FileGet("index.html" s)

 To insert time and out, use the Text dialog.
 For other functions, dialogs are not available.

 You can create a scheduled task to run this macro every day.
 Click Schedule in Properties, click OK, and set schedule.
 Also, in Properties you should change Priority to Wait or
 Run Simultaneously.
