# Basic syntax of Postgre-Sql
## Foreign Key
### ON DELETE ~~
Define action played when reference key deleted.
- RESTRICT
Refuse reference deletion.
- CASCADE
Delete also refering records.
- SET NULL
Replace value with NULL.
- NO ACTION
Default. Nothing happens.