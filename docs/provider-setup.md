# Provider Setup

ComPewter defaults to `Disabled`. In that state, the chat UI opens and explains how to configure a provider instead of failing.

## Anthropic

```json
{
  "Provider": "Anthropic",
  "Anthropic": {
    "BaseUrl": "https://api.anthropic.com",
    "Model": "claude-sonnet-4-5",
    "ApiKey": "sk-ant-..."
  }
}
```

ComPewter calls `/v1/messages` and sends the API key in `x-api-key`. Keys are read from config and redacted from logs.

## OpenAI

```json
{
  "Provider": "OpenAI",
  "OpenAI": {
    "BaseUrl": "https://api.openai.com",
    "Model": "gpt-4.1-mini",
    "ApiKey": "sk-..."
  }
}
```

ComPewter calls `/v1/chat/completions` with a bearer token.

## Ollama

```json
{
  "Provider": "Ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3.1"
  }
}
```

Start Ollama before asking a question. ComPewter calls `/api/chat` with `stream=false`.

## Custom

```json
{
  "Provider": "Custom",
  "Custom": {
    "BaseUrl": "https://example.local",
    "EndpointPath": "/v1/chat/completions",
    "Model": "my-model",
    "AuthHeaderName": "Authorization",
    "AuthToken": "Bearer token",
    "RequestFormat": "OpenAICompatible"
  }
}
```

The v1 custom provider supports OpenAI-compatible request and response JSON. Use `EndpointPath` when your endpoint is not `/v1/chat/completions`.

## Common Settings

- `RequestTimeoutSeconds`: provider timeout, clamped to 5-120 seconds.
- `MaxResponseTokens`: response budget, clamped to 100-4000.
- `MaxRetainedMessages`: current-session history cap, clamped to 0-100.
- `OpenMenuKey`: defaults to `F8`.
