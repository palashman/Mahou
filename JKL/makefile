name=jkl
gcc32=gcc -m32
dllflg=$(INDIR) -s -Os -shared -o
exeflg=$(INDIR) -s -Os -mwindows -o
all: ./bin/ ./obj/ objx64 objx86 dllDEL
	gcc ./obj/$(name).o $(dllflg) ./bin/$(name).dll
	$(gcc32) ./obj/$(name)x86.o $(dllflg) ./bin/$(name)x86.dll
	gcc ./obj/$(name).exe.o $(exeflg) ./bin/$(name).exe
	$(gcc32) ./obj/$(name)x86.exe.o -lpthread $(exeflg) ./bin/$(name)x86.exe
	
x86o: ./obj/ dllDEL
	$(gcc32) $(name).cxx -c $(dllflg) ./obj/$(name).o
	$(gcc32) $(name).c -c -D ARCH $(exeflg) ./obj/$(name).exe.o
	$(gcc32) ./obj/$(name).o $(dllflg) $(name).dll
	$(gcc32) ./obj/$(name).exe.o $(exeflg) $(name).exe
	
x64o: ./obj/ objx64 dllDEL
	gcc ./obj/$(name).o $(dllflg) ./bin/$(name).dll
	gcc ./obj/$(name).exe.o $(exeflg) ./bin/$(name).exe

objx86:
	$(gcc32) ./$(name).cxx -c $(dllflg) ./obj/$(name)x86.o
	$(gcc32) ./$(name)x86.c -c -lpthread $(exeflg) ./obj/$(name)x86.exe.o

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