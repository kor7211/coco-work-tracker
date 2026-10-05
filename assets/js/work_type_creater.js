/**
 * This file controls user's submittion of making a work type.
 * Format inputs for POST data to history api.
 * 
 * Required elememts:
 *  <form> with id="work-type-creater". Inside of this, elements below needed.
 *      <input> with name="nameEn"
 *      <input> with name="descriptionEn"
 *      <input> with name="nameJa"
 *      <input> with name="descriptionJa"
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

    const name_en_el = el.nameEn;
    const description_en_el = el.descriptionEn;
    const name_ja_el = el.nameJa;
    const description_ja_el = el.descriptionja;
    if (!name_en_el || !description_en_el || !name_ja_el || !description_ja_el) {
        return;
    }
    const name_en = name_en_el.value.trim();
    const description_en = description_en_el.value.trim();
    const name_ja = name_ja_el.value.trim();
    const description_ja = description_ja_el.value.trim();

    await createType(
        name_en,
        name_ja,
        description_en,
        description_ja
    );
    
    updateList();
}

async function updateList() {
    const list = document.querySelector("div#work-type-list");
    if (!list) {
        throw new Error("Necessary element not found: #work-type-list");
    };

    try {
        const types = await getTypes(10);

        for (const type of types) {
            const span = document.createElement("span");
            const name = document.createElement("p");
            const description = document.createElement("p");
            
            type.name.forEach(([key, value]) => {
                name.dataset[`lang${titleCase(key)}`] = value;
            });

            type.descriptions.forEach(([key, value]) => {
                description.dataset[`lang${titleCase(key)}`] = value;
            });

            span.append(name, description);

            list.append(span);
        }
    } catch (exception) {
        console.error(exception);
    }
}

/**
 * Return title case of given string.
 * Title Case -> Begin with upper case and others are lower case. 
 * @param {string} s 
 * @returns {string}
 */
function titleCase(s) {
    return s.toLowerCase()
        .split(' ')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}

// Initialize
updateList();