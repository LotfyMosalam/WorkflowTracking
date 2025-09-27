# Workflow Tracking System (WTS)

This project is a technical assessment demonstrating skills in **.NET 9.0 Web API**, **Entity Framework Core (EF Core)**, and clean, modular system architecture following the **Clean/Layered Architecture** principles.

---

## 📌 1. Concept

The Workflow Tracking System (WTS) allows an organization to define multi-step workflows (templates). It then enables users to execute processes based on those workflows, tracking the progress, assigning tasks, and enforcing validation rules at specific steps.

---

## 🏛 2. Solution Architecture

The system is structured into four main layers, ensuring **separation of concerns**, scalability, and testability.

| Project                   | Layer          | Responsibility                                                                           | Key Components                                                                                 |
| :------------------------ | :------------- | :--------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------- |
| `Workflow.Api`            | Presentation   | API Endpoints (Controllers), Configuration, and Dependency Injection (DI) setup.         | `WorkflowsController`, `ProcessesController`                                                   |
| `Workflow.Application`    | Application    | Defines business contracts (Interfaces), Request/Response DTOs, and business exceptions. | `IWorkflowService`, `IProcessService`, `CreateWorkflowDto`, `ProcessDto`                       |
| `Workflow.Infrastructure` | Infrastructure | EF Core persistence, external validation simulation, service implementations.            | `WorkflowDbContext` `ProcessRepository` |
| `Workflow.Core`           | Domain         | Core business entities and logic, dependency-free.                                       | `Workflow`, `Process`, Enums (`ActionType`, `ProcessStatus`)                                   |
| `Workflow.Tests.Unit`     | Testing        | Unit tests for core logic (Moq + EF Core In-Memory).                                     | `ProcessesControllerTests, WorkflowsControllerTests`                                                                          |

---

## ⚙️ 3. Setup and Configuration

### Prerequisites

1. **.NET 9.0 SDK** installed
2. **SQL Server** instance (`(localdb)\mssqllocaldb` used by default)
3. **Entity Framework CLI tool** installed:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

### Database Setup

1. Update `DefaultConnection` in `src/Workflow.Api/appsettings.json`.
2. Apply migrations:

   ```bash
   dotnet ef database update --project src/Workflow.Infrastructure --startup-project src/Workflow.Api
   ```

### Running the Application

```bash
dotnet run --project src/Workflow.Api
```

Swagger UI will be available at:
👉 `http://localhost:5055/swagger/index.html`

---

## 🔗 4. API Endpoints

| Method | Endpoint                | Description                                            |
| :----- | :---------------------- | :----------------------------------------------------- |
| `POST` | `/v1/workflows`         | Create a workflow template                             |
| `POST` | `/v1/processes/start`   | Start a new process from a workflow                    |
| `POST` | `/v1/processes/execute` | Execute the active step in a process (with validation) |
| `GET`  | `/v1/processes`         | Query processes (filter by workflow, status, user)     |
| `GET`  | `/v1/processes/{id}`    | Retrieve full process instance with history            |

---

## ✅ 5. Validation Mechanism

The custom validation mechanism is implemented in `ProcessService` using `SimulatedExternalValidationService`.

* **Step-Level Control:** Each `WorkflowStep` has a `RequiresValidation` flag.
* **Simulation Logic:** If the step name contains *"Finance"* and the payload amount is **over 10,000**, validation fails.
* **Behavior:**

  * If validation passes → process advances.
  * If validation fails → process is blocked, a `400 Bad Request` is returned, and failure details are logged into `ProcessStep.ValidationLog`.

---

## 🧪 6. Testing

Run unit tests:

```bash
dotnet test 
```

The project includes unit tests that verify both the controller endpoints and the core business logic.
---

