MSScript.ScriptControl k.


str js=
 function Test(){
   var s;
 var jsontext = '{"firstname":"Jesper","surname":"Aaberg","phone":["555-0100","555-0120"]}';
 var contact = JSON.parse(jsontext);
 var fullname = contact.surname + ", " + contact.firstname;
   return(fullname);
 }
JsAddCode(js)
out JsEval("Test();")
