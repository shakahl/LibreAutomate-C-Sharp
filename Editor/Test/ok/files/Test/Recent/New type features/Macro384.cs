 type T1 a {b c} d
 type T2 a { b c } d
 type T3 a {b c[2]} d
 type T4 a {{b c}} d
 type T5 a { {b c} } d
 type T6 a { { b c } } d
 type T7 a {{ b c }} d
 type T8 a {b []c} d
 type T9 a {[]b []c} d
 type T10 a b []c d
 type T11 a {b [0]c} d
 type T12 !a {!b c} d
 type T13 !a !b c d
 type T14 a aa {[0]!b c} d
 type T15 !a {b !c} !d
 type T16 !a {[]b [4]!c} !d
 type T17 !a {[]b [+5]!c} !d
 type T18 ^a {b !c} !d
 type T19 ^a {b []c} ^d
 type T20 !a {b []c} !d
 type T21 a {b c} []d
 type T22 a {b} [0]c []d
 type T23 a {b} [0]c d

 T23 a
 out "%i %i %i %i   %i" &a.a-&a &a.b-&a &a.c-&a &a.d-&a sizeof(a)


 type B1 {a b}
 type B2 {a b
 type B3 {a b}}
 type B4 {a} b
 type B5 a {b}

 B5 a
 out "%i %i   %i" &a.a-&a &a.b-&a sizeof(a)


 type C1 {a b} []{c d}
 type C2 []!a {b []c} [8]!d
 type C3 []!a [4]{b []c} [8]!d
 
 C3 a
 out "%i %i %i %i   %i" &a.a-&a &a.b-&a &a.c-&a &a.d-&a sizeof(a)


type D1 {{a b} [0]{c d}}
type D2 {{a []{b} [0]{c d}} }
type D3 a {{[]{b} [0]{c d}} }

D3 a
out "%i %i %i %i   %i" &a.a-&a &a.b-&a &a.c-&a &a.d-&a sizeof(a)

type T1 byte'a int'b
T1 a
out "%i %i   %i" &a.a-&a &a.b-&a sizeof(a)
















 type TTT a (b c[2]) d
 type TTT a [4](b c[2]) d
 type TTT a [4]type(b c[2]) d
 type TTT a [b c] d
 type TTT a [8][b c] d
 type TTT a b[5]c
 type TT a (b c) d
 type TT a type(b c) d
