/**
 * Control language selector element.
 * Requirement:
 *  Clickable element with language-selector-opener class.
 *  Element with language-selector class.
 *  Buttons with data-lang dataset.
 * Expected:
 *  language_selector.css attached.
 */
import { setLanguage } from "./language_setter.js";

const selector = document.querySelector(".language-selector");
if (!selector) {
    throw new Error("Necessary element not found: .language-selector");
};

const button = document.querySelector(".language-button");
if (!button) {
    throw new Error("Necessary element not found: .language-button");
};

const options = document.querySelectorAll("[data-lang]");

button.addEventListener("click", () => {
    selector.classList.add("open");
});

selector.addEventListener("mouseenter", () => {
    selector.classList.add("open");
});

selector.addEventListener("mouseleave", () => {
    selector.classList.remove("open");
});

options.forEach(option => {
    option.addEventListener("click", () => {
        const lang = option.dataset.lang;

        setLanguage(lang);

        selector.classList.remove("open");
    });
});