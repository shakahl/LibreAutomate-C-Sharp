function $email $etc

 email - the generated email address of the service account.



str jwtHeader="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9"

DateTime _t.FromComputerTime(1); int ti; RtlTimeToSecondsSince1970(+&_t &ti)
str jwtClaim=
F
 {{
  "iss":"{email}",
  "scope":"https://www.googleapis.com/auth/prediction",
  "aud":"https://www.googleapis.com/oauth2/v4/token",
  "exp":{ti+3600},
  "iat":{ti}
 }

str jwtSig=
 I don't know how to sign

str jwt=F"{jwtHeader}.{jwtClaim.encrypt(4)}.{jwtSig.encrypt(4)}"
