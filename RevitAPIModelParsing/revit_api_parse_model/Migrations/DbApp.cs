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

namespace revit_api_parse_model
{
    [Transaction(TransactionMode.Manual)]
    public class SaveNotify : IExternalApplication
    {
        private void CA_DocumentSaved(object sender, Autodesk.Revit.DB.Events.DocumentSavedEventArgs e)
        {
            TaskDialog.Show("Revit", $"Saved {e.Document.PathName}");
        }

        private void CA_DocumentSavedAs(object sender, Autodesk.Revit.DB.Events.DocumentSavedAsEventArgs e)
        {
            TaskDialog.Show("Revit", $"SavedAs {e.Document.PathName}");
        }
        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentSaved += CA_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs += CA_DocumentSavedAs;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentSaved -= CA_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs -= CA_DocumentSavedAs;
            return Result.Succeeded;
        }
    }
}
