# Project Memory

## Workflow
Only follow this 3-step process when making changes to the project (creating, editing, or deleting files):
1. **Propose a plan** — describe what will change and why. Wait for approval.
2. **Show the changes** — present actual diffs/code before touching files. Wait for approval.
3. **Make the changes** — only then apply edits.

For read-only tasks (builds, searches, analysis, running tests) — just do it directly.

## Scope
- Only read files inside `c:\Users\Main\Documents\Projects\ExpenseTracker`
- Only edit files inside `c:\Users\Main\Documents\Projects\ExpenseTracker`
- All memory is written to `.claude\memory.md` in this project — never to `C:\Users\Main\.claude\projects`

## Folder Structure
After creating or deleting any file or folder, always update `FolderStructure.md` to reflect the change using the existing ASCII tree format (├──, │, └──).
