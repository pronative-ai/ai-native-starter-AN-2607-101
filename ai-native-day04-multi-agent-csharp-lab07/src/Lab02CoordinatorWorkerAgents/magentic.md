```sh
┌────────────────────────────────────────────────────────┐
│            ORCHESTRATION LAYER: Magentic               │
│  (Maintains the plan ledger, selects next speaker)     │
└──────────────────────────┬─────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│ Worker Agent  │  │ Worker Agent  │  │ Worker Agent  │
│  (Harness)    │  │  (Harness)    │  │  (Standard)   │
│               │  │               │  │               │
│ - Has Shell   │  │ - Has Sandbox │  │ - Simple Text │
│ - Has Files   │  │ - Writes Code │  │   Generator   │
└───────────────┘  └───────────────┘  └───────────────┘

```