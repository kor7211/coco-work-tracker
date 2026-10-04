```mermaid
erDiagram
    HISTORY }|--|| WORK-TYPES : describe
    HISTORY ||--o{ IMAGE : contains
    HISTORY ||--|| TITLE : translate
    HISTORY ||--|| COMMENT : translate
    HISTORY {
        int id PK
        int type_id FK
        text timezone "IANA Time Zone ID"
        text uploaded "YYYY-MM-DDTHH:MM:SSZ"
        text started "YYYY-MM-DDTHH:MM:SSZ"
        int duration_minutes
        int break_minutes
    }

    TITLE {
        int history_id FK
        text en
        text ja
    }

    COMMENT {
        int history_id FK
        text en
        text ja
    }

    IMAGE {
        int id PK
        int history_id FK
        text path
    }
    
    WORK-TYPES ||--|| WORK-TYPE-NAME: translate
    WORK-TYPES ||--|| WORK-TYPE-DESCRIPTION: translate
    WORK-TYPES {
        int id PK
    }

    WORK-TYPE-NAME {
        int type_id FK
        text en
        text ja
    }

    WORK-TYPE-DESCRIPTION {
        int type_id FK
        text en
        text ja
    }
```