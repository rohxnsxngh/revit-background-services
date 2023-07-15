using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using revit_api_parse_model.migrations;

namespace revit_api_parse_model
{
    class Tests
    {
        static void Main()
        {
            //SQLDBConnect().GetDataSql("public SQLDBConnect testsql = new SQLDBConnect()")
            Console.WriteLine("Hello World");
            Console.WriteLine("--------------Test SQL---------------");
            Console.WriteLine("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");
            Console.WriteLine("---------------");
            SQLDBConnect.TestSQLConn();
            Console.WriteLine("------------Complete SQL-------------");
        }
    }
}
