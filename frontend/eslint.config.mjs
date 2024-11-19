// @ts-check

import eslint from '@eslint/js';
import eslintConfigPrettier from 'eslint-config-prettier';
import eslintPluginSvelte from 'eslint-plugin-svelte';
import globals from 'globals';
import svelteParser from 'svelte-eslint-parser';
import ts from 'typescript-eslint';

export default ts.config(
  {
    ignores: ['build', 'node_modules', '.svelte-kit']
  },
  eslint.configs.recommended,
  ...ts.configs.recommended,
  ...eslintPluginSvelte.configs['flat/recommended'],
  ...eslintPluginSvelte.configs['flat/prettier'],
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
        parser: ts.parser
      }
    }
  },
  {
    rules: {
      'svelte/valid-compile': ['error', { ignoreWarnings: true }],
      'svelte/no-at-html-tags': ['off']
    }
  }
);
