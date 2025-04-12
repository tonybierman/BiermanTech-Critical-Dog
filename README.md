# Critical Dog Application Overview

Critical Dog is designed to track and monitor the health and behavior of dogs. It provides a structured system for handlers and trainers to record observations, manage medical data, and categorize findings using meta-tags and scientific disciplines. Below is a detailed list of its features.

## Features

### 1. Subject Management
- **Purpose**: Manage individual dog profiles.
- **Use Case**: Register new dogs, update profiles, or review details for treatment planning.

### 2. Observation Tracking
- **Purpose**: Record quantitative and qualitative observations about dogs.
- **Details**:
  - **Observation Types**: Supports categories like Weight, Temperature, Behavior, and Medication (`ObservationType`).
  - **Observation Definitions**: Defines specific metrics (e.g., WeighIn, TempCheck) with ranges (MinimumValue, MaximumValue) and qualitative flags (`ObservationDefinition`).
  - **Records**: Logs observations in `SubjectRecord` with:
    - Subject ID, Observation Definition, Metric Type, Metric Value, Notes, Record Time, and Created By.
    - Supports null Metric Values for qualitative notes (e.g., behavior observations).
    - Ensures Metric Values are non-negative when applicable.
  - Indexes on SubjectId, RecordTime, MetricTypeId, and ObservationDefinitionId for efficient querying.
- **Use Case**: Track weight changes, monitor temperatures, note behaviors, or record medication doses over time.

### 3. Metric and Unit Management
- **Purpose**: Standardize measurement units and metrics.
- **Details**:
  - **Units**: Supports units like Kilograms (kg), Pounds (lb), Degrees Celsius (°C), and Milliliters (ml) in `Unit`.
  - **Metric Types**: Combines Observation Definitions with Units (e.g., Weight in kg or lb) in `MetricType`.
  - Ensures unique combinations of Observation Definition and Unit.
  - Descriptions for clarity (e.g., "Weight in kilograms").
- **Use Case**: Convert between units (e.g., kg to lb) or ensure consistent metric usage across records.

### 4. Meta-Tagging System
- **Purpose**: Categorize observations for easy filtering and analysis.
- **Details**:
  - Defines tags like "Routine Check," "Post-Treatment," or "Behavioral Issue" in `MetaTag`.
  - Associates tags with records via `SubjectRecordMetaTag`.
  - Supports multiple tags per record (many-to-many relationship).
- **Use Case**: Flag routine checkups, track post-treatment progress, or highlight behavioral concerns.

### 5. Scientific Discipline Integration
- **Purpose**: Align observations with relevant fields of study.
- **Details**:
  - Supports disciplines like Canine Biology, Nutrition Science, Veterinary Medicine, Ethology, and Pharmacology in `ScientificDiscipline`.
  - Links disciplines to observation definitions via `ObservationDefinitionDiscipline`.
  - Allows multiple disciplines per observation (e.g., WeighIn linked to Canine Biology and Nutrition Science).
- **Use Case**: Analyze data by discipline, such as studying weight trends in Nutrition Science or behavior in Ethology.

### 6. Data Integrity and Constraints
- **Purpose**: Ensure reliable and consistent data.
- **Details**:
  - **Foreign Keys**: Enforce relationships (e.g., SubjectRecord to Subject, MetricType to Unit).
  - **Unique Constraints**: Prevent duplicates (e.g., unique TagName, UnitSymbol, or Subject Name/Breed/ArrivalDate).
  - **Check Constraints**: Validate data (e.g., Age ≥ 0, Sex in (0,1,2), MinimumValue ≤ MaximumValue).
  - **Cascading Deletes**: Remove dependent records (e.g., SubjectRecord when Subject is deleted).
  - **Indexes**: Optimize queries on frequently accessed fields like SubjectId or RecordTime.
- **Use Case**: Maintain a robust dataset for accurate reporting and analysis.

### 7. Flexible Reporting
- **Purpose**: Generate insights from recorded data.
- **Details**:
  - Query `SubjectRecord` for trends (e.g., weight changes over time).
  - Filter by MetaTags (e.g., all "Behavioral Issue" records).
  - Group by Scientific Discipline or Observation Type for research.
  - Support for qualitative notes alongside quantitative metrics.
- **Use Case**: Produce reports on dog health, identify patterns, or prepare data for research.

### 8. Extensibility
- **Purpose**: Allow future enhancements.
- **Details**:
  - **IsActive Flags**: Enable soft deletion for MetaTags, Units, Observation Types, etc., preserving historical data.
  - **Generic Structure**: ObservationDefinition and MetricType allow new metrics without schema changes.
  - **Scalable Design**: Supports additional SubjectTypes or Disciplines as needed.
- **Use Case**: Add new observation types (e.g., heart rate) or disciplines (e.g., neurology) seamlessly.

## Summary
Critical Dog is a comprehensive tool, enabling detailed tracking of canine health and behavior. Its features include robust subject management, flexible observation recording, standardized metrics, meta-tagging, and discipline-based analysis, all underpinned by strong data integrity. The application supports both day-to-day care and long-term research, with room for future expansion.