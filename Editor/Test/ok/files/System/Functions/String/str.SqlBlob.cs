function$ !*data nBytes

 Formats hex string from binary data for use in SQL (a BLOB column in a database table).
 Returns: self.

 data, nBytes - pointer to binary data, and its size.

 REMARKS
 Creates string like x'XXXXXXXX'. It is used in Sqlite. Other database types may use different format.
 If data is 0, the string will be "null".

 Added in: QM 2.4.0.


if(!data) this="null"; ret this
str s.encrypt(8 _s.fromn(data nBytes))
ret this.from("x'" s "'")
