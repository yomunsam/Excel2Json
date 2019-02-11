using System;
using System.IO;
using NPOI.XSSF.UserModel;
using CsharpJson;
using System.Collections.Generic;

namespace xls2json_core
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length <= 0)
            {
                return;
            }
            var path = args[0];
            var ext = System.IO.Path.GetExtension(path).ToLower();
            //Console.WriteLine(path);

            //Console.WriteLine(ext);
            if (ext != ".xls" && ext != ".xlsx")
            {
                Console.WriteLine("不支持的文件格式");
                return;
            }
            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine("文件不存在");
            }

            string json_path;
            if(args.Length < 2)
            {
                json_path = path + ".json";
            }
            else
            {
                json_path = args[1];
            }


            Console.WriteLine("开始解析文件:" + path);
            var main_sheet = LoadXlsFiles(path);
            var json = ParseSheet(main_sheet);


            //写出
            Directory.CreateDirectory(Directory.GetParent(json_path).ToString());
            File.WriteAllText(json_path, json);

        }





        private static NPOI.SS.UserModel.ISheet LoadXlsFiles(string path)
        {
            XSSFWorkbook wb = new XSSFWorkbook(path);
            if (wb.NumberOfSheets <= 0)
            {
                Console.WriteLine("错误：未成功读取数据表信息或Sheets为空");
                return null;
            }
            else
            {
                Console.WriteLine("表格数：" + wb.NumberOfSheets);
            }

            var main_sheet = wb.GetSheetAt(0);
            return main_sheet;


        }


        private static string ParseSheet(NPOI.SS.UserModel.ISheet sheet)
        {
            Dictionary<int, S_FieldInfo> dict_fieldInfo = new Dictionary<int, S_FieldInfo>();
            JsonDocument main_json_doc = new JsonDocument();
            JsonArray main_arr = new JsonArray();

            //逐行读取
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var thisRow = sheet.GetRow(i);
                if (thisRow != null)
                {
                    //特殊行处理--字段描述行
                    if (i == 0)
                    {
                        //字段描述行

                        for (int j = 0; j < thisRow.LastCellNum; j++)
                        {

                            S_FieldInfo fieldInfo;
                            if (dict_fieldInfo.ContainsKey(j))
                            {
                                fieldInfo = dict_fieldInfo[j];
                            }
                            else
                            {
                                fieldInfo = new S_FieldInfo();
                                dict_fieldInfo.Add(j, fieldInfo);
                            }

                            var cell = thisRow.GetCell(j);
                            //读出描述字段
                            fieldInfo.name = cell.ToString();
                            //Console.WriteLine(fieldInfo.name);
                            dict_fieldInfo.Remove(j);
                            dict_fieldInfo.Add(j, fieldInfo);
                        }

                    }

                    //特殊行处理--字段key与类型行
                    if (i == 1)
                    {
                        //字段key与类型行

                        for (int j = 0; j < thisRow.LastCellNum; j++)
                        {

                            S_FieldInfo fieldInfo;
                            if (dict_fieldInfo.ContainsKey(j))
                            {
                                fieldInfo = dict_fieldInfo[j];
                            }
                            else
                            {
                                fieldInfo = new S_FieldInfo();
                                dict_fieldInfo.Add(j, fieldInfo);
                            }

                            var cell = thisRow.GetCell(j);

                            var _str = cell.ToString();

                            if (_str.Length < 2)
                            {
                                fieldInfo.type = E_FieldType.unknow;
                                fieldInfo.key = "";
                            }
                            else
                            {
                                var head = _str.Substring(0, 2).ToLower();
                                var content = _str.Substring(2, _str.Length - 2);
                                //Console.WriteLine(content);
                                switch (head)
                                {
                                    case "n_":
                                        fieldInfo.type = E_FieldType.num;
                                        fieldInfo.key = content;
                                        break;
                                    case "b_":
                                        fieldInfo.type = E_FieldType.boolean;
                                        fieldInfo.key = content;
                                        break;
                                    case "s_":
                                        fieldInfo.type = E_FieldType.str;
                                        fieldInfo.key = content;
                                        break;
                                    default:
                                        fieldInfo.type = E_FieldType.unknow;
                                        fieldInfo.key = "";
                                        break;
                                }
                            }

                            dict_fieldInfo.Remove(j);
                            dict_fieldInfo.Add(j, fieldInfo);
                        }

                    }

                    //正常行数据
                    if (i > 1)
                    {
                        JsonObject cur_obj = new JsonObject();

                        for (int j = 0; j < thisRow.LastCellNum; j++)
                        {
                            var cell_str = thisRow.GetCell(j).ToString();
                            
                            var f_info = dict_fieldInfo[j];
                            //Console.WriteLine(f_info.key + ":" + cell_str);

                            switch (f_info.type)
                            {
                                case E_FieldType.boolean:
                                    if (cell_str.ToLower() == "true")
                                    {
                                        //json_obj[f_info.key] = true;
                                        cur_obj[f_info.key] = true;
                                    }
                                    else
                                    {
                                        //json_obj[f_info.key] = false;
                                        cur_obj[f_info.key] = false;
                                    }
                                    break;
                                case E_FieldType.num:
                                    if (cell_str.Contains('.'))
                                    {
                                        //有小数
                                        //json_obj[f_info.key] = double.Parse(cell_str);
                                        cur_obj[f_info.key] = double.Parse(cell_str);
                                    }
                                    else
                                    {
                                        //整数
                                        //json_obj[f_info.key] = long.Parse(cell_str);
                                        cur_obj[f_info.key]  = long.Parse(cell_str);
                                    }
                                    break;
                                case E_FieldType.str:
                                    cur_obj[f_info.key] = cell_str;
                                    break;
                            }
                        }


                        main_arr.Add(cur_obj);
                    }
                }
            }


            var main_obj = new JsonObject();
            main_obj.Add("data", main_arr);
            main_json_doc.Object = main_obj;
            return main_json_doc.ToJson();
        }




        

    }

    /// <summary>
    /// 字段信息
    /// </summary>
    public struct S_FieldInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string name;
        /// <summary>
        /// 字段key
        /// </summary>
        public string key;

        public E_FieldType type;
    }


    /// <summary>
    /// 字段类型
    /// </summary>
    public enum E_FieldType
    {
        str,
        num,
        boolean,
        unknow
    }
}
