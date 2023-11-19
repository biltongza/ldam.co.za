import type { RequestEvent } from '@sveltejs/kit';

export function handleError({ error, event }) {
  try {
    if (
      event?.platform?.context?.log?.error &&
      typeof event.platform.context.log.error === 'function'
    ) {
      const details = getSimpleRequestDetails(event);
      event.platform.context.log.error({ error, details });
    }
  } catch (err) {
    console.error(err);
  }

  return {
    message: 'Something went wrong :('
  };
}

export async function handle({ event, resolve }) {
  const response = await resolve(event);
  try {
    if (
      event?.platform?.context?.log?.info &&
      typeof event.platform.context.log.info === 'function'
    ) {
      const simpleRequest = getSimpleRequestDetails(event);
      const simpleResponse = getSimpleResponseDetails(response);
      event.platform.context.log.info({ request: simpleRequest, response: simpleResponse });
    }
  } catch (err) {
    console.error(err);
  }
  return response;
}

function getSimpleRequestDetails(event: RequestEvent) {
  const { method, url, referrer, headers } = event.request;
  return { method, url, referrer, headers: Object.fromEntries(headers.entries()) };
}

function getSimpleResponseDetails(response: Response) {
  const { status, headers } = response;
  return { status, headers: Object.fromEntries(headers.entries()) };
}
