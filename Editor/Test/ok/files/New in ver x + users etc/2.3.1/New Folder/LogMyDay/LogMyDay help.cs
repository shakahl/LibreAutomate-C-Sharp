 Writes your time spent in each program to a MS Access database (mdb file).
 Creates new table each day. Like "04_05_2009", "05_05_2009", ...
 Issues:
    When day changes, writes new previous program's time span to new day's table (actually not tested).
    Writes time spent in hibernation or when you are away (actually not tested).
    Currently there is no Pause function.
    Logs only program exe names. Does not log friendly names and window titles.
    And maybe more.
    Don't know is it bad or good, but I did not use Excel because it is easier for me to use database functions but with Excel I don't know how to add tables (sheets).
    For all these reasons, it is almost not usable. Use it as an example. Extend it.
 Extending:
    MS Jet SQL knowledge probably will be needed. You can find the reference in MS Access help, if installed.

 EXAMPLE

 Before using this code, change the file path/name. The file must exist. Create it in MS Access, can be empty or not.
 For testing, run this macro. Press Pause to stop.
 Later:
   Place all this code in a function (name not important).
   Change 3 to 0. It will not show debug info and will not drop today's table when started.
   Run the function.
   Assign 'QM file loaded' trigger (not sync).
#compile "__LogMyDay"
LogMyDay x
x.Start("$personal$\log_my_day.mdb" 3)
