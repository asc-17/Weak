# Weak

A personal Cross-Platform Mobile app built with **.NET 10 (.NET MAUI)** that helps visualize **how heavy upcoming weeks are** and **how much work is actually getting done**.

This is not a traditional to-do list.  
It’s a **week-level workload forecast** designed for people juggling complex schedules (academics, multiple commitments, deadlines).

---

## ✨ Core Idea

The app answers two simple questions:

1. **How demanding is this week?**  
2. **How on track am I with it?**

Instead of focusing on daily noise, the app works at the **week level**, because that’s how humans actually plan.

---

## 🧠 Mental Model

The system borrows its logic from **GPA calculation**:

- **Effort** → credits  
- **Completion** → marks  
- **Weekly progress** → GPA (scaled to 0–10)

Each week is represented using **two independent bars**:

### 🔴 Load (Capacity)
- How heavy the week is
- Based on total effort of tasks due that week
- Color-coded (green → yellow → red)
- **Does not change** as tasks are completed

### 🔵 Progress (Execution)
- How much of the planned work is completed
- GPA-style weighted calculation
- Updates dynamically as tasks are completed

This separation avoids ambiguity and keeps the UI honest.

---

## 📱 Screens Overview

### Home
- Bird’s-eye view of the **next 3 weeks**
- Shows both **Load** and **Progress**
- Uses short contextual summaries instead of numbers

### Tasks
- Unified task list (manual + calendar-imported)
- Tasks have:
  - deadline
  - effort (1–10)
  - completion (checkbox + partial slider)
- No special treatment for imported tasks

### Calendar
- **Day + Month hybrid calendar**
- Designed to show **density and clustering**, not task duplication
- Month grid shows:
  - which days are heavy
  - which days are light
- Selecting a day reveals that day’s tasks in a detail panel

### Settings
- Import / export (JSON)
- Notifications
- Calendar sync

---

## 📆 Task Rules

- **Deadline is mandatory**
- No start date
- Each task belongs to **exactly one week** (based on its deadline)
- Overdue tasks contribute to the **current week’s load**
- All tasks are editable, regardless of source

---

## 📊 Weekly Calculation (Simplified)

For a given week:

### Load
TotalLoad = Σ Effort


### Progress
Progress = ( Σ (Effort × CompletionFraction) / Σ Effort ) × 10


- CompletionFraction = CompletionPercent / 100
- UI shows bars only, not numbers

---

## 🔔 Notifications

- **Deadline reminder**: 24 hours before a task is due
- **Weekly load warning**:
  - Triggered on Saturday
  - Time: 9:00 AM
  - Warns if the upcoming week is heavy

---

## 🛠 Tech Stack

- **.NET 10**
- **.NET MAUI (Android-only)**
- **MVVM architecture**
- **SQLite** for local storage
- Google Calendar (read-only import)

---

## 🧩 Architecture Highlights

- Single unified `Task` model
- Services:
  - WeekComputationService
  - TaskRepository
  - CalendarImportService
  - NotificationService
- UI is treated as **finalized** and logic is layered beneath it

---

## 🎯 Why This Exists

Managing complex workloads isn’t about doing *more*.  
It’s about understanding **when things get heavy** and **whether you’re keeping up**.

This app is a quiet tool to help with exactly that.

---

## 📄 License

Personal project.  
License to be decided.
