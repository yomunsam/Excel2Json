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

            if (File.Exists(json_path))
            {
                File.Delete(json_path);
            }
            File.WriteAllText(json_path, json);
            Console.WriteLine("写出到文件：" + json_path);

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
            Dictionary<int, S_FieldInfo> dict_fieldInfo = new Dictionary<int, S_FieldInfo>(); //字段与index的对应字典
            JsonDocument main_json_doc = new JsonDocument();
            JsonArray main_arr = new JsonArray();

            Console.WriteLine("表长度："+ sheet.LastRowNum);
            //逐行读取
            for (int i = 0; i <= sheet.LastRowNum; i++) //注意这里的循环下标，实际上是超出一位的
            {
                var thisRow = sheet.GetRow(i);
                if (thisRow != null)
                {
                    //特殊行处理--字段描述行
                    if (i == 0)
                    {
                        //字段描述行
                        Console.WriteLine("读字段描述：");
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
                            if(cell != null)
                            {
                                Console.WriteLine("    >"+ cell.ToString());
                                fieldInfo.name = cell.ToString();
                                dict_fieldInfo.Remove(j);
                                dict_fieldInfo.Add(j, fieldInfo);
                            }
                            
                            //Console.WriteLine(fieldInfo.name);
                            
                        }

                    }

                    //特殊行处理--字段key与类型行
                    if (i == 1)
                    {
                        //字段key与类型行
                        Console.WriteLine("读取定义行,共：" + thisRow.LastCellNum);
                        for (int j = 0; j < thisRow.LastCellNum; j++)
                        {
                            Console.WriteLine("    -- 第" + j + "项");
                            var flag = false;
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

                            if (cell != null)
                            {
                                
                                var _str = cell.ToString();
                                Console.WriteLine("         -- str：" + _str);

                                if (_str.Length < 2 || _str == string.Empty)
                                {
                                    fieldInfo.type = E_FieldType.unknow;
                                    fieldInfo.key = "";
                                    Console.WriteLine("             -- 无效定义");

                                }
                                else
                                {
                                    var head = _str.Substring(0, 2).ToLower();
                                    var content = _str.Substring(2, _str.Length - 2);
                                    //Console.WriteLine("    >" + content);
                                    //Console.WriteLine(content);
                                    switch (head)
                                    {
                                        case "n_":
                                            fieldInfo.type = E_FieldType.num;
                                            fieldInfo.key = content;
                                            flag = true;
                                            Console.WriteLine("             -- 有效定义");

                                            break;
                                        case "b_":
                                            fieldInfo.type = E_FieldType.boolean;
                                            fieldInfo.key = content;
                                            flag = true;
                                            Console.WriteLine("             -- 有效定义");
                                            break;
                                        case "s_":
                                            fieldInfo.type = E_FieldType.str;
                                            fieldInfo.key = content;
                                            flag = true;
                                            Console.WriteLine("             -- 有效定义");
                                            break;
                                        default:
                                            fieldInfo.type = E_FieldType.unknow;
                                            fieldInfo.key = "";
                                            flag = false;
                                            Console.WriteLine("             -- 无效定义");
                                            break;
                                    }
                                }

                                
                            }

                            if (dict_fieldInfo.ContainsKey(j))
                            {
                                dict_fieldInfo.Remove(j);
                            }

                            if (flag)
                            {
                                //Console.WriteLine("    有效定义：" + fieldInfo.key);
                                dict_fieldInfo.Add(j, fieldInfo);
                            }
                            

                        }

                        Console.WriteLine("读取到有效定义：" + dict_fieldInfo.Count + " 项");
                    }
                    
                    if(i == 2)
                    {
                        Console.WriteLine("\n读取数据行。");
                    }

                    //正常行数据
                    if (i > 1)
                    {
                        Console.WriteLine("\n");
                        JsonObject cur_obj = new JsonObject();
                        //Console.WriteLine("count:" + dict_fieldInfo.Count);

                        var flag = false; //只要下面的循环中，有任何一个字段不是空，则置为true
                        for (int j = 0; j < thisRow.LastCellNum; j++)
                        {
                            
                            var cell = thisRow.GetCell(j);
                            //if (!dict_fieldInfo.ContainsKey(j))
                            //{
                            //    Console.WriteLine("不存在有效定义字段，index:" + j);
                            //}
                            if (cell != null && dict_fieldInfo.ContainsKey(j))
                            {
                                var cell_str = cell.ToString();
                                var f_info = dict_fieldInfo[j];
                                if (!string.IsNullOrEmpty(cell_str) && f_info.type != E_FieldType.unknow)
                                {
                                    Console.WriteLine(f_info.key + ":" + cell_str);
                                    flag = true;
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
                                                cur_obj[f_info.key] = long.Parse(cell_str);
                                            }
                                            break;
                                        case E_FieldType.str:
                                            cur_obj[f_info.key] = cell_str;
                                            break;
                                    }
                                }

                                
                            }
                            
                        }

                        //检查 cur_object

                        if (flag)
                        {
                            main_arr.Add(cur_obj);

                        }
                        else
                        {
                            Console.WriteLine("    [空行]");
                        }
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
