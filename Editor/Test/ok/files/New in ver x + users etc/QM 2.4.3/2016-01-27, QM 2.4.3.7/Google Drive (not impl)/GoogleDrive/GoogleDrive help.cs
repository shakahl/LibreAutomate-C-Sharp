 Google Drive functions.
 At first call AuthX, then other functions.
 More functions will be added later, if somebody will need it.

 EXAMPLES

out
#compile "__GoogleDrive"
GoogleDrive x.AuthService("acc-939@testservice-1234.iam.gserviceaccount.com" "myAppSecret")
 out x.Upload("Q:\Test\test.cs" "/test.cs" "overwrite")
