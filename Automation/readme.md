# Server

- Handle auth with JWT and refresh tokens
- Use openapi for the supervisor document
- Communicate with workers (supervisor, client) with a redis database

Supervisor (api) :
- Select worker based on available, queue size and other worker parameters
- Connect to the redis server to get the available workers

Worker (windows service) :
- Register itself on startup
    - Safe guard with mac or some unique hardware ID
    - Parameters : number of parallel tasks, max queue size, ...
    - State (updated) : available, working, down, updating ...
- Update current task instance state (success, warning, failure)
- Show actual / history queue with state
- List of "allowed"  and last instance state
- Can resolve a task based on it's type and select the correct way to execute it
- Keep realtime data (data that make sense only if the worker is up) and all the rest in the mongodb database

# Data contract and separation of concern

- Having coherant data definition between the database, api and app
- Keep a clear separation of concern but keeping the properties that should be shared, can be achieve with :
    - Interfaces : having extra properties for the properties that need a specific implementation
        - Viable on big classes with a lot of properties ?
    - DTO : using clonning / properties
        - Not a fan since it double everything + no clear way to ensure the DTO is compatible with the model
- Better serparation between ViewModel and Model, some other solution than using wrapper ?

## Postmortem

Used interfaces, pretty heavy :
- Need 3 definitions for each classes (one for API, one for interfaces, one for client)
- Main issue is attributes for `JsonDerivedType` and `BsonKnownTypes` since I don't want to import bson dependencies in client I can't have shared definitions (that anyway can't be since client have definition at several levels)

Real solution would be to use a really flat data scheme (pretty hard constraint) with inheritance ? Or having the clients classes generated from the API with partial and then the definition tweaked ?

# To improve

## Control task handling
Currently handled with specific if that check in a dictionnary, could be more generic with :
- Seed the control task in the database (with a specific folder as well)
- Create a TargetClass for internal class
- Allow readonly tasks