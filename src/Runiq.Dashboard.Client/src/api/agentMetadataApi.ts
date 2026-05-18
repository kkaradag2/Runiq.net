export type AgentToolMetadata = {
  name: string;
  description?: string;
  inputType?: string;
  outputType?: string;
};

export type AgentMetadata = {
  id: string;
  name: string;
  instructions?: string;
  model?: string;
  reasoningEffort?: string;
  verbosity?: string;
  tools?: AgentToolMetadata[];
};

export async function getAgents(basePath: string): Promise<AgentMetadata[]> {
  const response = await fetch(`${basePath}/metadata/agents`);

  if (!response.ok) {
    throw new Error('Agents metadata could not be loaded.');
  }

  return response.json() as Promise<AgentMetadata[]>;
}