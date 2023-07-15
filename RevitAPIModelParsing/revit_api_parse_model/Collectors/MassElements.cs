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

namespace revit_api_parse_model.collectors
{
    public class ProcessMassElements
    {
        // futuure collectors will be individually moved into this area
    }
}