# Unity MCP Setup

AfterBlue uses MCP Unity to let Codex inspect and automate the Unity Editor.

## Installed Package

Unity Package Manager dependency:

```json
"com.gamelovers.mcp-unity": "https://github.com/CoderGamester/mcp-unity.git"
```

The package is declared in `Packages/manifest.json`. Unity resolves it when the project is opened.

## Codex MCP Server

The local Node MCP server is installed outside the project:

```text
C:/Users/minil/.codex/mcp/mcp-unity
```

Codex global config entry:

```toml
[mcp_servers.mcp-unity]
command = 'C:\Program Files\nodejs\node.exe'
args = ['C:\Users\minil\.codex\mcp\mcp-unity\Server~\build\index.js']
startup_timeout_sec = 30
```

## How To Start

1. Restart Codex so it loads the new MCP server.
2. Open the AfterBlue Unity project.
3. Wait for Unity Package Manager to resolve `com.gamelovers.mcp-unity`.
4. In Unity, open `Tools > MCP Unity > Server Window`.
5. Click `Start Server`.
6. In Codex, confirm Unity tools are available with `tool_search`.

## Notes

- The Unity Editor WebSocket server runs on port `8090` by default.
- If Unity enters Play Mode and the MCP connection drops, reconnect from the MCP Unity server window.
- Keep scene edits focused and commit Unity scene changes separately from code changes when practical.
