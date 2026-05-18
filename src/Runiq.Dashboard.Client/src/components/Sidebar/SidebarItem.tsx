type SidebarItemProps = {
  label: string;
  active: boolean;
  collapsed: boolean;
  onClick: () => void;
};

export function SidebarItem({
  label,
  active,
  collapsed,
  onClick,
}: SidebarItemProps) {
  return (
    <button
      type="button"
      title={label}
      onClick={onClick}
      className={[
        'flex items-center gap-3 rounded-lg px-3 py-2.5 text-left text-sm transition',
        collapsed ? 'lg:justify-center lg:px-0' : '',
        active
          ? 'bg-zinc-100 text-zinc-950 ring-1 ring-zinc-200 dark:bg-zinc-900 dark:text-white dark:ring-zinc-800'
          : 'text-zinc-500 hover:bg-zinc-100 hover:text-zinc-950 dark:text-zinc-400 dark:hover:bg-zinc-900/70 dark:hover:text-zinc-100',
      ].join(' ')}
    >
      <span
        className={[
          'size-2 shrink-0 rounded-full',
          active ? 'bg-zinc-950 dark:bg-white' : 'bg-zinc-400 dark:bg-zinc-600',
        ].join(' ')}
      />

      <span className={collapsed ? 'lg:hidden' : ''}>{label}</span>
    </button>
  );
}