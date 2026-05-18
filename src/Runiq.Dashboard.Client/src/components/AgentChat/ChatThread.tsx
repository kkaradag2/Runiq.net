import { useEffect, useRef } from 'react';

import './ChatThread.css';

import type { AgentChatMessage } from '../../types/agentChat';
import { TypingIndicator } from './TypingIndicator';

type ChatThreadProps = {
  messages: AgentChatMessage[];
  isWaiting: boolean;
};

export function ChatThread({ messages, isWaiting }: ChatThreadProps) {
  const bottomRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ block: 'end' });
  }, [messages, isWaiting]);

  return (
    <section className="flex min-h-0 flex-1 flex-col rounded-lg border border-zinc-200 bg-white shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none">
      {messages.length === 0 && !isWaiting ? (
        <div className="flex min-h-0 flex-1 items-center justify-center px-6 py-10">
          <div className="max-w-md text-center">
            <div className="text-sm font-medium text-zinc-950 dark:text-zinc-100">
              Start a conversation with this agent
            </div>

            <p className="mt-2 text-sm leading-6 text-zinc-600 dark:text-zinc-500">
              Messages will appear here.
            </p>
          </div>
        </div>
      ) : (
        <div className="agent-chat-scroll flex min-h-0 flex-1 flex-col gap-4 overflow-y-auto px-4 py-5">
          {messages.map((message) => (
            <article
              key={message.id}
              className={[
                'max-w-[78%] rounded-2xl px-4 py-3 text-sm leading-6',
                message.role === 'user'
                  ? 'ml-auto w-fit max-w-[70%] bg-zinc-950 text-white dark:bg-zinc-100 dark:text-zinc-950'
                  : '',
                message.role === 'assistant'
                  ? 'mr-auto w-fit max-w-[min(860px,82%)] border border-zinc-200 bg-white text-zinc-900 shadow-sm dark:border-zinc-800 dark:bg-zinc-900/60 dark:text-zinc-100 dark:shadow-none'
                  : '',
                message.role === 'error'
                  ? 'mr-auto w-fit max-w-[min(860px,82%)] border border-red-200 bg-red-50 text-red-700 dark:border-red-900/60 dark:bg-red-950/20 dark:text-red-300'
                  : '',
              ].join(' ')}
            >
              <div className="whitespace-pre-wrap break-words">
                {message.content ? message.content : <TypingIndicator />}
              </div>
            </article>
          ))}

          {isWaiting && (
            <article className="mr-auto w-fit max-w-[min(860px,82%)] rounded-2xl border border-zinc-200 bg-white px-5 py-3 text-sm leading-7 text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-900/60 dark:text-zinc-400 dark:shadow-none">
              <TypingIndicator />
            </article>
          )}

          <div ref={bottomRef} />
        </div>
      )}
    </section>
  );
}