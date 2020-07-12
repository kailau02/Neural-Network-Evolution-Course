using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileInterpreter;

public class RunnerFactory : MonoBehaviour
{
    public GameObject runnerObject;
    [Range(2,150)]
    public int numberOfRunners;
    [Header("% chance that a weight or bias is mutated")]
    [Range(0f,1f)]
    public float mutationPercentage;
    [Header("The potential strength of a mutation")]
    [Range(1f,5f)]
    public float mutationScale;
    [Header("The strength of the fitness")]
    [Range(1f, 2f)]
    public float fitnessInfluence;
    public int generation;


    public void Start()
    {
        // Spawn all runners
        for (int runnerCount = 0; runnerCount < numberOfRunners; runnerCount++)
        {
            GameObject runner = Instantiate(runnerObject) as GameObject;
            runner.GetComponent<RunnerCtrl>().SetupRunner(runnerCount);
        }
        generation = FileCtrl.GetStatistics()[0];
    }

    // Hold all values of each runner after they die
    public List<RunnerDeathStats> deathList = new List<RunnerDeathStats>();

    // Add 1 to death count when runner dies and save that runner's stats
    int deathCount = 0;
    public void RunnerDies(int framesAlive, NN_Data nn_data)
    {
        deathCount++;
        deathList.Add(new RunnerDeathStats((int)Mathf.Pow((float)framesAlive, fitnessInfluence), nn_data));

        // When all runners are dead use their stats to reproduce
        if (deathCount == numberOfRunners) Reproduce();
    }

    // Genetic algorithm to create new neural networks
    void Reproduce()
    {
        // Breed and save children to file
        for (int ID = 0; ID < numberOfRunners - 1; ID++)
        {
            // Selection
            NN_Data[] parents = SelectParents();

            // Cross Over to child DNA and mutate the child's DNA
            NN_Data childDNA = CrossOver(parents);
            
            // Save new genes
            FileCtrl.WriteData(ID, childDNA);
        }

        // Save the fittest runner on his own
        FileCtrl.WriteData(deathList.Count - 1, deathList[deathList.Count - 1].nn_data);

        FileCtrl.SaveStats(generation + 1, deathList[deathList.Count - 1].framesAlive);

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    // Selection function to find most probable parents of the generation
    NN_Data[] SelectParents()
    {
        NN_Data[] parents = new NN_Data[2];

        // Get the fitness of all runners combined
        int combinedFitness = 0;
        foreach (var stats in deathList)
        {
            combinedFitness += stats.framesAlive;
        }

        // Get a random value between 0 and the combinedFitness. Use this to find a probable parent
        for (int p_Index = 0; p_Index < 2; p_Index++)
        {
            int randFitness = Random.Range(0, combinedFitness) - deathList[0].framesAlive;
            int locatedID = 0;
            while (randFitness > 0)
            {
                locatedID++;
                randFitness -= deathList[locatedID].framesAlive;
            }
            parents[p_Index] = deathList[locatedID].nn_data;
        }
        return parents;
    }

    // Combine 2 parents' DNA using crossover & apply mutations
    NN_Data CrossOver(NN_Data[] parents)
    {
        float[,] ih_w = new float[parents[0].ih_w.GetLength(0), parents[0].ih_w.GetLength(1)];
        float[] ih_b = new float[parents[0].ih_b.Length];
        float[,] ho_w = new float[parents[0].ho_w.GetLength(0), parents[0].ho_w.GetLength(1)];
        float[] ho_b = new float[parents[0].ho_b.Length];


        for (int row = 0; row < ih_w.GetLength(0); row++)
        {
            for (int col = 0; col < ih_w.GetLength(1); col++)
            {
                ih_w[row, col] = parents[Random.Range(0, 2)].ih_w[row, col];
                if(mutationPercentage > Random.value)
                {
                    ih_w[row, col] += (Random.value - .5f) * mutationScale;
                }
            }
        }

        for (int element = 0; element < ih_b.Length; element++)
        {
            ih_b[element] = parents[Random.Range(0, 2)].ih_b[element];
            if (mutationPercentage > Random.value)
            {
                ih_b[element] += (Random.value - .5f) * mutationScale;
            }
        }

        for (int row = 0; row < ho_w.GetLength(0); row++)
        {
            for (int col = 0; col < ho_w.GetLength(1); col++)
            {
                ho_w[row, col] = parents[Random.Range(0, 2)].ho_w[row, col];
                if (mutationPercentage > Random.value)
                {
                    ho_w[row, col] += (Random.value - .5f) * mutationScale;
                }
            }
        }
        for (int element = 0; element < ho_b.Length; element++)
        {
            ho_b[element] = parents[Random.Range(0, 2)].ho_b[element];
            if (mutationPercentage > Random.value)
            {
                ho_b[element] += (Random.value - .5f) * mutationScale;
            }
        }

        NN_Data childDNA = new NN_Data();
        childDNA.UpdateData(ih_w, ih_b, ho_w, ho_b);

        return childDNA;
    }

}
