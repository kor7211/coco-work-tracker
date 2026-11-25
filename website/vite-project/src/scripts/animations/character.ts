export interface Animation {
    initialize(): void;
    update(elapsed:number, totalTime: number): void;
}

/**
 * Load svg file xml as html elements.
 * @param path 
 * @returns 
 */
export async function loadSVG(path: string): Promise<SVGSVGElement> {
    const response = await fetch(path);

    if (!response.ok) {
        throw new Error(`Failed to load SVG: ${path}`);
    }

    const svgText = await response.text();

    const parser = new DOMParser();
    const svgDocument = parser.parseFromString(
        svgText,
        "image/svg+xml"
    );

    const parserError = svgDocument.querySelector("parsererror");

    if (parserError) {
        console.error("SVG Parser Error:", parserError.textContent);
        console.error("SVG Source:", svgText);
        throw new Error("Invalid SVG document");
    }

    const svgElement = svgDocument.documentElement;

    if (svgElement.localName !== "svg") {
        throw new Error("The document root is not an SVG element");
    }

    return document.importNode(
        svgElement,
        true
    ) as unknown as SVGSVGElement;
}

export abstract class Character {
    // Members
    /**
     * Origin (0, 0) width:100 * hight:150
     * North(0, -75) | East (50, 0)
     */
    public svg: SVGElement;
    private animationInstance: number | false = false;
    private lastUpdate: number | null = null;
    private totalTime: number = 0;
    private animation: Animation;
    public static SVG_NS: string = "http://www.w3.org/2000/svg"; 
    
    // Initialize
    constructor() {
        // Setup Svg
        this.svg = document.createElementNS(
            Character.SVG_NS,
            "svg"
        ) as SVGElement;
        this.svg.setAttribute("viewBox", "-50 -75 100 150"); // Set origin (0, 0). North(0, -75) | East (50, 0)
        this.svg.setAttribute("preserveAspectRatio", "xMidYMid meet");

        this.animation = this.createIdleAnimation();
    }
    public abstract loadAllData(): Promise<void>;
    // Animation controllers
    /**
     * Start animation. if new animation given, start the new animation.
     * @example chracter.startAnimation(character.idleAnimation.bind(character));
     * @param nextAnimation next animation function binded instance
     */
    public startAnimation(nextAnimation: Animation): void {
        this.animation = nextAnimation;

        this.animation.initialize();

        if (this.animationInstance === false) {
            this.lastUpdate = null;
            this.totalTime = 0;

            this.animationInstance = requestAnimationFrame(this.animate);
        }
    }

    public stopAnimation(): void {
        if (this.animationInstance !== false) {
            cancelAnimationFrame(this.animationInstance);
            this.animationInstance = false;
            this.lastUpdate = null;
        }
    }

    private animate  = (): void => {
        const now: number = performance.now()

        if (this.lastUpdate != null) {
            const elapsed: number = (now - this.lastUpdate) / 1000;
            this.lastUpdate = now;

            this.totalTime += elapsed;

            this.animation.update(elapsed, this.totalTime);
        } else {
            this.lastUpdate = now;
        }

        this.animationInstance = requestAnimationFrame(this.animate);
    }

    // Animations
    public abstract createIdleAnimation(): Animation;
}