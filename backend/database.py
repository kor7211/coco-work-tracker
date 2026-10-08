import sqlite3

DATABASE = "./backend/database.db"

def get_connection():
    conn = sqlite3.connect(DATABASE)
    conn.row_factory = sqlite3.Row
    return conn

def select_tables(tables):
    conn = get_connection()

    for table_name in tables:
        query = f"""
            SELECT *
            FROM {table_name}
        """
        cursor = conn.execute(query)
        result = cursor.fetchall()
        print(f"___{table_name}___")
        is_first = True
        keys = []
        for row in result:
            if is_first:
                keys = dict(row).keys()
                is_first = False
                print(" | ".join(keys))
            print(" | ".join([str(x) for x in dict(row).values()]))
        print("")

    conn.close()

def initialize_database():
    conn = get_connection()

    try:
        with conn:
            query = """
                DROP TABLE IF EXISTS images
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS comment
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS title
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS history
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS work_type_description
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS work_type_name
            """
            conn.execute(query)

            query = """
                DROP TABLE IF EXISTS work_types
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS work_types (
                    id INTEGER PRIMARY KEY
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS work_type_name (
                    type_id INTEGER NOT NULL,
                    en TEXT,
                    ja TEXT,
                    FOREIGN KEY (type_id) REFERENCES work_types(id)
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS work_type_description (
                    type_id INTEGER NOT NULL,
                    en TEXT,
                    ja TEXT,
                    FOREIGN KEY (type_id) REFERENCES work_types(id)
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS history (
                    id INTEGER PRIMARY KEY,
                    type_id INTEGER NOT NULL,
                    timezone TEXT NOT NULL,
                    started TEXT NOT NULL,
                    duration_minutes INTEGER NOT NULL,
                    break_minutes INTEGER NOT NULL DEFAULT 0,
                    uploaded TEXT NOT NULL,
                    title TEXT NOT NULL,
                    comment TEXT,
                    FOREIGN KEY (type_id) REFERENCES work_types(id)
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS title (
                    history_id INTEGER NOT NULL,
                    en TEXT,
                    ja TEXT,
                    FOREIGN KEY (history_id) REFERENCES work_types(id)
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS comment (
                    history_id INTEGER NOT NULL,
                    en TEXT,
                    ja TEXT,
                    FOREIGN KEY (history_id) REFERENCES work_types(id)
                )
            """
            conn.execute(query)

            query = """
                CREATE TABLE IF NOT EXISTS image (
                    id INTEGER PRIMARY KEY,
                    history_id NOT NULL,
                    path TEXT NOT NULL,
                    FOREIGN KEY (history_id) REFERENCES history(id)
                )
            """
            conn.execute(query)
    finally:
        conn.close()

def manual_control_database():
    conn = get_connection()

    query = """
    """
    conn.execute(query)

    conn.commit()
    conn.close()

if __name__ == "__main__":
    input = input("Run manual query \ny/n: ")
    if input == "y":
        print("manual query running...")
        # manual_control_database()
        initialize_database()
    print("__Completed__")

    select_tables([
        "history",
        "title",
        "comment",
        "image",
        "work_types",
        "work_type_name",
        "work_type_description"
        ])
