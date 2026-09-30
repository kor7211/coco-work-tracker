/**
 * This file controls user's submittion of making a new record.
 * Format inputs for POST data to history api.
 * 
 * Required elememts:
 *  <form> with id="recorder". Inside of this, elements below needed.
 *      <input> with name="title"
 *      <input> with name="comment"
 *      <select> with name="timezone"
 *      <input> with name="startedDate"
 *      <input> with name="startedTime"
 *      <select> with name="typeId"
 *      <input> with name="started"
 *      <input> with name="durationMinutes"
 *      <input> with name="breakMinutes"
 *      <input> with name="images"
 *      <input type="submit">
 */

import { getTypes, createType, createHistory } from "./api_history.js";

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
const timezones = Intl.supportedValuesOf("timeZone");
const select = form.querySelector("select#timezone");
if (!select) {
    throw new Error("Necessary element not found: #timezone");
};

for (const timezone of timezones) {
    const option = document.createElement("option");

    option.value = timezone;
    option.textContent = timezone;

    select.append(option);
}

const detected =
    Intl.DateTimeFormat().resolvedOptions().timeZone;

select.value = detected;