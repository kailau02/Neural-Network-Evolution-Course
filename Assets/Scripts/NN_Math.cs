using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NN_Math
{
    public static class Math
    {
        // Return the value after being passed through the sigmoid function
        public static float Sigmoid(float value)
        {
            float k = Mathf.Exp(value);
            return k / (1.0f + k);
        }

        // Create a matrix with random values
        public static float[,] RandomMatrix(int rows, int cols)
        {
            float[,] tempMatrix = new float[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    tempMatrix[row, col] = Random.Range(-2f, 2f);
                }
            }
            return tempMatrix;
        }
        // Create a list with random values
        public static float[] RandomLst(int cols)
        {
            float[] tempLst = new float[cols];
            for (int col = 0; col < cols; col++)
            {
                tempLst[col] = Random.Range(-2f, 2f);
            }

            return tempLst;
        }

        // Multiply a list by a matrix
        public static float[] LstMultMatrix(float[] inputs, float[,] weights, float[] biases)
        {
            float[] tempLst = new float[weights.GetLength(1)];
            for (int inputIndex = 0; inputIndex < inputs.Length; inputIndex++)
            {
                for (int rowIndex = 0; rowIndex < weights.GetLength(1); rowIndex++)
                {
                    tempLst[rowIndex] += inputs[inputIndex] * weights[inputIndex, rowIndex];
                }
            }
            for (int index = 0; index < tempLst.Length; index++)
            {
                tempLst[index] += biases[index];
            }
            return tempLst;
        }
    }
}

// Define a data class for a single runner's weight & bias values
public class NN_Data
{
    public float[,] ih_w;
    public float[] ih_b;

    public float[,] ho_w;
    public float[] ho_b;

    public void UpdateData(float[,] ih_w, float[] ih_b, float[,] ho_w, float[] ho_b)
    {
        this.ih_w = ih_w;
        this.ih_b = ih_b;
        this.ho_w = ho_w;
        this.ho_b = ho_b;
    }
}

// A class to hold all values of each runner after they die
public class RunnerDeathStats
{
    public int framesAlive;
    public NN_Data nn_data;
    public RunnerDeathStats(int framesAlive, NN_Data nn_data)
    {
        this.framesAlive = framesAlive;
        this.nn_data = nn_data;
    }
}
