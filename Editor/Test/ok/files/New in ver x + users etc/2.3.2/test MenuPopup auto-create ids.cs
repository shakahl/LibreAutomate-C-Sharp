out

 MenuPopup m
 m.AddItems("one[]two[]-[]>sub[]one[]two[]<[]" 1)
 m.AddItems("three[]four" 20)
 out m.Show

out ShowMenu("one[]two[]-[]>sub[]one[]two[]<[]three[]four" 0 0 2)
