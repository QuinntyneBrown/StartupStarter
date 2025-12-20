* Technical Specifications

** System Wide Specifications
- the system shall use flatten namespaces
- the Backend shall have 3 projects
    - {system}.Core
    - {system}.Infrastructure
    - {system}.Api
- the system shall NOT use AutoMapper. 
- the system shall create and use extensions methods of Core models shall be created in the Api layer with a ToDto method that returns the Mapped Dto
- the system shall not use IRepositories, instead use the I{system}Context interface
- the shalled include the name of the entity in for Ids.

	Do
	{entity}Id

	Don't 
	Id
- the system shall have exactly one (class, enum, record, etc..) per file.
- the system shall NOT have mutiple object defined in a file.

* Core Project Specifications

- the core project shall be named {system}.Core
- aggregates shall go in the {system}.Core\Model folder
- each aggregate shall have a folder in the {system}.Core\Model folder called {system}.Core\Model\{aggregate}Aggregate
- the core project shall contain an interface called I{system}Context with DbSet properties for each entity in system. The interface represents the persistence surface. The implemtation of the interface is in the Infrastructure project
- the core project shall contain a folder called Services which contains services (interface and classes) with core behaviour logic to the system. Authentication, Emailing, Azure AI Integration etc,,
** Aggregate Folder
- aggregate folder shall be named {system}.Core\Model\{aggregate}Aggregate
- inside the {system}.Core\Model\{aggregate}Aggregate contains all the Entities, Enums, Events and AggregateRoot, etc... 
- each of the types inside of {system}.Core\Model\{aggregate}Aggregate has their own folder. (Events folder, Enums folder, etc....)

** {system}.Infrastructure
- shall contain the I{system}Context implementation. The implementation class is called {system}Context
- shall contain EF Miigrations
- shall contain Entity Configurations
- shall contain Seeding services

** The Api Project
- the api project shall be named {system}.Api
- the api project shall have a folder called Features containing all Commands, Queries (using MediatR) grouped in folders by BoundedContext
- the subfolders within the Features folder shall contain the Dtos
- the api project shall have Api Controllers in a Controllers folder
- the shall optionally have MediatR behaviours in a folder called Behaviours
