import { ArrowLeft, Wrench } from 'lucide-react';

import type { AgentToolMetadata } from '../../../api/agentMetadataApi';

type AgentToolDetailPanelProps = {
  tool: AgentToolMetadata;
  onBack: () => void;
};

export function AgentToolDetailPanel({
  tool,
  onBack,
}: AgentToolDetailPanelProps) {
  return (
    <div className="flex min-h-0 flex-col">
      <button
        type="button"
        onClick={onBack}
        className="mb-3 inline-flex items-center gap-1.5 self-start text-sm font-medium text-zinc-500 transition hover:text-zinc-950 dark:text-zinc-400 dark:hover:text-zinc-100"
      >
        <ArrowLeft className="size-4" />
        Back
      </button>

      <div className="rounded-md border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40">
        <div className="flex items-start gap-2">
          <div className="rounded-md border border-zinc-200 bg-white p-1.5 text-zinc-500 dark:border-zinc-700 dark:bg-zinc-950 dark:text-zinc-400">
            <Wrench className="size-4" />
          </div>

          <div className="min-w-0">
            <div className="truncate text-sm font-semibold text-zinc-950 dark:text-zinc-100">
              {tool.name}
            </div>
            <div className="mt-1 text-xs text-zinc-500 dark:text-zinc-500">
              Tool
            </div>
          </div>
        </div>

        <div className="mt-4 space-y-4">
          <DetailSection title="Description">
            {formatDescription(tool.description)}
          </DetailSection>

          <DetailSection title="Input">
            {tool.inputType || 'Unknown'}
          </DetailSection>

          <DetailSection title="Output">
            {tool.outputType || 'Unknown'}
          </DetailSection>
        </div>
      </div>
    </div>
  );
}

function DetailSection({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <section>
      <div className="text-xs font-semibold uppercase tracking-[0.14em] text-zinc-500 dark:text-zinc-500">
        {title}
      </div>
      <div className="mt-1 break-words text-sm leading-6 text-zinc-800 dark:text-zinc-200">
        {children}
      </div>
    </section>
  );
}

function formatDescription(description: string | undefined): string {
  const trimmedDescription = description?.trim();

  if (!trimmedDescription) {
    return 'No description provided.';
  }

  return trimmedDescription;
}