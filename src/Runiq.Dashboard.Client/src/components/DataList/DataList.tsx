import type { ReactNode } from 'react';

export type DataListColumn<T> = {
  key: string;
  header: string;
  width: string;
  render: (row: T) => ReactNode;
};

type DataListProps<T> = {
  rows: T[];
  columns: DataListColumn<T>[];
  getRowKey: (row: T) => string;
  onRowClick?: (row: T) => void;
};

export function DataList<T>({
  rows,
  columns,
  getRowKey,
  onRowClick,
}: DataListProps<T>) {
  const gridTemplateColumns = columns.map((column) => column.width).join(' ');

  return (
    <div className="overflow-hidden rounded-2xl border border-zinc-200 bg-white shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none">
      <div
        className="hidden border-b border-zinc-200 px-5 py-3 text-xs font-medium uppercase tracking-[0.12em] text-zinc-500 dark:border-zinc-800 dark:text-zinc-500 lg:grid lg:gap-4"
        style={{ gridTemplateColumns }}
      >
        {columns.map((column) => (
          <div key={column.key}>{column.header}</div>
        ))}
      </div>

      <div className="divide-y divide-zinc-200 dark:divide-zinc-800">
        {rows.map((row) => (
          <DataListRow
            key={getRowKey(row)}
            row={row}
            columns={columns}
            gridTemplateColumns={gridTemplateColumns}
            onClick={onRowClick ? () => onRowClick(row) : undefined}
          />
        ))}
      </div>
    </div>
  );
}

type DataListRowProps<T> = {
  row: T;
  columns: DataListColumn<T>[];
  gridTemplateColumns: string;
  onClick?: () => void;
};

function DataListRow<T>({
  row,
  columns,
  gridTemplateColumns,
  onClick,
}: DataListRowProps<T>) {
  const isClickable = Boolean(onClick);

  return (
    <div
      role={isClickable ? 'button' : undefined}
      tabIndex={isClickable ? 0 : undefined}
      onClick={onClick}
      onKeyDown={(event) => {
        if (!onClick) {
          return;
        }

        if (event.key === 'Enter' || event.key === ' ') {
          event.preventDefault();
          onClick();
        }
      }}
      className={[
        'group px-5 py-4 transition-all duration-150 lg:grid lg:items-center lg:gap-4',
        isClickable
          ? 'cursor-pointer hover:bg-zinc-100 hover:shadow-[inset_3px_0_0_rgb(24_24_27)] focus:outline-none focus-visible:bg-zinc-100 focus-visible:shadow-[inset_3px_0_0_rgb(24_24_27)] dark:hover:bg-zinc-900/70 dark:hover:shadow-[inset_3px_0_0_rgb(244_244_245)] dark:focus-visible:bg-zinc-900/70 dark:focus-visible:shadow-[inset_3px_0_0_rgb(244_244_245)]'
          : '',
      ].join(' ')}
      style={{ gridTemplateColumns }}
    >
      {columns.map((column) => (
        <div key={column.key} className="min-w-0 [&+&]:mt-3 lg:[&+&]:mt-0">
          <div className="mb-1 text-[10px] font-medium uppercase tracking-[0.12em] text-zinc-500 dark:text-zinc-600 lg:hidden">
            {column.header}
          </div>

          {column.render(row)}
        </div>
      ))}
    </div>
  );
}