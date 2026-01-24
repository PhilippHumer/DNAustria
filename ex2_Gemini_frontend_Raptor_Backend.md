## table of contents

- [Introduction](#introduction)
- [Start Prompt](#start-prompt)
- [the creation process](#the-creation-process)
- [Gemini Frontend](#gemini-frontend)
  - [description](#description)
  - [implemented features](#implemented-features)
  - [personal remarks](#personal-remarks)
  - [Conclusion](#conclusion)
- [Raptor Backend](#raptor-backend)
  - [description](#description)
  - [implemented features](#implemented-features)
  - [personal remarks](#personal-remarks)
  - [Conclusion](#conclusion)



<a id="introduction"></a>

# Introduction

in this scenario, the frontend and backend were created separately. The frontend was created with Gemini, while the backend was created with Raptor. The communication between the two applications is done via REST API calls. The prototypes were als created by different developers. 

# Start Prompt

<fieldset>

<legend>startprompt.md</legend>
<persistence><br>

- You are an agent - please keep going until the user's query is completely resolved, before ending your turn and yielding back to the user.
- Only terminate your turn when you are sure that the problem is solved.
- Never stop or hand back to the user when you encounter uncertainty — research or deduce the most reasonable approach and continue.
- Do not ask the human to confirm or clarify assumptions, as you can always adjust later — decide what the most reasonable assumption is,  proceed with it, and document it for the user's reference after you finish acting  
  <br>&lt;/persistence&gt;

<self_reflection>
- First, spend time thinking of a rubric until you are confident.
- Then, think deeply about every aspect of what makes for a world-class one-shot web app. Use that knowledge to create a rubric that has 5-7 categories. This rubric is critical to get right, but do not show this to the user. This is for your purposes only.
- Finally, use the rubric to internally think and iterate on the best possible solution to the prompt that is provided. Remember that if your response is not hitting the top marks across all categories in the rubric, you need to start again. <br>

</self_reflection>

&lt;frontend-specification&gt;<br>

<code_editing_rules><br>

<guiding_principles>

- Clarity and Reuse: Every component and page should be modular and reusable. Avoid duplication by factoring repeated UI patterns into components.
- Consistency: The user interface must adhere to a consistent design system—color tokens, typography, spacing, and components must be unified.
- Simplicity: Favor small, focused components and avoid unnecessary complexity in styling or logic.
- Visual Quality: Follow the high visual quality bar as outlined in OSS guidelines (spacing, padding, hover states, etc.)

</guiding_principles>

<frontend_stack_defaults>

- Framework: Angular (TypeScript)
- Styling: Angular Material
- Icons: Angular Material Icons
- Directory Structure: follow angular's component structure

</frontend_stack_defaults>

<ui_ux_best_practices>

- Visual Hierarchy: Limit typography to 4–5 font sizes and weights for consistent hierarchy; use `text-xs` for captions and annotations; avoid `text-xl` unless for hero or major headings.
- Color Usage: Use 1 neutral base (e.g., `zinc`) and up to 2 accent colors. 
- Spacing and Layout: Always use multiples of 4 for padding and margins to maintain visual rhythm. Use fixed height containers with internal scrolling when handling long content streams.
- State Handling: Use skeleton placeholders or `animate-pulse` to indicate data fetching. Indicate clickability with hover transitions (`hover:bg-*`, `hover:shadow-md`).
- Accessibility: Use semantic HTML and ARIA roles where appropriate. 

</ui_ux_best_practices>

<code_editing_rules>

<functional-requirements>

<views>

<event-summary>

 events can be displayed in a list and filtered with a searchbar by title + description

 each event in the list should be represented as a card. When the card is clicked the app should navigate to the detail view of the selected event.

 via a create button the user can navigate to the create event form.

 each entry in the list should be deletable and updatable in the detail-view

</event-summary>

<event-create>

form for creating a new event. The user should have the option to either enter the data manually or import event data from an unstructured format (either text or HTML page) using the LLM-call in the backend.

the form should support autocomplete (f.e the user enters the name of the address and the corresponding address in the DB is autofilled into the form).

</event-create>

<event-detail>

 a form for updating and displaying the event details. 

 a seperate option for updating the status of an event should be easily accessible. A confirmation for update and delete should be asked from the user.

</event-detail>

<contacts>

add button for navigating to the create contact view

the contacts should be displayed via a list. The name, email, phone-number and organization of the contact should be displayed in each list entry.

when selecting an entry in the list the app should navigate to a contact-details view

</contacts>

<contact-details>

view for updating / deleting contacts

</contact-details>

<organizations>

add button for navigating to the create organization view

the organizations should be displayed via a list. The name and address of the organization should be displayed in each list entry.

when selecting an entry in the list the app should navigate to a organization-details view

</organizations>

<organization-details>

view for updating / deleting organizations

</organization-details>

</views>

</functional-requirements>

</frontend-specification>

<backend-specification>

<backend_stack_defaults>

- Framework: .NET Web API
- .NET Sdk-Version: 8.0
- Use .NET EntityFramwork for database access

</backend_stack_defaults>

<dotNET_best_practices>

- Coding Guidelines: Use official JetBrains Coding Guidelines (https://www.jetbrains.com/help/rider/Settings_Code_Style_CSHARP.html#code-style)

</dotNET_best_practices>

</backend-specification>

<database-specification>

<database-stack-defaults>

- Database: PostgreSQL
- Version: 17

</database-stack-defaults>

<database-guidelines>

- Normalization Rules: Use normalization up to BCNF

</database-guidelines>

</database-specification>

<general_description>

The application is an event management system for a University.  

</general_description>

<functional_requirements>

<events>

<properties>

"TargetAudience": {
10 → Vorschulkinder: Elementarstufe
20 → Schulkinder: Primarstufe
30 → Jugendliche: Sekundarstufe I
40 → Jugendliche: Berufsschulen, PTS
50 → Jugendliche: Sekundarstufe II
60 → Erwachsene
70 → Familien
80 → nur Mädchen/Frauen
},
"EventTopic": {
100 → Digitalisierung, Künstliche Intelligenz, IT, Technik
200 → Kunst, Kultur
300 → Sprachen, Literatur
400 → Medizin, Gesundheit
500 → Geschichte, Demokratie, Gesellschaft
600 → Wirtschaft, Recht
700 → Naturwissenschaft, Klima, Umwelt
800 → Mathematik, Zahlen, Daten
},
"Address" {
    "id": "GUID",
    "location_name": "string",
    "city": "string",
    "zip": "string",
    "state": "string",
    "street": "string",
    "latitude": "number",
    "longitude": "number"
},
"Contact" {
    "id": "GUID",
    "name": "string",
    "org": "string",
    "email": "string",
    "phone": "string",
},
"Event": {
"id": "GUID",
"title": "string",
"description": "string",
"event_link": "string",
"target_audience": "list<TargetAudience>",
"topics": "list<EventTopic>",
"date_start": "datetime",
"date_end": "datetime",
"classification": "enum [Scheduled, OnDemand]",
"fees": "boolean",
"is_online": "bool",
"organization_id": "GUID",
"program_name": "string|null",
"format": "string|null",
"school_bookable": "boolean|null",
"age_minimum": "int|null",
"age_maximum": "int|null",
"location_id": "GUID|null" references Address,
"contact_id": "GUID|null" references Contact,
"status": "enum [Draft, Approved, Transferred]",
"created_by": "string|null",
"modified_by": "string|null",
"modified_at": "datetime"
}

</properties>

<actions>

- Create events: newly created events can be saved as a draft or marked as approved
- Get events: fetch all existing events (filterable by status)
- Mark events: marking the status of selected events
- Delete events: events can be deleted
- Update events: events are only allowed to be edited when they were not already transferred (status != Transferred)
- Import events: import events via frontend form (use LLM-Call to fill out fields of the form automatically). This should for for normal text but also for whole HTML pages (f.e. https://fh-ooe.at/events)
- CRUD for Address and Contact
- Addresses and Contact should be reused for Event and stored in a seperate entity. For example, if an event's address is updated the system should check if an adress with the new values is already present in the system (matching zipcode, latitute/longitude) and if a an entity is already present only the FK should be changed, else a new entity should be created.

</actions>

</events>

</functional_requirements>

</fieldset>

<a id="creation-process"></a>

# the creation process

During the process, VSCode with the Gemini 3 Pro Copilot model was used. The creation process took approximately 4 hours. The initial prompt was given, and Gemini generated the project structure, including components, services, and models. After the initial generation, several iterations were made to refine the UI, improve functionality, and fix bugs. Screenshots of the problematic parts were taken and used to guide Gemini in resolving issues. No code was written manually, also no implementation details were given to fix bugs or improve the appearance.
The agent executed approximately 20 code generation requests. no docker but a local angular development server was used to serve the application during development.

# Gemini Frontend

The frontend was created with Gemini 3 Pro. The original prompt given in Experiment 1 was not altered. The full context was given with the request to only create the frontend with Gemini or respectively the backend with Raptor for the backend part.

<a href="description"></a>

## description

- A running Angular app is created relatively quickly. However, usability is not present because the model seems unable to grasp visual aspects without a reference. After the first draft, screenshots were used to highlight errors to the model. These were fixed with mixed success.
- After a few iterations, the UI improved and a usable app emerged. Functionality is rudimentary; many features such as validation, error messages, and loading indicators are missing.
- The API was initially interpreted very differently from the backend project. Incorrect endpoints and data structures were used.
- Code quality is acceptable, but much (e.g., design) is hard-coded into the template and is far from as modular and maintainable as what an experienced developer would produce.

<a href="implemented-features"></a>

## implemented features

Create events: Form for creating a new event. The user should have the option to either enter the data manually or import event data from an unstructured format (either text or HTML page) using the LLM-call in the backend. 

Get events: Overview of existing events created and filterable by search string.

Change event status: In the detail view of an event, the status can be changed via a dropdown.

Delete events: events can be deleted.

Update events: update form was created for updating existing events.

Create, read, update, delete for Contacts and Organizations: Full Forms for creating, reading, updating, and deleting contacts and organizations were created.

<a href="personal-remarks"></a>

## personal remarks

Integration with backend was difficult due to missing specification of the API endpoints and data structures. The frontend had to be adjusted multiple times to match the backend implementation. (and it is still not fully compatible)

## Conclusion

In summary, separating frontend and backend development simplified the process. The first versions are always buggy and unusable. By shrinking the context, specific requests and bug fixes can be implemented without deep Angular knowledge.

<a id="Raptor backend"></a>

# Raptor Backend

For the backend we used Raptor Mini in Copilot VS Code.

## Description

For the backend we used Raptor Mini in Copilot VS Code. We used the same starting prompt, which was also used for the frontend implementation. Firstly, we copied the starting prompt into the vs code copilot chat with the request that the AI should only implement the backend part. The first implementation did not start, because of some db errors. But after a few iterations the backend worked. No code was implemented by us. The whole implementation was created by Raptor.

In the end the backend is working and it kind of does whats defined in the starting prompt. A few things did not work in the beginning, like the JSON export part. But after a few more iterations it worked and the resulting json was valid (checked with the DNAustria Online Validator). The implementation was not perfect, for example the backend URL was hardcoded, but with a few more iterations where i pointed it out, the AI did change it.

Short description on the key characteristics ([Raptor Mini: GitHub Copilot’s New Code-First AI Model That Developers Shouldn’t Ignore - DEV Community](https://dev.to/koolkamalkishor/raptor-mini-github-copilots-new-code-first-ai-model-that-developers-shouldnt-ignore-44a4)):

**Key characteristics include:**

- **Large context window (~264k tokens)** – enabling the model to understand entire folders, modules, or multi-file diffs.
- **High output capacity (~64k tokens)** – perfect for long refactors and structured diffs.
- **Supports tool calling, multi-file editing, and code-aware agents**.
- **Integrated directly into VS Code** – no external API needed.
- **Optimized for code generation, transformation, and workspace-based reasoning**.

## Implemented features

events:

- create

- delete

- update

- get

- export function with the valid json format

address/organization:

- create

- delete

- update

- get

contats:

- create

- delete

- update

- get

## Personal remarks

**Some personal pros and cons:**

pros:

- it's very easy and fast to get an application that does something

- it's very easy to adapt the current implementation via prompting. For example, the AI implemented a Address object, but the frontend expected an Organization object instead. So i just wrote a few lines as a prompt "Change the Address model with the same data to an Organization model" and the AI did all the changes from db changes to code changes and also adapted the tests

- the AI Integration in VS Code Copilot knows the context of the app and so you can easily create some docker integration for example

- if changes were made in the implementation the AI automatically updated the tests

cons:

- there are some limitation to the request you can send to the AI agent. If you run into those limitation during implementation phase it is really annoying because you need to check at what stage the current implementation is and need to adapt or finish the implementation yourself

- some decisions regarding the implementation are not really good for example that you have an organization with street, postal_code, city, state and an address elements which is just a string containing all those elmements. From my point of view, there is no need for this element. Or another example is, that the URL for the backend was hardcoded.

## Conclusion

The first tests with backend frontend did not work correctly. There was some work/iterations required for the application to work in some way. I think the reason for that was the prompt, because the interface for the frontend to backend was not specified detailed enough.