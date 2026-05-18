import { useEffect, useState } from 'react';

import { sendAgentMessage, streamAgentMessage } from '../api/agentChatApi';
import { getAgents, type AgentMetadata } from '../api/agentMetadataApi';
import { AgentInspectorPanel } from '../components/AgentChat/AgentInspectorPanel';
import { ChatComposer } from '../components/AgentChat/ChatComposer';
import { ChatThread } from '../components/AgentChat/ChatThread';
import { getDashboardBasePath } from '../dashboardConfig';
import type { AgentChatMessage, AgentChatMethod } from '../types/agentChat';

type AgentChatPageProps = {
  agentId: string;
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

export function AgentChatPage({ agentId }: AgentChatPageProps) {
  const [agent, setAgent] = useState<AgentMetadata | null>(null);
  const [isLoading, setLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [chatMethod, setChatMethod] = useState<AgentChatMethod>('stream');
  const [messages, setMessages] = useState<AgentChatMessage[]>([]);
  const [isExecuting, setExecuting] = useState(false);

  useEffect(() => {
    let isMounted = true;

    async function loadAgent() {
      try {
        setLoading(true);
        setErrorMessage(null);

        const agents = await getAgents(getDashboardBasePath());
        const selectedAgent =
          agents.find((item) => item.id.toLowerCase() === agentId.toLowerCase()) ??
          null;

        if (!isMounted) {
          return;
        }

        if (!selectedAgent) {
          setAgent(null);
          setErrorMessage(`Agent '${agentId}' could not be found.`);
          return;
        }

        setAgent(selectedAgent);
      } catch (error) {
        if (!isMounted) {
          return;
        }

        setAgent(null);
        setErrorMessage(
          error instanceof Error ? error.message : 'Agent metadata could not be loaded.',
        );
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    }

    void loadAgent();

    return () => {
      isMounted = false;
    };
  }, [agentId]);

  async function handleSubmit(message: string) {
    const basePath = getDashboardBasePath();

    setMessages((current) => [...current, createMessage('user', message)]);
    setExecuting(true);

    try {
      if (chatMethod === 'generate') {
        const assistantResponse = await sendAgentMessage({
          basePath,
          agentId,
          message,
        });

        setMessages((current) => [
          ...current,
          createMessage('assistant', assistantResponse),
        ]);

        return;
      }

      const assistantMessage = createMessage('assistant', '');

      setMessages((current) => [...current, assistantMessage]);

      await streamAgentMessage(
        {
          basePath,
          agentId,
          message,
        },
        (chunk) => {
          if (!chunk) {
            return;
          }

          setMessages((current) =>
            current.map((item) =>
              item.id === assistantMessage.id
                ? {
                    ...item,
                    content: item.content + chunk,
                  }
                : item,
            ),
          );
        },
      );
    } catch (error) {
      setMessages((current) => [
        ...current,
        createMessage(
          'error',
          error instanceof Error ? error.message : 'Agent execution failed.',
        ),
      ]);
    } finally {
      setExecuting(false);
    }
  }

  if (isLoading) {
    return (
      <div className="flex h-full min-h-0 w-full items-center justify-center rounded-lg border border-zinc-200 bg-white text-sm text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-500 dark:shadow-none">
        Loading agent metadata...
      </div>
    );
  }

  if (errorMessage || !agent) {
    return (
      <div className="flex h-full min-h-0 w-full items-center justify-center rounded-lg border border-red-200 bg-red-50 px-6 text-center text-sm text-red-700 shadow-sm dark:border-red-900/60 dark:bg-red-950/20 dark:text-red-300 dark:shadow-none">
        {errorMessage ?? 'Agent metadata could not be loaded.'}
      </div>
    );
  }

  return (
    <div className="flex h-full min-h-0 w-full gap-3">
      <section className="flex min-w-0 flex-1 flex-col gap-2.5">
        <ChatThread
          messages={messages}
          isWaiting={isExecuting && chatMethod === 'generate'}
        />

        <ChatComposer disabled={isExecuting} onSubmit={handleSubmit} />
      </section>

      <AgentInspectorPanel
        agent={agent}
        chatMethod={chatMethod}
        onChatMethodChange={setChatMethod}
      />
    </div>
  );
}