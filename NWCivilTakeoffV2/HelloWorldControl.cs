#region Copyright
//
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
#region References
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
#endregion
namespace NWCivilTakeoffV2
{
    public partial class HelloWorldControl : UserControl
    {
        public HelloWorldControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A button in dock pane panel is clicked!");

            //This is to demo how to load other plugin dynamically
            //LoadPluginDynamic();
        }

        //load other plugin dynamically
        void LoadPluginDynamic()
        {
            //the name of the plugin which is to be loaded dynamically
            string plugin_name_to_load = "NWAPI_HelloWorldPlugin_AddInTab.ADSK";
            // plugin record
            PluginRecord dotest = null;
            dotest = Autodesk.Navisworks.Api.Application.Plugins.
                                            FindPlugin(plugin_name_to_load);
            if (dotest != null)
                // the plugin binary has been loaded. exit.
                MessageBox.Show("the plugin " + plugin_name_to_load + " has been loaded!");
            else
                // load the plugin binary from full file path name
                Autodesk.Navisworks.Api.Application.Plugins.AddPluginAssembly(
                             @"<Navisworks Install Path>\Lab-02.dll");

            // get the plugin record.
            PluginRecord otherpluginrecord =
                Autodesk.Navisworks.Api.Application.Plugins.
                                    FindPlugin(plugin_name_to_load);
            if (otherpluginrecord != null)
            {
                if (!otherpluginrecord.IsLoaded)
                {
                    //if not loaded, load the plugin
                    otherpluginrecord.LoadPlugin();
                    //get the plugin of the record
                    Plugin otherplugin = otherpluginrecord.LoadedPlugin;

                    //call one method of the plugin
                    //since we do not know the type of the other plugin,
                    //use InvokeMember
                    string[] pa = { "dummy" };
                    otherplugin.GetType().InvokeMember("Execute",
                                System.Reflection.BindingFlags.InvokeMethod,
                                null, otherplugin, pa);
                }
            }

        }

    }
}
