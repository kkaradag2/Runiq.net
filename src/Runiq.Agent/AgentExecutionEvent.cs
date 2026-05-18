namespace Runiq.Agents;

/// <summary>
/// Agent çalışması sırasında üretilen stream olayını temsil eder.
/// </summary>
public sealed record AgentExecutionEvent(
    AgentExecutionEventKind Kind,
    string? Content,
    string? ToolCallId = null)
{
    public static AgentExecutionEvent AssistantDelta(string content)
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.AssistantDelta,
            content);
    }

    public static AgentExecutionEvent ToolCallStarted(string toolCallId, string content)
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.ToolCallStarted,
            content,
            toolCallId);
    }

    public static AgentExecutionEvent ToolCallCompleted(string toolCallId, string content)
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.ToolCallCompleted,
            content,
            toolCallId);
    }

    public static AgentExecutionEvent ToolCallFailed(string toolCallId, string content)
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.ToolCallFailed,
            content,
            toolCallId);
    }

    public static AgentExecutionEvent Completed()
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.Completed,
            null);
    }

    public static AgentExecutionEvent Failed(string content)
    {
        return new AgentExecutionEvent(
            AgentExecutionEventKind.Failed,
            content);
    }
}

/// <summary>
/// Agent stream olay tiplerini belirtir.
/// </summary>
public enum AgentExecutionEventKind
{
    AssistantDelta = 0,
    ToolCallStarted = 1,
    ToolCallCompleted = 2,
    ToolCallFailed = 3,
    Completed = 4,
    Failed = 5
}
