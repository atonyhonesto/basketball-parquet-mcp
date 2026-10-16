# Project context and decisions

## The second effort

The initial Parquet MCP project explored a general way for an AI client to discover and query local columnar data through C# tools. The basketball follow-up applied that access pattern to an NBA basketball-team use case using historical playoff files and questions about player performance across a series.

The demonstration was shared in a team-related discussion. The public material deliberately omits private messages, recipient identities, employer-specific requirements and any suggestion of team endorsement. The existing general server remains a separate project; this repository makes the basketball application of the idea easy to find and run.

## What the public edition demonstrates

An analyst asks a question, the client discovers the available data, and tools return structured evidence. Some questions require several files or pages. The client must account for the complete series before making a series-wide claim.

For the featured free-throw example, this edition adds a deterministic server-side calculation. Sum makes and attempts across the series; apply the minimum-attempt condition to the combined attempts; calculate the percentage from the two sums. Do not average per-game percentages. Return the denominator and source filenames so an engineer can verify the answer.

## Data contract

Each row represents one player in one game. `PlayerId` identifies a player across games; `(GameId, PlayerId)` is unique across all loaded files. `GameNumber` orders games. The fields in `PlayerGame.cs` define the contract.

The sample contains three games and four fictional players. It is not a reconstruction of an actual playoff series. Duplicate game/player rows cause the query to fail, rather than inflating totals. Impossible count relationships and missing identities are rejected. Zero-attempt shooters do not enter the shooting leaderboard.

Historical source files can contain nested objects and repeated arrays with different grains. A production ingestion step must explicitly relate game, team and player records before writing flat rows. Assigning values from flattened leaf arrays by position is not a safe substitute. This edition accepts only the normalized schema and does not bundle an adapter for the original files.

## Limits and future work

- The local store reads validated data on demand. It is not an analytical database or a streaming service.
- Directory access is read-only through the tools. The separate sample generator writes fictional fixtures when explicitly invoked.
- Limits: 100 top-level files, 10 MiB per file, 20,000 combined rows, and 500 rows per requested page. Use trusted data only; these do not establish a hostile-file sandbox.
- Source filenames help track coverage, but immutable dataset manifests and content hashes would be needed for stronger provenance.
- Named player comparisons, general aggregates and play-by-play analyses are possible next slices. They are not claimed as implemented tools here.
- No model-generated narrative is treated as a validated finding. Tests cover deterministic data handling and arithmetic.

## Dependency maintenance

The public edition retains the original SDK/library family and explicitly uses the patched Snappier 1.3.1 transitive dependency. NuGet vulnerability auditing includes transitive dependencies and treats reported vulnerability warnings as restore errors. See the [upstream advisory](https://github.com/advisories/GHSA-pggp-6c3x-2xmx).
