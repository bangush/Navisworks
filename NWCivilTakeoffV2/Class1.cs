#region Copyright
// Copyright (C) 2010-2015 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
#endregion // Copyright
#region Version_Notes
// NWCivliTakeoffV2
// Generates General Min Max By Each Selected "Object" And Records To .txt File
// NOTES:
// General Code Cleanup From V1
// ?No Exit Code In Debug Message Class
// ?No Adding Selected Objects To A Timestamped Set
// ?No Error Check To See If NWAttPropIDNameString Was Not Assigned Value
// ?No Save Before Continuing
// ?No Error Handling If No Objects Are Selected
// ?No Unique ID Assignment Or Prevention Method To Avoid Overwriting IDs 
// ?No Unique ID Assignment To All Terminal Nodes In Selection Tree
// ?No Dialog Box To Inquire If Results Should Be Appended Or Replace Existing Output File
// ?No Saved Sets Or Viewpoints With Labels 
#endregion
#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Text;

// Reference Libraries 
using Autodesk.Navisworks.Api;              // .Net Based Since 2011
using Autodesk.Navisworks.Api.Plugins;                                                      // C:\Windows\Microsoft.NET\assembly\GAC_64\Autodesk.Navisworks.Api\v4.0_13.0.1278.94__d85e58fa5af9b484\Autodesk.Navisworks.Api.dll
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;                                      // C:\Windows\assembly\GAC_64\Autodesk.Navisworks.Interop.ComApi\13.0.1278.94__d85e58fa5af9b484\Autodesk.Navisworks.Interop.ComApi.dll
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;                              // C:\Windows\Microsoft.NET\assembly\GAC_64\Autodesk.Navisworks.ComApi\v4.0_13.0.1278.94__d85e58fa5af9b484\Autodesk.Navisworks.ComApi.dll
using Autodesk.Navisworks.Api.ComApi;
// As Of 2015: COM Required For *Custom Properties *Manipulate Object Hyper-link/Label *Quick Properties *Section *Zoom To Object
#endregion
namespace NWCivilTakeoffV2
{
    public class DebugMessage
    {
        private uint NumberOfDubugMessages = 0;
        public void Send(string Message)
        {
            ++NumberOfDubugMessages;
            MessageBox.Show(Message,"Debug Message #"+ NumberOfDubugMessages);
        }
    }
    struct KANSimpleVirtex
    {
        public float xCoord, yCoord, zCoord;
    }
    // Kellen Ashton Nobe Global Variables
    public static class KANGVars
    {
        public static string strg = "No Text Yet"; // Used To Globally Store Values Outside Of "Triangle" Method Scope For Recording
        public static ulong idNumCount = 0; // Used To Uniquely ID Any Taken Off Object In Model
        public static string PrimitiveOutputString = "";
        public const string OutputFileName = "Result_File";// File Where Min Max Values Are Stored
        public const string PrimitivesFileName = "Primitives_File";// File Where Raw Primitives Data Is Stored
    }
    public static class KANGlobalMinMaxCoords
    {
        // Used To Globally Store Values Outside Of "ComApi.InwSimplePrimitivesCB" Scope For Recording
        public static float minX = 100000, maxX = -100000, minY = 100000, maxY = -100000, minZ = 100000, maxZ = -100000;
    }
    // Primitives
    class CallbackGeomListener : ComApi.InwSimplePrimitivesCB
    {
        public void Line(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2)
        { }    // Unused
        public void Point(ComApi.InwSimpleVertex v1)
        { }    // Unused
        public void SnapPoint(ComApi.InwSimpleVertex v1)
        { }// Unused
        public void Triangle(ComApi.InwSimpleVertex v1, ComApi.InwSimpleVertex v2, ComApi.InwSimpleVertex v3)
        {
            // Clear String
            KANGVars.PrimitiveOutputString = "";
            // Get Primitive Data And Evaluate For Min/Max
            #region Vertex 1 //
            Array array_v1 = (Array)(object)v1.coord;
            // X
            float fVal_nw2013_VS2010 = (float)(array_v1.GetValue(1));                        // Cast .coord 1 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + KANGVars.strg + "," + "Vertex 1 " + ",";
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // x
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxX)                             // Check If New Maximum
            { KANGlobalMinMaxCoords.maxX = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minX)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minX = fVal_nw2013_VS2010; }
            // Y
            fVal_nw2013_VS2010 = (float)(array_v1.GetValue(2));                             // Cast .coord 2 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // y
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxY)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxY = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minY)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minY = fVal_nw2013_VS2010; }
            //Z
            fVal_nw2013_VS2010 = (float)(array_v1.GetValue(3));                             // Cast .coord 3 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + "\n";             // z
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxZ)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxZ = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minZ)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minZ = fVal_nw2013_VS2010; }
            #endregion// Vertex 1 //
            #region Vertex 2 //
            Array array_v2 = (Array)(object)v2.coord;
            // X
            fVal_nw2013_VS2010 = (float)(array_v2.GetValue(1));                             // Cast .coord 1 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + KANGVars.strg + "," + "Vertex 2 " + ",";
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // x
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxX)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxX = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minX)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minX = fVal_nw2013_VS2010; }
            // Y
            fVal_nw2013_VS2010 = (float)(array_v2.GetValue(2));                             // Cast .coord 2 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // y
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxY)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxY = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minY)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minY = fVal_nw2013_VS2010; }
            // Z
            fVal_nw2013_VS2010 = (float)(array_v2.GetValue(3));                             // Cast .coord 3 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + "\n";             // z
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxZ)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxZ = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minZ)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minZ = fVal_nw2013_VS2010; }
            #endregion// Vertex 2
            #region Vertex 3 //
            Array array_v3 = (Array)(object)v3.coord;
            // X
            fVal_nw2013_VS2010 = (float)(array_v3.GetValue(1));                             // Cast .coord 1 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + KANGVars.strg + "," + "Vertex 3 " + ",";
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // x
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxX)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxX = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minX)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minX = fVal_nw2013_VS2010; }
            // Y
            fVal_nw2013_VS2010 = (float)(array_v3.GetValue(2));                             // Cast .coord 2 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + ",";              // y
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxY)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxY = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minY)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minY = fVal_nw2013_VS2010; }
            // Z
            fVal_nw2013_VS2010 = (float)(array_v3.GetValue(3));                             // Cast .coord 3 Value As float
            KANGVars.PrimitiveOutputString = KANGVars.PrimitiveOutputString + fVal_nw2013_VS2010.ToString() + "\n\n";             // z
            if (fVal_nw2013_VS2010 > KANGlobalMinMaxCoords.maxZ)                            // Check If New Maximum
            { KANGlobalMinMaxCoords.maxZ = fVal_nw2013_VS2010; }
            if (fVal_nw2013_VS2010 < KANGlobalMinMaxCoords.minZ)                            // Check If New Minimum
            { KANGlobalMinMaxCoords.minZ = fVal_nw2013_VS2010; }
            #endregion// Vertex 3 //

            // Result Written To File
            System.IO.File.AppendAllText(@".\" + KANGVars.PrimitivesFileName + ".txt", KANGVars.PrimitiveOutputString);
            // Reset String
            KANGVars.PrimitiveOutputString = "";

        }
    }

    #region "Plug-in AddIn Tab"
    [PluginAttribute("NWAPI_NWPlugin_To_File_AddInTab",                                     // Plug-in name
                      "Kellen Nobe",                                                        // Developer ID or GUID
                      ToolTip = "Custom Take-off Tool Development.\nOnly Measures Bounding Box Of ''Object''\nV2.",// GUI Ribbon Button Name "Tooltip"
                      DisplayName = "Box Takeoff V2")]                                      // GUI Ribbon Button Name      
    public class NWPluginModel : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            DebugMessage DebugP = new DebugMessage();
            string EndMessage = "";
            #region "Interop COM Setup"
            /////////////////////////////////////////////////////////////////////////////////////////////////
            // .Net: Setup
            Document openDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            Model model = openDoc.Models[0];

            // .Net: get current selection from .NET API
            ModelItemCollection ObjectSelectionInNet = openDoc.CurrentSelection.SelectedItems;
            // .Net: get model item
            ModelItem ObjectsModelItem;
            try
            { 
                ObjectsModelItem = openDoc.CurrentSelection.SelectedItems[0];
            }
            catch
            {
                MessageBox.Show("No Objects Selected.");
                goto Exit;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////
            // COM: Declare "InwOpState" (COM API Main Entry) Object
            ComApi.InwOpState InteropState = ComBridge.State; // Analogous To .Net "Document"
                                                              // COM: Convert .Net Selection To COM Selection
            ComApi.InwOpSelection ObjectsSelectionInCOM = ComApiBridge.ToInwOpSelection(ObjectSelectionInNet);
            // COM: Convert .Net Path To Path In COM
            ComApi.InwOaPath ObjectsPathInCOM = ComApiBridge.ToInwOaPath(ObjectsModelItem);

            // convert to model item in .NET
            ModelItem ObjectsItemInNet = ComApiBridge.ToModelItem(ObjectsPathInCOM);
            #endregion

            // zoom in current view to selected objects
            ComApiBridge.State.ZoomInCurViewOnCurSel();

            // ???
            ComApi.InwOpState9 oStateAPI = ComApiBridge.State;
            //convert to COM selection

            ComApi.InwOpSelection CurrentGUISelection = ComApiBridge.ToInwOpSelection(ObjectSelectionInNet);

            // Debug Variables
            uint levelOneIterator = 0, levelTwoIterator = 0;
            // Takeoff Tracking Variables
            uint existingIDsCount = 0;
            string CurrentTime = DateTime.Now.ToString("h:mm:ss tt");
            //ComApi.InwSelectionPathsColl ModelObjectsPaths = 

            #region "Change Custom Properties"
            //get paths collection within the selection
            ComApi.InwSelectionPathsColl ObjectCurrentSelectPaths = CurrentGUISelection.Paths();
            // Custom Properties Variables
            const string PropertiesNameString = "Custom_Properties";
            const string NWAttPropIDNameString = "ID";
            uint PropertyIndex = 0; // Used To Locate The Node For Updating Properties
            int IDPropertyIndex = 0;

            // Find Existing Custom Properties
            //foreach (ComApi.InwOaPath3


            //add custom properties To Selected Objects
            // http://adndevblog.typepad.com/aec/2012/08/addmodifyremove-custom-attribute-using-com-api.html
            foreach (ComApi.InwOaPath3 ObjectPath in ObjectCurrentSelectPaths)
            {
                //// Make Sure All Children Are Iterated Through
                //foreach(ComApi.InwOaPath3 terminalPath in oPath)
                //{ }

                //get property categories
                ComApi.InwGUIPropertyNode2 NWPropertyNode = (ComApi.InwGUIPropertyNode2)oStateAPI.GetGUIPropertyNode(ObjectPath, true);
                //create a new property category
                ComApi.InwOaPropertyVec NewPropertyCategory = (ComApi.InwOaPropertyVec)oStateAPI.ObjectFactory(
                            ComApi.nwEObjectType.eObjectType_nwOaPropertyVec, null, null);
                //create a new property
                ComApi.InwOaProperty NewProperty1 = (ComApi.InwOaProperty)oStateAPI.ObjectFactory(
                                    ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);
                NewProperty1.name = "Time";
                NewProperty1.UserName = "Takeoff Time";
                NewProperty1.value = CurrentTime;
                // Add To Category
                NewPropertyCategory.Properties().Add(NewProperty1);

                //create a new property
                ComApi.InwOaProperty NewProperty2 = (ComApi.InwOaProperty)oStateAPI.ObjectFactory(
                                    ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);
                NewProperty2.name = NWAttPropIDNameString;
                NewProperty2.UserName = NWAttPropIDNameString;
                NewProperty2.value = "ERROR: Unassigned ID";
                // Add To Category
                //NewPropertyCategory.Properties().Add(NewProperty2);
                

                //////////////////////////////////////////////////////////////////////////////////////////
                // Find User Defined GUIAttributes
                PropertyIndex = 0; // Reset Property Index (0 Indicates New Property Category)
                uint i = 0,j=0;
                foreach (ComApi.InwGUIAttribute2 NWAttribute in NWPropertyNode.GUIAttributes())
                {
                    if(NWAttribute.UserDefined == true)
                    {
                        ++i;// Move Index Finder
                        // If Particular Category Name (e.g. "Custom_Properties" Or Whatever PropertiesNameString Is Set To)
                        if (NWAttribute.ClassUserName == PropertiesNameString)
                        {
                            PropertyIndex = i;// Set Index
                            //DebugP.Send("NWAttribute.ClassUserName: " + NWAttribute.ClassUserName);
                            // Find Specific Property
                            foreach (ComApi.InwOaProperty NWProperty in NWAttribute.Properties())
                            {
                                //MessageBox.Show(NWProperty.name.ToString());
                                if (NWProperty.name.ToString() == NWAttPropIDNameString)
                                {
                                    IDPropertyIndex = (int)j;
                                    //DebugP.Send("Loop Here Found: " + NWProperty.name.ToString());
                                }
                                ++j;
                            }
                        }
                    }
                }

                //////////////////////////////////////////////////////////////////////////////////////
                try// Update Takeoff Time (Don't Change ID) If Property Exists
                {
                    //DebugP.Send("Trying To Update With Index: " + PropertyIndex.ToString());
                    //DebugP.Send("Start Property Update");
                    //DebugP.Send("a1 IDPropertyIndex Value: " + IDPropertyIndex.ToString());
                    //DebugP.Send("a2 Current Value: " + NewProperty2.value.ToString());
                    NewProperty2.value = existingIDsCount.ToString();
                    //DebugP.Send("a3 ID Value Changed");
                    //DebugP.Send("a4 Current Value: " + NewProperty2.value.ToString());
                    //NewPropertyCategory.Properties().Remove(IDPropertyIndex);
                    //DebugP.Send("a5 ID Value Changed");
                    //NewPropertyCategory.Properties().Clear();// Works
                    //DebugP.Send("a6 Properties Category Cleared");

                    NewPropertyCategory.Properties().Add(NewProperty2);
                    //DebugP.Send("a7 Properties Re-Added");
                    NWPropertyNode.SetUserDefined((int)PropertyIndex, PropertiesNameString, "Custom_Properties_InteralName", NewPropertyCategory);
                    ++existingIDsCount;
                    //DebugP.Send("a8 Update Properties Complete");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed To Change Custom Properties.\nExiting.", "ERROR: Exception Catch");
                    goto Exit;
                }
            }
            #endregion

            //////////////////////////////////////////////////////////////////////////////////////
            // Comb For Existing ID Objects
            // ?ERROR Possibility of Doubling Up Count Because Of Children Nodes
            //++existingIDsCount;

            // Show Location Of Working Directory
            //MessageBox.Show(Directory.GetCurrentDirectory());
            System.IO.File.AppendAllText(@".\" + KANGVars.PrimitivesFileName + ".txt", "File Made At " + CurrentTime + "\n\n");
            System.IO.File.AppendAllText(@".\" + KANGVars.OutputFileName + ".csv", "File Made At " + CurrentTime + "\n\nName,,X,Y,Z\n");

            // create the callback object
            CallbackGeomListener callbkListener = new CallbackGeomListener();
            foreach (ComApi.InwOaPath3 objectPath in CurrentGUISelection.Paths())// Each Object In The Selection As Distinguished By Individual Paths 
            {
                // Properties
                KANGVars.strg = levelOneIterator.ToString();

                foreach (PropertyCategory oPC in ObjectSelectionInNet[(int)levelOneIterator].PropertyCategories)
                {
                    //DebugP.Send("Loop Value: " + oPC.DisplayName);
                }

                //MessageBox.Show("Begin Level1: " + levelOneIterator.ToString());
                int TriangleExceptionCount = 0;
                foreach (ComApi.InwOaFragment3 frag in objectPath.Fragments())// Each Fragment Part Of Object
                {
                    //DebugP.Send("Begin Level2: " + levelTwoIterator.ToString());
                    // generate the primitives
                    frag.GenerateSimplePrimitives(ComApi.nwEVertexProperty.eNORMAL, callbkListener);
                    try
                    {
                        callbkListener.Triangle(frag.Geometry, frag.Geometry, frag.Geometry);

                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("An Error Occurred Calling Callback Method: '{0}'", e.ToString());
                        // Keep Count Of These Exceptions?
                        ++TriangleExceptionCount;
                        if(TriangleExceptionCount>1)
                        {
                            EndMessage = EndMessage + "\nERROR: Primitives Exceptions Exceed 1!";
                            //DebugP("ERROR: Primitives Exceptions Exceed 1!");
                        }
                    }
                    ++levelTwoIterator;

                }
                ++levelOneIterator;
                // XYZ Lengths From Min Max Values
                string XYZLengths = "";
                float calcValue;
                calcValue = KANGlobalMinMaxCoords.maxX - KANGlobalMinMaxCoords.minX;
                XYZLengths = "," + calcValue.ToString();
                calcValue = KANGlobalMinMaxCoords.maxY - KANGlobalMinMaxCoords.minY;
                XYZLengths = XYZLengths + "," + calcValue.ToString();
                calcValue = KANGlobalMinMaxCoords.maxZ - KANGlobalMinMaxCoords.minZ;
                XYZLengths = XYZLengths + "," + calcValue.ToString() + "\n";
                System.IO.File.AppendAllText(@".\" + KANGVars.OutputFileName + ".csv", KANGVars.strg + ",");
                System.IO.File.AppendAllText(@".\" + KANGVars.OutputFileName + ".csv", XYZLengths);
                #region Reset Global Min Max
                KANGlobalMinMaxCoords.minX = 100000;
                KANGlobalMinMaxCoords.maxX = -100000;
                KANGlobalMinMaxCoords.minY = 100000;
                KANGlobalMinMaxCoords.maxY = -100000;
                KANGlobalMinMaxCoords.minZ = 100000;
                KANGlobalMinMaxCoords.maxZ = -100000;
                #endregion // Reset Global Min Max

            }


            EndMessage = EndMessage + "\n Objects Evaluated: " + ObjectSelectionInNet.Count.ToString();

            Exit:
            EndMessage = EndMessage + "\n\nPlugin Operation Exit.";
            MessageBox.Show(EndMessage);
            return 0;
        }


    }
    #endregion
    // Not Sure How These Classes Work
    #region "Plug-in in Context Menu"
    //Plugin name
    [PluginAttribute("WAPI_NWModel_ContextMenu",//Plugin name                      
                     "KAN",//Developer ID or GUID                     
                           //The tooltip for the item in the ribbon    
                     ToolTip = "Navisworks Plugin Model In AddIn Tab Line 67 .cs",
                      //Display name for the Plugin in the Ribbon
                      DisplayName = "Navisworks Model Plugin ContextMenu")]

    [AddInPlugin(AddInLocation.CurrentSelectionContextMenu)]
    public class HelloWorldContextMenu : AddInPlugin//FirstNWPlugin
    {
        public override int Execute(params string[] parameters)
        {
            MessageBox.Show("Message No. 4456");
            return 0;
        }

    }
    #endregion
    #region "Dock Pane Plug-in" 
    [Plugin("NWAPI_NWModel_DockPane", //Plugin name
            "ADSK", //Developer ID or GUID         
            DisplayName = "NW Model DockPane Plugin",
            ToolTip = "NW Model DockPane Plugin")]
    [DockPanePlugin(100, 300)]
    public class BasicDockPanePlugin : DockPanePlugin
    {

        public override Control CreateControlPane()
        {
            //create the control that will be used to display in the pane
            HelloWorldControl control = new HelloWorldControl();
            control.Dock = DockStyle.Fill;
            //localisation
            control.Text = this.TryGetString("Localization Text?");
            //create the control
            control.CreateControl();
            return control;
        }

        public override void DestroyControlPane(Control pane)
        {
            pane.Dispose();
        }
    }
    #endregion 
}
