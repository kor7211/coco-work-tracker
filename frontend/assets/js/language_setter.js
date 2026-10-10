/**
 * This JavaScript file controls site languages.
 */
import { titleCase } from "./case_formatter.js";

//Error related to language settings.
class LanguageError extends Error{
    constructor(message) {
        super(message);
    }
}

// Dictionary of path for language json files.
const translations = {
    ja: "../lang/ja.json",
    en: "../lang/en.json"
};

// Current language. Default "en".
let currentLang = "en";
export function getCurrentLangage() {
    return currentLang;
}

/**
 * Replace all translatable contents with the given language.
 *
 * Arguments:
 *     lang (string): Valid language code.
 *         ja, en
 *
 * Side Effects:
 *     - Replace all translatable texts.
 *     - Set the HTML lang attribute.
 */
async function setLanguage(lang) {
    if (!Object.hasOwn(translations, lang)) {
        throw new LanguageError(
            `language_setter.js:\nGiven language is invalid: '${lang}'`
        );
    }

    const path = translations[lang];

    const module = await import(path, {
        with: { type: "json" }
    });

    const data = module.default;

    document.documentElement.lang = lang;

    const elements = document.querySelectorAll("[data-i18n]");

    for (const element of elements) {
        const key = element.dataset.i18n;
        const value = _getTranslation(data, key);

        if (value == undefined) {
            console.error(`Translation key not found: ${key}`)
            continue;
        }

        element.textContent = value;
    }

    // Set instance elements language
    const key = `[data-lang-${lang}]`;
    const elememts = document.querySelectorAll(key);

    for (const el of elememts) {
        const content = el.dataset[`lang${titleCase(lang)}`];
        if (content) {
            el.textContent = content;
        }
    }

    localStorage.setItem("language",lang);

    currentLang = lang;

    console.log("set language: " + lang);
}

/**
 * This function return a value in given data along with given key.
 * @param {*} data Target language data. Expected as json default data.
 * @param {string} key Use . to express nested structure.
 * @returns
 */
function _getTranslation(data, key){
    return key.split(".").reduce((current, part) => current?.[part], data);
}

/**
 * Runs at first load.
 * This function gets local cache and try to set that.
 * If the value is invalid, set 'en' as default.
 */
function main(){
    const lang = localStorage.getItem("language");

    try{
        if (!lang) lang = "en";
        setLanguage(lang);
    } catch (e) {
        if (typeof(e) == LanguageError) {
            localStorage.removeItem("language");
            console.error(`${e.message}`);
        } else {
            throw e;
        }
    } 
}

main()