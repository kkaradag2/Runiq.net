import { useEffect, useState } from 'react';

import { getTeams, type TeamMetadata } from '../api/agentMetadataApi';
import { ChatComposer } from '../components/AgentChat/ChatComposer';
import { ChatThread } from '../components/AgentChat/ChatThread';
import { TeamInspectorPanel } from '../components/TeamChat/TeamInspectorPanel';
import { getDashboardBasePath } from '../dashboardConfig';
import type { AgentChatMessage } from '../types/agentChat';

type TeamChatPageProps = {
  teamId: string;
};

function createMessage(role: AgentChatMessage['role'], content: string): AgentChatMessage {
  return {
    id:
      typeof crypto !== 'undefined' && 'randomUUID' in crypto
        ? crypto.randomUUID()
        : `${Date.now()}-${Math.random()}`,
    role,
    content,
  };
}

export function TeamChatPage({ teamId }: TeamChatPageProps) {
  const [team, setTeam] = useState<TeamMetadata | null>(null);
  const [isLoading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [messages] = useState<AgentChatMessage[]>([
    createMessage(
      'assistant',
      'Team playground is ready. Multi-agent execution endpoint will be connected next.',
    ),
  ]);

  useEffect(() => {
    let isMounted = true;

    async function loadTeam() {
      try {
        setLoading(true);
        setErrorMessage(null);

        const teams = await getTeams(getDashboardBasePath());
        const selectedTeam =
          teams.find((item) => item.id.toLowerCase() === teamId.toLowerCase()) ??
          null;

        if (!isMounted) {
          return;
        }

        if (!selectedTeam) {
          setTeam(null);
          setErrorMessage(`Agent team '${teamId}' could not be found.`);
          return;
        }

        setTeam(selectedTeam);
      } catch (error) {
        if (!isMounted) {
          return;
        }

        setTeam(null);
        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Agent team metadata could not be loaded.',
        );
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    void loadTeam();

    return () => {
      isMounted = false;
    };
  }, [teamId]);

  if (isLoading) {
    return (
      <div className="flex h-full min-h-0 w-full items-center justify-center rounded-lg border border-zinc-200 bg-white text-sm text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-500 dark:shadow-none">
        Loading agent team metadata...
      </div>
    );
  }

  if (errorMessage || !team) {
    return (
      <div className="flex h-full min-h-0 w-full items-center justify-center rounded-lg border border-red-200 bg-red-50 px-6 text-center text-sm text-red-700 shadow-sm dark:border-red-900/60 dark:bg-red-950/20 dark:text-red-300 dark:shadow-none">
        {errorMessage ?? 'Agent team metadata could not be loaded.'}
      </div>
    );
  }

  return (
    <div className="flex h-full min-h-0 w-full gap-3">
      <section className="flex min-w-0 flex-1 flex-col gap-2.5">
        <ChatThread messages={messages} isWaiting={false} />

        <ChatComposer disabled onSubmit={() => {}} />
      </section>

      <TeamInspectorPanel team={team} />
    </div>
  );
}