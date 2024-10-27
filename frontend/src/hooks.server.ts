export function handleError({ error, event }) {
  try {
    if (
      event?.platform?.context?.log?.error &&
      typeof event.platform.context.log.error === 'function'
    ) {
      event.platform.context.log.info('SvelteKit Error', {
        method: event.request.method,
        url: event.url.href,
        route: event.route.id,
        error
      });
    }
  } catch (err) {
    console.error(err);
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
    if (
      event?.platform?.context?.log?.info &&
      typeof event.platform.context.log.info === 'function'
    ) {
      event.platform.context.log.info('SvelteKit Event', {
        method: event.request.method,
        url: event.url.href,
        route: event.route.id,
        duration: end - start,
        status: response.status
      });
    }
  } catch (err) {
    console.error(err);
  }
  return response;
}
