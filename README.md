# Music Archive Management Application

This is a Music Archinve Management Application built with ASP.NET Core 8, Entity Framework, and PostgreSQL, fully containerized using Docker. The project follows the MVC (Model-View-Controller) design pattern and supports user authentication and song management through two separate databases: one for user authentication and one for managing archive's content.

Upon first running the Docker setup, both databases are automatically created and configured, with initial data seeded from test SQL dump files. The application supports all four CRUD operations (Create, Read, Update, Delete), and includes built-in search functionality and sorting capabilities.

Authentication and authorization are handled using the Microsoft Identity system, with role-based access control managed through authorization policies. 

The Music Archive database is handled using C# reflection, so with a few tweeks can be switched.

The frontend is developed using Razor and styled with Bootstrap, offering a clean and responsive user interface.


## Prerequisites

Make sure you have Docker installed on your machine (https://www.docker.com/)

## Getting Started

To get started with this application, follow these steps:

### 1. Clone the repository

Clone this repository to your local machine. Example using Git:

```bash
git clone https://github.com/baudii/music-db.git
```

Navigate to the project directory:

```bash
cd @path@/music-db
```

### 2. Run the application

The application is fully containerized using Docker. To run the application, simply use the following command in the project directory:

```bash
docker-compose up --build
```

This will:
- Build the Docker images for the project
- Set up the PostgreSQL databases
- Launch the web application on port 5000

#### This process might take some time

### 3. Access the Application

Once the setup is complete, the application will be available at:

http://localhost:5000

### Database Information

- The application uses two databases:
  - `Songsdump`: Stores information related to the song.
  - `authenticatedb`: Manages user authentication.
  
Both databases are automatically set up by Docker using the provided SQL dump files located in the `dumps/` folder.

### Environment Variables

The necessary environment variables for the database connections are already configured in the `docker-compose.yml` file:
- `IS_DOCKER`: used to configure special setup for application if it is running in the Docker

### Usage

Once the application is up and running, you can manage songs, users, and other functionalities related to the archive. The authentication system is integrated to allow user roles and permissions.

- Users with the Member role can only view the archive.
- Users with the Moderator role can edit the archive.
- Users with the Admin role have full control, including a separate admin interface for managing user accounts. Admins can create, delete, and edit any account, assign roles, and manage permissions.

To log in as an admin use:

email - `admin@admin.com`

password - `Admin1@`

## Stopping the Application

To stop the application, use:

```bash
docker-compose down
```

This will stop and remove the containers, but the data will persist in the Docker volumes.

---

Feel free to modify the configuration and customize the project to fit your needs. If you encounter any issues or have questions, feel free to open an issue on GitHub.
