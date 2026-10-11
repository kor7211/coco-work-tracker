/**
 * This module allows to fetch data from supabase.
 */

import { supabase } from "./supabase_client.js";

/**
 * Get histories from supabase.
 * @param {int} limit Default 10. Limit of response data length. Must be more than 0.
 * @param {timestamptz} before Default null. UTC timestamptz formted string. Response will be before this time. When this is null, latest records will be returned.
 * @param {int} before_id  Default null. When 'before' is not null, this must be also not null. When there are more than one records whose 'started' are same, less than this id will be returned.
 * @returns 
 */
export async function getHistories(
    limit = 10, 
    before = null, 
    before_id = null) {
    // Valification
    // limit
    if (!Number.isInteger(limit)) {
        throw new Error("limit must be integer");
    }
    if (limit < 1 || limit > 100) {
        throw new Error("limit must be in range of 1 to 100");
    }
    // before
    if (!isTimestamp(before) && before != null) {
        throw new Error("before must be timestamptz format");
    }
    // before_id
    if (!Number.isInteger(before_id) && before_id != null) {
        throw new Error("limit must be integer");
    }

    let histories = null;
    //API connect
    if (before == null || before_id == null) {
        //Get latest data.
        const { data, error } = await supabase
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
        const { data, error } = await supabase
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
            history.images.map(image => getImageUrl(image.path))
        );
    }

    console.log(histories);
}

/**
 * Return result of testing if string is timestamptz format.
 * @param {string} str Target string.
 * @returns {boolean}
 */
function isTimestamp(str) {
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
async function getImageUrl(path) {
    const { data, error } = supabase
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
 * @param {int} limit 
 * @returns
 */
export async function getTypes(limit = 10) {
    if (!Number.isInteger(limit)) {
        throw new Error("Limit need to be integer");
    }
    if (limit < 1 && limit > 100) {
        throw new Error("Limit need to be in range of 1 to 100");
    }

    const { data, error } = supabase
        .from('work-types')
        .select(`
            *,
            work-type-names(en, ja),
            work-type-descriptions(en, ja)
        `)
        .order('id', { ascending: false })
        .limit(limit);

    if (error) {
        throw error;
    }

    return data
}