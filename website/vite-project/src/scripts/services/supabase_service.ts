import client from "./supabase_client.ts";

/**
 * Get histories from supabase.
 * @param {int} limit Default 10. Limit of response data length. Must be more than 0.
 * @param {timestamptz} before Default null. UTC timestamptz formted string. Response will be before this time. When this is null, latest records will be returned.
 * @param {int} before_id  Default null. When 'before' is not null, this must be also not null. When there are more than one records whose 'started' are same, less than this id will be returned.
 * @returns 
 */
export async function getHistories (
    limit: number = 10, 
    before: string | null = null, 
    before_id: number | null = null) : Promise<any> {
    // Valification
    // limit
    if (limit < 1 || limit > 100) {
        throw new Error("limit must be in range of 1 to 100");
    }
    // before
    if (before != null) {
        if (!isTimestamp(before)) {
            throw new Error("before must be timestamptz format");
        }
        if (before_id == null) {
            throw new Error("before_id must be specified when before is specified");
        }
    }

    let histories = null;
    //API connect
    if (before == null) {
        //Get latest data.
        const { data, error } = await client
            .from('histories')
            .select(`
                *,
                titles(en, ja),
                comments(en, ja),
                images(path)
            `)
            .order('started', { ascending: false })
            .order('id', { ascending: false })
            .limit(limit);

        if (error) {
            throw new Error(error);
        }

        histories = data;
    } else {
        const { data, error } = await client
            .from('histories')
            .select(`
                *,
                titles(en, ja),
                comments(en, ja),
                images(path)
            `)
            .or(
                `started.lt.${before},and(started.eq.${before},id.lt.${before_id})`
            )
            .order('started', { ascending: false })
            .order('id', { ascending: false })
            .limit(limit);

        if (error) {
            throw error;
        }

        histories = data;
    } 

    // Translate path to url
    for (const history of histories) {
        history.images = await Promise.all(
            history.images.map((image: { path: string }) => getImageUrl(image.path))
        );
    }

    return histories;
}

/**
 * Return result of testing if string is timestamptz format.
 * @param {string} str Target string.
 * @returns {boolean}
 */
function isTimestamp(str: string): boolean {
    const regex =
        /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z$/;

    if (!regex.test(str)) {
        return false;
    }

    const date = new Date(str);

    return (
        !Number.isNaN(date.getTime()) &&
        date.toISOString() === str
    );
}

/**
 * Get url of image from supabase storage path
 * @param {str} path Path for supabase storage
 * @returns {str} Url for image.
 */
async function getImageUrl(path: string): Promise<string> {
    const { data, error } = client
        .storage
        .from('history-images')
        .getPublicUrl(path);
    
    if (error) {
        throw error;
    }

    return data.publicUrl;
}

/**
 * Return list of work types.
 * @returns
 */
export async function getTypes(): Promise<any> {
    const { data, error } = client
        .from('work-types')
        .select(`
            *,
            work-type-names(en, ja),
            work-type-descriptions(en, ja)
        `)
        .order('id', { ascending: false });

    if (error) {
        throw error;
    }

    return data
}