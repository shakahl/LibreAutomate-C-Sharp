out
Http h.Connect("http://www.quickmacros.com")
h.PostAdd("debug" "1")
h.PostAdd("test_ipn" "0")
h.PostAdd("payment_status" "Completed")
h.PostAdd("txn_id" "DEBUG")
h.PostAdd("quantity" "1")
h.PostAdd("payer_email" "qmgindi@gmail.com")
h.PostAdd("first_name" "Gintaras")
h.PostAdd("last_name" "Di")
h.PostAdd("option_selection1" "comp")
h.PostFormData("reg/pp_ipn.php" 0 _s)
out _s
