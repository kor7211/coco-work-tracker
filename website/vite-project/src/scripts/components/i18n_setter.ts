import { getCurrentLanguage, registerLanguageChangeCallback } from "../core/language_manager.ts";
import i18n_en from "../../i18n/en.json" with { type: "json" };
import i18n_ja from "../../i18n/ja.json" with { type: "json" };

const i18n_collection: Record<string, Record<string, any>> = {
    en: i18n_en,
    ja: i18n_ja
}

/**
 * Set the text content of elements with the "data-i18n" attribute based on the current language.
 * This function retrieves the current language using the getCurrentLanguage function and updates the text content of all elements with the "data-i18n" attribute to the corresponding value from the i18n dictionary.
 * If the current language is not found in the i18n collection, a warning is logged to the console.
 * @param language Current language.
 */
function setI18n(language: string): void {
    const elements: NodeListOf<HTMLElement> = document.querySelectorAll<HTMLElement>("[data-i18n]");
    const i18n = i18n_collection[language];
    if (!i18n) {
        console.warn(`No i18n data found for language: ${language}`);
        return;
    }

    elements.forEach(element => {
        const key: string | null = element.getAttribute("data-i18n");
        if (key) {
            const value = getI18nValue(i18n, key);
            if (value) {
                element.textContent = value;
            }
        } else {
            console.warn(`No i18n key found for element:`, element);
        }
    });
}

/**
 * Get the value of a nested key in the i18n dictionary.
 * @param i18n Dictionary represents i18n key value pairs.
 * @param key Key of i18n. Dot means nest. Eg: Title.Home -> {Title: {Home: "Home"}}
 * @returns If value exists, return value. Otherwise, return undefined.
 */
function getI18nValue(i18n: Record<string, any>, key: string): string | undefined {
    const keys = key.split(".");
    let value: string | Record<string, any> | undefined = i18n;

    for (const k of keys) {
        if (typeof value === "object" && value !== null && k in value) {
            value = (value as Record<string, any>)[k];
        } else {
            return undefined;
        }
    }
    return typeof value === "string" ? value : undefined;
}

// Initialize i18n on page load
setI18n(getCurrentLanguage());

// Register a callback to update i18n when the language changes
registerLanguageChangeCallback((newLanguage: string) => {
    setI18n(newLanguage);
});