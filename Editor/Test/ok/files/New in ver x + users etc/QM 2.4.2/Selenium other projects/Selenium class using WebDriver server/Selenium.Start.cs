 /Selenium help
function $browser [$openURL] [flags] ;;flags: 1 start Selenium server if not running, 2 activate browser window, 4 call End when destroying variable

 browser - one of: "firefox", "chrome", "internet explorer", "htmlunit".
   Or it can be Capabilities JSON Object (string).

 REMARKS

 TODO: Selenium does not delete session when user closes browser window...


 str test=
  {"state":null,"sessionId":"fabf6076-bda8-4015-a917-8a8c10cdd01c","hCode":17163112,"empty":"","emptyArray":[],
  "value":{"applicationCacheEnabled":true,"rotatable":false,"handlesAlerts":true,"databaseEnabled":true,"version":"31.0","platform":"XP","nativeEvents":false,"acceptSslCerts":true,"webdriver.remote.sessionid":"fabf6076-bda8-4015-a917-8a8c10cdd01c","webStorageEnabled":true,"locationContextEnabled":true,"browserName":"firefox","takesScreenshot":true,"javascriptEnabled":true,"cssSelectorsEnabled":true},"class":"org.openqa.selenium.remote.Response","status":0}
  {"state":"success","sessionId":null,"hCode":22551911,"array":[{"capabilities":{["applicationCacheEnabled"]:true,"rotatable":false,"handlesAlerts":true,"databaseEnabled":true,"version":"31.0","platform":"XP","nativeEvents":false,"acceptSslCerts":true,"webStorageEnabled":true,"locationContextEnabled":true,"browserName":"firefox","takesScreenshot":true,"javascriptEnabled":true,"cssSelectorsEnabled":true},"id":"fe40523d-6f42-40b2-8fa6-a6c50599d458","hCode":24909012,"class":"org.openqa.selenium.remote.server.handler.GetAllSessions$SessionInfo"},{"capabilities":{"applicationCacheEnabled":true,"rotatable":false,"handlesAlerts":true,"databaseEnabled":true,"version":"31.0","platform":"XP","nativeEvents":false,"acceptSslCerts":true,"webStorageEnabled":true,"locationContextEnabled":true,"browserName":"firefox","takesScreenshot":true,"javascriptEnabled":true,"cssSelectorsEnabled":true},"id":"a9f25f08-0221-449f-a911-af72708b14da","hCode":8086370,"class":"org.openqa.selenium.remote.server.handler.GetAllSessions$SessionInfo"}],"class":"org.openqa.selenium.remote.Response","sta]tus":0}
 
 out _JsonGetValue(test "emptyArray" _s)
 out "%i  %s" _s _s
 
 
 #ret


opt noerrorshere 1
if(m_sessionId.len) end "Call End() with this variable."

m_flags=flags
if(flags&3) Selenium_FindServer(flags&3)

str sr cap
if(browser[0]='{') cap=browser
else
	cap.from("{''desiredCapabilities'':{" _JsonPairStr("browserName" browser 1))
	if(!StrCompare(browser "chrome")) cap+=",''chromeOptions'':{''args'':[''--test-type'']}"
	 cap+_JsonPair("takesScreenshot" "false" 1)) ;;ignores takesScreenshot (TODO: test, because was string instead of bool)
	cap+"}}"

_Post("/session" cap sr)
if(!_JsonGetValue(sr "sessionId" m_sessionId 1)) end sr

if(!empty(openURL)) urlOpen(openURL)
