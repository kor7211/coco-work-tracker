import { History } from "../models/history.ts";
import { WorkType } from "../models/work_type.ts";

export class Planet {
    private static _planets: Planet[] = []; // Collection of Planet instance to control state 

    public static generatePlanetFromHistory(history: History): HTMLDivElement {
        // Base planet element
        const planet = document.createElement("div");
        planet.classList.add("planet");

        // Visual of planet
        const visual = this.getVisual();

        //Add component to planet
        planet.append(
            visual
        );

        return planet;
    }

    /**
     * Create and return svg element.
     * Instantiate new Planet class to control svg animation. 
     * @param option 
     * @returns svg
     */
    private static getVisual(option: Record<string, any> = {}): SVGSVGElement {
        const SVG_NS = "http://www.w3.org/2000/svg";
        
        // Create option
        const defaultOption: Record<string, any> = {
            seed: randint(0, 100), // Seed for random values. Same seed generate same planet. 0 to 99.
            core: { // Core settings
                radius: 5, // Core's radius. 0 to 50.
                color: "hsl(0, 0%, 84%)", // Color in hsl
            },
            light: { // Planetary light settings
                amount: 5, // Amount of light needles
                minRadius: 15, // Minimum height of light radius. 0 to 50. Higher than core radius. 
                maxRadius: 40, // Minimum height of light radius. 0 to 50. Higher than core radius. Heigher than min radius.
                color: "hsl(0, 0%, 96%)", // Color in hsl
            }
        };
        const mergedOption = {
            ...defaultOption,
            ...option,
            core: {
                ...defaultOption.core,
                ...option.core
            },
            light: {
                ...defaultOption.light,
                ...option.light
            }
        };

        const validatedOption = this.validateOptions(mergedOption);

        // Svg container
        const svg = document.createElementNS(
            SVG_NS,
            "svg"
        ) as SVGSVGElement;
        svg.setAttribute("viewBox", "-50 -50 100 100"); // Set origin (0, 0). North(0, -50) | East (50, 0)
        svg.setAttribute("preserveAspectRatio", "xMidYMid meet");

        // Core circle
        const coreCircle = document.createElementNS(
            SVG_NS,
            "circle"
        ) as SVGCircleElement;
        coreCircle.setAttribute("fill", validatedOption.core.color); // Set core color
        coreCircle.setAttribute("r", validatedOption.core.radius); // Set core radius
        coreCircle.setAttribute("cx", "0");
        coreCircle.setAttribute("cy", "0");
        
        // Pranetary light needles
        const path = document.createElementNS(
            SVG_NS,
            "path"
        ) as SVGPathElement;
        path.setAttribute("fill", validatedOption.light.color);

        //Append
        svg.append(
            path,
            coreCircle
        );

        // Instantiate Planet
        new Planet(
            validatedOption.seed,
            validatedOption.light.amount,
            validatedOption.light.minRadius,
            validatedOption.light.maxRadius,
            path
        );

        return svg;
    }

    /**
     * Validate options for planet visual.
     * Return valid option
     * Output validation result in console
     * @param options 
     */
    private static validateOptions(options: Record<string, any>): Record<string, any> {
        // Seed
        options.seed = validateSeed(options.seed);

        // Core
        options.core.radius = validateCoreRadius(options.core.radius);
        options.core.color = validateColor(options.core.color);

        // Light
        options.light.amount = validateLightAmount(options.light.amount);
        options.light.minRadius = validateLightMinRadius(options.light.minRadius);
        options.light.maxRadius = validateLightMaxRadius(
            options.light.maxRadius,
            options.light.minRadius
        );
        options.light.color = validateColor(options.light.color);

        return options;
    }

    private sectionAmount: number;
    private path: SVGPathElement; // Pranetary light
    private animationInstance: number | false = false;
    private lastUpdate: number | null = null;
    private totalTime:number =0;
    private masterSeed: number;
    private lightMinRadius: number;
    private lightMaxRadius: number;
    private constructor(
        seed: number,
        sectionAmount: number,
        lightMinRadius: number,
        lightMaxRadius: number,
        pathElement: SVGPathElement,
    ) {
        this.masterSeed = seed;
        this.sectionAmount = sectionAmount;
        this.lightMinRadius = lightMinRadius;
        this.lightMaxRadius = lightMaxRadius;
        this.path = pathElement;

        Planet._planets.push(this);

        this.drow();
        this.startAnimation();
    }

    private animate  = (): void => {
        const now: number = performance.now()

        if (this.lastUpdate != null) {
            const elapsed: number = (now - this.lastUpdate) / 1000;
            this.lastUpdate = now;

            this.totalTime += elapsed;

            this.drow();
        } else {
            this.lastUpdate = now;
        }

        this.animationInstance = requestAnimationFrame(this.animate);
    }

    private drow() {
        // Generate path attribute
        const command: string = this.createPathCommand();

        // Update path
        this.path.setAttribute("d", command);
    }

    private createPathCommand(): string {
        const points: [number, number][] = []; // top of light needle
        const controls: [number, number][] = []; // positions represent control point to drow curve
        const anglePerSection = 360 / this.sectionAmount;

        for (let i = 0; i < this.sectionAmount; i++) {
            const length = calcurateRadius(
                this.totalTime,
                hashNumbers(this.masterSeed, 0),
                this.lightMinRadius,
                this.lightMaxRadius,
            ); // Height of point from core
            const angle = radian(anglePerSection * i);
            points.push(getPosFromVector(angle, length));

            const controlAngle = radian(anglePerSection * (i + 0.5));
            controls.push(getPosFromVector(controlAngle, this.lightMinRadius));
        }

        let command: string = move(points[points.length - 1]); // Move to last point
        for (let i = 0; i < points.length; i++) {
            command += curve(
                controls[i],
                points[i]
            );
        } // Drow curves

        return command;
    }

    public setAnimationState(): void {

    }

    public startAnimation(): void {
    if (this.animationInstance === false) {
        this.animationInstance = requestAnimationFrame(this.animate);
    }
}

    public stopAnimation(): void {
        if (this.animationInstance !== false) {
            cancelAnimationFrame(this.animationInstance);
            this.animationInstance = false;
        }
    }
}


/**
 * Return a svg path command string to move to absolute position
 * @param position 
 * @returns 
 */
function move(position: [number, number]): string {
    return `M ${position[0]} ${position[1] }`;
}

/**
 * Return a svg path command string to drow a curve
 * @param control Control position to create a curve
 * @param target 
 * @returns 
 */
function curve(control: [number, number], target: [number, number]): string {
    return `Q ${control[0]} ${control[1]} ${target[0]} ${target[1]} `;
}

/**
 * 
 * @param time Current total time
 * @param speed Speed of iteration 0 - 1
 * @param min Minimum height from core
 * @param max Maximum height from core
 * @returns 
 */
function calcurateRadius(time:number, speed: number, min: number, max: number): number {
    return Math.abs(((max - min) * Math.sin(time * speed))) + min;
}

/**
 * 
 * @param angle radian
 * @param length 
 * @param origin 
 * @returns 
 */
function getPosFromVector(angle: number, length: number, origin: [number, number] = [0, 0]): [number, number] {
    return [
        Math.cos(angle) * length + origin[0],
        Math.sin(angle) * length + origin[1]
    ];
}

function radian(angle: number): number {
    return angle / 180 * Math.PI;
}

/**
 * 
 * @param start included
 * @param end excluded
 * @returns Random integer
 */
function randint(start: number, end: number): number {
    return Math.floor(Math.random() * (end - 1)) + start;
}

function hashNumbers(a: number, b: number): number {
    let seed = (a * 31 + b) | 0;

    seed = Math.imul(seed ^ (seed >>> 16), 0x45d9f3b);
    seed = Math.imul(seed ^ (seed >>> 16), 0x45d9f3b);
    seed = seed ^ (seed >>> 16);

    return (seed >>> 0) / 4294967296;
}

// Validations
function validateSeed(seed: any): number {
    const DEFAULT = 0;

    // type
    if (!Number.isInteger(seed)) {
        console.error(`${seed} is not a number.`);
        return DEFAULT;
    } else if (
        0 > seed ||
        seed >= 100 
    ) {
        console.error(`${seed} must be 0 to 99.`);
        return DEFAULT;
    } 
    return seed;
}

function validateCoreRadius(radius: any): number {
    const DEFAULT = 5;

    if (!Number.isInteger(radius)) {
        console.error(`${radius} is not an integer.`);
        return DEFAULT;
    } else if (radius < 0 || radius > 50) {
        console.error(`${radius} must be 0 to 50.`);
        return DEFAULT;
    }

    return radius;
}

function validateColor(color: any): string {
    const DEFAULT = "hsl(0, 100%, 100%)";

    if (
        typeof color !== "string" ||
        !/^hsl\(\s*\d+(?:\.\d+)?\s*,\s*\d+(?:\.\d+)?%\s*,\s*\d+(?:\.\d+)?%\s*\)$/i.test(color)
    ) {
        console.error(`${color} is not a valid HSL color.`);
        return DEFAULT;
    }

    return color;
}

function validateLightAmount(amount: any): number {
    const DEFAULT = 5;

    if (!Number.isInteger(amount)) {
        console.error(`${amount} is not an integer.`);
        return DEFAULT;
    } else if (amount < 1 || amount > 100) {
        console.error(`${amount} must be 1 to 100.`);
        return DEFAULT;
    }

    return amount;
}

function validateLightMinRadius(radius: any): number {
    const DEFAULT = 15;

    if (typeof radius !== "number" || !Number.isFinite(radius)) {
        console.error(`${radius} is not a number.`);
        return DEFAULT;
    } else if (radius < 0 || radius > 50) {
        console.error(`${radius} must be 0 to 50.`);
        return DEFAULT;
    }

    return radius;
}

function validateLightMaxRadius(radius: any, minRadius: number): number {
    const DEFAULT = 30;

    if (typeof radius !== "number" || !Number.isFinite(radius)) {
        console.error(`${radius} is not a number.`);
        return DEFAULT;
    } else if (radius < minRadius || radius > 50) {
        console.error(`${radius} must be ${minRadius} to 50.`);
        return DEFAULT;
    }

    return radius;
}