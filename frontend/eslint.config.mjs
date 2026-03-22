// @ts-check

import eslint from '@eslint/js';
import eslintConfigPrettier from 'eslint-config-prettier';
import eslintPluginSvelte from 'eslint-plugin-svelte';
import { defineConfig } from 'eslint/config';
import globals from 'globals';
import svelteParser from 'svelte-eslint-parser';
import tseslint from 'typescript-eslint';
export default defineConfig(
  {
    ignores: ['build', 'node_modules', '.svelte-kit']
  },
  eslint.configs.recommended,
  tseslint.configs.recommended,
  eslintPluginSvelte.configs['flat/recommended'],
  eslintPluginSvelte.configs['flat/prettier'],
  eslintConfigPrettier,
  {
    files: ['*.{svelte,svelte.ts}', '**/*.{svelte,svelte.ts}'],
    languageOptions: {
      globals: {
        __VERSION__: 'readonly',
        __LASTMOD__: 'readonly',
        ...globals.browser
      },
      parser: svelteParser,
      parserOptions: {
        parser: tseslint.parser
      }
    }
  },
  {
    rules: {
      'svelte/valid-compile': ['error', { ignoreWarnings: true }],
      'svelte/no-at-html-tags': ['off'],
      'svelte/no-navigation-without-resolve': 'off'
    }
  }
);
