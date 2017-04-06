function$ GUID&guid

 Creates GUID string from binary GUID.
 Returns: self.

 See also: <uuidof> (string to binary).
 Added in: QM 2.3.2.

 EXAMPLE
 _s.FromGUID(uuidof(Typelib.Class))


word* w
StringFromIID(&guid &w)
ansi(w)
CoTaskMemFree(w)
ret this
