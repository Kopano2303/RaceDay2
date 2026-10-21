# RaceDay API Endpoint Plan

## Authentication

| HTTP Method | Route                | Description                    | Role Required | Request Body                                      | Expected Response                       |
| ----------- | -------------------- | ------------------------------ | ------------- | ------------------------------------------------- | --------------------------------------- |
| POST        | `/api/Auth/register` | Register a new RaceDay user    | Public        | Email, Password, FirstName, LastName, PhoneNumber | 200 OK with registration confirmation   |
| POST        | `/api/Auth/login`    | Authenticate a registered user | Public        | Email, Password                                   | 200 OK with authentication result/token |

## User Profile

| HTTP Method | Route                     | Description                     | Role Required      | Request Body                                                                      | Expected Response           |
| ----------- | ------------------------- | ------------------------------- | ------------------ | --------------------------------------------------------------------------------- | --------------------------- |
| GET         | `/api/Users/{id}`         | Retrieve a user's basic details | Authenticated User | None                                                                              | 200 OK with user details    |
| GET         | `/api/Users/{id}/profile` | Retrieve a user's profile       | Authenticated User | None                                                                              | 200 OK with profile details |
| PUT         | `/api/Users/{id}/profile` | Update a user's profile         | Authenticated User | DateOfBirth, Gender, EmergencyContactName, EmergencyContactPhone, ProfileImageUrl | 200 OK with updated profile |

## Events

| HTTP Method | Route              | Description                   | Role Required | Request Body                                                                                        | Expected Response                 |
| ----------- | ------------------ | ----------------------------- | ------------- | --------------------------------------------------------------------------------------------------- | --------------------------------- |
| GET         | `/api/Events`      | Retrieve all available events | Public        | None                                                                                                | 200 OK with list of events        |
| GET         | `/api/Events/{id}` | Retrieve a specific event     | Public        | None                                                                                                | 200 OK with event details         |
| POST        | `/api/Events`      | Create a new event            | Organiser     | OrganiserID, EventName, Description, EventType, EventDate, StartTime, Venue, City, Province, Status | 200 OK with created event         |
| PUT         | `/api/Events/{id}` | Update an existing event      | Organiser     | EventName, Description, EventType, EventDate, StartTime, Venue, City, Province, Status              | 200 OK with updated event         |
| DELETE      | `/api/Events/{id}` | Delete an event               | Organiser     | None                                                                                                | 200 OK with deletion confirmation |

## Categories

| HTTP Method | Route                  | Description                  | Role Required | Request Body                                        | Expected Response                 |
| ----------- | ---------------------- | ---------------------------- | ------------- | --------------------------------------------------- | --------------------------------- |
| GET         | `/api/Categories`      | Retrieve all race categories | Public        | None                                                | 200 OK with list of categories    |
| GET         | `/api/Categories/{id}` | Retrieve a specific category | Public        | None                                                | 200 OK with category details      |
| POST        | `/api/Categories`      | Create a race category       | Organiser     | CategoryName, DistanceKM, CategoryType, Description | 200 OK with created category      |
| PUT         | `/api/Categories/{id}` | Update a race category       | Organiser     | CategoryName, DistanceKM, CategoryType, Description | 200 OK with updated category      |
| DELETE      | `/api/Categories/{id}` | Delete a race category       | Organiser     | None                                                | 200 OK with deletion confirmation |

## Event Categories

| HTTP Method | Route                                  | Description                                    | Role Required | Request Body                                       | Expected Response                    |
| ----------- | -------------------------------------- | ---------------------------------------------- | ------------- | -------------------------------------------------- | ------------------------------------ |
| GET         | `/api/EventCategories`                 | Retrieve all event-category combinations       | Public        | None                                               | 200 OK with list of event categories |
| GET         | `/api/EventCategories/{id}`            | Retrieve a specific event category             | Public        | None                                               | 200 OK with event category details   |
| GET         | `/api/EventCategories/event/{eventId}` | Retrieve all categories available for an event | Public        | None                                               | 200 OK with event category list      |
| POST        | `/api/EventCategories`                 | Add a category to an event                     | Organiser     | EventID, CategoryID, EntryFee, MaximumParticipants | 200 OK with created event category   |
| PUT         | `/api/EventCategories/{id}`            | Update an event category                       | Organiser     | EventID, CategoryID, EntryFee, MaximumParticipants | 200 OK with updated event category   |
| DELETE      | `/api/EventCategories/{id}`            | Remove a category from an event                | Organiser     | None                                               | 200 OK with deletion confirmation    |

## Enrolments

| HTTP Method | Route                           | Description                                | Role Required      | Request Body                                            | Expected Response                     |
| ----------- | ------------------------------- | ------------------------------------------ | ------------------ | ------------------------------------------------------- | ------------------------------------- |
| GET         | `/api/Enrolments`               | Retrieve all enrolments                    | Organiser          | None                                                    | 200 OK with list of enrolments        |
| GET         | `/api/Enrolments/{id}`          | Retrieve a specific enrolment              | Authenticated User | None                                                    | 200 OK with enrolment details         |
| GET         | `/api/Enrolments/user/{userId}` | Retrieve enrolments for a specific user    | Authenticated User | None                                                    | 200 OK with user's enrolments         |
| POST        | `/api/Enrolments`               | Enrol a participant into an event category | Authenticated User | UserID, EventCategoryID, PaymentStatus, EnrolmentStatus | 200 OK with created enrolment         |
| PUT         | `/api/Enrolments/{id}`          | Update an enrolment                        | Authenticated User | UserID, EventCategoryID, PaymentStatus, EnrolmentStatus | 200 OK with updated enrolment         |
| DELETE      | `/api/Enrolments/{id}`          | Cancel an enrolment                        | Authenticated User | None                                                    | 200 OK with cancellation confirmation |

## Results

| HTTP Method | Route                                  | Description                                  | Role Required      | Request Body                                                       | Expected Response                 |
| ----------- | -------------------------------------- | -------------------------------------------- | ------------------ | ------------------------------------------------------------------ | --------------------------------- |
| GET         | `/api/Results`                         | Retrieve all race results                    | Organiser          | None                                                               | 200 OK with list of results       |
| GET         | `/api/Results/{id}`                    | Retrieve a specific result                   | Authenticated User | None                                                               | 200 OK with result details        |
| GET         | `/api/Results/enrolment/{enrolmentId}` | Retrieve the result for an enrolment         | Authenticated User | None                                                               | 200 OK with result                |
| GET         | `/api/Results/user/{userId}`           | Retrieve a participant's performance history | Authenticated User | None                                                               | 200 OK with user's results        |
| POST        | `/api/Results`                         | Record a participant's race result           | Organiser          | EnrolmentID, FinishTime, OverallPosition, CategoryPosition, Status | 200 OK with created result        |
| PUT         | `/api/Results/{id}`                    | Update a race result                         | Organiser          | EnrolmentID, FinishTime, OverallPosition, CategoryPosition, Status | 200 OK with updated result        |
| DELETE      | `/api/Results/{id}`                    | Delete a race result                         | Organiser          | None                                                               | 200 OK with deletion confirmation |

## Routes

| HTTP Method | Route                         | Description                     | Role Required | Request Body                                                            | Expected Response                 |
| ----------- | ----------------------------- | ------------------------------- | ------------- | ----------------------------------------------------------------------- | --------------------------------- |
| GET         | `/api/Routes`                 | Retrieve all event routes       | Public        | None                                                                    | 200 OK with list of routes        |
| GET         | `/api/Routes/{id}`            | Retrieve a specific route       | Public        | None                                                                    | 200 OK with route details         |
| GET         | `/api/Routes/event/{eventId}` | Retrieve the route for an event | Public        | None                                                                    | 200 OK with route details         |
| POST        | `/api/Routes`                 | Create an event route           | Organiser     | EventID, RouteName, DistanceKM, ElevationGain, RouteMapUrl, Description | 200 OK with created route         |
| PUT         | `/api/Routes/{id}`            | Update an event route           | Organiser     | RouteName, DistanceKM, ElevationGain, RouteMapUrl, Description          | 200 OK with updated route         |
| DELETE      | `/api/Routes/{id}`            | Delete an event route           | Organiser     | None                                                                    | 200 OK with deletion confirmation |

## Weather

| HTTP Method | Route                          | Description                               | Role Required | Request Body | Expected Response                     |
| ----------- | ------------------------------ | ----------------------------------------- | ------------- | ------------ | ------------------------------------- |
| GET         | `/api/Weather`                 | Retrieve weather records                  | Public        | None         | 200 OK with weather records           |
| GET         | `/api/Weather/{id}`            | Retrieve a specific weather record        | Public        | None         | 200 OK with weather details           |
| GET         | `/api/Weather/event/{eventId}` | Retrieve weather information for an event | Public        | None         | 200 OK with event weather information |

## Planned Future Integration

The Weather functionality will later be connected to an external weather service/API to provide current or forecast weather information for RaceDay events.
