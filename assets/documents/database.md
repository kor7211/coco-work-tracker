```mermaid
erDiagram
    HISTORY }|--|| WORK-TYPE : describe
    HISTORY ||--o{ IMAGE : contains
    HISTORY {
        int id PK
        int typeId FK
        text timezone "IANA Time Zone ID"
        text uploaded "YYYY-MM-DDTHH:MM:SSZ"
        text started "YYYY-MM-DDTHH:MM:SSZ"
        int durationMinutes
        int breakMinutes
        text title
        text comment
    }

    IMAGE {
        int id PK
        int historyId FK
        text path
    }
    
    WORK-TYPE {
        int id PK
        text name
        text description
    }
```