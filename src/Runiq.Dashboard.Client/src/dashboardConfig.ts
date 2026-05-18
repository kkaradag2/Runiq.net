declare global {
  interface Window {
    __RUNIQ_DASHBOARD__?: {
      basePath?: string;
      title?: string;
    };
  }
}

export function getDashboardBasePath(): string {
  const configuredBasePath = window.__RUNIQ_DASHBOARD__?.basePath ?? '/runiq';

  if (configuredBasePath === '/') {
    return '';
  }

  return configuredBasePath.replace(/\/+$/, '');
}

export function getDashboardTitle(): string {
  return window.__RUNIQ_DASHBOARD__?.title ?? 'Runiq Studio';
}