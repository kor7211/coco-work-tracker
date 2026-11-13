import { getCurrentLanguage ,registerLanguageChangeCallback } from "../core/language_manager.ts";

const fonts: Record<string, Record<string, string>> = {
    "en": {
        "--ui-font": '"EnUI", sans-serif'
    },
    "ja": {
        "--ui-font": '"EnUI", sans-serif'
    }
}

function setFont(language: string): void {
    if (language in fonts) {
        for (const [variable, font] of Object.entries(fonts[language])) {
            document.documentElement.style.setProperty(
                variable,
                font
            );
        }
    } else {
        console.warn(`No i18n data found for language: ${language}`);
    }
}

// Initialize
setFont(
    getCurrentLanguage()
);

registerLanguageChangeCallback(
    (lang) => setFont(lang)
);