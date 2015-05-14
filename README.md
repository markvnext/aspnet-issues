# aspnet-issues
ASP.NET 5 site that shows ASP.NET 5 open issues

## Running

```
dnu restore
dnx . kestrel
```

You'll need to set the `GitHubToken` option, either in config.json or as an environment variable.

## Usage

N.B. On the milestone page, you can put e.g. `*-beta5` to show all open issues across 1.0.0-beta5 and 6.0.0-beta5.
