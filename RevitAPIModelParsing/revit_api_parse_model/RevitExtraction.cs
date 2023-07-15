using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Diagnostics;
using Newtonsoft.Json;
using SharpGLTF.Scenes;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using System.Numerics;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Mechanical;
using System.Collections;
using revit_api_parse_model.models;
using revit_api_parse_model.migrations;
// using revit_api_parse_model.collectors;

namespace revit_api_parse_model
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RevitExtractionAddIn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            return ExtractionMethod(commandData);
        }

        private Result ExtractionMethod(ExternalCommandData commandData)
        {
            try
            {
                //Get application and document objects
                UIApplication uiapp = commandData.Application;

                return extractRevitDataReference(uiapp);
            }

            catch (Exception ex)
            {
                TaskDialog taskD = new TaskDialog("---ERROR---");
                StringBuilder sb = new StringBuilder();
                sb.Append("Insert ERROR \n").ToString();
                sb.AppendLine($"Error Msg: {ex.Message}");
                taskD.MainContent = sb.ToString();
                taskD.Show();
                return Result.Cancelled;
            }
        }

        public Result extractRevitDataReference(UIApplication uiapp)
        {
            Document doc = uiapp.ActiveUIDocument.Document;
            TaskDialog taskDialog = new TaskDialog("Factory Design");

            // taskDialog.Show();
            Guid sync_id = Guid.NewGuid();
            // var debugmode = false;

            var sb = new StringBuilder();
            var s_id = sync_id.ToString().ToUpper().Replace("-", string.Empty);
            var version = new revitAppVersion()
            {
                Version = "17.0.0"
                //ReleaseDate = DateTime.Parse("08/01/2022 12:00:00")
            };

            var latestver = SQLDBConnect.getAppVersion();
            var buildingLevelsMap = SQLDBConnect.getBuildingLevelsMap();
            var hubId = doc.GetHubId();
            var selectedModelSchema = (hubId == "b.d4d9f881-8f37-4242-8a5c-407a7fba9f23") ? "revitapi" : "revitapi_nova";
            if (hubId == "b.2c6b9e31-b823-4288-80f7-1abc3de8838f")
            {
                selectedModelSchema = "revitapi_nova";
            }
            else if (hubId == "b.d4d9f881-8f37-4242-8a5c-407a7fba9f23")
            {
                selectedModelSchema = "revitapi";
            }
            else
            {
                TaskDialog modelSchemaDialog = new TaskDialog("Error!");
                modelSchemaDialog.MainContent = "The hubdId of this model is currently not mapped to a schema, or this is not a cloudsharing model. Please reach out to an admin for further assistance.";
                modelSchemaDialog.Show();
                return Result.Cancelled;
            }

            string[] ver = version.Version.Split('.');
            int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));

            RevitExtraction.extractRevitData(doc);


            return Result.Succeeded;

        }
    }

    public class RevitExtraction
    {
        public static Result extractRevitData(Document doc)
        {
            /*TASK DIALOG SETUP*/
            //Document doc = uiapp.ActiveUIDocument.Document;
            TaskDialog taskDialog = new TaskDialog("Factory Design");

            // taskDialog.Show();
            Guid sync_id = Guid.NewGuid();
            // var debugmode = false;

            var sb = new StringBuilder();
            var s_id = sync_id.ToString().ToUpper().Replace("-", string.Empty);
            var version = new revitAppVersion()
            {
                Version = "17.0.0"
                //ReleaseDate = DateTime.Parse("08/01/2022 12:00:00")
            };

            var latestver = SQLDBConnect.getAppVersion();
            var buildingLevelsMap = SQLDBConnect.getBuildingLevelsMap();
            var hubId = doc.GetHubId();
            var selectedModelSchema = (hubId == "b.d4d9f881-8f37-4242-8a5c-407a7fba9f23") ? "revitapi" : "revitapi_nova";
            if (hubId == "b.2c6b9e31-b823-4288-80f7-1abc3de8838f")
            {
                selectedModelSchema = "revitapi_nova";
            }
            else if (hubId == "b.d4d9f881-8f37-4242-8a5c-407a7fba9f23")
            {
                selectedModelSchema = "revitapi";
            }
            else
            {
                TaskDialog modelSchemaDialog = new TaskDialog("Error!");
                modelSchemaDialog.MainContent = "The hubdId of this model is currently not mapped to a schema, or this is not a cloudsharing model. Please reach out to an admin for further assistance.";
                modelSchemaDialog.Show();
                return Result.Cancelled;
            }

            string[] ver = version.Version.Split('.');
            int[] appversions = Array.ConvertAll(ver, s => int.Parse(s));

            if (latestver != null && latestver[0] > appversions[0])
            {
                taskDialog.MainContent = $"Please update to the latest version of the Factory Design Extractor Addin: {string.Join(".", latestver)}!!!";
                taskDialog.Show();
                return Result.Cancelled;
            }
            else if (latestver != null && latestver[1] > appversions[1])
            {
                taskDialog.MainContent = $"Warning: your version is behind latest version: {string.Join(".", latestver)}";
                taskDialog.Show();
            }


            //initiate project variables common to document
            var model_desc = "MFG";
            var doc_id = (doc.GetWorksharingCentralModelPath().GetModelGUID() != null) ? doc.GetWorksharingCentralModelPath().GetModelGUID().ToString() : "Not Found";
            var guid_project = (doc.GetWorksharingCentralModelPath().GetProjectGUID() != null) ? doc.GetWorksharingCentralModelPath().GetProjectGUID().ToString() : "Not Found"; //element.Document.ActiveProjectLocation.UniqueId,
            var model_name = doc.Title;
            var model_path = doc.PathName;
            var sync_time = DateTime.UtcNow;

            //var s_id = sync_id.ToString().ToUpper().Replace("-", string.Empty);
            var user_name = Environment.UserName;
            var user_machine = Environment.MachineName;

            taskDialog.MainContent = $"sid: {s_id}";
            taskDialog.Show();

            var revitDoc = new revit_document()
            {
                guid_project = guid_project,
                doc_id = doc_id,
                model_name = model_name,
                model_desc = model_desc,
                model_path = model_path,
                sync_time = sync_time,
                sync_id = s_id,
                user_name = user_name,
                user_machine = user_machine
            };

            taskDialog.MainContent = SQLDBConnect.postDocData(revitDoc, selectedModelSchema);
            taskDialog.Show();
            // string strJson = JsonConvert.SerializeObject(new_json_array);
            string strJsons = JsonConvert.SerializeObject(revitDoc).ToString();


            sb.AppendLine("Start Data Transfer");
            sb.AppendLine("elem type: " + doc.GetType().ToString());
            sb.AppendLine("doc_id" + doc_id);
            sb.AppendLine("guid_proj" + guid_project);
            sb.AppendLine("model_name: " + model_name);
            sb.AppendLine("model_desc: " + model_desc);
            sb.AppendLine("model_path: " + model_path);
            sb.AppendLine("sync_id: " + s_id);
            sb.AppendLine("Sync Time: " + DateTime.UtcNow);


            /* ELEMENT COLLECTORS, ADD WITH NEW CATEGORIES */
            FilteredElementCollector mass_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Mass).WhereElementIsNotElementType();
            FilteredElementCollector aisle_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_NurseCallDevices).WhereElementIsNotElementType();
            FilteredElementCollector door_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType();
            FilteredElementCollector grid_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType();
            FilteredElementCollector generic_model_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).WhereElementIsNotElementType();

            ProcessMassElementsCollectors(doc, s_id, buildingLevelsMap, selectedModelSchema, mass_collector);
            ProcessAisleElementsCollectors(doc, s_id, buildingLevelsMap, selectedModelSchema, aisle_collector);
            ProcessDoorElementsCollectors(doc, s_id, buildingLevelsMap, selectedModelSchema, door_collector);
            ProcessGridElementsCollectors(s_id, buildingLevelsMap, selectedModelSchema, grid_collector);
            ProcessGenericModelCollectors(doc, s_id, buildingLevelsMap, selectedModelSchema, generic_model_collector);


            taskDialog.MainContent = sb.ToString();
            taskDialog.MainContent = "Data Transfer Complete";
            taskDialog.Show();

            return Result.Cancelled;

        }

        private static void ProcessGenericModelCollectors(Document doc, string s_id, Dictionary<string, string>? buildingLevelsMap, string selectedModelSchema, FilteredElementCollector generic_model_collector)
        {
            // Get Generic Model Elements
            foreach (Element element in generic_model_collector)
            {
                // break;
                try
                {
                    // Debug.WriteLine(element + element.Name + "----" + element.LookupParameter("Family").AsValueString());
                    // element.LookupParameter("Family").AsValueString().Contains("TSLA_MFG_KOS")
                    // var selectGenericModelFamilies = new[] { "GEN_OHC_STL_Overhead_Coiling_Door", "TSLA_MFE_DOCK DOOR" }
                    if (element.LookupParameter("Family").AsValueString().Contains("TSLA_MFE_VRC"))
                    {
                        var newVRCElem = GetBaseElementProperties(element, s_id, buildingLevelsMap!, doc);
                        // check if rectangle block, then insert W/L/H parameters
                        try
                        {

                            newVRCElem.width = element.LookupParameter("TSLA Width").AsDouble();
                            newVRCElem.length = element.LookupParameter("TSLA Length").AsDouble();
                            newVRCElem.height = element.LookupParameter("TSLA Height").AsDouble();
                            // newVRCElem.height = element.LookupParameter("2M").AsDouble();

                            //Check if VRC is double vrc
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        try
                        {
                            newVRCElem.is_double_vrc = element.LookupParameter("Double VRC").AsInteger() == 1 ? true: false;

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        //call my sqldb stored procedure function here
                        SQLDBConnect.postDataSQL(revElem: newVRCElem, schema_name: selectedModelSchema);
                    }

                    if (element.LookupParameter("Family").AsValueString().Contains("TSLA_MFE_PARAMETRIC RAMP"))
                    {

                        var newRampElem = GetBaseElementProperties(element, s_id, buildingLevelsMap!, doc);

                        newRampElem.width = element.LookupParameter("Ramp Clear Width").AsDouble();
                        newRampElem.length = element.LookupParameter("Ramp Length").AsDouble();
                        newRampElem.height = element.LookupParameter("Ramp Height").AsDouble();

                        /*                                IList<Subelement> dependentElements = element.GetSubelements() as IList<Subelement>;

                                                        foreach (Subelement dependentElement in dependentElements)
                                                        {
                                                            var newRampNode = GetBaseElementProperties(dependentElement, s_id, buildingLevelsMap, doc);

                                                        }*/


                        //call my sqldb stored procedure function here
                        SQLDBConnect.postDataSQL(revElem: newRampElem, schema_name: selectedModelSchema);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("Mass Element Insertion Failed: (ELEMENT_ID): " + element.Id.IntegerValue + " TSLA Name: " + element.LookupParameter("TSLA Name").AsString() + element.DesignOption);
                }


            }
        }

        private static void ProcessGridElementsCollectors(string s_id, Dictionary<string, string>? buildingLevelsMap, string selectedModelSchema, FilteredElementCollector grid_collector)
        {
            // Get Grids
            foreach (Element element in grid_collector)
            {
                try
                {
                    // Debug.WriteLine("GRID ELEMENT: " + element.UniqueId + element.LookupParameter("Workset").AsValueString());
                    // Debug.WriteLine("DOOR: " + element.LookupParameter("Family").AsValueString() + (element.Location as LocationPoint).Point.X);
                    // var newGridElem = GetBaseElementProperties(element, s_id, buildingLevelsMap, doc) as fd_element_mass;

                    var newGridElem = new fd_element()
                    {
                        element_uid = element.UniqueId,
                        element_id = element.Id.IntegerValue,
                        sync_id = s_id,
                        design_option_set = (element.LookupParameter("Design Option").AsString() != null) ? element.LookupParameter("Design Option").AsString() : "Main Model",
                        design_option_name = (element.DesignOption != null) ? element.DesignOption.Name.ToString() : "Main Model",
                        design_option_is_primary = (element.DesignOption != null) ? element.DesignOption.IsPrimary : true,
                        category_name = element.LookupParameter("Category").AsValueString(),
                        workset_name = element.LookupParameter("Workset").AsValueString(),
                        level_name = "GRID",
                        level_name_standardized = "GRID",
                        phase_created = "GRID",
                        phase_demolished = "GRID",
                        comments = "",
                        family_name = element.LookupParameter("Family").AsValueString(),
                        type_name = element.LookupParameter("Type").AsValueString(),
                        point_1_x = ((element as Grid)!.Curve as Line)!.GetEndPoint(0).X,
                        point_1_y = ((element as Grid)!.Curve as Line)!.GetEndPoint(0).Y,
                        point_1_z = ((element as Grid)!.Curve as Line)!.GetEndPoint(0).Z,
                        point_2_x = ((element as Grid)!.Curve as Line)!.GetEndPoint(1).X,
                        point_2_y = ((element as Grid)!.Curve as Line)!.GetEndPoint(1).Y,
                        point_2_z = ((element as Grid)!.Curve as Line)!.GetEndPoint(1).Z,
                        tsla_name = element.Name,
                        // element_glb = glb_string
                        // default to false because this is a GRID LINE
                        is_mirrored = false,
                        facing_flipped = false,
                        hand_flipped = false
                    };

                    // add level name based on revit database table "building_levels"
                    if (buildingLevelsMap!.TryGetValue(newGridElem.level_name, out string mapped_value))
                    {
                        newGridElem.level_name_standardized = mapped_value;
                    }
                    else
                    {
                        // Debug.WriteLine("LEVEL NAME MAPPING: TEST: " + " PARAMETER VALUE: " + element.LookupParameter("Level").AsValueString() + "CHECKING DICT: ...");
                        newGridElem.level_name_standardized = newGridElem.level_name;
                    }

                    // design option set name workaround
                    if (newGridElem.design_option_set != "Main Model")
                    {
                        int charLocation = newGridElem.design_option_set.IndexOf(":", StringComparison.Ordinal);

                        if (charLocation > 0)
                        {
                            newGridElem.design_option_set = newGridElem.design_option_set.Substring(0, charLocation - 1);
                        }
                    }
                    else
                    {
                        newGridElem.design_option_set = "Main Model";
                    }

                    SQLDBConnect.postDataSQL(revElem: newGridElem, schema_name: selectedModelSchema);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("Grid Element Insertion Failed: (ELEMENT_ID): " + element.Id.IntegerValue + " Family Name: " + element.LookupParameter("Family").AsValueString());
                }
            }
        }

        private static void ProcessDoorElementsCollectors(Document doc, string s_id, Dictionary<string, string>? buildingLevelsMap, string selectedModelSchema, FilteredElementCollector door_collector)
        {
            // Get Dock Doors (ARCH models)
            // List<fd_element_mass> doorElems = new List<fd_element_mass>();
            var allowedDoorFamilies = new[] { "GEN_OHC_STL_Overhead_Coiling_Door", "TSLA_MFE_DOCK DOOR" };
            foreach (Element element in door_collector)
            {
                try
                {
                    if (allowedDoorFamilies.Any(element.LookupParameter("Family").AsValueString().Contains))
                    {
                        var newDoorElem = GetBaseElementProperties(element, s_id, buildingLevelsMap!, doc);
                        newDoorElem.width = 10.0;
                        newDoorElem.length = 10.0;
                        newDoorElem.height = 10.0;

                        //call my sqldb stored procedure function here
                        SQLDBConnect.postDataSQL(revElem: newDoorElem, schema_name: selectedModelSchema);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("Door Element Insertion Failed: (ELEMENT_ID): " + element.Id.IntegerValue + " Family Name: " + element.LookupParameter("Family").AsValueString());
                }
            }
        }

        private static void ProcessAisleElementsCollectors(Document doc, string s_id, Dictionary<string, string>? buildingLevelsMap, string selectedModelSchema, FilteredElementCollector aisle_collector)
        {
            // Get Aisle Elements
            foreach (Element element in aisle_collector)
            {
                try
                {
                    var newAisleElem = GetBaseElementProperties(element, s_id, buildingLevelsMap!, doc);

                    newAisleElem.point_2_x = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.X : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(1).X;
                    newAisleElem.point_2_y = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.Y : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(1).Y;
                    newAisleElem.point_2_z = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.Z : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(1).Z;

                    //try catch is for aisle and wip elements that have null for the following values and want to move on with code
                    try
                    {
                        newAisleElem.aisle_two_way = element.LookupParameter("2 Way Aisle").AsValueString().Equals("Yes") ? true : false;
                        newAisleElem.a_load_width = element.LookupParameter("A Load Width").AsDouble();
                        newAisleElem.b_load_width = element.LookupParameter("B Load Width").AsDouble();
                        newAisleElem.aisle_clearance_height = (float)element.LookupParameter("Aisle Clearance Height").AsDouble();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    // for mhe aisle family parameters
                    try
                    {
                        newAisleElem.pedestrian_width = (float)element.LookupParameter("Pedestrian Width").AsDouble();
                        newAisleElem.pedestrian_clearance_height = (float)element.LookupParameter("Pedestrian Clearance Height").AsDouble();
                        newAisleElem.has_pedestrian = element.LookupParameter("Pedestrian").AsValueString().Equals("Yes") ? true : false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    // for pedestrian family parameters
                    try
                    {
                        newAisleElem.pedestrian_width = (float)element.LookupParameter("Ped Width").AsDouble();
                        newAisleElem.pedestrian_clearance_height = (float)element.LookupParameter("Pedestrian Clearance Height").AsDouble();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    try
                    {

                        newAisleElem.wip_name = (element.LookupParameter("TSLA WIP Name") != null) ? element.LookupParameter("TSLA WIP Name").AsString() : "";
                        newAisleElem.wip_part_quantity = (element.LookupParameter("TSLA MFG Part Quantity") != null) ? element.LookupParameter("TSLA MFG Part Quantity").AsInteger() : 0;
                        newAisleElem.wip_part_description = (element.LookupParameter("TSLA MFG Part Description") != null) ? element.LookupParameter("TSLA MFG Part Description").AsString() : "";

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    try
                    {
                        newAisleElem.width = (element.LookupParameter("Width") != null) ? element.LookupParameter("Width").AsDouble() : element.LookupParameter("Total Width").AsDouble();
                        newAisleElem.length = (element.LookupParameter("Length") != null) ? element.LookupParameter("Length").AsDouble() : newAisleElem.width;
                        newAisleElem.height = (element.LookupParameter("Height") != null) ? element.LookupParameter("Height").AsDouble() : 3.0; // 3.0 is temporary, need to standardize aisle intersection property naming
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    //Debug.WriteLine("WIDTH: " + newAisleElem.width + "AISLE CLEARANCE HEIGHT: " + newAisleElem.aisle_clearance_height + "PEDESTRIAN WIDTH: " + newAisleElem.pedestrian_width + "PEDESTRIAN CLEARANCE HEIGHT: " + newAisleElem.pedestrian_clearance_height);
                    //call my sqldb stored procedure function here

                    //var results = new SQLDBConnect(); //this will add each element and its properties to my db
                    if (newAisleElem.family_name == "TSLA_MFE_NODE_KOS")
                    {
                        // Debug.WriteLine("I AM A MFNODE");
                        newAisleElem.width = element.LookupParameter("W").AsDouble();
                        newAisleElem.length = element.LookupParameter("W").AsDouble();
                        newAisleElem.height = element.LookupParameter("H").AsDouble();
                        newAisleElem.level_name = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().Where(x => x.ProjectElevation <= (element.Location as LocationPoint)!.Point.Z + 0.1).OrderByDescending(x => x.ProjectElevation).First().Name;
                        newAisleElem.tsla_node_type = element.LookupParameter("TSLA Type").AsString();

                        // add level name based on revit database table "building_levels" -- needs to be after try/catch level names
                        if (buildingLevelsMap!.TryGetValue(newAisleElem.level_name, out string mapped_value))
                        {
                            newAisleElem.level_name_standardized = mapped_value;
                        }
                        else
                        {
                            // Debug.WriteLine("LEVEL NAME MAPPING: TEST: " + " PARAMETER VALUE: " + element.LookupParameter("Level").AsValueString() + "CHECKING DICT: ...");
                            newAisleElem.level_name_standardized = newAisleElem.level_name;
                        }
                    }

                    if (newAisleElem.family_name == "TSLA_MFE_NODE")
                    {
                        // Debug.WriteLine("I AM A MFNODE");
                        newAisleElem.width = element.LookupParameter("W").AsDouble();
                        newAisleElem.length = element.LookupParameter("L").AsDouble();
                        newAisleElem.height = element.LookupParameter("H").AsDouble();
                        newAisleElem.level_name = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().Where(x => x.ProjectElevation <= (element.Location as LocationPoint)!.Point.Z + 0.1).OrderByDescending(x => x.ProjectElevation).First().Name;
                        newAisleElem.tsla_node_type = element.LookupParameter("TSLA Type").AsString();
                        if (element.LookupParameter("IN").AsValueString() == "Yes")
                        {
                            if (element.LookupParameter("OUT").AsValueString() == "No")
                            {
                                newAisleElem.tsla_node_is_in = true;
                            }
                            else
                            {
                                newAisleElem.tsla_node_is_in = null;
                            }
                        }
                        else if (element.LookupParameter("OUT").AsValueString() == "Yes")
                        {
                            if (element.LookupParameter("IN").AsValueString() == "No")
                            {
                                newAisleElem.tsla_node_is_in = false;
                            }
                            else
                            {
                                newAisleElem.tsla_node_is_in = null;
                            }
                        }

                        // add level name based on revit database table "building_levels" -- needs to be after try/catch level names
                        if (buildingLevelsMap!.TryGetValue(newAisleElem.level_name, out string mapped_value))
                        {
                            newAisleElem.level_name_standardized = mapped_value;
                        }
                        else
                        {
                            // Debug.WriteLine("LEVEL NAME MAPPING: TEST: " + " PARAMETER VALUE: " + element.LookupParameter("Level").AsValueString() + "CHECKING DICT: ...");
                            newAisleElem.level_name_standardized = newAisleElem.level_name;
                        }

                        // get host element parameters (VRC parent of MFNODES)
                        try
                        {
                            // Debug.WriteLine("THE MFNODE HOST IS: " + (element as FamilyInstance).SuperComponent.LookupParameter("TSLA Name").AsValueString());
                            newAisleElem.tsla_name = (element as FamilyInstance)!.SuperComponent.LookupParameter("TSLA Name").AsValueString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        // Debug.WriteLine("I AM A MF NODE" + newAisleElem.level_name + newAisleElem.tsla_node_type + element.LookupParameter("IN").AsValueString());
                    }

                    /*if (newAisleElem.type_name.Contains("intersection"))
                    {
                        // Debug.WriteLine("INTERSECTION: " + newAisleElem.type_name);
                    }*/

                    SQLDBConnect.postDataSQL(revElem: newAisleElem, schema_name: selectedModelSchema);

                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("Nurse Call Devices Element Insertion Failed: (ELEMENT_ID): " + element.Id.IntegerValue + " Family Name: " + element.LookupParameter("Family").AsValueString());
                }
            }
        }

        private static void ProcessMassElementsCollectors(Document doc, string s_id, Dictionary<string, string>? buildingLevelsMap, string selectedModelSchema, FilteredElementCollector mass_collector)
        {
            // Get Mass Elements
            foreach (Element element in mass_collector)
            {
                try
                {
                    Debug.WriteLine(element + element.Name + "----" + element.LookupParameter("Family").AsValueString());
                    // element.LookupParameter("Family").AsValueString().Contains("TSLA_MFG_KOS")
                    if (element.LookupParameter("Family").AsValueString().Contains("TSLA_MFG_KOS"))
                    {
                        Debug.WriteLine("-------revit extraction start process-------");
                        var newMassElem = GetBaseElementProperties(element: element, sync_id: s_id, buildingLevelsMap: buildingLevelsMap!, doc: doc);
                        Debug.WriteLine("-------revit extraction processing-------");

                        // check if triangle block, then insert P1X P2Y parameters
                        if (newMassElem.family_name!.Contains("TRIANGLE")!)
                        {
                            try
                            {
                                newMassElem.width = element.LookupParameter("P1 X").AsDouble();
                                newMassElem.length = element.LookupParameter("P2 Y").AsDouble();
                                newMassElem.height = element.LookupParameter("Height").AsDouble();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }
                        // check if rectangle block, then insert W/L/H parameters
                        else
                        {
                            try
                            {

                                newMassElem.width = element.LookupParameter("W").AsDouble();
                                newMassElem.length = element.LookupParameter("L").AsDouble();
                                newMassElem.height = element.LookupParameter("H").AsDouble();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }

                        //call my sqldb stored procedure function here
                        SQLDBConnect.postDataSQL(revElem: newMassElem, schema_name: selectedModelSchema);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Debug.WriteLine("Mass Element Insertion Failed: (ELEMENT_ID): " + element.Id.IntegerValue + " TSLA Name: " + element.LookupParameter("TSLA Name").AsString() + element.DesignOption);
                }
            }
        }

        public static fd_element GetBaseElementProperties(Element element, string sync_id, Dictionary<string, string> buildingLevelsMap, Document doc)
        {
            var newFdBaseElement = CreateBaseElement(element, sync_id, doc);
            AdjustDesignOptionSetName(newFdBaseElement);
            AdjustLevelName(newFdBaseElement, element, doc);
            SetTSLANames(newFdBaseElement, element);
            SetTSLASpaceAllocationID(newFdBaseElement, element);
            SetTSLAScopeID(newFdBaseElement, element);
            SetHandOrientation(newFdBaseElement, element);
            SetParentElementUID(newFdBaseElement, element);
            AddStandardizedLevelName(newFdBaseElement, buildingLevelsMap);

            return newFdBaseElement;
        }

        private static fd_element CreateBaseElement(Element element, string sync_id, Document doc)
        {
            var newFdBaseElement = new fd_element()
            {
                element_uid = element.UniqueId,
                element_id = element.Id.IntegerValue,
                sync_id = sync_id,
                design_option_set = GetParameterValue(element, "Design Option", "Main Model")!,
                design_option_name = GetDesignOptionName(element),
                design_option_is_primary = GetDesignOptionIsPrimary(element),
                category_name = GetParameterValue(element, "Category", "")!,
                workset_name = GetParameterValue(element, "Workset", ""),
                level_name = GetLevelName(element, doc),
                phase_created = GetParameterValue(element!, "Phase Created", "")!,
                phase_demolished = GetParameterValue(element, "Phase Demolished", ""),
                comments = GetParameterValue(element, "Comments", ""),
                family_name = GetParameterValue(element, "Family", ""),
                type_name = GetParameterValue(element, "Type", ""),
                point_1_x = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.X : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(0).X,
                point_1_y = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.Y : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(0).Y,
                point_1_z = (element.Location as LocationPoint != null) ? (element.Location as LocationPoint)!.Point.Z : ((element.Location as LocationCurve)!.Curve as Line)!.GetEndPoint(0).Z,
                is_mirrored = (element as FamilyInstance)!.Mirrored,
                facing_flipped = (element as FamilyInstance)!.FacingFlipped,
                hand_flipped = (element as FamilyInstance)!.HandFlipped
            };

            return newFdBaseElement;
        }

        private static string? GetParameterValue(Element element, string parameterName, string? defaultValue)
        {
            var parameter = element.LookupParameter(parameterName);
            return parameter != null ? parameter.AsValueString() ?? defaultValue : defaultValue;
        }

        private static string GetDesignOptionName(Element element)
        {
            return element.DesignOption != null ? element.DesignOption.Name.ToString() : "Main Model";
        }

        private static bool GetDesignOptionIsPrimary(Element element)
        {
            return element.DesignOption != null ? element.DesignOption.IsPrimary : true;
        }

        private static string GetLevelName(Element element, Document doc)
        {
            if (element.Location is not LocationCurve locationCurve)
            {
                return GetParameterValue(element, "Level", "")!;
            }

            var endPoint = (locationCurve.Curve as Line)!.GetEndPoint(0).Z + 0.1;
            var level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .Cast<Level>()
                .Where(x => x.ProjectElevation <= endPoint)
                .OrderByDescending(x => x.ProjectElevation)
                .First();

            return level.Name;
        }

        private static void AdjustDesignOptionSetName(fd_element newFdBaseElement)
        {
            if (newFdBaseElement.design_option_set != "Main Model")
            {
                int charLocation = newFdBaseElement.design_option_set.IndexOf(":", StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    newFdBaseElement.design_option_set = newFdBaseElement.design_option_set.Substring(0, charLocation - 1);
                }
            }
            else
            {
                newFdBaseElement.design_option_set = "Main Model";
            }
        }

        private static void AdjustLevelName(fd_element newFdBaseElement, Element element, Document doc)
        {
            if (string.IsNullOrEmpty(newFdBaseElement.level_name))
            {
                var locationPoint = element.Location as LocationPoint;
                if (locationPoint != null)
                {
                    var endPoint = locationPoint.Point.Z + 0.1;
                    var level = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .Cast<Level>()
                        .Where(x => x.ProjectElevation <= endPoint)
                        .OrderByDescending(x => x.ProjectElevation)
                        .First();

                    newFdBaseElement.level_name = level.Name;
                }
            }
        }

        private static void SetTSLANames(fd_element newFdBaseElement, Element element)
        {
            try
            {
                newFdBaseElement.tsla_name = GetParameterValue(element, "TSLA Name", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                newFdBaseElement.tsla_shop = GetParameterValue(element, "TSLA Shop", null);
                newFdBaseElement.tsla_program = GetParameterValue(element, "TSLA Program", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SetTSLASpaceAllocationID(fd_element newFdBaseElement, Element element)
        {
            try
            {
                newFdBaseElement.tsla_space_allocation_id = GetParameterValue(element, "TSLA Space Allocation ID", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SetTSLAScopeID(fd_element newFdBaseElement, Element element)
        {
            try
            {
                newFdBaseElement.tsla_scope_id = GetParameterValue(element, "TSLA_SCOPE_ID", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SetHandOrientation(fd_element newFdBaseElement, Element element)
        {
            try
            {
                newFdBaseElement.hand_orientation = (element as FamilyInstance)?.HandOrientation.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void SetParentElementUID(fd_element newFdBaseElement, Element element)
        {
            try
            {
                newFdBaseElement.parent_element_uid = (element as FamilyInstance)?.SuperComponent?.UniqueId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private static void AddStandardizedLevelName(fd_element newFdBaseElement, Dictionary<string, string> buildingLevelsMap)
        {
            if (buildingLevelsMap.TryGetValue(newFdBaseElement.level_name, out string mapped_value))
            {
                newFdBaseElement.level_name_standardized = mapped_value;
            }
            else
            {
                newFdBaseElement.level_name_standardized = newFdBaseElement.level_name;
            }
        }

    }
}