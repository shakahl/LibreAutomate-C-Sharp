 Why "byte takes up 4 places"?

byte* x=calloc(1 3)
out x[1]
out x[2]
out sizeof(byte) ;; NOTE: if byte takes up 4 places, how come this returns 1?