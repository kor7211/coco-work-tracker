/**
 * This file controls user's submittion of making a work type.
 * Format inputs for POST data to history api.
 * 
 * Required elememts:
 *  <form> with id="work-type-creater". Inside of this, elements below needed.
 *      <input> with name="name"
 *      <input> with name="description"
 *  <div> with id="work-type-list"
 */

import { getTypes, createType } from "./api_history.js";

const form = document.querySelector("form#work-type-creater");

if (!form) {
    throw new Error("Necessary element not found: #work-type-creater");
};

form.addEventListener("submit", async (event) => {
    event.preventDefault();
    await createWorkType();
})


async function createWorkType() {
    const el = form.elements;

    const name = el.name;
    const description = el.description;

    await createType(name, description);
    
    updateList();
}

async function updateList() {
    const list = document.querySelector("div#work-type-list");
    if (!list) {
        throw new Error("Necessary element not found: #work-type-list");
    };

    try {
        const types = getTypes(10);
        for (const type in types) {
            const p = document.createElement("p");
            p.innerText = `name: ${type.name} | ${type.description}`;

            list.append(p);
        }
    } catch (exception) {
        console.error(exception);
    }
}

// Initialize
updateList();