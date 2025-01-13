namespace NetNine.Client;

using Microsoft.JSInterop;
public class WebLLMService
{
	private string model = "Llama-3.2-1B-Instruct-q4f16_1-MLC";
	private readonly Lazy<Task<IJSObjectReference>> moduleTask;

	private const string ModulePath = "./webllm-interop.js";
	public WebLLMService(IJSRuntime jsRuntime)
	{
		moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
		"import", $"{ModulePath}").AsTask());
	}

	public async Task Initialize()
	{
		var module = await moduleTask.Value;
		await module.InvokeVoidAsync("initialize", model, DotNetObjectReference.Create(this));
	}

	public event Action<InitProgress>? OnInitializingChanged;

	[JSInvokable]
	public Task OnInitializing(InitProgress status)
	{
		OnInitializingChanged?.Invoke(status);
		return Task.CompletedTask;
	}

	public async Task CompleteStreamAsync(IList<Message> messages)
	{
		var module = await moduleTask.Value;
		await module.InvokeVoidAsync("completeStream", messages);
	}

	public event Func<WebLLMCompletion, Task>? OnChunkCompletion;

	[JSInvokable]
	public Task ReceiveChunkCompletion(WebLLMCompletion response)
	{
		OnChunkCompletion?.Invoke(response);
		return Task.CompletedTask;
	}

}
public record InitProgress(float Progress, string Text, double TimeElapsed);
// A chat message
public record Message(string Role, string Content);
// A partial chat message
public record Delta(string Role, string Content);
// Chat message "cost"
public record Usage(double CompletionTokens, double PromptTokens, double TotalTokens);
// A collection of partial chat messages
public record Choice(int Index, Message? Delta, string Logprobs, string FinishReason);

// A chat stream response
public record WebLLMCompletion(
	string Id,
	string Object,
	string Model,
	string SystemFingerprint,
	Choice[]? Choices,
	Usage? Usage
	)
{
	// The final part of a chat message stream will include Usage
	public bool IsStreamComplete => Usage is not null;
} 

