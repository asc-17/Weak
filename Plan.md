# Implementation Plan - Update 1: Lists Overhaul & Unified Creation Flow

This plan outlines the steps needed to implement the changes described in `Update1.md`.

## 1. Data Model & Logic Updates
- [x] **Update `Models/TaskList.cs`**:
    - Add `WeightedCompletionPercent` (CGPA-style calculation: `Σ(subtask effort × subtask completion) / Σ(subtask effort) × 100`).
    - Note: List subtasks are 0 or 100% complete (binary checkbox).
    - Add `AverageEffort` calculated as the mean of all subtask efforts.
- [x] **Update `Services/TaskListRepository.cs`**:
    - Ensure subtask operations (add/remove/toggle) trigger recalculation of the parent list's properties.
- [x] **Update `Services/TaskRepository.cs`**:
    - Ensure tasks belonging to a list are excluded from standalone task lists where appropriate.

## 2. Unified Creation Flow
- [x] **Create/Modify Creation View**:
    - Build a single page or bottom sheet `UnifiedCreationPage.xaml` with two tabs: **Create Task** and **Create List**.
    - Implement tab-switching logic in `UnifiedCreationViewModel.cs`.
- [x] **Data Persistence Across Tabs**:
    - Bind overlapping fields (Name/List Name, Subject, Date) to common properties in the ViewModel so switching tabs doesn't clear them.
- [x] **Update `TasksView.xaml` (To-Do Page)**:
    - Replace the separate "Add Task" and "Add List" buttons with a single unified "+" FAB.

## 3. To-Do Page Display Changes
- [x] **Update `Selectors/TaskEntryTemplateSelector.cs`**: 
    - Ensure it correctly switches between `TaskItem` and `TaskList` templates.
- [x] **Style `TaskList` in `TasksView.xaml`**:
    - Show `Name`, `Subject` (if any), `DueDate`, `SubtaskProgress` (e.g. "3/5 complete"), `WeightedCompletionPercent`, and `AverageEffort`.
    - Ensure lists are visually distinct from standalone tasks.
    - Set tap action to navigate to the new dedicated `ListPage`.

## 4. Dedicated List Page (New)
- [x] **Create `Views/ListPage.xaml`**:
    - **Header**: Inline editable `Name`, `Subject`, and `DueDate`.
    - **Summary**: Large display of `WeightedCompletionPercent` and `AverageEffort`.
    - **Subtask Area**: List all tasks in the group.
        - Binary completion checkbox.
        - Effort slider (1-10).
    - **Actions**: "+ Add Task" button (opens inline form), Swipe-to-delete subtasks.
- [x] **Create `ViewModels/ListViewModel.cs`**:
    - Handle subtask CRUD operations and recalculations.

## 5. Agenda (Timeline & Day Views)
- [x] **Update `Selectors/DayTimelineTemplateSelector.cs`**:
    - Add support for `TaskList` template.
- [x] **Update `Views/CalendarView.xaml`**:
    - Implement `ListTemplate` for both Timeline and Day views.
    - Show list name, subject, effort, progress, and weighted completion.
    - Implement inline expand/collapse behavior on tap to show subtasks.
    - Subtasks in this view should use binary completion checkboxes.
- [x] **Update `Services/WeekComputationService.cs`**:
    - Include lists in load score calculation (list treated as single item: Effort = avg subtask effort, Completion = weighted completion %).
    - Exclude subtasks from standalone task counts.

## 6. Testing & Refinement
- [x] Verify Load Score calculation on Home Page includes Lists correctly (Average Effort & Weighted Completion).
- [ ] Test the unified creation flow transitions.
- [ ] Ensure swipe-to-delete works for both items and subtasks.
