export function handleError({ error, event }) {
  if (event?.platform?.context?.log?.error && typeof event.platform.context.log.error === 'function') {
    const {method, url, referrer, headers} = event.request;
    event.platform.context.log.error(error, {method, url, referrer, headers: Object.fromEntries(headers.entries())});
  }

  return {
    message: 'Something went wrong :('
  };
}
