# Local Release Workflow

ComPewter releases are published from a local build with `make publish`.

This is intentional: the SMAPI build package needs Stardew Valley and SMAPI assemblies at build time. Your Mac already has those files, while GitHub-hosted runners do not.

## Requirements

- Stardew Valley installed
- SMAPI installed
- .NET SDK 6.x
- GitHub CLI installed and authenticated
- Commitizen installed if you want `cz bump` to update versions and tags

Authenticate GitHub CLI once:

```sh
gh auth login
```

## Build A Release Zip

```sh
make package
```

This builds Release configuration without deploying to your Mods folder and generates:

```text
bin/Release/net6.0/ComPewter <version>.zip
```

## Publish A GitHub Release

From a clean `main` branch:

```sh
cz bump
make publish
```

`make publish` will:

- verify you are on `main`
- verify the working tree is clean
- build the Release zip locally
- create the version tag if it does not already exist
- verify an existing tag points at `HEAD`
- push `main`
- push the version tag
- create the GitHub release, or replace the uploaded zip if the release already exists

The release tag is derived from `manifest.json`. For example, version `0.3.1` publishes tag `v0.3.1`.

## If A Tag Points At The Wrong Commit

Move the tag only after confirming the current commit is the one you want to release:

```sh
git tag -f v0.3.1
git push origin v0.3.1 --force
```

Then run:

```sh
make publish
```

Use tag force-pushes carefully because they rewrite the published meaning of that version tag.
