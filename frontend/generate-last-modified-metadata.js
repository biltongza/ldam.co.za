import fs from 'fs';
import { glob } from 'glob';
import path from 'path';
const routesPath = path.join('src', 'routes');
glob(`${routesPath}/**/*.svelte`).then((matches) => {
	const lastModified = matches.map((match) => ({
		path: match,
		lastModified: fs.statSync(match).mtimeMs
	}));
	const lastModifiedJson = `export const metadata = ${JSON.stringify(lastModified)}`;
	fs.writeFileSync(path.join('src', 'lib', '.metadata.js'), lastModifiedJson);
	console.log('wrote thing!');
}, (err) => {
	if (err) {
		throw err;
	}
});
