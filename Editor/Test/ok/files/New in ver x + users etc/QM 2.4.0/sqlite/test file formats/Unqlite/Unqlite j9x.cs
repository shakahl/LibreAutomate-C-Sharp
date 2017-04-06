str sf="$my qm$\test\test-doc.uql"

#compile "__Unqlite"
Unqlite x.Open(sf 0 0)

str code=
 /* Create the collection 'users'  */
  if( !db_exists('users') ){
     /* Try to create it */
    $rc = db_create('users');
    if ( !$rc ){
      //Handle error
       print db_errlog();
    return;
    }
 }
 //The following is the JSON objects to be stored shortly in our 'users' collection
 $zRec = [
 {
    name : 'james',
    age  : 27,
    mail : 'dude@example.com'
 },
 {
    name : 'robert',
    age  : 35,
    mail : 'rob@example.com'
 },
 {
    name : 'monji',
    age  : 47,
    mail : 'monji@example.com'
 },
 {
  name : 'barzini',
   age  : 52,
   mail : 'barz@mobster.com'
 }
 ];
 //Store our records
 $rc = db_store('users',$zRec);
 if( !$rc ){
 //Handle error
     print db_errlog();
   return;
 }
 //One more record
 $rc = db_store('users',{ name : 'alex', age : 19, mail : 'alex@example.com'  });
 if( !$rc ){
 //Handle error
     print db_errlog();
   return;
 }
 print "Total number of stored records: ",db_total_records('users'),JX9_EOL;
 //Fetch data using db_fetch_all(), db_fetch_by_id() and db_fetch().

UnqliteVm y.Compile(x code)
y.Exec


 out GetFileFragmentation(sf)
