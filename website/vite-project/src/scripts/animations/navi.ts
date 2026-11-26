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

    private body: SVGGElement | null = null;
    private head: SVGGElement | null = null;
    private armRight: SVGGElement | null = null;
    private armLeft: SVGGElement | null = null;
    initialize(): void {
        if (this.character.body) {
            this.character.svg = this.character.body;

            const body = this.character.svg.querySelector("#BodyAnimation");
            if (!(body instanceof SVGGElement)) {
                throw new Error("Body not found.");
            }
            this.body = body;

            const head = this.character.svg.querySelector("#HeadAnimation");
            if (!(head instanceof SVGGElement)) {
                throw new Error("Head not found.");
            }
            this.head = head;

            const armRight = this.character.svg.querySelector("#ArmRightAnimation");
            if (!(armRight instanceof SVGGElement)) {
                throw new Error("ArmRight not found.");
            }
            this.armRight = armRight;

            const armLeft = this.character.svg.querySelector("#ArmLeftAnimation");
            if (!(armLeft instanceof SVGGElement)) {
                throw new Error("ArmLeft not found.");
            }
            this.armLeft = armLeft;

            this.armLeft.setAttribute(
                "style",
                "transform-box: fill-box; transform-origin: left top;"
            );
            this.armRight.setAttribute(
                "style",
                "transform-box: fill-box; transform-origin: right top;"
            );
        }
    }

    update(elapsed: number, totalTime: number): void {
        const frequency = 2;
        const sin = Math.sin(totalTime * frequency);
        // Body translate
        const m: number = 3; // Magnitude
        const y = m * sin + m;
        
        this.head?.setAttribute("transform", `translate(0, ${y})`);
        this.body?.setAttribute("transform", `translate(0, ${y})`);

        // Arms floating
        const maxAngle: number = 2;
        const angle = maxAngle * sin;
        
        this.armLeft?.setAttribute("transform", `rotate(${-angle})`);
        this.armRight?.setAttribute("transform", `rotate(${angle})`);
        
    }
}
