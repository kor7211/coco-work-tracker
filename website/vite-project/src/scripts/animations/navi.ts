import { Character, type Animation } from "./character.ts";

export class Navi extends Character {
    public override createIdleAnimation(): Animation {
        return new IdleAnimation(this);
    }
}

class IdleAnimation implements Animation {
    private character: Navi;
    constructor(character: Navi) {
        this.character = character;
    }

    initialize(): void {
        const head = document.createElementNS(
            Character.SVG_NS,
            "g"
        );

        const circle = document.createElementNS(
            Character.SVG_NS,
            "circle"
        )
        circle.setAttribute("r", "30");

        this.character.svg.append(circle);
    }

    update(elapsed: number, totalTime: number): void {
        
    }
}
