# Coco Work Tracker
This repository contains all related services to a website, "Coco Work Tracker", which concept is to enable tracking Coco-K's daily work progresses.
# Usage Restrictions
This repository is publicly available for viewing purposes only.

You may not copy, reproduce, modify, distribute, or use any part of this repository without prior written permission from the owner.

All rights reserved.
# Folder structure
## *frontend/*
This is the main web site part of this project. Get data from backend and show it as a HTML page. Also, it contains navigation to other descriptions about me.
## *supabase/ (backend)*
This is backend files which run in supabase. The important part is "migrations" because this represents structure of database writen by postgresql. Actual service where provide API feature to access the database is "Supabase", which means files in this repository is not something works independently.
## *recorder/ (maui application)*
This is not something external people can use. This MAUI application is for me to create history (I call this "record"). This works with private key.