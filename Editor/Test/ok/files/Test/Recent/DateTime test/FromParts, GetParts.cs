DateTime x
x.FromParts(2000 2 3 4 5 6 7 999.9)
out x.ToStr(8)
int Y M D h m s ms wd; double mcs
x.GetParts(Y M D h m s ms mcs wd)
out "%i %i %i %i %i %i %i %g %i" Y M D h m s ms mcs wd
