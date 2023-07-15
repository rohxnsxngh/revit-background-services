# Revit API Model Parsing

This repository contains code for extracting data from Autodesk Revit and storing it in a database using the Revit API.

## Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Usage](#usage)

## Introduction

The **revit_api_parse_model** project is a C# application that utilizes the Autodesk Revit API to extract data from a Revit model and store it in a database. The extracted data includes information about various elements in the model, such as masses, nurse call devices, doors, grids, and generic models. The extracted data is then processed and stored in a database using the provided collectors and methods.

## Installation

1. Clone the repository to your local machine:

```
git clone <repository_url>
```

2. Open the solution file **revit_api_parse_model.sln** in Visual Studio.

3. Build the solution to compile the project.

4. Set up the required dependencies:
    - Ensure that the following NuGet packages are installed:
        - Newtonsoft.Json
        - SharpGLTF.Scenes
        - SharpGLTF.Geometry
        - SharpGLTF.Geometry.VertexTypes
        - SharpGLTF.Materials

5. Acquir the revitDataExtractor.addin and place it in the correct path and install the Revit API Add in for this project.

    - Contact Forest (flam@tesla.com) or Christian (csasam@tesla.com) for more information.

## Usage

The entry point of the application is the **RevitExtractionAddIn** class, which implements the **IExternalCommand** interface. This class contains the **Execute** method, which is called when the Revit add-in is executed. The **Execute** method, in turn, calls the **ExtractionMethod** method to initiate the extraction process.

### Execution Flow

1. The **ExtractionMethod** method is called, which sets up error handling and retrieves the **UIApplication** object.
2. The **extractRevitDataReference** method is called, passing the **UIApplication** object as a parameter.
3. The **extractRevitDataReference** method extracts various information from the Revit model, such as the document, model schema, version, building levels map, and hub ID.
4. The extracted data is used to initialize project variables.
5. The **extractRevitData** method is called, passing the Document object as a parameter.
6. The **extractRevitData** method performs the data extraction process, including collecting elements using collectors and processing them.
7. The extracted data is stored in a database using the **SQLDBConnect** class.

### Collectors

The application uses several collectors to retrieve specific elements from the Revit model. The collectors are organized by category and include the following:

- **mass_collector:** Collects elements of category **OST_Mass**.
- **aisle_collector:** Collects elements of category **OST_NurseCallDevices**.
- **door_collector:** Collects elements of category **OST_Doors**.
- **grid_collector:** Collects elements of category **OST_Grids**.
- **generic_model_collector:** Collects elements of category **OST_GenericModel**.

### Processing Methods

The application includes several processing methods that handle the collected elements. These methods are called for each collector and include:

**ProcessMassElementsCollectors:** Processes the collected mass elements.
**ProcessAisleElementsCollectors:** Processes the collected aisle elements.
**ProcessDoorElementsCollectors:** Processes the collected door elements.
**ProcessGridElementsCollectors:** Processes the collected grid elements.
**ProcessGenericModelCollectors:** Processes the collected generic model elements.

These methods utilize the extracted data, building levels map, selected model schema, and other parameters to process the elements and store the data in the database.