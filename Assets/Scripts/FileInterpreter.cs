using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace FileInterpreter
{
    public static class FileCtrl
    {
        static string path = Application.dataPath + "/SaveData/";

        // Save NN data to a playerID
        public static void WriteData(int playerID, NN_Data data)
        {
            // Add all data values to List<string> and save to txt file
            List<string> lines = new List<string>() { "", "", "", "" };
            
            for (int row = 0; row < data.ih_w.GetLength(0); row++)
            {
                for (int col = 0; col < data.ih_w.GetLength(1); col++)
                {
                    lines[0] += data.ih_w[row, col].ToString() + ",";
                }
                lines[0] += ":";
            }
            for (int element = 0; element < data.ih_b.Length; element++)
            {
                lines[1] += data.ih_b[element].ToString() + ",";
            }

            for (int row = 0; row < data.ho_w.GetLength(0); row++)
            {
                for (int col = 0; col < data.ho_w.GetLength(1); col++)
                {
                    lines[2] += data.ho_w[row, col].ToString() + ",";
                }
                lines[2] += ":";

            }
            for (int element = 0; element < data.ho_b.Length; element++)
            {
                lines[3] += data.ho_b[element].ToString() + ",";
            }
            // Clean up line endings
            for (int line = 0; line < lines.Count; line++)
            {
                lines[line] = lines[line].Replace(",:", ":");
                lines[line] = lines[line].TrimEnd(':');
                lines[line] = lines[line].TrimEnd(',');
            }

            File.WriteAllLines(path + "Runner" + playerID.ToString() + ".txt", lines);
        }

        // Get saved NN data from a playerID
        public static NN_Data ReadData(int playerID)
        {
            if (File.Exists(path + "Runner" + playerID.ToString() + ".txt"))
            {
                string[] lines = File.ReadAllLines(path + "Runner" + playerID.ToString() + ".txt");


                string[] ih_w_rows = lines[0].Split(':');
                float[,] ih_w = new float[ih_w_rows.Length,ih_w_rows[0].Split(',').Length];
                for (int row = 0; row < ih_w.GetLength(0); row++)
                {
                    string[] ih_w_cols = ih_w_rows[row].Split(',');
                    for (int col = 0; col < ih_w.GetLength(1); col++)
                    {
                        //File.WriteAllText(path + "Debug.txt", ih_w_cols[col]);
                        ih_w[row, col] = float.Parse(ih_w_cols[col]);
                    }
                }

                string[] ih_b_elements = lines[1].Split(',');
                float[] ih_b = new float[ih_b_elements.Length];
                for (int element = 0; element < ih_b_elements.Length; element++)
                {
                    ih_b[element] = float.Parse(ih_b_elements[element]);
                }


                string[] ho_w_rows = lines[2].Split(':');
                float[,] ho_w = new float[ho_w_rows.Length, ho_w_rows[0].Split(',').Length];
                for (int row = 0; row < ho_w.GetLength(0); row++)
                {
                    string[] ho_w_cols = ho_w_rows[row].Split(',');
                    for (int col = 0; col < ho_w.GetLength(1); col++)
                    {
                        ho_w[row, col] = float.Parse(ho_w_cols[col]);
                    }
                }

                string[] ho_b_elements = lines[3].Split(',');
                float[] ho_b = new float[ho_b_elements.Length];
                for (int element = 0; element < ho_b_elements.Length; element++)
                {
                    ho_b[element] = float.Parse(ho_b_elements[element]);
                }

                NN_Data readData = new NN_Data();
                readData.UpdateData(ih_w, ih_b, ho_w, ho_b);

                return readData;
            }
            else
            {
                return null;
            }
        }

        // Writes current statistics of the runners
        public static void SaveStats(int nextGen, int topFitness)
        {
            File.WriteAllText(path + "Stats.txt", nextGen.ToString() + "," + topFitness.ToString());
        }
        // Gets saved statistics of the runners
        public static int[] GetStatistics()
        {
            if (File.Exists(path + "Stats.txt"))
            {
                string[] sArray = File.ReadAllText(path + "Stats.txt").Split(',');
                return new int[2] { int.Parse(sArray[0]), int.Parse(sArray[1]) };
            }
            else
            {
                return new int[2] { 1, 0 };
            }
        }

        // Delete all saved data
        public static void DeleteData()
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach(FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}

