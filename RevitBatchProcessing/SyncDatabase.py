# For example assume your DLL is called MyUtilities.dll and you have a static function called SomeClass.DoSomeWork() in namespace MyNameSpace:
# Assume this python script exists in the same folder as MyUtilities.dll.
# import clr;

# clr.AddReference("MyUtilities")
# from MyNameSpace import SomeClass

# # Invoke your static function, passing in any parameters you need.
# SomeClass.DoSomeWork(doc)

# This section is common to all of these scripts. 
import clr
import System

clr.AddReference("RevitAPI")
clr.AddReference("RevitAPIUI")
clr.AddReference("revit_api_parse_model")
from Autodesk.Revit.DB import *

import revit_script_util
from revit_script_util import Output
from revit_api_parse_model import RevitExtraction

uiapp = revit_script_util.GetUIApplication()
Output("Hello Revit world!1")
doc = revit_script_util.GetScriptDocument()
Output("Hello Revit world!2")
Output(doc.PathName)
# uidoc = uiapp.OpenAndActivateDocument(doc.PathName)
Output("Hello Revit world!3")
# Output(uidoc)
RevitExtraction.extractRevitData(doc)

# Output(RevitExtraction.testRevitExtraction())

sessionId = revit_script_util.GetSessionId()
uiapp = revit_script_util.GetUIApplication()

# NOTE: these only make sense for batch Revit file processing mode.
doc = revit_script_util.GetScriptDocument()
revitFilePath = revit_script_util.GetRevitFilePath()


# The code above is boilerplate, everything below is yours!

Output()
Output("Hello Revit world!")