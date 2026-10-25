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

## Functions
- ### Create history
**function**: create_history 

**arguments**: p_history (jsonb) 
```json
{
	type_id: (bigint),
	timezone: (string),
	started: (timestamptz),
    uploaded:(timestamptz),
    duration_minutes: (integer),
    break_minutes:(integer),
	titles: {
		em: (string),
		ja: (string)
	},
	descriptions: {
		em: (string),
		ja: (string)
	}
}
```
**returns**: id (bigint)

**feature**: Create new history from json data.

- ### Create work type
**function**: create_work_type

**arguments**: p_work_type (jsonb) 
```json
{
	names: {
		em: (string),
		ja: (string)
	},
	descriptions: {
		em: (string),
		ja: (string)
	}
}
```
**returns**: id (bigint)

**feature**: Create new work type from json data. 

- ### Update history
**function**: update_history

**arguments**: p_payload (jsonb) 
```json
{
	history: {
		id: (bigint), 
		type_id: (bigint),
		timezone: (string),
		started: (timestamptz),
		uploaded:(timestamptz),
		duration_minutes: (integer),
		break_minutes:(integer)
	}
	titles: {
		em: (string),
		ja: (string)
	},
	descriptions: {
		em: (string),
		ja: (string)
	},
	images: []
}
```
**returns**: json object
```json
{
	history_id: (bigint),
	image_count: (int)
}
```

**feature**: Update existing history from given json data.
	images: Update images tagged to the history. If images is empty, all images' path will be deleted.

- ### Update work type
**function**: update_work_type

**arguments**: p_payload (jsonb) 
```json
{
	id: (bigint), 
	names: {
		em: (string),
		ja: (string)
	},
	descriptions: {
		em: (string),
		ja: (string)
	}
}
```
**returns**: void

**feature**: Update existing work type from given json data.