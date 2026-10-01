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


async function createRecord() {
    const el = form.elements;

    const title = el.title;
    const comment = el.comment;
    const started = el.started;
    const duration_minutes = el.durationMinutes;
    const break_minutes = el.breakMinutes;
    const type_id = el.typeId;
    const images = el.images;

}

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