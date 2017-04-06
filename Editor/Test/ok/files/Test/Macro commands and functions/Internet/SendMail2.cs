str s sf.expandpath("$desktop$\c.eml")
SmtpMail sm
 sm.AddMessage("test@quickmacros.com" "" "" 0 "" "" "gindi@takas.lt" "" sf)
 sm.AddMessage("gindi@takas.lt" "" "" 0 "" "" "" "" sf)
 sm.AddMessage("gindi@takas.lt" "sub" "" 0 "" "support@quickmacros.com" "test@quickmacros.com" "Date:" sf)
sm.AddMessage("gindi@takas.lt" "sub" sf 2 "" "test@quickmacros.com" "support@quickmacros.com" "X-Mailer: Mailer[]Reply-To: test@quickmacros.com")
 sm.AddMessage("" "" "" 0 "$desktop$\a.eml" "" "" "" sf)
 int t1=GetTickCount
 sm.AddMessage2("to" "sub" "" 0 "" "cccc" "bcccc" "X-Test: hhhfdefd[]X-dsd: dsdfsd[]X-Test: hhhfdefd[]X-dsd: dsdfsd" sf)
 sm.AddMessage2("to" "sub" "" 0 "" "cccc" "bcccc" "X-Test: hhhfdefd[]X-dsd: dsdfsd[]X-Test: hhhfdefd[]X-dsd: dsdfsd" sf)
 int t2=GetTickCount
 sm.AddMessage("to" "sub" "" 0 "" "cccc" "bcccc" "X-Test: hhhfdefd[]X-dsd: dsdfsd[]X-Test: hhhfdefd[]X-dsd: dsdfsd" sf)
 sm.AddMessage("to" "sub" "" 0 "" "cccc" "bcccc" "X-Test: hhhfdefd[]X-dsd: dsdfsd[]X-Test: hhhfdefd[]X-dsd: dsdfsd" sf)
 int t3=GetTickCount
 out "%i %i" t2-t1 t3-t2
 sm.AddMessage("gindi@takas.lt" "" "" 0 "" "cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com," "cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com,cccc@aaaaaaa.com," "X-Test: long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; long header; " sf)

 sm.AddMessage("gindi@takas.lt" "" "" 0 "" "" "test@quickmacros.com")
 sm.Save("$desktop$")
 sm.SetSaveFolder("$desktop$")
sm.Send(0x100)
 sm.Send(0x30000)
 sm.Save("$desktop$")
