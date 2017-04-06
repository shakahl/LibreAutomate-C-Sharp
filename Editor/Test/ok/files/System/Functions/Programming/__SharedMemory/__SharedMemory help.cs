 Creates or opens a shared memory block using the file mapping method.
 The memory can be used by multiple processes. One process creates, other processes open. All can write and read.
 For example, can be used to share data between processes of your QM-made exe(s).

 See also: <__ProcessMemory help>
 Added in: QM 2.3.5.

 EXAMPLES

 assume this code is running in process A
__SharedMemory sm1
byte* m1=sm1.Create("QM test" 4096)
m1[0]=5

 assume this code is running in process B
__SharedMemory sm2
byte* m2=sm2.Open("QM test")
out m2[0]
