// Import styles
import "../../styles/universe.css";

// Import scripts
import "../components/i18n_setter.ts";
import "../components/i18n_button.ts";
import { History } from "../models/history.ts";
import { WorkType } from  "../models/work_type.ts";
import { Planet } from "../components/planet.ts";
import { getHistories, getTypes } from "../services/supabase_service.ts";

// Global Variables
let oldestRow: HTMLTableRowElement | null = null; // Row which contains oldest date.
let startOfOldestRow: Date | null = null; // Start date in oldest row. Expected Sunday.

const LIMIT = 30;
let before: Date | null = null;
let beforeId: bigint | null = null;
let isLoading: boolean = false;
let hasMore: boolean = true;

// Query elements
const tbody = document.getElementById("history-table-body");
if (!(tbody instanceof HTMLTableSectionElement)) {
    throw new Error("History table body was not found.");
}

// Functions
/**
 * 
 */
async function loadMore() : Promise<void> {
    if (isLoading || !hasMore) return;

    isLoading = true;

    const response = await getHistories(LIMIT, before, beforeId);

    for (const historyJson of response) {
        const historyInstance = History.fromJson(historyJson);
        before = historyInstance.started;
        beforeId = historyInstance.id;

        addPlanet(historyInstance);
    }

    hasMore = response.length === LIMIT;

    if (oldestRow) {
        updateObserverTarget(oldestRow);
    }

    isLoading = false;
}

/**
 * Generate and add planet elements to table. Planet represent gicen history record.
 * @param history 
 * @returns 
 */
function addPlanet(history: History): void {
    const planet = Planet.generatePlanetFromHistory(history);

    let counter = 100;

    while (isNewRowNeeded(history.started)) {
        if (--counter < 0) {
            throw new Error("Too many rows generated.");
        }

        addNewRow();
    }

    if (!oldestRow || !startOfOldestRow) {
        throw new Error("Oldest row has not been initialized.");
    }

    const cells = oldestRow.querySelectorAll<HTMLTableCellElement>(
        "[data-date]"
    );

    const targetDate = formatDateKey(history.started);

    for (const cell of cells) {
        if (cell.dataset.date === targetDate) {
            cell.append(planet);
            return;
        }
    }

    console.error("Planet could not be appended.");
}

/**
 * Add new row(week) to tbody. Row contains 7 columns which represent Sunday to Satuday.
 * Each cell in row has data-date which is string parsed number representing date.
 * 
 * Sideeffect: Update oldestRow. Update startOfOldestRow.
 */
function addNewRow(): void {
    const row = document.createElement("tr");
    row.classList.add("history-row", "oldest");

    oldestRow?.classList.remove("oldest");

    const newStart = startOfOldestRow
        ? new Date(startOfOldestRow)
        : getStartOfWeek(new Date());

    if (startOfOldestRow) {
        newStart.setDate(newStart.getDate() - 7);
    }

    startOfOldestRow = newStart;

    for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
        const cell = document.createElement("td");
        cell.classList.add("history-cell");

        const cellDate = new Date(newStart);
        cellDate.setDate(cellDate.getDate() + dayIndex);

        cell.dataset.date = formatDateKey(cellDate);

        row.appendChild(cell);
    }

    oldestRow = row;
    tbody?.appendChild(row);
}

function formatDateKey(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`;
}

/**
 * Check if date is in oldest row.
 * Expected startOfOldestRow is Sunday of oldest week.
 * 
 * Row is expected to start on Sunday and end on Saturday. If the oldest date is in the middle of the week, the row will still be considered to end on Saturday.
 * @param newDate Target date compared to oldest date
 * @returns Returns true if date is in older week, false if in same week.
 */
function isNewRowNeeded(newDate: Date): boolean {
    if (!startOfOldestRow) return true;

    return getStartOfWeek(newDate) < startOfOldestRow;
}

/**
 * Return start(Sunday) of target week.
 * @param dateInWeek Date which is in target week.
 * @returns Set hours to 00:00 am
 */
function getStartOfWeek(dateInWeek: Date): Date {
    const startOfWeek = new Date(dateInWeek);

    startOfWeek.setDate(dateInWeek.getDate() - dateInWeek.getDay());
    startOfWeek.setHours(0, 0, 0, 0);

    return startOfWeek;
}


// Obserb element intersection to load more.
const observer = new IntersectionObserver(entries => {
    if (entries.some(entry => entry.isIntersecting)) {
        void loadMore();
    }
});
let currentTarget: HTMLElement | null = null;
function updateObserverTarget(element: HTMLElement): void {
    if (currentTarget) {
        observer.unobserve(currentTarget);
    }
    currentTarget = element;
    observer.observe(element);
}





// Initialize

// Load work types
const response = await getTypes();
for (const type_json of response) {
    WorkType.fromJson(type_json);
}

// Load initial histories
await loadMore();