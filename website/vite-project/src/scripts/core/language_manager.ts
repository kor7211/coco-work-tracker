const ALLOWED_LANGUAGES: string[] = [
    "en",
    "ja"
]

const DEFAULT_LANGUAGE: string = "en";

const STORAGE_KEY: string = "language";

let currentLanguage: string = "en";

// Array to hold callbacks for when the language changes
const onLanguageChanged: ((language: string) => void)[] = [];

/**
 * Registers a callback function that will be called whenever the current language is updated.
 * Function will be called with the new language as first argument.
 * @param callback Callback function fired on current language is updated.
 */
export function registerLanguageChangeCallback(callback: (language: string) => void): void {
    onLanguageChanged.push(callback);
}

/**
 * Gets the current language.
 * @returns Current language represented as two characters.
 */
export function getCurrentLanguage(): string {
    return currentLanguage;
}

/**
 * Sets the current language.
 * This will fire all registered callbacks and store the language in localStorage.
 * @param language Language represented as two characters.
 */
export function setCurrentLanguage(language: string): void {
    if (ALLOWED_LANGUAGES.includes(language)) {
        currentLanguage = language;
        onLanguageChanged.forEach(callback => callback(language));

        localStorage.setItem(STORAGE_KEY, language);
    }
}

// Initialize the current language from localStorage if available
let language = localStorage.getItem(STORAGE_KEY);
if (!language || 
    !ALLOWED_LANGUAGES.includes(language)) {
    // If no valid language is stored, set to default language
    language = DEFAULT_LANGUAGE;
} 
setCurrentLanguage(language);