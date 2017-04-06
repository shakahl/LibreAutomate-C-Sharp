out
Http h.Connect("www.quickmacros.com")
h.PostAdd("transactioncode" "test-bdj-1")
h.PostAdd("quantity" "1")
h.PostAdd("email" "support@quickmacros.com")
h.PostAdd("firstname" "G")
h.PostAdd("lastname" "Didžgalvis")
h.PostAdd("debug_499273godhy36h" "1")
h.PostFormData("reg/bitsdujour.php" 0 _s)
out _s

