async function _request(url, options = {}) {
    const response = await fetch(url, options);

    if (!response.ok) {
        throw new Error(
            `HTTP ${response.status}: ${response.statusText}`
        );
    }

    return await response.json();
}


export async function getHistories(
    limit = 10, 
    before = null, 
    before_id = null) {
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
    title_en,
    title_ja,
    comment_en,
    comment_ja,
    type_id,
    timezone,
    uploaded,
    started,
    duration_minutes,
    break_minutes,
    images = []
) {
    return await _request(
        "http://localhost:8000/api/history",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                title_en,
                title_ja,
                comment_en,
                comment_ja,
                type_id,
                timezone,
                uploaded,
                started,
                duration_minutes,
                break_minutes,
                images
            })
        }
    );
}


export async function createType(
    name_en,
    name_ja,
    description_en,
    description_ja
) {
    return await _request(
        "http://localhost:8000/api/history/types",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                name_en,
                name_ja,
                description_en,
                description_ja
            })
        }
    );
}
