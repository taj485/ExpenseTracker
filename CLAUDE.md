@.claude/memory.md

## Repo map
- `Client/`: Angular SPA. Conventions in `Client/CLAUDE.md`.
- `ExpenseTrackerAPI/`, `ExpenseTracker.Application/`, `.Domain/`, `.Infrastructure/`, `.Tests/`: .NET API (Clean Architecture). Conventions in `ExpenseTracker.Application/CLAUDE.md`.
- Full file map: `FolderStructure.md`. Don't read it up front; use the `folder-structure` skill when you need to find something.
- To run locally without Auth0, use AI dev mode (see README).

## Git conventions
- Branch naming: feat/short-description, fix/issue-number-description
- Commit format: conventional commits (feat:, fix:, refactor:, chore:)
- Never commit directly to main
- Always run tests/linting before committing
- Include "Fixes #N" in commit messages when resolving an issue
- Before committing, review `git status` / `git diff` to confirm only intended files are staged
- Prefer `git add <specific files>` over `git add -A` when a commit should be scoped to one change
