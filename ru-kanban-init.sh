#!/bin/bash
REP_REMOTE_URL="git@github.com:Car3man/ruKanban.git"
REP_LOCAL_PATH="./ruKanban"

if [[ -d $REP_LOCAL_PATH ]]
then
	echo $REP_LOCAL_PATH "folder exists, please remove this dir before execute script"
	exit
fi

git clone git@github.com:Car3man/ruKanban.git

mkdir volumes
mkdir volumes/db