 SAFEARRAY sa
 SAFEARRAY* psa=&sa
 ARRAY(VARIANT)* pa=+&psa


 ARRAY(int) _a.create(1)
 SAFEARRAY* psa=_a.psa
 ARRAY(int) a=+psa


 dll user32 [IsWindow]ArrArg ARRAY*a ;;same if eg ARRAY(BSTR)*a, because dl fun args don't know array type
 SAFEARRAY sa; SAFEARRAY* psa=&sa; ArrArg +&psa
 ARRAY(byte) ab; ArrArg &ab


 SAFEARRAY sa
 SAFEARRAY* psa=&sa; Function248 +&psa
  ARRAY(byte) ab; Function248 +&ab ;;error (correct)


 dll user32 [IsWindow]ArrArg2 ARRAY'a
 SAFEARRAY sa
  SAFEARRAY* psa=&sa; ArrArg2 psa
 ArrArg2 +&sa
  ARRAY(int) a; ArrArg2 a


 dll user32 [IsWindow]ArrArg3 ARRAY(BSTR)'a
 SAFEARRAY sa
 SAFEARRAY* psa=&sa; ArrArg3 psa
 ArrArg3 +&sa
 ARRAY(BSTR) a; ArrArg3 a
