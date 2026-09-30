from pydantic import BaseModel
from fastapi import APIRouter, Query
import backend.database as database

router = APIRouter(prefix="/api/history")


@router.get("/")
def get_histories(
    limit: int = Query(default=10, ge=1, le=100),
    before: str | None = None,
    before_id: int = Query(default=1)
):
    conn = database.get_connection()

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

    if history_ids:
        placeholders = ",".join("?" for _ in history_ids)

        query = f"""
            SELECT history_id, path
            FROM images
            WHERE history_id IN ({placeholders})
            ORDER BY id
        """

        rows = conn.execute(query, history_ids).fetchall()

        for row in rows:
            images.setdefault(row["history_id"], []).append(
                row["path"]
            )

    conn.close()

    return [
        {
            **dict(history),
            "images": images.get(history["id"], [])
        }
        for history in histories
    ]

@router.get("/types")
def get_types(
    limit : int = Query(default=10, ge=1, le=100)
):
    conn = database.get_connection()

    query = """
        SELECT *
        FROM work_types
        ORDER BY id DESC
        LIMIT ?
    """

    cursor = conn.execute(query, (limit,))
    result = cursor.fetchall()

    conn.close()

    return result

@router.get("/types/{type_id}")
def get_type(type_id: int):
    conn = database.get_connection()

    query = """
        SELECT *
        FROM work_types
        WHERE id = ?
    """

    cursor = conn.execute(query, (type_id,))
    result = cursor.fetchone()

    conn.close()

    return result

@router.get("/{history_id}")
def get_history(history_id: int):
    conn = database.get_connection()

    query = """
        SELECT *
        FROM history
        WHERE id = ?
    """

    cursor = conn.execute(query, (history_id,))
    result = cursor.fetchone()

    

    conn.close()

    return result

class HistoryCreate(BaseModel):
    title: str
    comment: str
    type_id: int
    timezone: str
    uploaded: str
    started: str
    duration_minutes: int
    break_minutes: int = 0

@router.post("/")
def create_history(history: HistoryCreate):
    conn = database.get_connection()

    query = """
        INSERT INTO history 
        (
            type_id, 
            timezone, 
            uploaded, 
            started, 
            duration_minutes, 
            break_minutes, 
            title, 
            comment
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
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
            history.title,
            history.comment,
        )
    )
    history_id = cursor.lastrowid
    
    conn.commit()
    conn.close()

    return {"id": history_id}

class ImageCreate(BaseModel):
     path : str

@router.post("/{history_id}/images")
def add_image_to_history(
    history_id : int,
    image : ImageCreate
):
    conn = database.get_connection()

    query = """
        INSERT INTO image 
        (history_id, path)
        VALUES (?, ?)
    """

    cursor = conn.execute(
        query,
        (
            history_id,
            image.path
        )
    )
    image_id = cursor.lastrowid
    
    conn.commit()
    conn.close()

    return {"id":image_id}

class TypeCreate(BaseModel):
     name : str
     description : str

@router.post("/types")
def create_type(
    work_type: TypeCreate
):
    conn = database.get_connection()

    query = """
        INSERT INTO work_types
        (name, description)
        VALUES (?, ?)
    """

    cursor = conn.execute(
        query,
        (
            work_type.name,
            work_type.description
        )
    )
    work_id = cursor.lastrowid
    
    conn.commit()
    conn.close()

    return {"id": work_id}