using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileInterpreter;
using NN_Math;

public class NN
{
    public int[] inputs;
    public int[] hidden;
    public int[] outputs;

    public NN_Data nn_data = new NN_Data();

    public NN(int playerID, int inputNodes, int hiddenNodes, int outputNodes)
    {
        NN_Data saveData = FileCtrl.ReadData(playerID);
        // If file exists for this playerID, read file and set NN data
        if (saveData != null)
        {
            nn_data.ih_w = saveData.ih_w;
            nn_data.ih_b = saveData.ih_b;
            nn_data.ho_w = saveData.ho_w;
            nn_data.ho_b = saveData.ho_b;

        }
        // If file does not exist for this playerID, create new set of NN_Data
        else
        {
            nn_data.ih_w = Math.RandomMatrix(inputNodes, hiddenNodes);
            nn_data.ih_b = Math.RandomLst(hiddenNodes);
            nn_data.ho_w = Math.RandomMatrix(hiddenNodes, outputNodes);
            nn_data.ho_b = Math.RandomLst(outputNodes);
        }
    }

    // Feed inputs through network weights and biases to result the output rotation
    public float GetOutputValue(float[] inputs)
    {
        float[] h_outputs = Math.LstMultMatrix(inputs, nn_data.ih_w, nn_data.ih_b);

        for (int i = 0; i < h_outputs.Length; i++)
        {
            h_outputs[i] = Math.Sigmoid(h_outputs[i]);
        }

        float[] o_outputs = Math.LstMultMatrix(h_outputs, nn_data.ho_w, nn_data.ho_b);

        return Math.Sigmoid(o_outputs[0]);
    }
}