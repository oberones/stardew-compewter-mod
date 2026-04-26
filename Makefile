SHELL := /bin/bash

PROJECT := ComPewter.csproj
MOD_NAME := ComPewter
CONFIGURATION ?= Release
TARGET_FRAMEWORK := net6.0
VERSION := $(shell python3 -c 'import json; print(json.load(open("manifest.json"))["Version"])')
TAG := v$(VERSION)
PACKAGE_PATH := bin/$(CONFIGURATION)/$(TARGET_FRAMEWORK)/$(MOD_NAME) $(VERSION).zip
RELEASE_NOTES ?= CHANGELOG.md

.PHONY: help build package verify-release ensure-tag push-tag publish clean

help:
	@echo "ComPewter release helpers"
	@echo ""
	@echo "Targets:"
	@echo "  make build          Build the mod locally without deploying to Stardew."
	@echo "  make package        Build the Release zip locally."
	@echo "  make publish        Build locally, push main/tag, and create or update the GitHub release."
	@echo "  make clean          Remove build output."
	@echo ""
	@echo "Current version: $(VERSION)"
	@echo "Current tag:     $(TAG)"

build:
	dotnet build $(PROJECT) -p:EnableModDeploy=false

package:
	dotnet build $(PROJECT) --configuration $(CONFIGURATION) -p:EnableModDeploy=false
	@test -f "$(PACKAGE_PATH)" || (echo "Expected package was not generated: $(PACKAGE_PATH)" && exit 1)
	@echo "Package ready: $(PACKAGE_PATH)"

verify-release:
	@command -v dotnet >/dev/null || (echo "dotnet is required." && exit 1)
	@command -v gh >/dev/null || (echo "GitHub CLI is required. Install it and run: gh auth login" && exit 1)
	@command -v python3 >/dev/null || (echo "python3 is required." && exit 1)
	@test "$$(git branch --show-current)" = "main" || (echo "Release publishing must run from main." && exit 1)
	@test -z "$$(git status --porcelain)" || (echo "Working tree is not clean. Commit or stash changes before publishing." && exit 1)
	@manifest_version="$$(python3 -c 'import json; print(json.load(open("manifest.json"))["Version"])')" && \
	  test "$(VERSION)" = "$$manifest_version" || (echo "Makefile version cache does not match manifest.json." && exit 1)

ensure-tag:
	@if git rev-parse --verify --quiet "refs/tags/$(TAG)" >/dev/null; then \
	  echo "Using existing tag $(TAG)."; \
	else \
	  git tag -a "$(TAG)" -m "$(MOD_NAME) $(TAG)"; \
	  echo "Created tag $(TAG)."; \
	fi

push-tag:
	@if git ls-remote --exit-code --tags origin "refs/tags/$(TAG)" >/dev/null 2>&1; then \
	  echo "Remote tag $(TAG) already exists; leaving it unchanged."; \
	else \
	  git push origin "$(TAG)"; \
	fi

publish: verify-release package ensure-tag
	git push origin main
	$(MAKE) push-tag
	@if gh release view "$(TAG)" >/dev/null 2>&1; then \
	  gh release upload "$(TAG)" "$(PACKAGE_PATH)" --clobber; \
	  echo "Updated GitHub release asset for $(TAG)."; \
	else \
	  if [ -f "$(RELEASE_NOTES)" ]; then \
	    gh release create "$(TAG)" "$(PACKAGE_PATH)" --title "$(MOD_NAME) $(TAG)" --notes-file "$(RELEASE_NOTES)"; \
	  else \
	    gh release create "$(TAG)" "$(PACKAGE_PATH)" --title "$(MOD_NAME) $(TAG)" --notes "$(MOD_NAME) release $(TAG)."; \
	  fi; \
	  echo "Created GitHub release $(TAG)."; \
	fi

clean:
	dotnet clean $(PROJECT)
