# Blender MCP Setup

AfterBlue uses a local Blender MCP bridge so Codex can inspect and modify Blender scenes through tools instead of only generating blind Python scripts.

## Installed Components

- MCP server clone: `C:\Users\minil\.codex\mcp\blender-mcp-server`
- Server executable: `C:\Users\minil\.codex\mcp\blender-mcp-server\.venv\Scripts\blender-mcp-server.exe`
- Blender add-on: `Blender MCP Bridge`
- Blender bridge port: `127.0.0.1:9876`
- Approved script root: `C:\Users\minil\GameMaking\AfterBlue`

## Codex Configuration

The local Codex config contains:

```toml
[mcp_servers.blender]
command = 'C:\Users\minil\.codex\mcp\blender-mcp-server\.venv\Scripts\blender-mcp-server.exe'
startup_timeout_sec = 30
```

Restart Codex after changing MCP config. The current session usually cannot see newly registered MCP tools until a restart.

## Blender Startup

Open Blender with the add-on enabled. The MCP side panel should show:

```text
Listening on 127.0.0.1:9876
```

If needed, open:

```text
Edit > Preferences > Add-ons > Blender MCP Bridge
```

Verify:

- Port: `9876`
- Allow Inline Code: enabled
- Approved Script Roots: `C:\Users\minil\GameMaking\AfterBlue`

## Connection Test

From the MCP server folder:

```powershell
.\.venv\Scripts\python.exe scripts\blender_scene_info.py
```

Expected result:

```json
{
  "success": true
}
```

## Usage Rule

Use Blender MCP for visible asset work that needs inspection, transforms, previews, or export checks. Use plain Blender Python only for batch generation or repeatable asset scripts.

