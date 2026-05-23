import { Bot, FileText } from 'lucide-react';
import { useState, type ReactNode } from 'react';

import type { ContextSpaceMetadata } from '../../api/agentMetadataApi';
import { getDashboardBasePath } from '../../dashboardConfig';

type ContextSpaceInspectorPanelProps = {
  contextSpace: ContextSpaceMetadata;
};

type ContextSpaceInspectorTab = 'overview' | 'sources';

const tabs: Array<{
  key: ContextSpaceInspectorTab;
  label: string;
}> = [
  { key: 'overview', label: 'Overview' },
  { key: 'sources', label: 'Sources' },
];

export function ContextSpaceInspectorPanel({
  contextSpace,
}: ContextSpaceInspectorPanelProps) {
  const [activeTab, setActiveTab] =
    useState<ContextSpaceInspectorTab>('overview');

  return (
    <aside className="hidden w-[320px] shrink-0 rounded-lg border border-zinc-200 bg-white shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none xl:flex xl:min-h-0 xl:flex-col">
      <div className="border-b border-zinc-200 px-3 py-3 dark:border-zinc-800">
        <div className="truncate text-sm font-semibold text-zinc-950 dark:text-zinc-100">
          {contextSpace.name || contextSpace.id}
        </div>

        <div className="mt-2 grid grid-cols-2 rounded-md bg-zinc-100 p-1 text-xs font-medium dark:bg-zinc-900">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              type="button"
              onClick={() => setActiveTab(tab.key)}
              className={[
                'rounded px-2 py-1 transition',
                activeTab === tab.key
                  ? 'bg-white text-zinc-950 shadow-sm dark:bg-zinc-800 dark:text-zinc-100'
                  : 'text-zinc-500 hover:text-zinc-950 dark:text-zinc-500 dark:hover:text-zinc-200',
              ].join(' ')}
            >
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      <div className="min-h-0 flex-1 overflow-hidden p-3">
        {activeTab === 'overview' && (
          <ContextSpaceOverviewTab contextSpace={contextSpace} />
        )}

        {activeTab === 'sources' && (
          <ContextSpaceSourcesTab contextSpace={contextSpace} />
        )}
      </div>
    </aside>
  );
}

function ContextSpaceOverviewTab({
  contextSpace,
}: {
  contextSpace: ContextSpaceMetadata;
}) {
  return (
    <div className="flex min-h-0 flex-col gap-3">
      <InspectorCard title="Description">
        <p className="text-sm leading-6 text-zinc-600 dark:text-zinc-400">
          {contextSpace.description || 'No description.'}
        </p>
      </InspectorCard>

      <InspectorCard title="Summary">
        <div className="space-y-3 text-sm">
          <InspectorRow label="Sources">
            <span className="font-medium text-zinc-800 dark:text-zinc-200">
              {contextSpace.sources.length}
            </span>
          </InspectorRow>

          <InspectorRow label="Agents">
            <span className="font-medium text-zinc-800 dark:text-zinc-200">
              {contextSpace.attachedAgents.length}
            </span>
          </InspectorRow>
        </div>
      </InspectorCard>

      <ContextSpaceAttachedAgentsCard contextSpace={contextSpace} />
    </div>
  );
}

function ContextSpaceSourcesTab({
  contextSpace,
}: {
  contextSpace: ContextSpaceMetadata;
}) {
  return (
    <div className="flex min-h-0 flex-col gap-3 overflow-y-auto pr-1 [scrollbar-width:thin] [scrollbar-color:rgb(161_161_170)_transparent] dark:[scrollbar-color:rgb(82_82_91)_transparent] [&::-webkit-scrollbar]:w-1.5 [&::-webkit-scrollbar-track]:bg-transparent [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-thumb]:bg-zinc-300 dark:[&::-webkit-scrollbar-thumb]:bg-zinc-700">
      {contextSpace.sources.length === 0 ? (
        <InspectorCard title="Sources">
          <span className="text-sm font-medium text-zinc-800 dark:text-zinc-200">
            No sources attached
          </span>
        </InspectorCard>
      ) : (
        contextSpace.sources.map((source) => (
          <InspectorCard key={source.id} title={source.name || source.id}>
            <div className="flex items-start gap-3">
              <span className="inline-flex size-7 shrink-0 items-center justify-center rounded-lg border border-zinc-200 bg-white text-zinc-600 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300">
                <FileText className="size-3.5" />
              </span>

              <div className="min-w-0">
                <div className="rounded-md border border-zinc-200 bg-white px-2 py-0.5 text-xs text-zinc-600 dark:border-zinc-700 dark:bg-zinc-900 dark:text-zinc-400">
                  {formatSourceKind(source.kind)}
                </div>

                <div className="mt-2 truncate font-mono text-xs text-zinc-500 dark:text-zinc-500">
                  {source.id}
                </div>

                {source.description ? (
                  <p className="mt-3 text-sm leading-6 text-zinc-600 dark:text-zinc-400">
                    {source.description}
                  </p>
                ) : null}
              </div>
            </div>
          </InspectorCard>
        ))
      )}
    </div>
  );
}

function ContextSpaceAttachedAgentsCard({
  contextSpace,
}: {
  contextSpace: ContextSpaceMetadata;
}) {
  return (
    <InspectorCard title="Attached agents">
      {contextSpace.attachedAgents.length === 0 ? (
        <span className="text-sm font-medium text-zinc-800 dark:text-zinc-200">
          No agents attached
        </span>
      ) : (
        <div className="space-y-2">
          {contextSpace.attachedAgents.map((agent) => (
            <button
              key={agent.id}
              type="button"
              onClick={() => navigateToAgent(agent.id)}
              className="flex w-full items-center gap-3 rounded-xl border border-zinc-200 bg-white px-3 py-2 text-left transition hover:border-zinc-300 hover:bg-zinc-100 dark:border-zinc-700 dark:bg-zinc-900 dark:hover:border-zinc-600 dark:hover:bg-zinc-800"
            >
              <span className="inline-flex size-7 items-center justify-center rounded-lg border border-zinc-200 bg-zinc-50 text-zinc-600 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300">
                <Bot className="size-3.5" />
              </span>

              <div className="min-w-0">
                <div className="truncate text-sm font-medium text-zinc-950 dark:text-zinc-100">
                  {agent.name}
                </div>

                <div className="truncate font-mono text-xs text-zinc-500 dark:text-zinc-500">
                  {agent.id}
                </div>
              </div>
            </button>
          ))}
        </div>
      )}
    </InspectorCard>
  );
}

function InspectorCard({
  title,
  children,
  className = '',
}: {
  title: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <section
      className={[
        'rounded-md border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40',
        className,
      ].join(' ')}
    >
      <div className="mb-3 text-xs font-semibold uppercase tracking-[0.14em] text-zinc-500 dark:text-zinc-500">
        {title}
      </div>

      {children}
    </section>
  );
}

function InspectorRow({
  label,
  children,
}: {
  label: string;
  children: ReactNode;
}) {
  return (
    <div className="flex items-center justify-between gap-4">
      <span className="text-zinc-500 dark:text-zinc-500">{label}</span>
      <div className="min-w-0 text-right">{children}</div>
    </div>
  );
}

function navigateToAgent(agentId: string) {
  const basePath = getDashboardBasePath().replace(/\/+$/g, '');

  window.history.pushState(
    {},
    '',
    `${basePath}/agents/${encodeURIComponent(agentId)}/chat/new`,
  );

  window.dispatchEvent(new PopStateEvent('popstate'));
}

function formatSourceKind(value: string): string {
  return value
    .replace(/([a-z])([A-Z])/g, '$1 $2')
    .replace(/[-_]+/g, ' ')
    .trim();
}