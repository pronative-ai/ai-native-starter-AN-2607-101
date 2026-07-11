# Student Support Triage Flow

Use this order:

1. Local runtime
   - Check `.NET SDK 10`.
   - Check VS Code and Foundry Toolkit extension.
   - Check Git clone or copied starter repository.

2. Identity
   - Confirm the student is using the assigned training identity.
   - Ask for `az account show` only if Azure access is part of the failure.

3. Foundry
   - Confirm the student can see the shared Foundry project.
   - If they can edit trainer-owned objects unexpectedly, escalate RBAC review.

4. Azure resource access
   - Check resource group access for the student-specific resource group.
   - Check managed identity role assignments if runtime calls fail.

5. Service dependency
   - Azure AI Search issues usually need trainer or platform action.
   - Model deployment or quota issues usually need trainer action.

Output format:

- symptom
- likely category
- evidence requested
- next action
- escalation owner
