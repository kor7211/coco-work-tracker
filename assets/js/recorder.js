/**
 * This file controls user's submittion of making a new record.
 * Format inputs for POST data to history api.
 * 
 * Required elememts:
 *  <form> with id="recorder". Inside of this, elements below needed.
 *      <input> with name="title"
 *      <input> with name="comment"
 *      <select> with name="timezone", id="timezone"
 *      <input> with name="startedDate"
 *      <input> with name="startedTime"
 *      <select> with name="typeId", id="work-type"
 *      <input> with name="started"
 *      <input> with name="durationMinutes"
 *      <input> with name="breakMinutes"
 *      <input> with name="images"
 *      <input type="submit">
 */

import { getTypes, createHistory } from "./api_history.js";

const form = document.querySelector("form#recorder");
if (!form) {
    throw new Error("Necessary element not found: #recoder");
};

form.addEventListener("submit", async (event) => {
    event.preventDefault();
    await createRecord();
})

/**
 * Create new record to history API.
 */
async function createRecord() {
    const el = form.elements;

    const title_el = el.title;
    const comment_el = el.comment;
    const timezone_el = el.timezone;
    const started_date_el = el.startedDate;
    const started_time_el = el.startedTime;
    const duration_minutes_el = el.durationMinutes;
    const break_minutes_el = el.breakMinutes;
    const type_id_el = el.typeId;
    const images_el = el.images;

    const title = title_el.value.trim();
    const comment = comment_el.value.trim();
    const type_id = type_id_el.value;
    
    const timezone = timezone_el.value;
    const started_date = started_date_el.value;
    const started_time = started_time_el.value;
    const started = localDateTimeToUTC(
        started_date,
        started_time,
        timezone
    );
    const uploaded = new Date().toISOString().slice(0, 19) + "Z";

    const duration_minutes = duration_minutes_el.value;
    const break_minutes = break_minutes_el.value;
    
    // const images = []; images treatment required here
    
    try {
        const response = await createHistory(
            title,
            comment,
            type_id,
            timezone,
            uploaded,
            started,
            duration_minutes,
            break_minutes
        );
        // use response.id to post images
    } catch (error) {
        console.error(error);
    };
};

/**
 * Translate local datetime to UTC
 * @param {string} date YYYY-MM-DD formated local day
 * @param {string} time HH:MM formated local time
 * @param {string} timezone valid timezone id from Intl 
 * @returns UTC
 */
function localDateTimeToUTC(date, time, timezone) {
    if (!isValidTimezone(timezone)) {
        throw new Error(`Invalid timezone: ${timezone}`);
    }

    const [year, month, day] = date.split("-").map(Number);
    const [hour, minute] = time.split(":").map(Number);

    const utcGuess = new Date(
        Date.UTC(year, month - 1, day, hour, minute, 0)
    );

    const parts = new Intl.DateTimeFormat("en-US", {
        timeZone: timezone,
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        hourCycle: "h23",
    }).formatToParts(utcGuess);

    const values = {};

    for (const part of parts) {
        if (part.type !== "literal") {
            values[part.type] = Number(part.value);
        }
    }

    const localAsUTC = Date.UTC(
        year,
        month - 1,
        day,
        hour,
        minute,
        0
    );

    const timezoneAsUTC = Date.UTC(
        values.year,
        values.month - 1,
        values.day,
        values.hour,
        values.minute,
        0
    );

    const offset = timezoneAsUTC - localAsUTC;

    const result = new Date(utcGuess.getTime() - offset);

    return result.toISOString().slice(0, 19) + "Z";
}

/**
 * Validate Intl timzone id. 
 * @param {string} timezone 
 * @returns {boolean} true for valid
 */
function isValidTimezone(timezone) {
    try {
        new Intl.DateTimeFormat("en", {
            timeZone: timezone
        });

        return true;
    } catch {
        return false;
    }
}

// Initialize
// timezone
const timezones = Intl.supportedValuesOf("timeZone");
const timezone_select = form.querySelector("select#timezone");
if (!timezone_select) {
    throw new Error("Necessary element not found: #timezone");
};

for (const timezone of timezones) {
    const option = document.createElement("option");

    option.value = timezone;
    option.textContent = timezone;

    timezone_select.append(option);
};

const detected =
    Intl.DateTimeFormat().resolvedOptions().timeZone;

timezone_select.value = detected;

// work type
const select = document.querySelector("select#work-type");
if (!select) {
    throw new Error("Necessary element not found: #work-type");
};

try {
    const types = await getTypes(50);

    for (const type of types) {
        const option = document.createElement("option");
        option.value = type.id;
        option.innerText = type.name;

        select.append(option);
    }
} catch (error) {
    console.error(error);
}