function$

 Replaces all ' characters with ''.
 Returns: self.

 REMARKS
 In SQL (database query), strings must be enclosed in ' ' and must not contain ' characters. As escape sequence for ' is used ''.

 Added in: QM 2.3.2.


this.findreplace("'" "[39]'")
ret this
