# Project_Restaurant_Blazor
> Project Restaurant Backend Net 7 Frontend Blazor

# General info
Project_Restaurant_Blazor contains two applications: server and client. Server was written in .Net 7, client Blazor. Project simulate simple ordering foods. Priveledged users can add / remove food and logged users can order it. Client and server shares data via gRPC Web protocol. 

# Server application 
Server was divided into 7 projects including tests projects: Api, Application, Core, Infrastructure, Migrations. Api project has configure the whole project. Application contains business logic. Infrastructure has gRPC services, database connection, services that typically talk to external resources. Core contains domain model and Migrations has infomrations about database scheme. On start application when database scheme changed, FluentMigrator using Migrations project migrate database scheme.

# Technologies

# Project