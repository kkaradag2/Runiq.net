import { Search, Users } from 'lucide-react';
import { useEffect, useMemo, useState } from 'react';

import { getTeams, type TeamMetadata } from '../api/agentMetadataApi';
import { DataList, type DataListColumn } from '../components/DataList/DataList';
import { getDashboardBasePath } from '../dashboardConfig';

const teamColumns: DataListColumn<TeamMetadata>[] = [
    {
        key: 'team',
        header: 'Team',
        width: 'minmax(240px, 1fr)',
        render: (team) => (
            <div className="flex min-w-0 items-center gap-3">
                <span className="inline-flex size-8 shrink-0 items-center justify-center rounded-xl border border-zinc-200 bg-zinc-50 text-zinc-600 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-300">
                    <Users className="size-4" />
                </span>

                <div className="min-w-0">
                    <div className="truncate text-sm font-semibold text-zinc-950 dark:text-zinc-100">
                        {team.name || team.id}
                    </div>
                </div>
            </div>
        ),
    },
    {
        key: 'mode',
        header: 'Mode',
        width: 'minmax(140px, 0.6fr)',
        render: (team) => (
            <span className="inline-flex max-w-full truncate rounded-full border border-zinc-200 bg-zinc-100 px-2.5 py-1 text-xs font-medium text-zinc-800 dark:border-zinc-800 dark:bg-zinc-950 dark:text-zinc-300">
                {team.executionMode}
            </span>
        ),
    },
    {
        key: 'members',
        header: 'Members',
        width: '120px',
        render: (team) => (
            <div className="text-sm font-medium text-zinc-700 dark:text-zinc-300">
                {team.members.length}
            </div>
        ),
    },
    {
        key: 'instructions',
        header: 'Instructions',
        width: 'minmax(360px, 2fr)',
        render: (team) => (
            <div
                className="truncate text-sm text-zinc-600 dark:text-zinc-400"
                title={team.instructions}
            >
                {truncateText(team.instructions || 'No instructions configured.', 72)}
            </div>
        ),
    },
];

export function TeamsPage() {
    const [teams, setTeams] = useState<TeamMetadata[]>([]);
    const [filter, setFilter] = useState('');
    const [isLoading, setLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    useEffect(() => {
        let isMounted = true;

        async function loadTeams() {
            try {
                setLoading(true);
                setErrorMessage(null);

                const result = await getTeams(getDashboardBasePath());

                if (!isMounted) {
                    return;
                }

                setTeams(result);
            } catch (error) {
                if (!isMounted) {
                    return;
                }

                setErrorMessage(
                    error instanceof Error
                        ? error.message
                        : 'Teams metadata could not be loaded.',
                );
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        }

        void loadTeams();

        return () => {
            isMounted = false;
        };
    }, []);

    const filteredTeams = useMemo(() => {
        const normalizedFilter = filter.trim().toLowerCase();

        if (!normalizedFilter) {
            return teams;
        }

        return teams.filter((team) => {
            return (
                team.id.toLowerCase().includes(normalizedFilter) ||
                team.name.toLowerCase().includes(normalizedFilter) ||
                team.executionMode.toLowerCase().includes(normalizedFilter) ||
                team.instructions?.toLowerCase().includes(normalizedFilter) ||
                team.members.some((member) =>
                    `${member.agentId} ${member.role} ${member.instructions ?? ''}`
                        .toLowerCase()
                        .includes(normalizedFilter),
                )
            );
        });
    }, [filter, teams]);

    if (isLoading) {
        return (
            <div className="rounded-2xl border border-zinc-200 bg-white p-6 text-sm text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-500 dark:shadow-none">
                Loading teams...
            </div>
        );
    }

    if (errorMessage) {
        return (
            <div className="rounded-2xl border border-red-200 bg-red-50 p-6 text-sm text-red-700 shadow-sm dark:border-red-900/60 dark:bg-red-950/20 dark:text-red-300 dark:shadow-none">
                {errorMessage}
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="rounded-2xl border border-zinc-200 bg-white p-6 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:shadow-none">
                <div className="flex items-center justify-between gap-4">
                    <div className="flex min-w-0 items-start gap-3">
                        <span className="inline-flex size-9 shrink-0 items-center justify-center rounded-xl border border-zinc-200 bg-zinc-50 text-zinc-700 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-300">
                            <Users className="size-4" />
                        </span>

                        <div className="min-w-0">
                            <div className="text-sm font-semibold text-zinc-950 dark:text-zinc-100">
                                Agent Teams
                            </div>

                            <p className="mt-1 text-sm text-zinc-600 dark:text-zinc-400">
                                Coordinated groups of specialized agents registered in this runtime.
                            </p>
                        </div>
                    </div>

                    <div className="inline-flex shrink-0 items-center gap-2 rounded-full border border-zinc-200 bg-zinc-50 px-4 py-2 text-sm font-medium text-zinc-700 dark:border-zinc-800 dark:bg-zinc-900 dark:text-zinc-300">
                        <span className="size-1.5 rounded-full bg-emerald-500" />
                        {teams.length} teams
                    </div>
                </div>
            </div>

            <div className="max-w-xl">
                <label className="relative block">
                    <Search className="pointer-events-none absolute left-4 top-1/2 size-4 -translate-y-1/2 text-zinc-400 dark:text-zinc-600" />

                    <input
                        value={filter}
                        onChange={(event) => setFilter(event.target.value)}
                        placeholder="Filter by name"
                        className="h-11 w-full rounded-xl border border-zinc-200 bg-white pl-11 pr-4 text-sm text-zinc-950 outline-none transition placeholder:text-zinc-400 focus:border-zinc-400 dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-100 dark:placeholder:text-zinc-600 dark:focus:border-zinc-600"
                    />
                </label>
            </div>

            {teams.length === 0 ? (
                <div className="rounded-2xl border border-zinc-200 bg-white p-8 text-sm text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-500 dark:shadow-none">
                    No teams registered.
                </div>
            ) : filteredTeams.length === 0 ? (
                <div className="rounded-2xl border border-zinc-200 bg-white p-8 text-sm text-zinc-500 shadow-sm dark:border-zinc-800 dark:bg-zinc-950/40 dark:text-zinc-500 dark:shadow-none">
                    No teams found.
                </div>
            ) : (
                <DataList
                    rows={filteredTeams}
                    columns={teamColumns}
                    getRowKey={(team) => team.id}
                    onRowClick={(team) => {
                        window.history.pushState(
                            {},
                            '',
                            buildTeamChatPath(getDashboardBasePath(), team.id),
                        );

                        window.dispatchEvent(new PopStateEvent('popstate'));
                    }}
                />
            )}
        </div>
    );
}

function buildTeamChatPath(basePath: string, teamId: string): string {
  const normalizedBasePath = basePath.replace(/\/+$/g, '');

  return `${normalizedBasePath}/teams/${encodeURIComponent(teamId)}/chat/new`;
}

function truncateText(value: string, maxLength: number): string {
    if (value.length <= maxLength) {
        return value;
    }

    return `${value.slice(0, maxLength - 1)}…`;
}