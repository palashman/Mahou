name=jkl
dllflg=-s -Os -shared -o
exeflg=-s -Os -mwindows -o
all: ./bin/ ./obj/ objx64 objx86 dllDEL
	gcc ./obj/$(name).o $(dllflg) ./bin/$(name).dll
	gcc ./obj/$(name)x86.o -m32 $(dllflg) ./bin/$(name)x86.dll
	gcc ./obj/$(name).exe.o $(exeflg) ./bin/$(name).exe
	gcc ./obj/$(name)x86.exe.o -m32 $(exeflg) ./bin/$(name)x86.exe
	
x86o: ./obj/ dllDEL
	gcc $(name).cxx -c -m32 $(dllflg) ./obj/$(name).o
	gcc $(name).c -c -D ARCH -m32 $(exeflg) ./obj/$(name).exe.o
	gcc ./obj/$(name).o -m32 $(dllflg) $(name).dll
	gcc ./obj/$(name).exe.o -m32 $(exeflg) $(name).exe
	
x64o: ./obj/ objx64 dllDEL
	gcc ./obj/$(name).o $(dllflg) ./bin/$(name).dll
	gcc ./obj/$(name).exe.o $(exeflg) ./bin/$(name).exe

objx86:
	gcc ./$(name).cxx -c -m32 $(dllflg) ./obj/$(name)x86.o
	gcc ./$(name)x86.c -c -m32 $(exeflg) ./obj/$(name)x86.exe.o

objx64:
	gcc ./$(name).cxx -c $(dllflg) ./obj/$(name).o
	gcc ./$(name).c -c $(exeflg) ./obj/$(name).exe.o
	
./obj/:
	+@mkdir -p ./obj

./bin/:
	+@mkdir -p ./bin/

clean: dllDEL
	-@rm -f ./bin/$(name)*.exe || true
	-@rm -rf ./obj ./bin || true

dllDEL:
	-@rm -f ./bin/$(name)*.dll || true

re: clean all

rer: re run

zip: 
	upx -9 ./bin/*

run:
	./bin/$(name).exe