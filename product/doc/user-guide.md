# Discover.DNAustria User Guide

This guide explains the main user flows of the Discover.DNAustria application based on the current interface screenshots.

## Overview

Discover.DNAustria is an event management system for FH Upper Austria. It helps local administrators manage educational events and review related information such as organizations, contacts, and locations.

The main navigation is shown in the left sidebar. From there, users can access:

- Dashboard
- Events
- Contacts
- Locations
- Organizations
- Export
- LLM Analyzer

At the bottom of the sidebar, users can sign out of the application.

## Dashboard

The dashboard gives a quick overview of the current system data.

![Dashboard](images/dashboard.png)

On this page, users can:

- See the total number of events
- View how many events are published
- View how many events are ready for transmission
- View how many events are still drafts
- See the number of contacts
- See the number of organizations

The dashboard tiles are clickable and act as shortcuts to the corresponding management pages:

- `Total Events` opens the full Events page
- `Published Events` opens the Events page filtered to published events
- `Ready for Transmission` opens the Events page filtered to events that are ready for transmission
- `Draft Events` opens the Events page filtered to draft events
- `Contacts` opens the Contacts page
- `Organizations` opens the Organizations page

This page is useful as a starting point because it summarizes the current state of the event database.

## Recent Events

The lower section of the dashboard contains a list of the most recent events.

![Recent Events](images/dashboard_recentEvents.png)

Each event card in this section shows:

- The event title
- A short preview of the description
- The event date
- The current status, for example `Draft` or `Ready For Transmission`

The `View All` button opens the complete event overview.

## Event Overview

The event overview is the central place for browsing and managing events.

![Event Overview](images/events_overview.png)

On this page, users can:

- Browse all available events
- Open the create dialog with `Create Event`
- Edit an event using the pencil icon
- Delete an event using the trash icon
- Review the current publication status directly on each event card

Each event card typically contains:

- The event title
- The current status badge
- A short text preview
- Action buttons for editing and deleting

## Filtering Events

The `Filter Events` area helps users narrow down the result list.

### Search by Name

Users can type a keyword into the search field to find events by name.

![Filter Events by Name](images/events_filterName.png)

This is useful when looking for a specific event or a group of similarly named events.

### Filter by Status

Users can also filter the list by event status.

![Filter Events by Status](images/events_filterStatus.png)

Example use cases:

- Show only draft events
- Show only published events
- Show only events ready for transmission

Name search and status filtering can be used together to reduce the list further.

## Event Details

The event details page provides a complete view of a single event.

![Event Details](images/events_detail.png)

This page includes:

- Breadcrumb navigation back to the event overview
- The event title
- The current status
- The event classification
- A full description
- Start and end date/time
- Event mode, for example online or on-site
- Fee information
- Organization details
- Contact person details
- Location information

Users can also use the action buttons in the top-right area to:

- Go back to the overview
- Edit the event
- Delete the event

This screen is intended for reviewing event information in more detail before making changes.

## Creating a New Event

Users can create a new event from the event overview by clicking `Create Event`.

![Create Event](images/events_create.png)

The event creation dialog contains structured sections, for example:

- AI Event Prefill
- Basics
- Additional event data further down in the form

In the visible part of the form, users can already enter:

- Event name
- Classification
- Status
- Description

Required fields are marked with an asterisk (`*`).

At the bottom of the dialog, users can:

- Cancel the creation process
- Save the new event with `Create Event`

## AI Event Prefill

The create dialog also includes an `AI Event Prefill` section.

This feature allows users to paste unstructured event information and use AI support to prefill the form automatically. This can save time when event data already exists in free-text form, such as copied announcements or descriptions.

## Typical Workflow

A common workflow in Discover.DNAustria is:

1. Open the dashboard to review current numbers and recent events.
2. Open `Events` from the sidebar.
3. Use the search field or status filter to find a specific event.
4. Open an event to review its full details.
5. Edit or delete the event if necessary.
6. Create a new event from the overview when new content needs to be added.

## Notes for Users

- The sidebar is the main navigation element throughout the application.
- Status badges help identify the current processing stage of each event quickly.
- The event overview is optimized for day-to-day management tasks.
- The event details page is better suited for careful review of one specific entry.
- The create dialog uses structured sections to make data entry easier.

## Summary

Discover.DNAustria provides a simple workflow for managing educational events:

- Start on the dashboard
- Review recent activity
- Manage events in the overview
- Filter events by name or status
- Open full event details
- Create, edit, or delete events as needed

This combination gives administrators a clear and efficient interface for maintaining the event database.
