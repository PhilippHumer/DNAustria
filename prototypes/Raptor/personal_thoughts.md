**Process:**

For the backend we used Raptor Mini in Copilot VS Code. We used the same starting prompt, which was also used for the frontend implementation. Firstly, we copied the starting prompt into the vs code copilot chat with the request that the AI should only implement the backend part. The first implementation did not start, because of some db errors. But after a few iterations the backend worked. No code was implemented by us. The whole implementation was created by Raptor. 

In the end the backend is working and it kind of does whats defined in the starting prompt. A few things did not work in the beginning, like the JSON export part. But after a few more iterations it worked and the resulting json was valid (checked with the DNAustria Online Validator). The implementation was not perfect, for example the backend URL was hardcoded, but with a few more iterations where i pointed it out, the AI did change it.

Short description on the key characteristics ([Raptor Mini: GitHub Copilot’s New Code-First AI Model That Developers Shouldn’t Ignore - DEV Community](https://dev.to/koolkamalkishor/raptor-mini-github-copilots-new-code-first-ai-model-that-developers-shouldnt-ignore-44a4)):

**Key characteristics include:**

- **Large context window (~264k tokens)** – enabling the model to understand entire folders, modules, or multi-file diffs.
- **High output capacity (~64k tokens)** – perfect for long refactors and structured diffs.
- **Supports tool calling, multi-file editing, and code-aware agents**.
- **Integrated directly into VS Code** – no external API needed.
- **Optimized for code generation, transformation, and workspace-based reasoning**.

**Some personal pros and cons:**

pros:

- it's very easy and fast to get an application that does something

- it's very easy to adapt the current implementation via prompting. For example, the AI implemented a Address object, but the frontend expected an Organization object instead. So i just wrote a few lines as a prompt "Change the Address model with the same data to an Organization model" and the AI did all the changes from db changes to code changes and also adapted the tests

- the AI Integration in VS Code Copilot knows the context of the app and so you can easily create some docker integration for example 

- if changes were made in the implementation the AI automatically updated the tests

cons:

- there are some limitation to the request you can send to the AI agent. If you run into those limitation during implementation phase it is really annoying because you need to check at what stage the current implementation is and need to adapt or finish the implementation yourself

- some decisions regarding the implementation are not really good for example that you have an organization with street, postal_code, city, state and an address elements which is just a string containing all those elmements. From my point of view, there is no need for this element. Or another example is, that the URL for the backend was hardcoded.
