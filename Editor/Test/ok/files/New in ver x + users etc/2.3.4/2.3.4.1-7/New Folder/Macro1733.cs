int i
str s
for i 0 100
	out F"str+ g_A{i} g_B{i} g_C{i} g_D{i} g_E{i} g_F{i} g_G{i} g_H{i} g_I{i} g_J{i}"
	out F"g_A{i}.getsel; g_B{i}.getsel; g_C{i}.getsel; g_D{i}.getsel; g_E{i}.getsel; g_F{i}.getsel; g_G{i}.getsel; g_H{i}.getsel; g_I{i}.getsel; g_J{i}.getsel;"

out "[][]"
for i 0 100
	out F"out g_A{i}; out g_B{i}; out g_C{i}; out g_D{i}; out g_E{i}; out g_F{i}; out g_G{i}; out g_H{i}; out g_I{i}; out g_J{i}"
