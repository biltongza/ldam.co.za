import azure from 'svelte-adapter-azure-swa';
import { sveltePreprocess } from 'svelte-preprocess';

/** @type {import('@sveltejs/kit').Config} */
const config = {
  // Consult https://github.com/sveltejs/svelte-preprocess
  // for more information about preprocessors
  preprocess: sveltePreprocess(),

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
    }),
    prerender: {
      handleHttpError: (details) => {
        throw details;
      },
      origin: 'http://localhost:4280'
    }
  },
  compilerOptions: {
    warningFilter: (warning) => {
      if (warning.code === 'a11y_click_events_have_key_events') return false;
      if (warning.code === 'a11y_no_static_element_interactions') return false;
      return true;
    }
  }
};

export default config;
