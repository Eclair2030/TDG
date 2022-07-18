using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace TDG_Vision
{
    internal class Recipes
    {
        public static string RECIPE_CODE;
        private static string RECIPE_PATH = @"D:\Develop\BondingTest\TDG_Vision\bin\Debug\Recipes.xml";

        private static string RECIPE_NODE_NAME = @"Recipe";
        private static string MODEL_NODE_NAME = @"Model";
        private static string CAMERA_NODE_NAME1 = @"Camera1";
        private static string CAMERA_NODE_NAME2 = @"Camera2";
        private static string DELAY_NODE_NAME = @"Delay";
        private static string OFSET_NODE_NAME = @"Offset";
        private static string SPEC_NODE_NAME = @"Spec";
        private static string YMAP_NODE_NAME = @"Ymap";
        private static string MARKS_NODE_NAME = @"Marks";
        private static string MARK_NODE_NAME = @"Mark";
        private static string CALIB_NODE_NAME = @"Calib";
        private static string ATTR_USE = @"In_use";
        private static string ATTR_CODE = @"Code";
        private static string ATTR_NAME = @"Name";
        private static string ATTR_VALUE = @"Value";
        private static string ATTR_X = @"X";
        private static string ATTR_Y = @"Y";
        private static string ATTR_T = @"T";
        private static string ATTR_RESOLUTION_X = @"Resolution_X";
        private static string ATTR_RESOLUTION_Y = @"Resolution_Y";
        private static string ATTR_CENTER_X = @"Center_X";
        private static string ATTR_CENTER_Y = @"Center_Y";
        public static string PRB_TOOL_CODE_VALUE = @"1";
        private static string PRB_TOOL_NAME_VALUE = @"PBD_Tool";
        public static string PRB_STAGE_CODE_VALUE = @"2";
        private static string PRB_STAGE_NAME_VALUE = @"PBD_Stage";
        private static string DEF_NUMBER_VALUE = @"0";
        private static string DEF_SPEC_VALUE = @"0.005";
        public static string INUSE_VALUE = @"1";
        private static string NOTUSE_VALUE = @"0";
        public static string CAMERA_CODE1 = @"1";
        public static string CAMERA_CODE2 = @"2";
        private static string MARK_CODE1 = @"1";
        private static string MARK_CODE2 = @"2";
        private static string MARK_CODE3 = @"3";
        public static string MARK_PATH = @"D:\Develop\BondingTest\TDG_Vision\bin\Debug\Marks\";
        public static string MARK_NAME1 = @"Mark1.bmp";
        public static string MARK_NAME2 = @"Mark2.bmp";
        public static string MARK_NAME3 = @"Mark3.bmp";
        private static string MARK_CENTER_DEFAULT = @"0";

        public Recipes() { }

        public static List<RecipeListItem> LoadAllRecipe()
        {
            List<RecipeListItem> listview = new List<RecipeListItem> ();
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_USE].Value == INUSE_VALUE)
                {
                    RECIPE_CODE = recipeList[i].Attributes[ATTR_CODE].Value;
                }
                RecipeListItem item = new RecipeListItem(recipeList[i].Attributes[ATTR_CODE].Value, 
                    recipeList[i].Attributes[ATTR_NAME].Value,
                    recipeList[i].Attributes[ATTR_USE].Value);
                listview.Add(item);
            }
            return listview;
        }

        public static int CreateRecipe(string codeNew, string name)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            if (codeNew == null || codeNew == string.Empty)
            {
                codeNew = (GetMaxRecipeCode(baseNode) + 1).ToString();
            }
            else
            {
                if (FindSameCode(codeNew, baseNode) != 0)
                {
                    return 1;//recipe id 已存在
                }
            }
            
            XmlElement newRecipe = xml.CreateElement(RECIPE_NODE_NAME);
            newRecipe.SetAttribute(ATTR_CODE, codeNew);
            newRecipe.SetAttribute(ATTR_NAME, name);
            newRecipe.SetAttribute(ATTR_USE, NOTUSE_VALUE);

            XmlElement newToolModel = xml.CreateElement(MODEL_NODE_NAME);
            newToolModel.SetAttribute(ATTR_CODE, PRB_TOOL_CODE_VALUE);
            newToolModel.SetAttribute(ATTR_NAME, PRB_TOOL_NAME_VALUE);

            XmlElement newDelay = xml.CreateElement(DELAY_NODE_NAME);
            newDelay.SetAttribute(ATTR_VALUE, DEF_NUMBER_VALUE);
            XmlElement newOffset = xml.CreateElement(OFSET_NODE_NAME);
            newOffset.SetAttribute(ATTR_X, DEF_NUMBER_VALUE);
            newOffset.SetAttribute(ATTR_Y, DEF_NUMBER_VALUE);
            newOffset.SetAttribute(ATTR_T, DEF_NUMBER_VALUE);
            XmlElement newSpec = xml.CreateElement(SPEC_NODE_NAME);
            newSpec.SetAttribute(ATTR_X, DEF_SPEC_VALUE);
            newSpec.SetAttribute(ATTR_Y, DEF_SPEC_VALUE);
            newSpec.SetAttribute(ATTR_T, DEF_SPEC_VALUE);
            XmlElement newCalibMove = xml.CreateElement(CALIB_NODE_NAME);
            newCalibMove.SetAttribute(ATTR_X, DEF_NUMBER_VALUE);
            newCalibMove.SetAttribute(ATTR_Y, DEF_NUMBER_VALUE);
            newCalibMove.SetAttribute(ATTR_T, DEF_NUMBER_VALUE);
            XmlElement newYmap = xml.CreateElement(YMAP_NODE_NAME);
            newYmap.SetAttribute(ATTR_VALUE, DEF_NUMBER_VALUE);
            newYmap.SetAttribute(ATTR_USE, NOTUSE_VALUE);

            XmlElement newCamera1 = xml.CreateElement(CAMERA_NODE_NAME1);
            newCamera1.SetAttribute(ATTR_CODE, CAMERA_CODE1);
            XmlElement newMarks = xml.CreateElement(MARKS_NODE_NAME);
            XmlElement mark1_1 = xml.CreateElement(MARK_NODE_NAME);
            mark1_1.SetAttribute(ATTR_CODE, MARK_CODE1);
            mark1_1.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark1_1.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark1_1.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark1_1.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME1);
            XmlElement mark1_2 = xml.CreateElement(MARK_NODE_NAME);
            mark1_2.SetAttribute(ATTR_CODE, MARK_CODE2);
            mark1_2.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark1_2.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark1_2.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark1_2.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME2);
            XmlElement mark1_3 = xml.CreateElement(MARK_NODE_NAME);
            mark1_3.SetAttribute(ATTR_CODE, MARK_CODE3);
            mark1_3.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark1_3.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark1_3.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark1_3.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME3);
            if (!Directory.Exists(MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\"))
            {
                Directory.CreateDirectory(MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\");
            }
            
            XmlElement newCalib = xml.CreateElement(CALIB_NODE_NAME);
            newCalib.SetAttribute(ATTR_RESOLUTION_X, DEF_NUMBER_VALUE);
            newCalib.SetAttribute(ATTR_RESOLUTION_Y, DEF_NUMBER_VALUE);
            newCalib.SetAttribute(ATTR_CENTER_X, DEF_NUMBER_VALUE);
            newCalib.SetAttribute(ATTR_CENTER_Y, DEF_NUMBER_VALUE);
            newMarks.AppendChild(mark1_1);
            newMarks.AppendChild(mark1_2);
            newMarks.AppendChild(mark1_3);
            newCamera1.AppendChild(newMarks);
            newCamera1.AppendChild(newCalib);

            XmlElement newCamera2 = xml.CreateElement(CAMERA_NODE_NAME2);
            newCamera2.SetAttribute(ATTR_CODE, CAMERA_CODE2);
            XmlElement newMarks2 = xml.CreateElement(MARKS_NODE_NAME);
            XmlElement mark2_1 = xml.CreateElement(MARK_NODE_NAME);
            mark2_1.SetAttribute(ATTR_CODE, MARK_CODE1);
            mark2_1.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark2_1.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark2_1.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark2_1.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME1);
            XmlElement mark2_2 = xml.CreateElement(MARK_NODE_NAME);
            mark2_2.SetAttribute(ATTR_CODE, MARK_CODE2);
            mark2_2.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark2_2.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark2_2.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark2_2.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME2);
            XmlElement mark2_3 = xml.CreateElement(MARK_NODE_NAME);
            mark2_3.SetAttribute(ATTR_CODE, MARK_CODE3);
            mark2_3.SetAttribute(ATTR_USE, NOTUSE_VALUE);
            mark2_3.SetAttribute(ATTR_X, MARK_CENTER_DEFAULT);
            mark2_3.SetAttribute(ATTR_Y, MARK_CENTER_DEFAULT);
            mark2_3.SetAttribute(ATTR_VALUE, MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME3);
            if (!Directory.Exists(MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\"))
            {
                Directory.CreateDirectory(MARK_PATH + codeNew + @"\" + PRB_TOOL_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\");
            }

            newMarks2.AppendChild(mark2_1);
            newMarks2.AppendChild(mark2_2);
            newMarks2.AppendChild(mark2_3);
            newCamera2.AppendChild(newMarks2);
            newCamera2.AppendChild(newCalib.Clone());

            newToolModel.AppendChild(newDelay);
            newToolModel.AppendChild(newOffset);
            newToolModel.AppendChild(newSpec);
            newToolModel.AppendChild(newCalibMove);
            newToolModel.AppendChild(newYmap);
            newToolModel.AppendChild(newCamera1);
            newToolModel.AppendChild(newCamera2);
            newRecipe.AppendChild(newToolModel);


            XmlElement newStageModel = xml.CreateElement(MODEL_NODE_NAME);
            newStageModel.SetAttribute(ATTR_CODE, PRB_STAGE_CODE_VALUE);
            newStageModel.SetAttribute(ATTR_NAME, PRB_STAGE_NAME_VALUE);

            XmlNode ca1 = newCamera1.Clone();
            XmlNodeList list = ca1.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
            list[0].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME1;
            list[1].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME2;
            list[2].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME3;
            if (!Directory.Exists(MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\"))
            {
                Directory.CreateDirectory(MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE1 + @"\");
            }
            XmlNode ca2 = newCamera2.Clone();
            XmlNodeList list2 = ca2.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
            list2[0].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME1;
            list2[1].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME2;
            list2[2].Attributes[ATTR_VALUE].Value = MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME3;
            if (!Directory.Exists(MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\"))
            {
                Directory.CreateDirectory(MARK_PATH + codeNew + @"\" + PRB_STAGE_CODE_VALUE + @"\" + CAMERA_CODE2 + @"\");
            }

            newStageModel.AppendChild(newDelay.Clone());
            newStageModel.AppendChild(newOffset.Clone());
            newStageModel.AppendChild(newSpec.Clone());
            newStageModel.AppendChild(newCalibMove.Clone());
            newStageModel.AppendChild(newYmap.Clone());
            newStageModel.AppendChild(ca1);
            newStageModel.AppendChild(ca2);
            newRecipe.AppendChild(newStageModel);

            baseNode.AppendChild(newRecipe);
            xml.Save(RECIPE_PATH);
            return 0;
        }

        public static int EditRecipe(string code, RecipeModel recipe)
        {
            int result = 2;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (code == recipeList[i].Attributes[ATTR_CODE].Value)
                {
                    XmlNodeList mlist = recipeList[i].SelectNodes(MODEL_NODE_NAME);
                    result = 1;
                    foreach (XmlNode model in mlist)
                    {
                        if (model.Attributes[ATTR_CODE].Value == recipe.MODEL_CODE)
                        {
                            XmlNode delay = model.SelectSingleNode(DELAY_NODE_NAME);
                            delay.Attributes[ATTR_VALUE].Value = recipe.DELAY.ToString();
                            XmlNode ofset = model.SelectSingleNode(OFSET_NODE_NAME);
                            ofset.Attributes[ATTR_X].Value = recipe.OFFSET_X.ToString();
                            ofset.Attributes[ATTR_Y].Value = recipe.OFFSET_Y.ToString();
                            ofset.Attributes[ATTR_T].Value = recipe.OFFSET_T.ToString();
                            XmlNode spec = model.SelectSingleNode(SPEC_NODE_NAME);
                            spec.Attributes[ATTR_X].Value = recipe.SPEC_X.ToString();
                            spec.Attributes[ATTR_Y].Value = recipe.SPEC_Y.ToString();
                            spec.Attributes[ATTR_T].Value = recipe.SPEC_T.ToString();
                            XmlNode calibMove = model.SelectSingleNode(CALIB_NODE_NAME);
                            calibMove.Attributes[ATTR_X].Value = recipe.CALIB_X.ToString();
                            calibMove.Attributes[ATTR_Y].Value = recipe.CALIB_Y.ToString();
                            calibMove.Attributes[ATTR_T].Value = recipe.CALIB_T.ToString();
                            XmlNode ymap = model.SelectSingleNode(YMAP_NODE_NAME);
                            ymap.Attributes[ATTR_VALUE].Value = recipe.Y_MAP.ToString();
                            ymap.Attributes[ATTR_USE].Value = recipe.USE_Y_MAP ? INUSE_VALUE : NOTUSE_VALUE;

                            XmlNode camera1 = model.SelectSingleNode(CAMERA_NODE_NAME1);
                            XmlNodeList marks1 = camera1.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                            for (int j = 0; j < marks1.Count; j++)
                            {
                                marks1[j].Attributes[ATTR_USE].Value = recipe.L_CAMERA_RECIPE.MARK_ENABLE_LIST[j].ToString();
                            }
                            XmlNode calib1 = camera1.SelectSingleNode(CALIB_NODE_NAME);
                            calib1.Attributes[ATTR_RESOLUTION_X].Value = recipe.L_CAMERA_RECIPE.Resolution_X.ToString();
                            calib1.Attributes[ATTR_RESOLUTION_Y].Value = recipe.L_CAMERA_RECIPE.Resolution_Y.ToString();
                            calib1.Attributes[ATTR_CENTER_X].Value = recipe.L_CAMERA_RECIPE.RotateCenter_X.ToString();
                            calib1.Attributes[ATTR_CENTER_Y].Value = recipe.L_CAMERA_RECIPE.RotateCenter_Y.ToString();

                            XmlNode camera2 = model.SelectSingleNode(CAMERA_NODE_NAME2);
                            XmlNodeList marks2 = camera2.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                            for (int j = 0; j < marks2.Count; j++)
                            {
                                marks2[j].Attributes[ATTR_USE].Value = recipe.R_CAMERA_RECIPE.MARK_ENABLE_LIST[j].ToString();
                            }
                            XmlNode calib2 = camera2.SelectSingleNode(CALIB_NODE_NAME);
                            calib2.Attributes[ATTR_RESOLUTION_X].Value = recipe.R_CAMERA_RECIPE.Resolution_X.ToString();
                            calib2.Attributes[ATTR_RESOLUTION_Y].Value = recipe.R_CAMERA_RECIPE.Resolution_Y.ToString();
                            calib2.Attributes[ATTR_CENTER_X].Value = recipe.R_CAMERA_RECIPE.RotateCenter_X.ToString();
                            calib2.Attributes[ATTR_CENTER_Y].Value = recipe.R_CAMERA_RECIPE.RotateCenter_Y.ToString();
                            result = 0;
                        }
                        break;
                    }
                    break;
                }
            }
            xml.Save(RECIPE_PATH);
            return result;
        }

        public static RecipeModel GetRecipe(string code, string mode_code)
        {
            RecipeModel rm = null;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (code == recipeList[i].Attributes[ATTR_CODE].Value)
                {
                    XmlNodeList mlist = recipeList[i].SelectNodes(MODEL_NODE_NAME);
                    foreach (XmlNode model in mlist)
                    {
                        if (model.Attributes[ATTR_CODE].Value == mode_code)
                        {
                            rm = new RecipeModel();
                            rm.MODEL_CODE = mode_code;
                            XmlNode delay = model.SelectSingleNode(DELAY_NODE_NAME);
                            double.TryParse(delay.Attributes[ATTR_VALUE].Value, out rm.DELAY);
                            XmlNode ofset = model.SelectSingleNode(OFSET_NODE_NAME);
                            double.TryParse(ofset.Attributes[ATTR_X].Value, out rm.OFFSET_X);
                            double.TryParse(ofset.Attributes[ATTR_Y].Value, out rm.OFFSET_Y);
                            double.TryParse(ofset.Attributes[ATTR_T].Value, out rm.OFFSET_T);
                            XmlNode spec = model.SelectSingleNode(SPEC_NODE_NAME);
                            double.TryParse(spec.Attributes[ATTR_X].Value, out rm.SPEC_X);
                            double.TryParse(spec.Attributes[ATTR_Y].Value, out rm.SPEC_Y);
                            double.TryParse(spec.Attributes[ATTR_T].Value, out rm.SPEC_T);
                            XmlNode calibMove = model.SelectSingleNode(CALIB_NODE_NAME);
                            double.TryParse(calibMove.Attributes[ATTR_X].Value, out rm.CALIB_X);
                            double.TryParse(calibMove.Attributes[ATTR_Y].Value, out rm.CALIB_Y);
                            double.TryParse(calibMove.Attributes[ATTR_T].Value, out rm.CALIB_T);
                            XmlNode ymap = model.SelectSingleNode(YMAP_NODE_NAME);
                            double.TryParse(ymap.Attributes[ATTR_VALUE].Value, out rm.Y_MAP);
                            rm.USE_Y_MAP = ymap.Attributes[ATTR_USE].Value == INUSE_VALUE ? true : false;

                            XmlNode camera1 = model.SelectSingleNode(CAMERA_NODE_NAME1);
                            XmlNodeList marks1 = camera1.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                            for (int j = 0; j < marks1.Count; j++)
                            {
                                int.TryParse(marks1[j].Attributes[ATTR_USE].Value, out rm.L_CAMERA_RECIPE.MARK_ENABLE_LIST[j]);
                                rm.L_CAMERA_RECIPE.MARK_CEN[j].X = double.Parse(marks1[j].Attributes[ATTR_X].Value);
                                rm.L_CAMERA_RECIPE.MARK_CEN[j].Y = double.Parse(marks1[j].Attributes[ATTR_Y].Value);
                            }
                            XmlNode calib1 = camera1.SelectSingleNode(CALIB_NODE_NAME);
                            double.TryParse(calib1.Attributes[ATTR_RESOLUTION_X].Value, out rm.L_CAMERA_RECIPE.Resolution_X);
                            double.TryParse(calib1.Attributes[ATTR_RESOLUTION_Y].Value, out rm.L_CAMERA_RECIPE.Resolution_Y);
                            double.TryParse(calib1.Attributes[ATTR_CENTER_X].Value, out rm.L_CAMERA_RECIPE.RotateCenter_X);
                            double.TryParse(calib1.Attributes[ATTR_CENTER_Y].Value, out rm.L_CAMERA_RECIPE.RotateCenter_Y);

                            XmlNode camera2 = model.SelectSingleNode(CAMERA_NODE_NAME2);
                            XmlNodeList marks2 = camera2.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                            for (int j = 0; j < marks2.Count; j++)
                            {
                                int.TryParse(marks2[j].Attributes[ATTR_USE].Value, out rm.R_CAMERA_RECIPE.MARK_ENABLE_LIST[j]);
                                rm.R_CAMERA_RECIPE.MARK_CEN[j].X = double.Parse(marks2[j].Attributes[ATTR_X].Value);
                                rm.R_CAMERA_RECIPE.MARK_CEN[j].Y = double.Parse(marks2[j].Attributes[ATTR_Y].Value);
                            }
                            XmlNode calib2 = camera2.SelectSingleNode(CALIB_NODE_NAME);
                            double.TryParse(calib2.Attributes[ATTR_RESOLUTION_X].Value, out rm.R_CAMERA_RECIPE.Resolution_X);
                            double.TryParse(calib2.Attributes[ATTR_RESOLUTION_Y].Value, out rm.R_CAMERA_RECIPE.Resolution_Y);
                            double.TryParse(calib2.Attributes[ATTR_CENTER_X].Value, out rm.R_CAMERA_RECIPE.RotateCenter_X);
                            double.TryParse(calib2.Attributes[ATTR_CENTER_Y].Value, out rm.R_CAMERA_RECIPE.RotateCenter_Y);
                            break;
                        }
                        
                    }
                    break;
                }
            }
            return rm;
        }

        public static int CopyRecipe(string code)
        {
            int result = 1;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            if (FindSameCode(code, baseNode) == 0)
            {
                return 2;
            }
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_CODE].Value == code)
                {
                    XmlNode x = recipeList[i].Clone();
                    x.Attributes[ATTR_CODE].Value = (GetMaxRecipeCode(baseNode) + 1).ToString();
                    x.Attributes[ATTR_USE].Value = NOTUSE_VALUE;
                    XmlNodeList list = x.SelectNodes(MODEL_NODE_NAME);
                    foreach (XmlNode item in list)
                    {
                        XmlNodeList marks1 = item.SelectSingleNode(CAMERA_NODE_NAME1).SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                        string path1 = MARK_PATH + x.Attributes[ATTR_CODE].Value + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\";
                        marks1[0].Attributes[ATTR_VALUE].Value = path1 + MARK_NAME1;
                        marks1[1].Attributes[ATTR_VALUE].Value = path1 + MARK_NAME2;
                        marks1[2].Attributes[ATTR_VALUE].Value = path1 + MARK_NAME3;
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME1))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME1, path1 + MARK_NAME1, true);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME2))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME2, path1 + MARK_NAME2, true);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME3))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE1 + @"\" + MARK_NAME3, path1 + MARK_NAME3, true);
                        }

                        XmlNodeList marks2 = item.SelectSingleNode(CAMERA_NODE_NAME2).SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                        string path2 = MARK_PATH + x.Attributes[ATTR_CODE].Value + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\";
                        marks2[0].Attributes[ATTR_VALUE].Value = path2 + MARK_NAME1;
                        marks2[1].Attributes[ATTR_VALUE].Value = path2 + MARK_NAME2;
                        marks2[2].Attributes[ATTR_VALUE].Value = path2 + MARK_NAME3;
                        if (!Directory.Exists(path2))
                        {
                            Directory.CreateDirectory(path2);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME1))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME1, path2 + MARK_NAME1, true);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME2))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME2, path2 + MARK_NAME2, true);
                        }
                        if (File.Exists(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME3))
                        {
                            File.Copy(MARK_PATH + code + @"\" + item.Attributes[ATTR_CODE].Value + @"\" + CAMERA_CODE2 + @"\" + MARK_NAME3, path2 + MARK_NAME3, true);
                        }
                    }
                    baseNode.AppendChild(x);
                    result = 0;
                    break;
                }
            }
            xml.Save(RECIPE_PATH);
            return result;
        }

        public static int ChangeRecipe(string code)
        {
            int result = 1;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_CODE].Value == code)
                {
                    recipeList[i].Attributes[ATTR_USE].Value = INUSE_VALUE;
                    RECIPE_CODE = code;
                    result = 0;
                }
                else
                {
                    recipeList[i].Attributes[ATTR_USE].Value = NOTUSE_VALUE;
                }
            }
            xml.Save(RECIPE_PATH);
            return result;
        }

        public static int DeleteRecipe(string code)
        {
            int result = 1;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_CODE].Value == code)
                {
                    baseNode.RemoveChild(recipeList[i]);
                    result = 0;
                    break;
                }
            }
            xml.Save(RECIPE_PATH);
            return result;
        }

        public static int GetMaxRecipeCode(XmlElement baseElement)
        {
            int maxCode = 0;
            XmlNodeList list = baseElement.GetElementsByTagName(RECIPE_NODE_NAME);
            foreach (XmlNode node in list)
            {
                int code = 0;
                if (int.TryParse(node.Attributes[ATTR_CODE].Value, out maxCode) && code > maxCode)
                {
                    maxCode = code;
                }
            }
            return maxCode;
        }

        public static int FindSameCode(string code, XmlElement baseElement)
        {
            int result = 0;
            XmlNodeList recipeList = baseElement.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_CODE].Value == code)
                {
                    result = 1;
                    break;
                }
            }

            return result;
        }

        public static int SetMarkCenterData(string modelCode, string cameraCode, string markCode, string x, string y)
        {
            int result = -1;
            XmlDocument xml = new XmlDocument();
            xml.Load(RECIPE_PATH);
            XmlElement baseNode = xml.DocumentElement;
            XmlNodeList recipeList = baseNode.SelectNodes(RECIPE_NODE_NAME);
            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Attributes[ATTR_USE].Value == INUSE_VALUE)
                {
                    XmlNodeList nl = recipeList[i].SelectNodes(MODEL_NODE_NAME);
                    for (int j = 0; j < nl.Count; j++)
                    {
                        if (nl[j].Attributes[ATTR_CODE].Value == modelCode)
                        {
                            XmlNode cam = nl[j].SelectSingleNode(@"Camera" + cameraCode);
                            XmlNodeList markList = cam.SelectSingleNode(MARKS_NODE_NAME).SelectNodes(MARK_NODE_NAME);
                            for (int k = 0; k < markList.Count; k++)
                            {
                                if (markList[k].Attributes[ATTR_CODE].Value == markCode)
                                {
                                    markList[k].Attributes[ATTR_X].Value = x;
                                    markList[k].Attributes[ATTR_Y].Value = y;
                                    result = 0;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            xml.Save(RECIPE_PATH);
            return result;
        }
    }
}
