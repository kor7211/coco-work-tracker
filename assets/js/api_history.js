async function _request(url, options = {}) {
    const response = await fetch(url, options);

    if (!response.ok) {
        throw new Error(
            `HTTP ${response.status}: ${response.statusText}`
        );
    }

    return await response.json();
}


export async function getHistories(limit = 10, before = null, before_id = null) {
    if (before == null) {
        return await _request(
            `http://localhost:8000/api/history?limit=${limit}`
        );
    } else {
        return await _request(
            `http://localhost:8000/api/history?limit=${limit}&before=${before}&before_id=${before_id}`
        );
    }
}


export async function getHistory(id) {
    return await _request(
        `http://localhost:8000/api/history/${id}`
    );
}


export async function getTypes(limit = 10) {
    return await _request(
        `http://localhost:8000/api/history/types?limit=${limit}`
    );
}


export async function getType(id) {
    return await _request(
        `http://localhost:8000/api/history/types/${id}`
    );
}


export async function createHistory(
    title,
    comment,
    type_id,
    timezone,
    uploaded,
    started,
    duration_minutes,
    break_minutes = 0,
) {
    return await _request(
        "http://localhost:8000/api/history",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                title,
                comment,
                type_id,
                timezone,
                uploaded,
                started,
                duration_minutes,
                break_minutes
            })
        }
    );
}


export async function addImageToHistory(
    history_id,
    path
) {
    return await _request(
        `http://localhost:8000/api/history/${history_id}/images`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                path
            })
        }
    );
}


export async function createType(
    name,
    description
) {
    return await _request(
        "http://localhost:8000/api/history/types",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                name,
                description
            })
        }
    );
}


