import { sveltekit } from '@sveltejs/kit/vite';
import { exec } from 'child_process';
import { promisify } from 'util';
import { defineConfig } from 'vite';

// Get current tag/commit and last commit date from git
const pexec = promisify(exec);
const promises = await Promise.allSettled([
  pexec('git describe --tags || git rev-parse --short HEAD'),
  pexec('git log -1 --format=%cd --date=format:"%Y-%m-%d %H:%M"')
]);
console.log('VITE CONFIG: promises', promises);

const [version, lastmod] = promises.map((v) =>
  JSON.stringify(v.status == 'fulfilled' && v.value?.stdout.trim())
);
console.log(`VITE CONFIG: commit hash`, version);
console.log(`VITE CONFIG: commit time`, lastmod);

export default defineConfig({
  plugins: [sveltekit()],
  server: {
    port: 5050
  },
  define: {
    __VERSION__: version,
    __LASTMOD__: lastmod
  }
});
