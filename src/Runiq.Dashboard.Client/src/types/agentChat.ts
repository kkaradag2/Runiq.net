export type AgentChatMethod = 'generate' | 'stream';

export type AgentChatMessageRole = 'user' | 'assistant' | 'error';

export type AgentChatMessage = {
  id: string;
  role: AgentChatMessageRole;
  content: string;
};