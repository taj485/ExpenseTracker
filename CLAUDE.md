@.claude/memory.md
@FolderStructure.md


## Git conventions
- Branch naming: feat/short-description, fix/issue-number-description
- Commit format: conventional commits (feat:, fix:, refactor:, chore:)
- Never commit directly to main
- Always run tests/linting before committing
- Include "Fixes #N" in commit messages when resolving an issue
- Before committing, review `git status` / `git diff` to confirm only intended files are staged
- Prefer `git add <specific files>` over `git add -A` when a commit should be scoped to one change