# Supabase CLI-Migration-based database usage and sequences.
## 1.Create / Edit SQL file.
### Edit existing migration
If you are fine with resetting local existing database, you can edit prexisting migration file.
### Create new migration
Run this command on powershell with replacing file name.
```
npx supabase migration new YOUR_FILE_NAME
```
New sql file will be created coco-work-tracking/supabase/*. File name starts with auto-generated timestamp.
Eg: coco-work-tracking/supabase/20261010120000_YOUR_FILE_NAME.sql
## 2. Write SQL.
### Syntax
Follow PostgreSql syntax.
### What can be in one migration file?
Such as all related tables creation can be written in one migration file. Not like one query per one file.
## 3. Test in local.  
### Ready Docker Desktop
Turn on Docker Desktop.
Run this command on powershell.
```
npx supabase start
```
It takes sometime especially longer at first time.
### What you can do for testing?
- **Valification**
Run this command on powershell.
```
npx supabase db lint
```
This will return result of sql and scheme test.
- **Reset Migrations**
Run this command on powershell.
```
npx supabase db reset
```
*This command recreate local database which means you __lost__ data saved on local, but does not effect on remote one.*
- **Check remote and local condition**
- **Reset Migrations**
Run this command on powershell.
```
npx supabase migration list
```
You can see how deep migration history applied to local and remote db.
### Check chenges.
Open local supabase studio. You can see updated tables with GUI.
## 4. Save chenges on github.
If you worked on sub branch, you will pull chenges from the branch to main branch.
Push everything.
## 5. Upload chenges to origin.
Run this commnad on powershell.
```
npx supabase db push --dry-run
```
If there is no issues, run this command.
```
npx supabase db push
```