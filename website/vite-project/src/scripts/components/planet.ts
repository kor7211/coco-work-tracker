import { History } from "../models/history.ts";
import { WorkType } from "../models/work_type.ts";

export function generatePlanet(history: History): HTMLDivElement {
    // Base planet element
    const planet = document.createElement("div");
    planet.classList.add("planet");
    planet.dataset.id = history.id.toString();

    // Visual of planet
    const image = document.createElement("img");
    image.src = "../../assets/images/planet.png";

    //Add component to planet
    planet.append(image);

    return planet;
}