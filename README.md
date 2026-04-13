<img width="1062" height="659" alt="image" src="https://github.com/user-attachments/assets/137b11ea-db2c-4bb2-ad53-eb7e5f0405e0" />

# Event-Driven Salesforce Integration Platform

This repository contains my final 5th semester project for the Datamatiker (AP Degree in Computer Science) program at UCL.

The project focuses on designing and implementing an **event-driven integration platform** between **Salesforce Experience Cloud** and a **.NET-based microservices environment**.  
The emphasis is on architecture, reliability, and scalability — not UI or end-user features.

---

## 🎯 Purpose

The purpose of this project is to demonstrate how:

- **Event-driven architecture** can be used to integrate Salesforce with external systems
- **Robust event handling** can be achieved using idempotency and asynchronous messaging
- Salesforce can remain **intentionally thin**, acting only as an event producer and consumer
- External .NET services can handle business logic, AI enrichment, and downstream processing

---

## 🧠 High-Level Architecture

The system is designed as a **distributed, event-driven platform**:

- **Salesforce Experience Cloud**
  - Publishes Platform Events
  - Receives result events back from the platform
- **SalesforceService (.NET)**
  - Subscribes to Salesforce Platform Events via Pub/Sub (gRPC + Avro)
  - Validates and normalizes incoming events
  - Ensures idempotent processing using ReplayId / CorrelationId
  - Publishes internal integration events
- **ContentModerationService (.NET)**
  - Consumes internal events
  - Enriches data using external AI (Azure Content Safety)
  - Persists moderation results using append-only storage
- **Event Infrastructure**
  - Asynchronous pub/sub communication
  - Loosely coupled services
- **Runtime & Deployment**
  - Docker Compose
  - Oracle Cloud Infrastructure (Ubuntu ARM)

---

## 🔁 Event Flow (Simplified)

1. Salesforce publishes a Platform Event  
2. SalesforceService receives the event via Pub/Sub (gRPC)
3. Event is validated and normalized
4. An internal event is published
5. Downstream services process the event asynchronously
6. A result event is published back to Salesforce

---

## 🛠️ Technologies Used

- **.NET 9**
- **Salesforce Pub/Sub API (gRPC + Avro)**
- **Event-Driven Architecture**
- **Microservices**
- **Docker & Docker Compose**
- **Oracle Cloud Infrastructure**
- **Azure Content Safety (AI moderation)**
- **PostgreSQL**
- **Dapr (pub/sub abstraction)**

---

## 🧩 Key Architectural Decisions

- Salesforce contains **no domain logic**
- All incoming events are validated at integration boundaries
- Idempotency is enforced using unique correlation identifiers
- Services communicate asynchronously to avoid temporal coupling
- AI enrichment is treated as a **pluggable capability**, not a core dependency
- Failure scenarios (timeouts, retries, duplicate delivery) are considered first-class design concerns

---

## ⚠️ Failure Scenarios Covered

The design explicitly accounts for:

- Salesforce event replay
- Duplicate event delivery
- Partial service outages
- External AI timeouts
- Temporary downstream unavailability

The system favors **eventual consistency** and recovery over synchronous guarantees.

---
