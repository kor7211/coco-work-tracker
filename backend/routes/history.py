from pydantic import BaseModel, Field
from fastapi import APIRouter, Query, HTTPException
import backend.database as database

router = APIRouter(prefix="/api/history")


'''
==========================
Functions for GET methods
==========================
'''
@router.get("/")
def get_histories(
    limit: int = Query(default=10, ge=1, le=100),
    before: str | None = None,
    before_id: int = Query(default=1)
):
    conn = database.get_connection()

    try:
        if before is None:
            query = """
                SELECT *
                FROM history
                ORDER BY started DESC, id DESC
                LIMIT ?
            """
    
            histories = conn.execute(query, (limit,)).fetchall()
    
        else:
            query = """
                SELECT *
                FROM history
                WHERE started < ?
                    OR (started = ? AND id < ?)
                ORDER BY started DESC, id DESC
                LIMIT ?
            """
    
            histories = conn.execute(
                query,
                (before, before, before_id, limit)
            ).fetchall()
    
        history_ids = [history["id"] for history in histories]
    
        images = {}
        titles = {}
        comments = {}
    
        if history_ids:
            placeholders = ",".join("?" for _ in history_ids)
            # Process images
            query = f"""
                SELECT history_id, path
                FROM image
                WHERE history_id IN ({placeholders})
                ORDER BY id
            """
    
            rows = conn.execute(query, history_ids).fetchall()
    
            for row in rows:
                images.setdefault(row["history_id"], []).append(
                    row["path"]
                )
    
            # Process titles 
            query = f"""
                SELECT history_id, en, ja
                FROM title
                WHERE history_id IN ({placeholders})
                ORDER BY history_id
            """
    
            rows = conn.execute(query, history_ids).fetchall()
    
            for row in rows:
                titles[row["history_id"]] = {
                    "ja": row["ja"],
                    "en": row["en"]
                }
    
            # Process comment 
            query = f"""
                SELECT history_id, en, ja
                FROM comment
                WHERE history_id IN ({placeholders})
                ORDER BY history_id
            """
    
            rows = conn.execute(query, history_ids).fetchall()
    
            for row in rows:
                comments[row["history_id"]] = {
                    "ja": row["ja"],
                    "en": row["en"]
                }
    
        return [
            {
                **dict(history),
                "images": images.get(history["id"], []),
                "title": titles.get(history["id"], {"ja": "", "en": ""}),
                "comment": comments.get(history["id"], {"ja": "", "en": ""})
            }
            for history in histories
        ]
    finally:
        conn.close()

@router.get("/types")
def get_types(
    limit: int = Query(default=10, ge=1, le=100)
):
    conn = database.get_connection()

    try:
        query = """
            SELECT id
            FROM work_types
            ORDER BY id DESC
            LIMIT ?
        """

        types = conn.execute(query, (limit,)).fetchall()

        type_ids = [row["id"] for row in types]

        names = {}
        descriptions = {}

        if type_ids:
            placeholders = ",".join("?" for _ in type_ids)

            query = f"""
                SELECT type_id, en, ja
                FROM work_type_name
                WHERE type_id IN ({placeholders})
            """

            rows = conn.execute(query, type_ids).fetchall()

            for row in rows:
                names[row["type_id"]] = {
                    "en": row["en"],
                    "ja": row["ja"]
                }

            query = f"""
                SELECT type_id, en, ja
                FROM work_type_description
                WHERE type_id IN ({placeholders})
            """

            rows = conn.execute(query, type_ids).fetchall()

            for row in rows:
                descriptions[row["type_id"]] = {
                    "en": row["en"],
                    "ja": row["ja"]
                }

        return [
            {
                "id": work_type["id"],
                "name": names.get(
                    work_type["id"],
                    {"en": "", "ja": ""}
                ),
                "description": descriptions.get(
                    work_type["id"],
                    {"en": "", "ja": ""}
                )
            }
            for work_type in types
        ]

    finally:
        conn.close()  

@router.get("/types/{type_id}")
def get_type(type_id: int):
    conn = database.get_connection()

    try:
        query = """
            SELECT id
            FROM work_types
            WHERE id = ?
        """

        work_type = conn.execute(
            query,
            (type_id,)
        ).fetchone()

        if work_type is None:
            raise HTTPException(
                status_code=404,
                detail="Work type not found"
            )

        query = """
            SELECT en, ja
            FROM work_type_name
            WHERE type_id = ?
        """

        name = conn.execute(
            query,
            (type_id,)
        ).fetchone()

        query = """
            SELECT en, ja
            FROM work_type_description
            WHERE type_id = ?
        """

        description = conn.execute(
            query,
            (type_id,)
        ).fetchone()

        return {
            "id": work_type["id"],
            "name": {
                "en": name["en"] if name else "",
                "ja": name["ja"] if name else ""
            },
            "description": {
                "en": description["en"] if description else "",
                "ja": description["ja"] if description else ""
            }
        }

    finally:
        conn.close()

'''
==========================
Functions for POST methods
==========================
'''

class HistoryCreate(BaseModel):
    title_en: str
    title_ja: str
    comment_en: str
    comment_ja: str
    type_id: int
    timezone: str
    uploaded: str
    started: str
    duration_minutes: int
    break_minutes: int
    images: list[str] = Field(default_factory=list)


@router.post("/")
def create_history(history: HistoryCreate):
    conn = database.get_connection()

    try:
        with conn:
            query = """
                INSERT INTO history (
                    type_id,
                    timezone,
                    uploaded,
                    started,
                    duration_minutes,
                    break_minutes
                )
                VALUES (?, ?, ?, ?, ?, ?)
            """

            cursor = conn.execute(
                query,
                (
                    history.type_id,
                    history.timezone,
                    history.uploaded,
                    history.started,
                    history.duration_minutes,
                    history.break_minutes,
                )
            )

            history_id = cursor.lastrowid

            query = """
                INSERT INTO title (history_id, en, ja)
                VALUES (?, ?, ?)
            """

            conn.execute(
                query,
                (
                    history_id,
                    history.title_en,
                    history.title_ja,
                )
            )

            query = """
                INSERT INTO comment (history_id, en, ja)
                VALUES (?, ?, ?)
            """

            conn.execute(
                query,
                (
                    history_id,
                    history.comment_en,
                    history.comment_ja,
                )
            )

            if history.images:
                query = """
                    INSERT INTO image (history_id, path)
                    VALUES (?, ?)
                """

                conn.executemany(
                    query,
                    [
                        (history_id, path)
                        for path in history.images
                    ]
                )

        return {"id": history_id}

    finally:
        conn.close()

class TypeCreate(BaseModel):
    name_en : str
    name_ja : str
    description_en : str
    description_ja : str

@router.post("/types")
def create_type(work_type: TypeCreate):
    conn = database.get_connection()

    try:
        with conn:
            cursor = conn.execute("""
                INSERT INTO work_types DEFAULT VALUES
            """)

            type_id = cursor.lastrowid

            conn.execute("""
                INSERT INTO work_type_name (type_id, en, ja)
                VALUES (?, ?, ?)
            """, (
                type_id,
                work_type.name_en,
                work_type.name_ja,
            ))

            conn.execute("""
                INSERT INTO work_type_description (type_id, en, ja)
                VALUES (?, ?, ?)
            """, (
                type_id,
                work_type.description_en,
                work_type.description_ja,
            ))

        return {"id": type_id}

    finally:
        conn.close()