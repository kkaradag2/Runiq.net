import type { AgentMetadata } from '../../../api/agentMetadataApi';

type AgentMemoryTabProps = {
  agent: AgentMetadata;
};

export function AgentMemoryTab({ agent: _agent }: AgentMemoryTabProps) {
  return (
    <div className="space-y-3">
      <InspectorCard title="Memory">
        <OverviewRow label="Status" value="Off" />
      </InspectorCard>

      <p className="text-sm leading-6 text-zinc-500 dark:text-zinc-500">
        This agent does not have memory configured yet.
      </p>
    </div>
  );
}

function InspectorCard({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <section className="rounded-md border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40">
      <div className="text-xs font-semibold uppercase tracking-[0.14em] text-zinc-500 dark:text-zinc-500">
        {title}
      </div>

      <div className="mt-3 space-y-2.5 text-sm">{children}</div>
    </section>
  );
}

function OverviewRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-center justify-between gap-4">
      <span className="text-zinc-500 dark:text-zinc-500">{label}</span>
      <span className="truncate text-right font-medium text-zinc-800 dark:text-zinc-200">
        {value}
      </span>
    </div>
  );
}