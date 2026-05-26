import { Users } from 'lucide-react';

import type { TeamMetadata } from '../../api/agentMetadataApi';

type TeamInspectorPanelProps = {
  team: TeamMetadata;
};

export function TeamInspectorPanel({ team }: TeamInspectorPanelProps) {
  return (
    <aside className="hidden w-[320px] shrink-0 rounded-lg border border-zinc-200 bg-white shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none xl:flex xl:min-h-0 xl:flex-col">
      <div className="border-b border-zinc-200 px-3 py-3 dark:border-zinc-800">
        <div className="truncate text-sm font-semibold text-zinc-950 dark:text-zinc-100">
          {team.name || team.id}
        </div>

        <div className="mt-1 text-xs text-zinc-500 dark:text-zinc-500">
          Agent Team
        </div>
      </div>

      <div className="agent-chat-scroll min-h-0 flex-1 space-y-3 overflow-y-auto p-3">
        <section className="rounded-lg border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40">
          <div className="flex items-center gap-2 text-xs font-semibold uppercase tracking-wide text-zinc-500 dark:text-zinc-500">
            <Users className="size-3.5" />
            Overview
          </div>

          <dl className="mt-3 space-y-2 text-sm">
            <div>
              <dt className="text-xs text-zinc-500 dark:text-zinc-500">Mode</dt>
              <dd className="mt-1 text-zinc-950 dark:text-zinc-100">
                {team.executionMode}
              </dd>
            </div>

            <div>
              <dt className="text-xs text-zinc-500 dark:text-zinc-500">Members</dt>
              <dd className="mt-1 text-zinc-950 dark:text-zinc-100">
                {team.members.length}
              </dd>
            </div>
          </dl>
        </section>

        <section className="rounded-lg border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40">
          <div className="text-xs font-semibold uppercase tracking-wide text-zinc-500 dark:text-zinc-500">
            Team Members
          </div>

          <div className="mt-3 space-y-2">
            {team.members.length === 0 ? (
              <div className="text-sm text-zinc-500 dark:text-zinc-500">
                No members configured.
              </div>
            ) : (
              team.members.map((member) => (
                <div
                  key={`${member.agentId}:${member.role}`}
                  className="rounded-md border border-zinc-200 bg-white p-2.5 dark:border-zinc-800 dark:bg-zinc-950/50"
                >
                  <div className="text-sm font-medium text-zinc-950 dark:text-zinc-100">
                    {member.role}
                  </div>

                  <div className="mt-1 truncate text-xs text-zinc-500 dark:text-zinc-500">
                    {member.agentId}
                  </div>

                  {member.instructions && (
                    <p className="mt-2 text-xs leading-5 text-zinc-600 dark:text-zinc-400">
                      {member.instructions}
                    </p>
                  )}
                </div>
              ))
            )}
          </div>
        </section>

        <section className="rounded-lg border border-zinc-200 bg-zinc-50 p-3 dark:border-zinc-800 dark:bg-zinc-900/40">
          <div className="text-xs font-semibold uppercase tracking-wide text-zinc-500 dark:text-zinc-500">
            Instructions
          </div>

          <p className="mt-3 whitespace-pre-wrap text-sm leading-6 text-zinc-700 dark:text-zinc-300">
            {team.instructions || 'No instructions configured.'}
          </p>
        </section>
      </div>
    </aside>
  );
}