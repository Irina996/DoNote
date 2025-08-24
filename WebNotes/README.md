# WebNotes — API for Notes and Tasks

**WebNote** is a web API built with **ASP.NET Core** that allows users to create, edit, and manage notes and tasks.  
The application uses **JWT authentication**, **PostgreSQL**, and is deployed on **Render.com**.

---

## 🚀 Features

- ✅ User registration and login (JWT)
- ✅ Notes management (CRUD)
- ✅ Tasks management (CRUD)
- ✅ Categories for notes and tasks

---

## 🛠 Technologies

- **Backend**: ASP.NET Core 9.0
- **Database**: PostgreSQL
- **Authentication**: JWT, ASP.NET Identity
- **ORM**: Entity Framework Core
- **Hosting**: [Render.com](https://render.com)
- **API**: RESTful

---

## ☁️ Demo

The API is available at: https://donote-api.onrender.com/api

---

## 📂 Project Structure

DoNote/
├── Controllers/            # API controllers
├── Data/                   # DbContext and factory
├── Entities/               # Entities (Note, Task, Category)
├── Models/                 # DTOs (Create, Update, Response)
├── Migrations/             # EF Core migrations
├── Program.cs              # DI, auth, middleware setup
├── Dockerfile              # For deployment on Render
└── appsettings.json        # Configuration

---

## ✍️ Example requests (using curl)

### Register:
curl -X POST "https://donote.onrender.com/api/auth/register" -H "Content-Type: application/json" -d "{\"email\":\"user@example.com\", \"password\":\"Password123!\", \"confirmpassword\":\"Password123!\"}"

### Get notes (needed token):
curl -X GET "https://donote.onrender.com/api/notes" -H "Authorization: Bearer eyJhbGciOiJIUzI1Ni..."

---

## API endpoints

 HTTP Verbs | Endpoints | Action |
| --- | --- | --- |
| POST | /api/auth/register | To sign up a new user account |
| POST | /api/auth/login | To login an existing user account |
| POST | /api/notes | To create a new note |
| GET | /api/notes | To retrieve all notes of the user |
| GET | /api/notes/{id} | To retrieve note of the user with noteId |
| PUT | /api/notes/{id} | To edit the details of a note |
| DELETE | /api/notes/{id} | To delete a note |
| POST | /api/categories | To create a new note category |
| GET | /api/categories | To retrieve all notes categories of the user |
| GET | /api/categories/{id} | To retrieve note category with id |
| PUT | /api/categories/{id} | To edit a note category |
| DELETE | /api/categories/{id} | To delete a note category |
| POST | /api/tasks | To create a new task |
| GET | /api/tasks | To retrieve all tasks of the user |
| GET | /api/tasks/{id} | To retrieve task with id |
| PUT | /api/tasks/{id} | To edit a task |
| DELETE | /api/tasks/{id} | To delete a task |
| POST | /api/taskcategories | To create a new task category |
| GET | /api/taskcategories | To retrieve all task categories of the user |
| GET | /api/taskcategories/{id} | To retrieve task category with id |
| PUT | /api/taskcategories/{id} | To edit a task category |
| DELETE | /api/taskcategories/{id} | To delete a task category |