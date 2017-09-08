' Builds AS_dict.txt from every file in Dictionaries-origin folder.
' P.S. Do not modify the AS_dict.txt for pull-request, modify only the ones in Dictionaries-origin folder.
set fso = CreateObject("Scripting.FileSystemObject")
dim AS_DICT
for each dict in fso.GetFolder("Dictionaries-origin").Files
  AS_DICT = AS_DICT & dict.OpenAsTextStream(1,-2).ReadAll() & vbCcrLf
next
set asd = fso.CreateTextFile("AS_dict.txt", true)
asd.Write(AS_DICT)