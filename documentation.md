# Gemini Frontend + Backed documenatation

## table of contents
- [Introduction](#introduction)
- [The creation process](#creation-process)
- [docker setup](#docker-setup)
- [backend](#backend)
- [frontend](#frontend)


<a id="introduction"></a>
# Introduction

in this scenario, both the angular frontend and the .NET Web API backend were created using the LLM Gemini 3 Pro. The goal was to create a full ready-to-run system which meets our defined requirements. In the process of the agent creating the system we (as perspective of software developers) did not give hints in terms of architectural - or coding knowledge. The requirements of the system + some config settings for the LLM were provided via the following start prompt:

<fieldset>

<legend>startprompt.md</legend>
&lt;persistence&gt;<br>

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

For development we used VS Code as a "development environment" (in our case we primarily used it for code highlighting) and selected the Gemini 3 Pro Model. We did not give additional information, we only copy pasted the start prompt and hit enter. During the development process of the agent we only gave permissions to execute commands on the operating system, we did not give any hints / improvements. 

<a id="docker-setup"></a>
# docker setup

we did not give any information about the development environment, we only specified the framework versions. Gemini decided to create a docker compose file with a postgres database container:

```
version: '3.8'

services:
  db:
    image: postgres:17
    container_name: dnaustria_db
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
      POSTGRES_DB: dnaustria
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:

```

a positive remark is that Gemini automatically created a volume for the database storage directory so that container data is not lost between container restarts. The most negavite aspect here is that the database password is defined as plain text (not passed via environment). This could lead to huge security risks where non-aware users push clear text passwords to a repository. It is worth to mention that since we did not specify to use docker as a runtime environment the agent may have implemented this differently.


<a href="backend"></a>
# backend

Gemini used .NET Web API as the solution framework as specified in the start prompt. For database communication EntityFramework was used. The finished backend started without any errors on port http://localhost:5088. A big problem was getting the database communication running because of EntityFramework. In the generated ReadME where Gemini added the setup steps no mention of applying EF migrations were present. This issue was easily fixed running dotnet ef database update (the migrations were already generated by Gemini).

In terms of an architectural point of view Gemini created all source files in a single DNAustria.API project. A clean three layer architecture (Database => Service => Controller) was used by the agent. The configuration of the database was performed va a ConnectionString in the appsettings (also clear password). 

