﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Configuration;

namespace FlakeWrap
{
    class Program
    {
        static int times = Convert.ToInt16(ConfigurationManager.AppSettings["requiredInstances"]);
        static string url = "http://plato.delphi04.euromoneydigital.com/flake/id?n=10";
        
        
        static void Main(string[] args)
        {

            List<string> collection = new List<string>();
            using (var wb = new WebClient())
            {
                for (int i = 0; i < times; i++)
                {
                    var response = wb.DownloadString(url);
                    List<string> flakeIDs = response.Split('\n').ToList<string>();
                    foreach (string flakeID in flakeIDs)
                    {
                        collection.Add(flakeID);
                    }
                }
            }

            cleanTable();
            writeTable(collection);
        }

        private static void cleanTable()
        {
            using (MyDataClassDataContext context = new MyDataClassDataContext())
            {
                context.FlakeWraps.DeleteAllOnSubmit(context.FlakeWraps);
                context.SubmitChanges();
            }            
        }

        private static void writeTable(List<string> collection)
        {
            using (MyDataClassDataContext context = new MyDataClassDataContext())
            {                
                foreach (string item in collection)
                {
                    FlakeWrap fw = new FlakeWrap();
                    fw.FlakeID = item;                    
                    context.FlakeWraps.InsertOnSubmit(fw);
                }
                
                context.SubmitChanges();
            }
        }
    }
}
