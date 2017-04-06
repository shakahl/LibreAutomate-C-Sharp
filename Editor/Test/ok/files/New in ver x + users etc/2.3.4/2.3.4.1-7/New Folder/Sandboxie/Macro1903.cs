_s.expandpath("$desktop$\testhk.dll")
RunConsole2 F"upx.exe --compress-exports=0 ''{_s}''"