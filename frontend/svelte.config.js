import azure from 'svelte-adapter-azure-swa';
import preprocess from 'svelte-preprocess';

/** @type {import('@sveltejs/kit').Config} */
const config = {
  // Consult https://github.com/sveltejs/svelte-preprocess
  // for more information about preprocessors
  preprocess: preprocess(),

  kit: {
    adapter: azure({
      customStaticWebAppConfig: {
        platform: {
          apiRuntime: 'node:18'
        },
        globalHeaders: {
          'X-Content-Type-Options': 'nosniff',
          'X-Frame-Options': 'DENY',
          'Content-Security-Policy-Report-Only':
            "default-src 'self'; img-src cdn.ldam.co.za; style-src cdn.jsdelivr.net fonts.googleapis.com; script-src az416426.vo.msecnd.net cdn.jsdelivr.net"
        }
      }
    })
  }
};

export default config;
