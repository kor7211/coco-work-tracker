import { getHistories } from "./api_history.js";

const container = document.getElementById("history-container");
if (!container) {
    throw new Error("Necessary DOMElement not fond: #history-container");
}

const LIMIT = 20; // Amount of items loaded per one unit

let loading = false; // Loading on progress or not
let hasMore = true; // database has more reocords or not
let before = null; // oldest loaded record's started time
let before_id = null; // id of oldest loaded reocrd.

async function loadMore() {
    if (loading || !hasMore) return;

    loading = true;

    try {
        const data = await getHistories(LIMIT, before, before_id);

        for (const history of data) {
            container.append(createHistoryElement(history));
            before = history.started;
            before_id = history.id;
        }

        hasMore = data.hasMore;
    } finally {
        loading = false;
    }
}

/**
 * Return new created DOMElement based on given data.
 * @param  history data of history from history api
 * @returns html element
 */
function createHistoryElement(history) {
    const template = document.querySelector("#history-template");
    if (!template) {
        throw new Error("Necessary DOMElement not fond: #history-template");
    }
    const clone = template.content.cloneNode(true);

    clone.querySelector(".title").textContent = history.title;
    clone.querySelector(".comment").textContent = history.comment;
    
    return clone;
}



/* Runs at initializing */

const observer = new IntersectionObserver(entries => {
    if (entries[0].isIntersecting) {
        loadMore();
    }
});

const scroll_trigger = document.getElementById("scroll-trigger");
if (!scroll_trigger) {
    throw new Error("Necessary DOMElement not fond: #scroll-trigger");
}

observer.observe(scroll_trigger);