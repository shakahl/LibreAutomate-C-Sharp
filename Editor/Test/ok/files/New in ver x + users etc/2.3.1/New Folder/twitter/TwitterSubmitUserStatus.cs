 /Macro948
function $username $password $tweet

str user.encrypt(4 _s.from(username ":" password) "" 3)
str status.from("status=" tweet)
Http h.Connect("twitter.com")
str resp
h.Post("statuses/update.xml" status resp _s.from("Authorization: Basic " user))
out resp
err+ end _error


 str user.encrypt(4 _s.from(username ":" password) "" 3)
 str status.from("status=" tweet)
 Http h.Connect("twitter.com" "" "" 443)
  Http h.Connect("twitter.com")
 h.Post("status/update" status 0 _s.from("Authorization: Basic " user))
 err+ end _error
