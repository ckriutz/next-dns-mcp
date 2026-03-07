# NextDNS MCP Server

A .NET 10 MCP (Model Context Protocol) server that provides a natural-language interface to the [NextDNS API](https://nextdns.github.io/api/).

## Features

- **Connection verification** — Confirm your API key and profile are valid
- **Analytics** — Query DNS analytics (status breakdown, top domains, devices)
- **Parental Controls** — Check and toggle YouTube blocking

## Tools

| Tool | Description |
|------|-------------|
| `check_connection` | Verify API connectivity and profile info |
| `get_analytics_status` | Query counts by status (default/blocked/allowed) |
| `get_top_domains` | Top queried domains with optional filters |
| `get_devices` | Device list with query counts |
| `get_youtube_status` | Check if YouTube is blocked |
| `set_youtube_blocked` | Block or unblock YouTube |

## Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A NextDNS account with an API key ([find yours here](https://my.nextdns.io/account))
- Your NextDNS profile ID (visible in the URL when viewing a profile: `https://my.nextdns.io/<PROFILE_ID>`)

### Build

```bash
dotnet build
```

### Configure in Claude Desktop

Add this to your Claude Desktop config (`~/Library/Application Support/Claude/claude_desktop_config.json` on macOS):

```json
{
  "mcpServers": {
    "nextdns": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/NextDnsMcp"],
      "env": {
        "NEXTDNS_API_KEY": "your-api-key-here",
        "NEXTDNS_PROFILE_ID": "your-profile-id"
      }
    }
  }
}
```

### Configure in VS Code (GitHub Copilot)

Add to your `.vscode/mcp.json`:

```json
{
  "servers": {
    "nextdns": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/NextDnsMcp"],
      "env": {
        "NEXTDNS_API_KEY": "your-api-key-here",
        "NEXTDNS_PROFILE_ID": "your-profile-id"
      }
    }
  }
}
```

## Usage Examples

Once connected, you can ask your AI assistant things like:

- *"Is YouTube blocked right now?"*
- *"Block YouTube on the kids profile"*
- *"Unblock YouTube"*
- *"How many queries were blocked today?"*
- *"What are the top domains in the last 7 days?"*
- *"What devices are connected?"*
- *"Is my NextDNS connection working?"*
