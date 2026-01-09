import type { RequestEvent } from '@sveltejs/kit';

export function handleError({ error, event }) {
  const deets = extractUsefulDetails(event);
  console.error('handleError', error, deets);
  try {
    if (event?.platform?.context?.error && typeof event.platform.context.error === 'function') {
      event.platform.context.error('SvelteKit Error', {
        ...deets,
        error
      });
    }
  } catch (err) {
    console.error('handleError => logging failed!', err);
  }

  return {
    message: 'Something went wrong :('
  };
}

export async function handle({ event, resolve }) {
  const start = Date.now();
  const response = await resolve(event);
  const end = Date.now();
  try {
    if (event?.platform?.context?.info && typeof event.platform.context.info === 'function') {
      event.platform.context.info('SvelteKit Event', {
        ...extractUsefulDetails(event),
        duration: end - start,
        status: response.status
      });
    }
  } catch (err) {
    console.error(err);
  }
  return response;
}

function extractUsefulDetails(event: RequestEvent) {
  return {
    method: event.request.method,
    url: event.url.href,
    route: event.route.id
  };
}
