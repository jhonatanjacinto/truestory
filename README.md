# Truestory API
In order to run this sample application is necessary to have Docker/Docker Compose installed in your machine.

## Running the application
- Clone this repo in your local machine
- cd/ into root solution folder (/truestory)
- Run `docker compose up` to spin up all the required services

## Web API
- The WebApi is set to be executed at http://localhost:5001
- SwaggerUI is set up and can be accessed at http://localhost:5001/swagger so the structure of the Web API and its features can be explored

## Front-end App
- There's a sample front-end app (built using Blazor SSR) that consumes the WebApi
- It runs on http://localhost:5000 and exposes a basic CRUD that uses some of the WebApi functionalities
- For styling, TailwindCSS was used