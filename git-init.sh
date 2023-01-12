#!/bin/bash
NAME="Arthur Klopov"
EMAIL="car3man@gmail.com"
DEFAULT_BRANCH="main"

git config --global user.name $NAME
git config --global user.email $EMAIL
git config --global init.defaultBranch $DEFAULT_BRANCH

ssh-keygen -o -t rsa -C $EMAIL

echo "Your ssh key:"
cat ~/.ssh/id_rsa.pub
