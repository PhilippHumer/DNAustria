import { InsertEventDto } from './../../src/app/api/model/insertEventDto';
import { EventDto } from './../../src/app/api/model/eventDto';
export async function createDemoEvent(name:string = "demo-event"): Promise<EventDto> {
  const eventData:InsertEventDto = {
    name: name,
    ageMaximum: 50,
    ageMinimum: 0,
    classification: 0,
    contact: null,
    description: "this is a demo event!",
    endDate: new Date("2026-03-30T14:35:22.123Z").toISOString(),  // ISO format recommended
    startDate: new Date("2026-03-28T14:35:22.123Z").toISOString(),
    hasFees: false,
    isOnline: false,
    link: "https://servas.com",
    location: null,
    organization: null,
    schoolBookable: false,
    status: 0,
    format: "my-temp",
    programName: "my-program!"
  };

  const response = await fetch('http://localhost:5001/api/events', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
      // add authorization header if your API requires it, e.g.
      // 'Authorization': `Bearer ${process.env.API_TOKEN}`
    },
    body: JSON.stringify(eventData)
  });

  if (!response.ok) {
    throw new Error(`Failed to create event: ${response.status} ${response.statusText}`);
  }

  const res = await response.json();
  return res;  // the created event object
}

export async function clearAllEvents(){
   const response = await fetch('http://localhost:5001/api/events', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json'
      // add authorization header if your API requires it, e.g.
      // 'Authorization': `Bearer ${process.env.API_TOKEN}`
    }
   });

  if (!response.ok) {
    throw new Error(`Failed to create event: ${response.status} ${response.statusText}`);
  }
  var events:EventDto[] = await response.json();

}
