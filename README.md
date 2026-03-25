# ResumeMatcherAI
## Project Overview

ResumeMatcher is an AI-powered system designed to index, search, and interact with candidate resumes using natural language.

The system combines vector search (RAG) with multiple AI agents to provide accurate candidate matching and conversational exploration over a private resume database.

Instead of relying on keyword-based filtering, users can describe their needs freely, and the system retrieves and ranks the most relevant candidates, while also allowing follow-up questions about each candidate.

This project reflects my approach to building practical AI systems, combining clean architecture, agent-based design, and real-world problem solving.
## Key Capabilities

- Resume ingestion and automatic indexing on startup  
- Semantic search using vector-based retrieval (RAG)  
- AI-based candidate ranking with scoring and explanations  
- Conversational interface for querying candidates and asking follow-up questions  
- Multi-agent architecture with dynamic routing between workflows  
- File system management via AI (organize, inspect, and manage resume files)  
- Secure communication using JWT-based authentication between services
## Flow

The system follows a modular, service-oriented design with clear separation between workflows, services, and AI agents.

Resumes are ingested from a local folder, transformed into structured data, and indexed into an in-memory vector store, enabling semantic retrieval using a RAG-based approach.

User requests are processed through a routing agent that selects the appropriate workflow. The system uses multiple specialized agents, including validation and ranking agents, to ensure accurate processing and high-quality results.
## Project Structure

The solution is organized into clear layers to separate concerns and keep the system maintainable and extensible:

- **Domain**  
  Core models such as `ResumeDocument` and related entities.

- **Core**  
  Interfaces, commands, results, workflows, and business logic.

- **Infrastructure**  
  AI agents, RAG pipeline (vector search with in-memory store), file storage, and resume extraction.

- **Backend.API**  
  Minimal API exposing endpoints for ingestion, search, and system operations.

- **Auth.API**  
  A lightweight authentication service responsible for issuing and validating JWT tokens.

- **Frontend.Console**  
  A simple client that interacts with the system through a conversational interface.
## Getting Started

### Prerequisites

- .NET 10 SDK  
- OpenAI API Key  
- A local folder containing resume files (.docx)

---

### 1. Configure Backend

Update `appsettings.json` in **Backend.API**:

    {
      "OpenAI": {
        "ApiKey": "your-key",
        "ChatModel": "your-model",
        "EmbeddingModel": "your-embedding-model"
      },
      "Storage": {
        "ResumeJsonDirectory": "C:\\Resumes\\json",
        "ResumeFilesDirectory": "C:\\Resumes\\files"
      }
    }

Place your resume files (.docx) inside the configured `ResumeFilesDirectory`.

---

### 2. Run Auth API

Start the authentication service:

    dotnet run --project Auth.API

---

### 3. Run Backend API

    dotnet run --project Backend.API

On startup, the system will automatically index all resumes from the configured folder.

---

### 4. Configure Console App

Set your OpenAI key using user secrets:

    dotnet user-secrets set "OpenAI:ApiKey" "your-key"

---

### 5. Run Console Client

    dotnet run --project Frontend.Console

---

### Notes

- Resume ingestion runs automatically on startup  
- Supported format: `.docx`  
- Make sure the configured directories are accessible by the application  
