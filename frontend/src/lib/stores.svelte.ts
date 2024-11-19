let title: string = $state<string | undefined>(undefined);
let meta: Record<string, string> = $state({});

export function useTitle() {
  return {
    get value() {
      return title;
    },

    set value(newValue) {
      title = newValue;
    }
  };
}

export function usePageMetadata() {
  return {
    get value() {
      return meta;
    },

    clear() {
      meta = {};
    },

    set(newMeta: Record<string, string>) {
      meta = newMeta;
    }
  };
}
