import { 
    getCurrentLanguage, 
    setCurrentLanguage,
    registerLanguageChangeCallback
} from "../core/language_manager.ts";

let currentLanguage = getCurrentLanguage();

const buttons = document.querySelectorAll(".i18n-button");
buttons.forEach(button => {
    button.addEventListener("click", () => {
        if (currentLanguage == "en") {
            currentLanguage = "ja";
        } else {
            currentLanguage = "en";
        }
        setCurrentLanguage(currentLanguage);
    })
})

registerLanguageChangeCallback((newLang: string) => {
    currentLanguage = newLang;
})