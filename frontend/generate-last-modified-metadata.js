import fs from 'fs';
import { glob } from 'glob';
import path from 'path';
const routesPath = path.join('src', 'routes');
const lastModified = (await glob(`${routesPath}/**/*.svelte`)).map((match) => ({
	path: match,
	lastModified: fs.statSync(match).mtimeMs
}));
const lastModifiedJson = `export const metadata = ${JSON.stringify(lastModified)}`;
fs.writeFileSync(path.join('src', 'lib', '.metadata.js'), lastModifiedJson);
