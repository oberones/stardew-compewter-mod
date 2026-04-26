# Troubleshooting

## ComPewter says no provider is configured

Set `Provider` to `Anthropic`, `OpenAI`, `Ollama`, or `Custom`, then fill in that provider's model and credentials.

## Invalid key or token

Check the configured key/token and provider account permissions. ComPewter maps authentication failures to a friendly chat error and redacts secrets from logs.

## Ollama unavailable

Make sure Ollama is running and the configured model is installed:

```sh
ollama list
ollama pull llama3.1
ollama serve
```

Keep `Ollama.BaseUrl` at `http://localhost:11434` unless your local setup uses a different host.

## Timeout

Increase `RequestTimeoutSeconds` or use a faster/local model. The game should remain responsive while a request is pending.

## Malformed response

For Custom providers, confirm the endpoint returns OpenAI-compatible JSON with `choices[0].message.content`.

## Context is not affecting answers

Enable:

```json
{
  "Privacy": {
    "ShareGameContext": true
  }
}
```

Then ask a question that depends on the current season, weather, inventory, or money.
