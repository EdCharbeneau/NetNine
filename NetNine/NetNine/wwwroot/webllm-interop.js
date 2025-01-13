import * as webllm from "webllm";

var engine; // <-- hold a reference to MLCEngine in the module
var dotnetInstance; // <-- hold a reference to the WebLLMService instance in the module

// Callback function to update model loading progress
const initProgressCallback = (initProgress) => {
    dotnetInstance.invokeMethodAsync("OnInitializing", initProgress);
}

export async function initialize(selectedModel, dotnet) {
    dotnetInstance = dotnet;
    console.log(dotnet);
    engine = await webllm.CreateMLCEngine(
        selectedModel,
        { initProgressCallback: initProgressCallback }, // engineConfig
    );
}

export async function complete(messages) {
    var reply = await engine.chat.completions.create({
        messages,
    });
    return reply;
}


export async function completeStream(messages) {
    // Chunks is an AsyncGenerator object
    const chunks = await engine.chat.completions.create({
        messages,
        temperature: 1,
        stream: true, // <-- Enable streaming
        stream_options: { include_usage: true },
    });

    for await (const chunk of chunks) {
        //console.log(chunk);
        await dotnetInstance.invokeMethodAsync("ReceiveChunkCompletion", chunk);
    }
}