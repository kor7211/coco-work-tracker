# Introduction of web part.
## Vite
I use vite to build and run web structure with logic writen in TypeScript. 
*./website/vite-project/* is the path where I develop the site. 
*./website/frontend/* is where actual site built. Cloudflare will publish this folder.
To run vite in loacl to test development, run this command in PowerShell.
```PowerShell
cd ./website/vite-project/
npm run dev
```
To exit
```PowerShell
Ctrl + c
```
To build site into /frontend/
```PowerShell
npm run build

git add -A
git commit -m "task: build website"
git push
```

## Folder structure and assets
### Fonts
#### English
- UI
Nunito-ValiableFont-wght.ttf