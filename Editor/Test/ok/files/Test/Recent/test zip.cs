  test normal
 zip "G:\test.zip" "G:\kompiuteris-gd.xls"
 zip- "G:\test.zip" "G:\kompiuteris-gd-2.xls"

  test when disk is almost full, and also when completely full
 zip "G:\test.zip" "G:\Copy (2) of ok.qml"
 zip- "G:\mysql-5.0.21-win32.zip" "G:\Unzip"

  test end thread while zipping
 zip "f:\test.zip" "f:\Software"
 zip- "F:\test.zip" "f:\test"
