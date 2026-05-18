import { ArrowUp, Mic, Paperclip } from 'lucide-react';
import { useState } from 'react';

type ChatComposerProps = {
  disabled?: boolean;
  onSubmit: (message: string) => void;
};

export function ChatComposer({ disabled = false, onSubmit }: ChatComposerProps) {
  const [message, setMessage] = useState('');

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedMessage = message.trim();

    if (!trimmedMessage || disabled) {
      return;
    }

    onSubmit(trimmedMessage);
    setMessage('');
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="rounded-lg border border-zinc-200 bg-white p-2.5 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none"
    >
      <div className="flex items-end gap-2">
        <button
          type="button"
          disabled={disabled}
          className="inline-flex size-10 shrink-0 items-center justify-center rounded-xl text-zinc-500 transition hover:bg-zinc-100 hover:text-zinc-950 disabled:cursor-not-allowed disabled:opacity-50 dark:text-zinc-500 dark:hover:bg-zinc-900 dark:hover:text-zinc-100"
          aria-label="Attach file"
        >
          <Paperclip size={18} strokeWidth={2} aria-hidden="true" />
        </button>

        <textarea
          rows={1}
          value={message}
          disabled={disabled}
          placeholder="Enter your message..."
          onChange={(event) => setMessage(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === 'Enter' && !event.shiftKey) {
              event.preventDefault();
              event.currentTarget.form?.requestSubmit();
            }
          }}
          className="max-h-36 min-h-10 flex-1 resize-none bg-transparent px-1 py-2 text-sm leading-6 text-zinc-950 outline-none placeholder:text-zinc-400 disabled:cursor-not-allowed dark:text-zinc-100 dark:placeholder:text-zinc-600"
        />

        <button
          type="button"
          disabled={disabled}
          className="inline-flex size-10 shrink-0 items-center justify-center rounded-xl text-zinc-500 transition hover:bg-zinc-100 hover:text-zinc-950 disabled:cursor-not-allowed disabled:opacity-50 dark:text-zinc-500 dark:hover:bg-zinc-900 dark:hover:text-zinc-100"
          aria-label="Use voice input"
        >
          <Mic size={18} strokeWidth={2} aria-hidden="true" />
        </button>

        <button
          type="submit"
          disabled={disabled || !message.trim()}
          className="inline-flex size-10 shrink-0 items-center justify-center rounded-xl bg-zinc-950 text-white transition hover:bg-black disabled:cursor-not-allowed disabled:opacity-50 dark:bg-white dark:text-black dark:hover:bg-zinc-200"
          aria-label="Send message"
        >
          <ArrowUp size={18} strokeWidth={2.4} aria-hidden="true" />
        </button>
      </div>
    </form>
  );
}