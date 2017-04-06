
 Button code:
<form action="https://www.paypal.com/cgi-bin/webscr" method="post">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="hosted_button_id" value="BUY47ESZW2FXS">
<table>
<tr><td><input type="hidden" name="on0" value="License to (optional)">License to (optional)</td></tr><tr><td><input type="text" name="os0" maxlength="200"></td></tr>
</table>
<input type="image" src="https://www.paypalobjects.com/en_US/i/btn/btn_buynowCC_LG.gif" border="0" name="submit" alt="PayPal - The safer, easier way to pay online!">
<img alt="" border="0" src="https://www.paypalobjects.com/en_US/i/scr/pixel.gif" width="1" height="1">
</form>

 http://www.quickmacros.com/reg/pp_ipn.php receives POST variables (listed only useful):
test_ipn - 1 if the message is a test message
payment_status - must be "Completed"
first_name - Customer’s first name
last_name - Customer’s last name
option_selection1 - edit field text (License to). If field empty, variable exists and is empty.
quantity - Quantity as entered by your customer or as passed by you, the merchant.
payer_email - Customer’s primary email address. Use this email to provide any credits.
txn_type - transaction type. Must be "web_accept" (Payment received; source is a Buy Now, Donation, or Auction Smart Logos button)
item_name - Quick Macros
btn_id - undocumented, but probably button id (like 2551774)
txn_id - transaction id.
 these are missing:
payer_business_name - Customer’s company name, if customer is a business
memo - Memo as entered by your customer in PayPal Website Payments note field. Length: 255 characters

 _____________________________

 Sandbox (test)

 sandbox account
https://developer.paypal.com/
login using normal PayPal account

 seller account
Name: Gintaras Didzgalvis
Email: suppor_1342245342_biz@quickmacros.com
Password: new 1342245365, old 342245317

 buyer account
Name: Test Buyer
Email: test_1342245219_per@quickmacros.com
Password: 342245168

 My IPN script
http://www.quickmacros.com/reg/pp_ipn.php

 button code:
<form action="https://www.sandbox.paypal.com/cgi-bin/webscr" method="post">
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="hosted_button_id" value="TC7R8FHQLAVHW">
<table>
<tr><td><input type="hidden" name="on0" value="License to (optional)">License to (optional)</td></tr><tr><td><input type="text" name="os0" maxlength="200"></td></tr>
</table>
<input type="image" src="https://www.sandbox.paypal.com/en_US/i/btn/btn_buynowCC_LG.gif" border="0" name="submit" alt="PayPal - The safer, easier way to pay online!">
<img alt="" border="0" src="https://www.sandbox.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1">
</form>
