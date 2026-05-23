import { Database, FileText } from 'lucide-react';
import type { ContextSpaceMetadata } from '../../api/agentMetadataApi';

type ContextSpaceSourcesPanelProps = {
  contextSpace: ContextSpaceMetadata;
};

export function ContextSpaceSourcesPanel({
  contextSpace,
}: ContextSpaceSourcesPanelProps) {
  return (
    <section className="flex min-h-0 min-w-0 flex-1 rounded-lg border border-zinc-200 bg-white shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none">
      <aside className="flex w-[340px] shrink-0 flex-col border-r border-zinc-200 dark:border-zinc-800">
        <div className="border-b border-zinc-200 px-5 py-4 dark:border-zinc-800">
          <div className="text-sm font-semibold text-zinc-950 dark:text-zinc-100">
            Sources
          </div>

          <p className="mt-1 text-sm leading-6 text-zinc-600 dark:text-zinc-400">
            Sources available inside this context space.
          </p>
        </div>

        <div className="min-h-0 flex-1 overflow-y-auto px-4 py-4 [scrollbar-width:thin] [scrollbar-color:rgb(161_161_170)_transparent] dark:[scrollbar-color:rgb(82_82_91)_transparent] [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-track]:bg-transparent [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-thumb]:bg-zinc-300 dark:[&::-webkit-scrollbar-thumb]:bg-zinc-700">
          {contextSpace.sources.length === 0 ? (
            <div className="rounded-md border border-zinc-200 bg-zinc-50 p-4 text-sm text-zinc-600 dark:border-zinc-800 dark:bg-zinc-900/40 dark:text-zinc-400">
              No sources attached.
            </div>
          ) : (
            <div className="space-y-2">
              {contextSpace.sources.map((source) => (
                <button
                  key={source.id}
                  type="button"
                  className="flex w-full items-start gap-3 rounded-md border border-zinc-200 bg-zinc-50 px-3 py-3 text-left transition hover:border-zinc-300 hover:bg-zinc-100 dark:border-zinc-800 dark:bg-zinc-900/40 dark:hover:border-zinc-700 dark:hover:bg-zinc-900"
                >
                  <span className="inline-flex size-8 shrink-0 items-center justify-center rounded-lg border border-zinc-200 bg-white text-zinc-600 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300">
                    <FileText className="size-4" />
                  </span>

                  <span className="min-w-0">
                    <span className="block truncate text-sm font-medium text-zinc-950 dark:text-zinc-100">
                      {source.name || source.id}
                    </span>

                    <span className="mt-0.5 block truncate font-mono text-xs text-zinc-500 dark:text-zinc-500">
                      {source.id}
                    </span>
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>
      </aside>

      <div className="flex min-h-0 min-w-0 flex-1 flex-col">
        <div className="border-b border-zinc-200 px-5 py-4 dark:border-zinc-800">
          <div className="text-sm font-semibold text-zinc-950 dark:text-zinc-100">
            Context Overview
          </div>

          <p className="mt-1 text-sm leading-6 text-zinc-600 dark:text-zinc-400">
            Read-only runtime boundary exposed to attached agents.
          </p>
        </div>

        <div className="min-h-0 flex-1 px-5 py-5">
          <div className="h-full rounded-md border border-dashed border-zinc-200 bg-zinc-50 p-5 dark:border-zinc-800 dark:bg-zinc-900/30">
            <div className="flex items-start gap-4">
              <span className="inline-flex size-10 shrink-0 items-center justify-center rounded-lg border border-zinc-200 bg-white text-zinc-700 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300">
                <Database className="size-5" />
              </span>

              <div className="min-w-0">
                <div className="text-base font-semibold text-zinc-950 dark:text-zinc-100">
                  {contextSpace.name || contextSpace.id}
                </div>

                <div className="mt-1 font-mono text-xs text-zinc-500 dark:text-zinc-500">
                  {contextSpace.id}
                </div>

                <p className="mt-4 max-w-3xl text-sm leading-6 text-zinc-600 dark:text-zinc-400">
                  {contextSpace.description || 'No description configured.'}
                </p>

                <div className="mt-6 grid max-w-xl grid-cols-2 gap-3">
                  <ContextMetric label="Sources" value={contextSpace.sources.length} />
                  <ContextMetric
                    label="Attached Agents"
                    value={contextSpace.attachedAgents.length}
                  />
                </div>
              </div>
            </div>

            {contextSpace.sources.length > 0 ? (
              <div className="mt-8">
                <div className="mb-3 text-xs font-semibold uppercase tracking-[0.14em] text-zinc-500 dark:text-zinc-500">
                  Source Details
                </div>

                <div className="space-y-3">
                  {contextSpace.sources.map((source) => (
                    <div
                      key={source.id}
                      className="rounded-md border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-950/50"
                    >
                      <div className="flex flex-wrap items-center gap-2">
                        <div className="text-sm font-semibold text-zinc-950 dark:text-zinc-100">
                          {source.name || source.id}
                        </div>

                        <span className="rounded-md border border-zinc-200 bg-zinc-50 px-2 py-0.5 text-xs text-zinc-600 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-400">
                          {formatSourceKind(source.kind)}
                        </span>
                      </div>

                      <div className="mt-1 font-mono text-xs text-zinc-500 dark:text-zinc-500">
                        {source.id}
                      </div>

                      {source.description ? (
                        <p className="mt-3 text-sm leading-6 text-zinc-600 dark:text-zinc-400">
                          {source.description}
                        </p>
                      ) : null}
                    </div>
                  ))}
                </div>
              </div>
            ) : null}
          </div>
        </div>
      </div>
    </section>
  );
}

function ContextMetric({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-md border border-zinc-200 bg-white px-4 py-3 dark:border-zinc-800 dark:bg-zinc-950/50">
      <div className="text-xs font-medium uppercase tracking-[0.12em] text-zinc-500 dark:text-zinc-500">
        {label}
      </div>

      <div className="mt-2 text-lg font-semibold text-zinc-950 dark:text-zinc-100">
        {value}
      </div>
    </div>
  );
}

function formatSourceKind(value: string): string {
  return value
    .replace(/([a-z])([A-Z])/g, '$1 $2')
    .replace(/[-_]+/g, ' ')
    .trim();
}