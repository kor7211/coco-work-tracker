import sqlite3

DATABASE = "./backend/database.db"

def get_connection():
    return sqlite3.connect(DATABASE)

def initialize_database():
    conn = get_connection()

    query = """
        CREATE TABLE IF NOT EXISTS work_types (
            id INTEGER PRIMARY KEY,
            name TEXT NOT NULL,
            description TEXT
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
        CREATE TABLE IF NOT EXISTS image (
            id INTEGER PRIMARY KEY,
            history_id NOT NULL,
            path TEXT NOT NULL,
            FOREIGN KEY (history_id) REFERENCES history(id)
        )
    """
    conn.execute(query)

    conn.commit()
    conn.close()

if __name__ == "__main__":
    user_input = input("Do you want to run initialize_databse?\ny/n: ")
    if (user_input.lower() == "y"):
        initialize_database()
        print("__Completed__")