type N !a {@b c !d} !e [pack1]
 type N !a [2]{@b c !d} !e [pack1]
N x

out "%i %i %i %i %i" sizeof(x)  &x.b-&x  &x.c-&x  &x.d-&x  &x.e-&x
