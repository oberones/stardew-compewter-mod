# Contract: Prompt And Context

## Prompt Envelope

Prompt construction outputs a provider-neutral envelope with these sections:

1. System instructions
2. Privacy and spoiler settings
3. Optional game context
4. Optional current-session conversation history
5. Current player question

## System Instruction Requirements

The assistant is instructed to:
- act as a cozy, friendly Stardew Valley gameplay helper;
- provide concise, practical advice;
- use current game context when available;
- avoid spoilers unless allowed or explicitly requested;
- admit uncertainty;
- avoid pretending to know modded content unless context provides it;
- recommend checking in-game sources when uncertain.

## Context Inclusion Rules

When `ShareGameContext` is false:
- no automatic game context is included;
- the prompt may include only assistant instructions, allowed history, and the
  player question.

When `ShareGameContext` is true:
- safe context categories may be included;
- sensitive categories require separate opt-in;
- unavailable categories are omitted or marked only when useful.

## Safe Context Categories

- date, season, year, day of week;
- weather and tomorrow weather when available;
- daily luck;
- current money;
- farm type;
- current location if not personally identifying;
- player skill levels;
- compact inventory summary;
- active quest summary;
- spoiler preference.

## Opt-In Context Categories

- friendship summary;
- installed mods list;
- multiplayer player data.

## Excluded By Default

- player name;
- farm name;
- save name;
- machine paths;
- multiplayer player names;
- secrets.

## Compactness Rules

- summarize lists;
- cap retained history;
- omit empty categories;
- avoid full save dumps;
- preserve enough context to answer daily planning questions.
