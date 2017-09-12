#!/bin/bash
woaft="words after ->"
wobtw="words between ====> & <===="
if [[ "$1" == "" ]]; then
	echo Duplicates finder in Mahou dictionaries.
	echo Usage:
	echo dduf.sh [dictionary1] [dictionary2] [scantype] [threads] [no-out]
	echo scantype by default is:
	echo "0(or any) - [from dictionary1's {$wobtw} in dictionary2's $woaft]"
	echo other scantypes:
	echo "1 - [from dictionary1's {$woaft} in dictionary2's {$wobtw}]"
	echo "2 - [from dictionary1's {$woaft} in dictionary2's {$woaft}]"
	echo "3 - [from dictionary1's {$wobtw} in dictionary2's {$wobtw}]"
	echo threads is number of lines processed by script at time, default 4.
	echo no-out if not null, script won\'t print \"Scanning...\" messages.
else 
	scan() {
		if [[ "$1" != "" ]]; then 
			if [[ "$noout" == "0" ]]; then 
				echo Scanning: "$1 on thread $2"
			fi
			fix=`echo "$1" | sed -r 's/-/\\\\-/g'` # fix for -, it by any way(even in quotes) determined as grep's switch...
			vas=$(grep -x \"$ix\" "$tmp2")
			if [[ $? -eq 0 ]]; then
				echo -e "Duplicate: [$1]:\n\tfrom [$dict1]{$info1}\n\tin [$dict2]{$info2}:\n$vas" >> .duplicate
			else 
				echo -e "Exclusive: [$1]:\n\tfrom [$dict1]{$info1}\n\tin [$dict2]{$info2}:\n$vas" >> .exclusive
			fi
		fi
	}
	> .duplicate
	> .exclusive
	STARTTIME=$(date +%s)
	dict1="$1"
	dict2="$2"
	noout=0
	if [[ "$5" != "" ]]; then
		noout="$5"
	fi
	threads=4
	if [[ "$4" != "" ]]; then
		threads="$4"
	fi
	awk '/->/ { print $0" Line:" NR} ' "$dict1" | sed -re 's/->(.+)/\1/g' > .tmp1short
	awk '/====>/ { print $0" Line:" NR}' "$dict1" | sed -re 's/====>(.+)<====/\1/g' > .tmp1big
	awk '/->/ { print $0" Line:" NR}' "$dict2" | sed -re 's/->(.+)/\1/g' > .tmp2short
	awk '/====>/ { print $0" Line:" NR}' "$dict2" | sed -re 's/====>(.+)<====/\1/g' > .tmp2big
	tmp1=".tmp1big"
	tmp2=".tmp2short"
	info1="$wobtw"
	info2="$woaft"
	mode=0
	if [[ "$3" == "" ]]; then
		mode="$3"
	fi
	if [[ "$mode" == 1 ]]; then
		info1="$woaft"
		info2="$wobtw"
		tmp1=".tmp1short"
		tmp2=".tmp2big"
	elif [[ $mode == 2 ]]; then
		info1="$woaft"
		info2="$woaft"
		tmp1=".tmp1short"
		tmp2=".tmp2short"
	elif [[ $mode == 3 ]]; then
		info1="$wobtw"
		info2="$wobtw"
		tmp1=".tmp1big"
		tmp2=".tmp2big"
	fi
	exec 5<"$tmp1"
	while read l1 <&5; do
		scan "$l1" 1 &
		if [[ $threads != 1 ]]; then
			for p in $(seq 2 $threads); do
				l="l$p"
				read "$l" <&5
				scan "${!l}" "$p" &
			done
		fi
	done
	wait
	exec 5<&-
	rm .tmp1* .tmp2*
	ENDTIME=$(date +%s)
	echo "Done in $(($ENDTIME - $STARTTIME)) seconds..."
fi