import { defineConfig } from "vite";
import { resolve } from "node:path";
import { fileURLToPath } from "node:url";

const rootDir = fileURLToPath(new URL(".", import.meta.url));

export default defineConfig({
    base: "/",

    build: {
        outDir: "../frontend",
        emptyOutDir: true,

        rollupOptions: {
            input: {
                home: resolve(rootDir, "index.html"),
                universe: resolve(rootDir, "universe.html")
            }
        }
    }
});