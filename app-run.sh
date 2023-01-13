#!/bin/bash
cd ./react-app 
npm run build
cd ..
rm -r -f ./web-server/usr/share/nginx/html/*
cp -a ./react-app/build/* ./web-server/usr/share/nginx/html
docker-compose up --build --remove-orphans