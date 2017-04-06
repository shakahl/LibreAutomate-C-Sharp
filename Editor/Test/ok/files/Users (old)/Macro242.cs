 Call Shell("RUNDLL32.EXE URL.DLL,FileProtocolHandler " & "your kml/kmz file path", vbNormalFocus)

str cl.from("URL.DLL,FileProtocolHandler " "your kml/kmz file path")
run "RUNDLL32.EXE" cl

 does not work
 dll url.dll FileProtocolHandler $file_
 FileProtocolHandler "http://www.google.com/"
