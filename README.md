# Critical Dog Application

## Overview
Critical Dog is a comprehensive application designed for managing and analyzing detailed canine health, behavior, and environmental data. It provides a robust platform for tracking and recording various metrics related to canine biology, veterinary care, nutrition, and behavior, catering to veterinarians, breeders, researchers, and dog owners. Built with a focus on standardized data collection, Critical Dog supports scientific disciplines such as canine biology, veterinary medicine, nutrition science, ethology, and pharmacology.

## Purpose
The Critical Dog application aims to:
- Facilitate precise tracking of canine health metrics, such as weight, heart rate, and temperature.
- Monitor behavioral patterns, reproductive cycles, and environmental conditions.
- Support nutritional analysis and dietary management for optimal canine health.
- Provide a structured framework for recording and analyzing data across multiple scientific disciplines.
- Enable users to make data-driven decisions for canine care, breeding, and research.

## Features
Critical Dog offers a rich set of features to support canine data management:

### Health and Vital Signs Tracking
- Record quantitative metrics like weight, heart rate, respiratory rate, body temperature, and hydration levels.
- Support for multiple units (e.g., kilograms, pounds, Celsius, Fahrenheit, beats per minute).
- Semi-quantitative scoring for metrics like pain assessment, coat condition, and dental health (e.g., 1–10 pain scale, 1–5 coat condition score).

### Behavioral Observations
- Capture qualitative notes on behavior, socialization, and training progress.
- Track training command mastery with semi-quantitative scores (e.g., 1–5 scale).
- Monitor behavioral traits relevant to ethology studies.

### Reproductive and Genetic Data
- Track reproductive metrics such as litter size, gestation progress, and estrus cycle stages.
- Record genetic data, including mitochondrial DNA (mtDNA), Y-chromosome DNA (Y-DNA), and health condition statuses.
- Support for Orthopedic Foundation for Animals (OFA) hip grading and other anatomical evaluations.

### Nutritional Management
- Monitor dietary intake, including daily caloric intake and appetite levels.
- Assess stool quality using a standardized 1–7 scale.
- Track life stage factors to tailor nutritional needs for puppies, adults, and seniors.

### Environmental Monitoring
- Record environmental conditions like ambient humidity and kennel space (e.g., square meters).
- Support for qualitative and quantitative environmental observations to ensure optimal living conditions.

### Scientific Discipline Integration
- Align data collection with disciplines such as:
  - **Canine Biology**: Anatomy, physiology, and genetics.
  - **Veterinary Medicine**: Diagnosis, treatment, and vital signs.
  - **Nutrition Science**: Dietary needs and energy intake.
  - **Ethology**: Behavioral studies.
  - **Pharmacology**: Medication administration and effects.
- Link observations to relevant disciplines for research and analysis.

### Standardized Data Framework
- Use predefined units, observation types, and metric definitions for consistency.
- Support both qualitative (e.g., behavior notes) and quantitative (e.g., weight in kilograms) data.
- Organize data with metadata tags like Feeding, Medication, Exercise, Vital Signs, and Reproduction for easy categorization.

### Subject Management
- Focus on canine subjects (Canis familiaris or Canis lupus familiaris), with extensible support for other subject types.
- Maintain detailed records for individual dogs, including health, behavior, and genetic profiles.

## Use Cases
- **Veterinarians**: Track patient health metrics, administer medications, and monitor vital signs.
- **Breeders**: Manage reproductive cycles, litter sizes, and genetic health conditions.
- **Researchers**: Collect standardized data for canine biology, ethology, or nutrition studies.
- **Dog Owners**: Monitor their pet’s health, behavior, and nutrition for optimal care.
- **Kennel Managers**: Ensure optimal environmental conditions and track exercise routines.

## Technical Details
- **Framework**: Built using .NET with Entity Framework Core for database operations.
- **Database**: Stores data in a relational structure with entities for units, observation types, scientific disciplines, and more.
- **Extensibility**: Designed to support additional metrics, units, and subject types as needed.

## Getting Started
To use Critical Dog:
1. Clone the repository from [repository URL].
2. Configure the database connection in the application settings.
3. Run the application to initialize the database with predefined data.
4. Start recording and analyzing canine data through the user interface or API.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue on the repository for bug reports, feature requests, or improvements.

## License
This project is licensed under the MIT License. See the LICENSE file for details.