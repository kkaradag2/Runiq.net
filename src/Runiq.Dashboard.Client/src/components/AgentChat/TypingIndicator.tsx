export function TypingIndicator() {
  return (
    <div
      className="flex items-center gap-1.5"
      aria-label="Agent is thinking"
      role="status"
    >
      <span className="size-2 animate-bounce rounded-full bg-zinc-400 [animation-delay:-0.24s] dark:bg-zinc-400" />
      <span className="size-2 animate-bounce rounded-full bg-zinc-400 [animation-delay:-0.12s] dark:bg-zinc-400" />
      <span className="size-2 animate-bounce rounded-full bg-zinc-400 dark:bg-zinc-400" />
    </div>
  );
}