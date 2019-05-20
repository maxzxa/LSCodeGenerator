using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LSCodeGenerator
{
    public class FileGenerator
    {
        public void CreateEntitesFile(String fileName, ColumnCollection columns, String TableName) {
            //string fileName = @"C:\Temp\Mahesh.txt";
            try
            {
                //Si no existe un directorio lo crea
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                //Si existe lo elimina
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    FileStream fsFileStream = File.Create(fileName);
                    fsFileStream.Close();
                }
                else
                {
                    FileStream fsFileStream = File.Create(fileName);
                    fsFileStream.Close();
                }

                AddReferences(fileName);
                AddNameSpace(fileName, "LSEntities");
                AddPublicClass(fileName, TableName.Remove(0, 2));
                foreach (Column column in columns)
                {
                    AddPrivateVariable(fileName, getCSharpType(column.DataType.Name), column.Name);
                    AddPublicVariable(fileName, getCSharpType(column.DataType.Name), column.Name);
                }
                
                AddClose(fileName,1);
                AddClose(fileName,0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateSPFile(String fileName, ColumnCollection columns, String Type, String TableName)
        {
            //string fileName = @"C:\Temp\Mahesh.txt";
            try
            {
                //Si no existe un directorio lo crea
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                //Si existe lo elimina
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    FileStream fsFileStream = File.Create(fileName);
                    fsFileStream.Close();
                }
                else
                {
                    FileStream fsFileStream = File.Create(fileName);
                    fsFileStream.Close();
                }

                switch (Type)
                {
                    case "Delete": CreateDeleteProcedure(fileName, columns,TableName); break;
                    case "Insert": CreateInsertProcedure(fileName, columns, TableName); break;
                    case "Update": CreateUpdateProcedure(fileName, columns, TableName); break;
                    case "SelectAll": CreateSelectAllProcedure(fileName, columns, TableName); break;
                    case "SelectById": CreateSelectByIdProcedure(fileName, columns, TableName); break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateDeleteProcedure(String FileName, ColumnCollection columns, String TableName) {
            String PrimaryKey = string.Empty;
            
            File.AppendAllText(FileName, "CREATE PROCEDURE sp" + TableName.Remove(0, 2) + "_Delete("
                                         + Environment.NewLine);
            foreach (Column column in columns)
            {
                if (column.InPrimaryKey)
                {
                    PrimaryKey = column.Name;
                    File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name
                                         + Environment.NewLine);
                }
            }
            File.AppendAllText(FileName, ") AS BEGIN"
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t DELETE " + TableName
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t WHERE " + PrimaryKey + " = @" + PrimaryKey 
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "END "
                                        + Environment.NewLine);
        }

        private void CreateInsertProcedure(String FileName, ColumnCollection columns, String TableName)
        {
            Int32 Aux = 1;

            File.AppendAllText(FileName, "CREATE PROCEDURE sp" + TableName.Remove(0, 2) + "_Insert("
                                         + Environment.NewLine);
            foreach (Column column in columns)
            {
                if (!column.InPrimaryKey)
                {
                    if ((columns.Count - 1) == Aux)
                    {
                        if (column.DataType.Name == "varchar")
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + " (" + column.DataType.MaximumLength + ")" + Environment.NewLine);
                        else
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name
                                                                  + Environment.NewLine);
                    }
                    else
                    {
                        if (column.DataType.Name == "varchar")
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + " (" + column.DataType.MaximumLength + ")" + ","
                                                                 + Environment.NewLine);
                         else
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + ","
                                                                     + Environment.NewLine);
                    }
                    Aux++;
                }
                
            }
            File.AppendAllText(FileName, ") AS BEGIN"
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t INSERT INTO " + TableName
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t VALUES (");

            Aux = 1;
            foreach (Column column in columns)
            {
                if (!column.InPrimaryKey)
                {
                    if ((columns.Count - 1) == Aux)
                    {
                        File.AppendAllText(FileName, "@" + column.Name);
                    }
                    else
                    {
                        File.AppendAllText(FileName, "@" + column.Name + ",");
                    }
                    Aux++;
                }
            }
            File.AppendAllText(FileName, ")" + Environment.NewLine);

            File.AppendAllText(FileName, "END "
                                        + Environment.NewLine);
        }

        private void CreateUpdateProcedure(String FileName, ColumnCollection columns, String TableName)
        {
            Int32 Aux = 1;

            File.AppendAllText(FileName, "CREATE PROCEDURE sp" + TableName.Remove(0, 2) + "_Update("
                                         + Environment.NewLine);
            foreach (Column column in columns)
            {
                    if ((columns.Count) == Aux)
                    {
                        if (column.DataType.Name == "varchar")
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + " (" + column.DataType.MaximumLength + ")" + Environment.NewLine);
                        else
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name
                                                                  + Environment.NewLine);
                    }
                    else
                    {
                        if (column.DataType.Name == "varchar")
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + " (" + column.DataType.MaximumLength + ")" + ","
                                                                 + Environment.NewLine);
                        else
                            File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name + ","
                                                                     + Environment.NewLine);
                    }
                    Aux++;
            }
            File.AppendAllText(FileName, ") AS BEGIN"
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t UPDATE " + TableName + " SET"
                                        + Environment.NewLine);
            Aux = 1;
            foreach (Column column in columns)
            {
                if (!column.InPrimaryKey)
                {
                    if ((columns.Count - 1) == Aux)
                    {
                        File.AppendAllText(FileName, column.Name +" = "+ "@" + column.Name);
                    }
                    else
                    {
                        File.AppendAllText(FileName, column.Name + " = " + "@" + column.Name + ",");
                    }
                    Aux++;
                }
            }

            foreach (Column column in columns)
            {
                if (column.InPrimaryKey)
                {
                    File.AppendAllText(FileName, Environment.NewLine);
                    File.AppendAllText(FileName, "WHERE " + column.Name + " = " + "@" + column.Name + Environment.NewLine);
                    File.AppendAllText(FileName, "END "
                                       + Environment.NewLine);
                    return;
                }
            }
        }

        private void CreateSelectAllProcedure(String FileName, ColumnCollection columns, String TableName)
        {
            String PrimaryKey = string.Empty;

            File.AppendAllText(FileName, "CREATE PROCEDURE sp" + TableName.Remove(0, 2) + "_SelectAll"
                                         + Environment.NewLine);

            File.AppendAllText(FileName, "AS BEGIN"
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t SELECT * FROM " + TableName
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "END "
                                        + Environment.NewLine);
        }

        private void CreateSelectByIdProcedure(String FileName, ColumnCollection columns, String TableName)
        {
            String PrimaryKey = string.Empty;

            File.AppendAllText(FileName, "CREATE PROCEDURE sp" + TableName.Remove(0, 2) + "_SelectById("
                                         + Environment.NewLine);
            foreach (Column column in columns)
            {
                if (column.InPrimaryKey)
                {
                    PrimaryKey = column.Name;
                    File.AppendAllText(FileName, "\t @" + column.Name + " " + column.DataType.Name
                                         + Environment.NewLine);
                }
            }
            File.AppendAllText(FileName, ") AS BEGIN"
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t SELECT * FROM " + TableName
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "\t WHERE " + PrimaryKey + " = @" + PrimaryKey
                                        + Environment.NewLine);
            File.AppendAllText(FileName, "END "
                                        + Environment.NewLine);
        }
        private void AddReferences(String FileName)
        {
            File.AppendAllText(FileName, "using System;"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "using System.Collections.Generic;"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "using System.Linq;"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "using System.Text;"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, Environment.NewLine);
        }

        private void AddNameSpace(String FileName, String NameSpace)
        {
            File.AppendAllText(FileName, "namespace " + NameSpace
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "{"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, Environment.NewLine);
        }

        private void AddPublicClass(String FileName, String ClassName)
        {
            File.AppendAllText(FileName, "\t public class " + ClassName
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "\t {"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, Environment.NewLine);
        }

        private void AddPrivateVariable(String FileName, String VariableType, String VariableName)
        {
            File.AppendAllText(FileName, "\t \t private " + VariableType + " _" + VariableName + ";"
                                         + Environment.NewLine);
        }

        private void AddPublicVariable(String FileName, String VariableType, String VariableName)
        {
            File.AppendAllText(FileName, "\t \t public " + VariableType + " " + VariableName 
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "\t \t {"
                                         + Environment.NewLine);
            File.AppendAllText(FileName, "\t \t \t get { return _" + VariableName + "; }" + Environment.NewLine);
            File.AppendAllText(FileName, "\t \t \t set { _" + VariableName + " = value; }" + Environment.NewLine);
            AddClose(FileName, 2);
        }
        private void AddClose (String FileName, int Level)
        {
            string Result = string.Empty;
            for (int i = 0; i < Level; i++)
            {
                Result = Result + "\t ";
            }
            Result = Result + "}";
            File.AppendAllText(FileName, Result
                                         + Environment.NewLine);
        }

        private string getCSharpType(String Type) {
            switch (Type)
            {
                case "int": return "int";
                case "varchar": return "String";
                case "bit": return "Boolean";
                default: return Type;
            }
        }
    }
}
