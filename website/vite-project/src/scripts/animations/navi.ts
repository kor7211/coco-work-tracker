import { Character, type Animation, loadSVG } from "./character.ts";

export class Navi extends Character {
    public body: SVGSVGElement | false  = false;
    async loadAllData(): Promise<void> {
        this.body = await loadSVG("/src/assets/images/navi/navi.svg");
    }

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
        if (this.character.body) {
            this.character.svg = this.character.body;
        }
    }

    update(elapsed: number, totalTime: number): void {
        
    }
}
