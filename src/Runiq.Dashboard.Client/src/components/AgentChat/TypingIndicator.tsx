export function TypingIndicator() {
  return (
    <div className="inline-flex items-center gap-2 rounded-full border border-zinc-200 bg-white px-3 py-2 text-xs text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/70 dark:text-zinc-400">
      <span className="inline-flex items-center gap-1">
        <span className="size-1.5 animate-bounce rounded-full bg-current [animation-delay:-0.2s]" />
        <span className="size-1.5 animate-bounce rounded-full bg-current [animation-delay:-0.1s]" />
        <span className="size-1.5 animate-bounce rounded-full bg-current" />
      </span>
      <span>Generating response</span>
    </div>
  );
}