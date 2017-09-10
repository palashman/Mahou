cd Dictionaries-origin
if [[ "$1" == "" ]]; then
	echo Usage: /build-AS_dict.sh {type} {type} {type} 
	echo -e "Example: \n\t/build-AS_dict.sh big custom\nwill create dictionary with big and custom dictionaries."
	echo If pass {type} as all it will combine all dictionaries to AS_dict.txt.
	echo -e "Possible {type}s(left to filename):\n"
else
	> ../AS_dict.t # temp
	iconv -c -f utf-8 -t utf-8 ../AS_dict.t
	file -i AS_dict.t
fi
all=0
changed=0
if [[ "${1,,}" == "all" ]] || [[ "${2,,}" == "all" ]] || [[ "${3,,}" == "all" ]]; then
	all=1
fi
for f in *.txt; do
	if [[ "$f" == *"big"*  ]]; then
		if [[ "${1,,}" == "big" ]] || [[ "${2,,}" == "big" ]] || [[ "${3,,}" == "big" ]] || [[ $all == 1 ]];  then
			echo Adding $f to dict...
			changed=1
			cat $f >> ../AS_dict.t
		else
			echo BIG: $f
		fi
	elif [[ "$f" == *!* ]]; then
		if [[ "${1,,}" == "tiny" ]] || [[ "${2,,}" == "tiny" ]] || [[ "${3,,}" == "tiny" ]] || [[ $all == 1 ]]; then
			echo Adding $f to dict...
			changed=1
			cat $f >> ../AS_dict.t
		else
			echo tiny: $f
		fi
	else
		if [[ "${1,,}" == "custom" ]] || [[ "${2,,}" == "custom" ]] || [[ "${3,,}" == "custom" ]] || [[ $all == 1 ]]; then
			echo Adding $f to dict...
			changed=1
			cat $f >> ../AS_dict.t
		else
			echo Custom: $f
		fi
	fi
done
if [[ $changed == 1 ]]; then
	cd ..
	# echo Converting to utf-8...
	# iconv -f `file -i AS_dict.t | awk 'match($0,".*charset=(.*)", a) {print a[1]}'` -t utf-8 AS_dict.t > AS_dict.txt
	# mv AS_dict.t AS_dict.txt
fi