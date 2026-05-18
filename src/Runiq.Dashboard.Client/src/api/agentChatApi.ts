type SendAgentMessageRequest = {
  basePath: string;
  agentId: string;
  message: string;
};

type AgentChatResponse = {
  response?: string;
  message?: string;
  content?: string;
};

function trimTrailingSlash(value: string): string {
  return value.endsWith('/') ? value.slice(0, -1) : value;
}

function resolveResponseText(response: AgentChatResponse): string {
  return response.response ?? response.message ?? response.content ?? '';
}

export async function sendAgentMessage({
  basePath,
  agentId,
  message,
}: SendAgentMessageRequest): Promise<string> {
  const response = await fetch(
    `${trimTrailingSlash(basePath)}/api/agents/${encodeURIComponent(agentId)}/chat`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ message }),
    },
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Agent chat request failed. Status: ${response.status}`);
  }

  const payload = (await response.json()) as AgentChatResponse;
  const responseText = resolveResponseText(payload);

  if (!responseText) {
    throw new Error('Agent response was empty.');
  }

  return responseText;
}

export async function streamAgentMessage(
  { basePath, agentId, message }: SendAgentMessageRequest,
  onChunk: (chunk: string) => void,
): Promise<void> {
  const response = await fetch(
    `${trimTrailingSlash(basePath)}/api/agents/${encodeURIComponent(agentId)}/chat/stream`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'text/event-stream',
      },
      body: JSON.stringify({ message }),
    },
  );

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || `Agent stream request failed. Status: ${response.status}`);
  }

  if (!response.body) {
    throw new Error('Agent stream response body was empty.');
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();

  let buffer = '';

  while (true) {
    const { value, done } = await reader.read();

    if (done) {
      break;
    }

    buffer += decoder.decode(value, { stream: true });

    const events = buffer.split(/\r?\n\r?\n/);
    buffer = events.pop() ?? '';

    for (const event of events) {
      const chunk = parseServerSentEvent(event);

      if (chunk) {
        onChunk(chunk);
      }
    }
  }

  buffer += decoder.decode();

  const finalChunk = parseServerSentEvent(buffer);

  if (finalChunk) {
    onChunk(finalChunk);
  }
}
function parseServerSentEvent(event: string): string {
  const dataLines = event
    .split('\n')
    .map((line) => line.trim())
    .filter((line) => line.startsWith('data:'));

  if (dataLines.length === 0) {
    return '';
  }

  return dataLines
    .map((line) => line.replace(/^data:\s?/, ''))
    .filter((data) => data && data !== '[DONE]')
    .map(extractChunkText)
    .join('');
}

function extractChunkText(data: string): string {
  try {
    const parsed = JSON.parse(data) as unknown;

    if (typeof parsed === 'string') {
      return parsed;
    }

    if (!parsed || typeof parsed !== 'object') {
      return '';
    }

    const chunk = parsed as {
      delta?: string;
      content?: string;
      message?: string;
      response?: string;
      text?: string;
      item?: {
        content?: Array<{
          text?: string;
        }>;
      };
    };

    return (
      chunk.delta ??
      chunk.content ??
      chunk.message ??
      chunk.response ??
      chunk.text ??
      chunk.item?.content?.map((content) => content.text ?? '').join('') ??
      ''
    );
  } catch {
    return data;
  }
}