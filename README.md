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
| `Workflow.Infrastructure` | Infrastructure | EF Core persistence, external validation simulation, service implementations.            | `WorkflowDbContext`, `WorkflowService`, `ProcessService`, `SimulatedExternalValidationService` |
| `Workflow.Core`           | Domain         | Core business entities and logic, dependency-free.                                       | `Workflow`, `Process`, Enums (`ActionType`, `ProcessStatus`)                                   |
| `Workflow.Tests.Unit`     | Testing        | Unit tests for core logic (Moq + EF Core In-Memory).                                     | `ProcessServiceTests`                                                                          |

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
👉 `https://localhost:7xxx/swagger/`

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
dotnet test tests/Workflow.Tests.Unit
```

Tests confirm validation logic, failure logging, and process flow correctness.

---

## 📄 7. Submission Notes

* **DTOs** ensure clean API contracts (`WorkflowDto`, `ProcessDto`).
* **Error Handling** maps domain exceptions to HTTP codes (`404`, `400`).
* **Code Quality** follows .NET best practices: DI, async/await, layered design.

---

# 📜 Original Task Description

The following is the original assessment brief provided by the company:

---

### Technical Assessment: Workflow Tracking System

**Dear Candidate,**

We would like you to complete a small project before your interview to demonstrate your skills in software development, analysis, and system architecture using .NET.

---

### Concept

You are required to build a Workflow Tracking System that allows organizations to define and manage workflows, where each workflow consists of multiple steps, and tasks are assigned to users based on predefined rules.

---

### Project Requirements

**1️⃣ Create an API for Workflow Management**

* Provide REST API endpoints to create, update, and retrieve workflows.

**Example – Create Workflow**
`POST /v1/workflows`

```json
{
  "name": "Approval Process",
  "description": "A workflow to approve purchase requests",
  "steps": [
    {
      "step_name": "Submit Request",
      "assigned_to": "employee",
      "action_type": "input",
      "next_step": "Manager Approval"
    },
    {
      "step_name": "Manager Approval",
      "assigned_to": "manager",
      "action_type": "approve_reject",
      "next_step": "Finance Approval"
    },
    {
      "step_name": "Finance Approval",
      "assigned_to": "finance",
      "action_type": "approve_reject",
      "next_step": "Completed"
    }
  ]
}
```

---

**2️⃣ Manage Process Execution & Tracking**

* Start a process from a workflow.
* Execute steps in sequence.

**Example – Start Process**
`POST /v1/processes/start`

```json
{
  "workflow_id": "123",
  "initiator": "user123"
}
```

**Example – Execute Step**
`POST /v1/processes/execute`

```json
{
  "process_id": "456",
  "step_name": "Manager Approval",
  "performed_by": "manager",
  "action": "approve"
}
```

---

**3️⃣ Retrieve Active and Completed Processes**

* `GET /v1/processes` supports filters:

  * `workflow_id`
  * `status` (Active, Completed, Pending)
  * `assigned_to`

---

### 🔍 Validation Middleware

* Certain steps require validation (external API call or simulation).
* If validation fails → process should not advance, return error, and log attempt.
* Example: *Finance Approval* step checks an external financial API before proceeding.

---

### ✅ Implementation Requirements

* Must use **.NET Core / .NET 5+ Web API** with **Entity Framework Core**.
* Code should be clean, modular, scalable.
* Include a documentation file explaining setup.

---

### 📬 Submission Instructions

* Submit within **3 days**.
* Assessment evaluates **problem solving, API integration, validation handling**.

---
